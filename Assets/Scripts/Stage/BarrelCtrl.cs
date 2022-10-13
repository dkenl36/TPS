using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour {
    // 폭발 효과 프리팹을 저장할 변수
    public GameObject expEffect;

    // 총알이 맞은 회수
    private int hitCount = 0;

    // Rigidbody 컴포넌트를 저장할 변수
    private Rigidbody rb;

    // 찌그러진 드럼통의 메쉬를 저장할 배열
    public Mesh[] meshes;

    // 드럼통의 텍스처를 저장할 배열
    public Texture[] textures;

    // MeshFilter 컴포넌트를 저장할 변수
    private MeshFilter meshFilter;

    // MeshRenderer 컴포넌트를 저장할 변수
    private MeshRenderer _renderer;

    // AudioSource 컴포넌트를 저장할 변수
    private AudioSource _audio;

    // 폭발 반경
    public float expRadius = 10.0f;

    // 폭발음 오디오 클립
    public AudioClip expSfx;

    // Shake 클래스를 저장할 변수
    public Shake shake;

	void Start ()
    {
        // Rigidbody 컴포넌트를 추출해 저장
        rb = GetComponent<Rigidbody>();

        // MeschFilter 컴포넌트를 추출해 저장
        meshFilter = GetComponent<MeshFilter>();

        // MeshRenderer 컴포넌트를 추출해 저장
        _renderer = GetComponent<MeshRenderer>();

        // AudioSource 컴포넌트를 추출해 저장
        _audio = GetComponent<AudioSource>();

        // Shake 스크립트를 추출
        shake = GameObject.Find("CameraRig").GetComponent<Shake>();

        // 난수를 발생시켜 불규칙적인 텍스처를 적용
        _renderer.material.mainTexture = textures[Random.Range(0, textures.Length)];
	}

    private void OnCollisionEnter(Collision coll)       // 내가 쏜 총알이 맞았을 떄
    {
        // 충돌한 게임오브젝트의 태그를 비교
        if(coll.collider.CompareTag("BULLET"))
        {
            // 총알의 충돌 횟수를 증가시키고 3발 이상 맞았는지 확인
            hitCount++;
            if (hitCount == 3)
            {
                ExpBarrel();
            }
        }
    }

    private void OnTriggerEnter(Collider coll)      // 적이 쏜 총알이 맞았을 때
    {
        // 충돌한 게임오브젝트의 태그를 비교
        if (coll.CompareTag("BULLET"))
        {
            // 총알의 충돌 횟수를 증가시키고 3발 이상 맞았는지 확인
            hitCount++;
            if (hitCount == 3)
            {
                ExpBarrel();
            }
        }
    }

    void ExpBarrel()
    {
        // 폭발 효과 프리팹을 동적으로 생성
        GameObject effect = Instantiate(expEffect, transform.position, Quaternion.identity);
        // 메모리에 폭발 효과가 남아있어서 2초후에 폭발 효과를 삭제하겠다는 코드
        Destroy(effect, 2.0f);

        // Rigidbody 컴포넌트의 mass를 1.0으로 수정해 무게를 가볍게 함, 20.0f일때 폭발하면 너무 무거워서 조금만 공중에 뜬다
        //rb.mass = 1.0f;

        //위로 솟구치는 힘을 가함
        //rb.AddForce(Vector3.up * 500.0f);

        // 난수를 발생
        int idx = Random.Range(0, meshes.Length);
        // 찌그러진 메쉬를 적용
        meshFilter.sharedMesh = meshes[idx];
        GetComponent<MeshCollider>().sharedMesh = meshes[idx];

        // 폭발력 생성
        IndirectDamage(transform.position);

        // 폭발음 발생
        _audio.PlayOneShot(expSfx, 1.0f);   // 1.0f는 볼륨

        // Shake 효과 호출
        StartCoroutine(shake.ShakeCamera(0.1f, 0.2f, 0.5f));

        ChangeBarrelMass(transform.position);
    }

    void IndirectDamage(Vector3 pos)
    {
        // 주변에 있는 드럼통을 모두 추출
        // 쉬프트 연산으로 8번 Layer 추출(Barrel의 8번 Layer)
        Collider[] colls = Physics.OverlapSphere(pos, expRadius, 1 << 8);

        foreach(var coll in colls)
        {
            // 폭발 범위에 포함된 드럼통의 Rigidbody 컴포넌트 추출
            var _rb = coll.GetComponent<Rigidbody>();
            // 드럼통의 무게를 가볍게 함
            _rb.mass = 1.0f;
            // 폭발력을 전달
            // 매개변수 1: 횡 폭발력 2: 폭발 원점  3: 폭발 반경 4: 위로 솟구치는 힘
            _rb.AddExplosionForce(500.0f, pos, expRadius, 500.0f);
        }
    }

    void ChangeBarrelMass(Vector3 pos)
    {
        Collider[] colls = Physics.OverlapSphere(pos, expRadius, 1 << 8);

        foreach (var coll in colls)
        {
            // 폭발 범위에 포함된 드럼통의 Rigidbody 컴포넌트 추출
            var _rb = coll.GetComponent<Rigidbody>();
            // 드럼통의 무게를 가볍게 함
            _rb.mass = 100.0f;
        }
    }
}
