using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class PlayerAnim
{
    public AnimationClip idle;
    public AnimationClip runF;
    public AnimationClip runB;
    public AnimationClip runL;
    public AnimationClip runR;
    public AnimationClip idleReloadSMG;
}

public class PlayerCtrl : MonoBehaviour
{
    private float h = 0.0f;
    private float v = 0.0f;
    private float r = 0.0f;

    // 접근해야하는 컴포넌트는 반드시 변수에 할당한 후 사용
    private Transform tr;

    // 이동 속도 변수(public으로 선언되어 Inspector에 노출됨)
    public float moveSpeed = 10.0f;

    // 회전 속도 변수, 즉 마우스 감도 조절
    public float rotSpeed = 80.0f;

    // 인스펙터 뷰에 표시할 애니메이션 클래스 변수
    public PlayerAnim playerAnim;
    // Animation 컴포넌트를 저장하기 위한 변수
    public Animation anim;

    private FireCtrl fireCtrl;

	void Start ()
    {
        // 스크립트가 실행된 후 처음 수행되는 Start 함수에서 Transform 컴포넌트 할당
        // 누구의 컴포넌트인지 지정하지 않으면 유니티에서 드래그 앤 드롭으로 준 객체의 트랜스폼을 불러온다
        tr = GetComponent<Transform>();

        // Animation 컴포넌트를 변수에 할당
        anim = GetComponent <Animation>();

        // fireCtrl 스크립트 추출
        fireCtrl = GameObject.Find("Player").GetComponent<FireCtrl>();

        // Animation 컴포넌트의 애니메이션 클립을 지정하고 실행
        anim.clip = playerAnim.idle;
        anim.Play();
	}
	
	void Update ()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        r = Input.GetAxis("Mouse X");
        //y = Input.GetAxis("Mouse Y");

        //Debug.Log("h=" + h.ToString());
        //Debug.Log("v=" + v.ToString());

        // Translate(이동 방향 * 속도 * 변위값 * Time.deltaTime, 기준 좌표)
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);                    // 이동방향, 전후좌우 모두 포함
        tr.Translate(moveDir.normalized * moveSpeed *  Time.deltaTime, Space.Self);       // 이동 명령 내릴때는 무조건 Time.deltaTime을 써야한다

        // Z축을 기준으로 rotSpeed만큼의 속도로 회전
        tr.Rotate(Vector3.up * rotSpeed * Time.deltaTime * r);
        //tr.Rotate(Vector3.right * rotSpeed * Time.deltaTime * y);

        // 키보드 입력값을 기준으로 동작할 애니메이션 수행
        if(v >= 0.1f)
        {
            anim.CrossFade(playerAnim.runF.name, 0.3f);     // 전진 애니메이션
        }
        else if(v <= -0.1f)
        {
            anim.CrossFade(playerAnim.runB.name, 0.3f);     // 후진 애니메이션
        }
        else if (h >= 0.1f)
        {
            anim.CrossFade(playerAnim.runR.name, 0.3f);     // 오른쪽 애니메이션
        }
        else if (h <= -0.1f)
        {
            anim.CrossFade(playerAnim.runL.name, 0.3f);     // 왼쪽 애니메이션
        }
        else if(fireCtrl.remainingBullet <= 0)             
        {
            anim.CrossFade(playerAnim.idleReloadSMG.name, 0.3f);    // 총알 수가 0 이하면 재장전
        }
        else
        {
            anim.CrossFade(playerAnim.idle.name, 0.3f);     // 정지시 idle 애니메이션
        }
    }
}
