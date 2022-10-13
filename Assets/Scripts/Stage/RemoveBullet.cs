﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    // 스파크 프리팹을 저장할 변수
    public GameObject sparkEffect;

    // 충돌이 시작할 때 발생하는 이벤트
    private void OnCollisionEnter(Collision coll)   // 내가 쏘는 총알이 맞을 때
    {
        // 충돌한 게임 오브젝트의 태그 값 비교
        if(coll.collider.tag == "BULLET")
        {
            // 스파크 효과 함수 호출
            ShowEffect(coll);
            // 충돌한 게임 오브젝트 삭제
            Destroy(coll.gameObject);
        }
    }

    // 충돌이 시작할 때 발생하는 이벤트
    private void OnTriggerEnter(Collider coll)  // 적이 쏘는 총알이 맞을 때
    {
        // 충돌한 게임 오브젝트의 태그 값 비교
        if (coll.tag == "BULLET")
        {
            // 충돌한 게임 오브젝트 삭제
            Destroy(coll.gameObject);
        }
    }

    void ShowEffect(Collision coll)
    {
        // 충돌 지점의 정보를 추출
        ContactPoint contact = coll.contacts[0];

        // 법선 벡터가 이루는 회전각도를 추출, Vector3.forward와 -contact.normal의 회전값(세타)을 구하는게 아래 코드
        Quaternion rot = Quaternion.FromToRotation(Vector3.forward, -contact.normal);

        // 스파크 효과를 생성
        GameObject spark = Instantiate(sparkEffect, contact.point + (-contact.normal * 0.05f), rot);
        // 스파크 효과의 부모를 드럼통 또는 벽으로 설정
        spark.transform.SetParent(this.transform);
    }
}
