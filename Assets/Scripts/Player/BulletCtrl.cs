using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour {
    // 총알 파괴력
    public float damage = 20.0f;

    // 총알 발사속도
    public float speed = 1000.0f;

    private ScreenCenter screenCenter;

    void Start ()
    {
        screenCenter = GameObject.Find("Main Camera").GetComponent<ScreenCenter>();

        transform.LookAt(screenCenter.hitPos);

        GetComponent<Rigidbody>().AddForce(transform.forward * speed);  // transform.forward == vector3.forward
    }
}
