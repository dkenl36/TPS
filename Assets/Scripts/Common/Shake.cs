using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour {
    // 셰이크 효과를 줄 카메라의 Transform을 저장할 변수 
    public Transform shakeCamera;

    // 회전시킬 것인지를 판달할 변수
    public bool shakeRotate = false;

    // 초기 좌표와 회전값을 저장할 변수
    private Vector3 originPos;
    private Quaternion originRot;

    void Start ()
    {
        // 카메라의 초깃값을 저장
        originPos = shakeCamera.localPosition;
        originRot = shakeCamera.localRotation;
	}
	
	// Update is called once per frame
	void Update ()
    {
        originPos = shakeCamera.localPosition;
        originRot = shakeCamera.localRotation;
    }

    // duration 흔들리는 시간, magnitudePos 흔들리는 정도
    public IEnumerator ShakeCamera(float duration = 0.05f, float magnitudePos = 0.03f, float magnitudeRot = 0.1f)
    {
        // 지나간 시간을 누적할 변수
        float passTime = 0.0f;

        // 진동 시간 동안 루프를 순회함
        while(passTime < duration)
        {
            // 불규칙한 위치를 산출
            // insideUnitSphere는 반지름이 1인 구 내부의 임의의 점의 위치를 반환
            Vector3 shakePos = Random.insideUnitSphere;
            // 카메라의 위치를 변경
            shakeCamera.localPosition = shakePos * magnitudePos;

            // 불규칙한 회전을 사용할 경우
            if (shakeRotate)
            {
                // 불규칙한 회전값을 펄린 노이즈 함수를 이용해 추출
                Vector3 shakeRot = new Vector3(0, 0, Mathf.PerlinNoise(Time.time * magnitudeRot, 0.0f));

                // 카메라의 회전값을 변경
                shakeCamera.localRotation = Quaternion.Euler(shakeRot);
            }
            // 진동 시간을 누적
            passTime += Time.deltaTime;

            yield return null;
        }

        // 진동이 끝난 후 카메라의 초깃값으로 설정
        shakeCamera.localPosition = originPos;
        shakeCamera.localRotation = originRot;
    }
}
