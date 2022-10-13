using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UI항목에 접근하기 위해 선언하는 네임스페이스
using UnityEngine.UI;

public class Damage : MonoBehaviour {
    private const string bulletTag = "BULLET";
    private const string enemyTag = "ENEMY";

    private float initHp = 100.0f;
    public float currHp;

    // 델리게이트 및 이벤트 선언
    public delegate void PlayerDieHandler();                // 함수 모양을 한 델리게이트 변수 선언
    public static event PlayerDieHandler OnPlayerDie;       // 위아래 2개의 변수 이름은 같아야한다

    // BloodScreen 텍스처를 저장하기 위한 변수
    public Image bloodScreen;

    // HP Bar Image를 저장하기 위한 변수
    public Image hpBar;

    // 생명 게이지의 처음 색상(녹색)
    private readonly Color initColor = new Vector4(0, 1.0f, 0.0f, 1.0f);
    private Color currColor;

	void Start ()
    {
        currHp = initHp;

        // 생명 게이지의 초기 색상을 설정
        hpBar.color = initColor;
        currColor = initColor;
	}

    // 충돌한 Collider의 IsTrigger 옵션이 체크됐을 때 발생
    private void OnTriggerEnter(Collider coll)
    {
        // 충돌한 Collider의 태그가 BULLET이면 Player의 currHp를 차감
        if(coll.tag == bulletTag)
        {
            Destroy(coll.gameObject);

            // 혈흔 효과를 표현할 코루틴 함수 호출
            StartCoroutine(ShowBloodScreen());

            currHp -= 5.0f;
            Debug.Log("Player HP = " + currHp.ToString());

            // 생명 게이지으 ㅣ색상 및 크기 변경 함수를 호출
            DisplayHpbar();

            // Player의 생명이 0이하라면 사망 처리
            if(currHp <= 0.0f)
            {
                PlayerDie();
            }
        }
    }

    // 충돌한 Collider의 IsTrigger 옵션이 체크됐을 때 발생
    private void OnCollisionEnter(Collision coll)
    {
        // 충돌한 Collider의 태그가 BULLET이면 Player의 currHp를 차감
        if (coll.collider.tag == bulletTag)
        {
            Destroy(coll.gameObject);

            // 혈흔 효과를 표현할 코루틴 함수 호출
            //StartCoroutine(ShowBloodScreen());

            currHp -= 5.0f;
            //Debug.Log("Player HP = " + currHp.ToString());

            // 생명 게이지으 ㅣ색상 및 크기 변경 함수를 호출
            //DisplayHpbar();

            // Player의 생명이 0이하라면 사망 처리
            if (currHp <= 0.0f)
            {
                PlayerDie();
            }
        }
    }

    // Player의 사망 처리 루틴
    void PlayerDie()
    {
        OnPlayerDie();

        /*
        Debug.Log("PlayerDie!");

        // "ENEMY" 태그로 지정된 모든 적 캐릭터를 추출해 배열에 저장
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        // 배열의 처음부터 순회하면서 적 캐릭터의 OnPlayerDie 함수를 호출
        for(int i = 0; i < enemies.Length; i++)
        {
            // DontRequireReceiver은 무슨 일이 있어도 답변을 보내지 않는다
            // 모든 컴포넌트를 검색한다
            // 컴포넌트가 많아지면 cpu에 부하가 많이 발생한다
            // EnemyAI에 OnPlayerDie가 있으면 실행
            enemies[i].SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        }
        */
    }

    IEnumerator ShowBloodScreen()
    {
        // BloodScreen 텍스처의 알파값을 불규칙하게 변경
        bloodScreen.color = new Color(1, 0, 0, Random.Range(0.2f, 0.3f));       // 프로그램에서는 1이 최대치
        yield return new WaitForSeconds(0.1f);                                             // 0.1초 간격으로 시간 줌

        // BloodScreen 텍스처의 색상을 모두 0으로 변경
        bloodScreen.color = Color.clear;
    }

    void DisplayHpbar()
    {
        // 생명 수치가 50%일 때까지는 녹색에서 노란색으로 변경
        if ((currHp / initHp) > 0.5f)
            currColor.r = (1 - (currHp / initHp)) * 2.0f;
        else    // 생명 수치가 0%일 때까지는 노란색에서 빨간색으로 변경
            currColor.g = (currHp / initHp) * 2.0f;

        // HpBar의 색상 변경
        hpBar.color = currColor;
        // HpBar의 크기 변경
        hpBar.fillAmount = (currHp / initHp);
    }
}
