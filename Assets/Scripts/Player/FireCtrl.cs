using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 총알 발사와 재장전 오디오 클립을 저장할 구조체
[System.Serializable]

public struct PlayerSfx
{
    public AudioClip[] fire;
    public AudioClip[] reload;
}

public class FireCtrl : MonoBehaviour {
    // 총알 프리팹
    public GameObject bullet;

    // 총알 발사 좌표
    public Transform firePos;

    // 탄피 추출 파티클
    public ParticleSystem cartridge;

    // 머즐 플래쉬
    private ParticleSystem muzzleFlash;

    public enum WeaponType
    {
        RIFLE = 0,      // 0의 의미는 디폴트 값이다.
        SHOTGUN
    }

    // 주인공이 현재 들고있는 무기를 저장할 변수
    public WeaponType currWeapon = WeaponType.RIFLE;

    // 오디오 클립을 저장할 변수
    public PlayerSfx playerSfx;

    // AudioSource 컴포넌트를 저장할 변수
    private AudioSource _audio;

    // Shake 클래스를 저장할 변수
    private Shake shake;

    // 탄창 이미지 Image UI
    public Image magazineImg;
    // 남은 총알 수 Text UI
    public Text magazineText;

    // 최대 총알 수
    public int maxBullet = 10;
    // 남은 총알 수 
    public int remainingBullet = 10;

    // 재장전 시간
    public float reloadTime = 2.0f;
    // 재장전 여부를 판단할 변수
    private bool isReloading = false;

	void Start ()
    {
        // FirePos 하위에 있는 컴포넌트 추출
        muzzleFlash = firePos.GetComponentInChildren<ParticleSystem>();

        // AudioSource 컴포넌트 추출
        _audio = GetComponent<AudioSource>();

        // Shake 스크립트를 추출
        shake = GameObject.Find("CameraRig").GetComponent<Shake>();
    }
	
	void Update ()
    {
		// 마우스 왼쪽 버튼을 클릭했을 때 Fire함수 호출
        if((isReloading == false) && (Input.GetMouseButtonDown(0)))
        {
            // 총알 수를 하나 감소
            for(int i = 0; i < 1; i++)
            {
                --remainingBullet;
                Fire();
                //StartCoroutine(TripleShot());
            }

            // 남은 총알이 없을 경우 재장전 코루틴 호출
            if(remainingBullet <= 0)
            {
                StartCoroutine(Reloading());
            }
        }

        Debug.DrawRay(firePos.position, firePos.forward * 20.0f, Color.red);
	}

    void Fire()
    {
        // 셰이크 효과 호출
        StartCoroutine(shake.ShakeCamera());

        // Bullet 프리팹을 동적으로 생성
        Instantiate(bullet, firePos.position, firePos.rotation);

        // 탄피 파티클
        cartridge.Play();

        // 머즐 파티클
        muzzleFlash.Play();

        // 사운드 발생
        FireSfx();

        // 재장전 이미지의 fillAmount 속성값 지정
        magazineImg.fillAmount = (float)remainingBullet / (float)maxBullet;

        // 남은 총알 수 갱신
        UpdateBulletText();
    }

    void FireSfx()
    {
        // 현재 들고 있는 무기의 오디오 클립을 가져옴
        var _sfx = playerSfx.fire[(int)currWeapon];
        // 사운드 발생, 1.0f 는 볼륨
        _audio.PlayOneShot(_sfx, 1.0f);
    }

    IEnumerator Reloading()
    {
        isReloading = true;

        yield return new WaitForSeconds(0.8f);

        _audio.PlayOneShot(playerSfx.reload[(int)currWeapon], 1.0f);    // PlayOneShot은 사운드 1번 발생, 1.0f는 볼륨

        // 재장전 오디오의 길이 + 0.3초 동안 대기
        // WaitForSeconds는 일시정지 효과 즉, 사운드 소리에다가 0.3초 더한 시간만큼 가만히 있는다.
        yield return new WaitForSeconds(playerSfx.reload[(int)currWeapon].length + 0.1f);

        // 각종 변수값의 초기화
        isReloading = false;
        magazineImg.fillAmount = 1.0f;  // fillAmount이 1이면 꽉 찬다
        remainingBullet = maxBullet;

        // 남은 총알 수 갱신
        UpdateBulletText();
    }

    void UpdateBulletText()
    {
        magazineText.text = string.Format("<color=#ff0000>{0}</color>/{1}", remainingBullet, maxBullet);
    }

    IEnumerator TripleShot()
    {
        yield return new WaitForSeconds(0.3f);
    }
}
