using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform target;    // 추적할 대상
    public float moveDamping = 15.0f;   // 이동 속도 계수
    public float rotateDamping = 10.0f;   // 이동 속도 계수
    public float distance = 5.0f;   // 이동 속도 계수
    public float height = 4.0f;   // 이동 속도 계수
    public float targetOffset = 2.0f;   // 이동 속도 계수

    // CameraRig의 Transform 컴포넌트
    private Transform tr;

    [Header("Wall Obstacle Setting")]
    public float heightAboveWall = 7.0f;        // 카메라가 올라갈 높이
    public float colliderRadius = 1.8f;         // 충돌체의 반지름
    public float overDamping = 5.0f;            // 이동 속도 계수
    private float originHeight;                 // 최초 높이 보관 변수

    void Start ()
    {
        // CameraRig의 Transform 컴포넌트 추출
        tr = GetComponent<Transform>();

        // 최초 카메라의 높이 저장
        originHeight = height;
	}

    void LateUpdate()
    {
        // 카메라의 높이와 거리를 계산
        var camPos = target.position - (target.forward * distance) + (target.up * height);

        // 이동할 때의 속도 계수를 적용
        tr.position = Vector3.Slerp(tr.position, camPos, Time.deltaTime * rotateDamping);

        // 회전할 때의 속도 계수를 적용
        tr.rotation = Quaternion.Slerp(tr.rotation, target.rotation, Time.deltaTime * rotateDamping);

        // 카메라를 추적 대상으로 Z축으로 회전시킴
        tr.LookAt(target.position + (target.up * targetOffset));
	}

    private void Update()
    {
        // 구체 형태의 충돌체고 충돌 여부 검사
        if (Physics.CheckSphere(tr.position, colliderRadius))
        {
            // 보간 함수를 사용해 카메라의 높이를 부드럽게 상승시킴
            height = Mathf.Lerp(height, heightAboveWall, Time.deltaTime * overDamping);
        }
        else
        {
            // 보간 함수를 사용해 카메라의 높이를 부드럽게 하강시킴
            height = Mathf.Lerp(height, originHeight, Time.deltaTime * overDamping);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        // 추적 및 시야를 맞출 위치를 표시
        Gizmos.DrawWireSphere(target.position + (target.up * targetOffset), 0.1f);
        // 메인 카메라와 추적 지점 간의 선을 표시
        Gizmos.DrawLine(target.position + (target.up * targetOffset), transform.position);

        Gizmos.DrawWireSphere(transform.position, colliderRadius);
    }
}
