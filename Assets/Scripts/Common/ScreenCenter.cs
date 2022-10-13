using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCenter : MonoBehaviour
{
    private Vector3 center;

    private float y = 0.0f;
    private Transform tr;
    public float rotSpeed = 80.0f;

    public RaycastHit raycastHit;
    public Vector3 hitPos;

    // Use this for initialization
    void Start ()
    {
        center = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);

        tr = GetComponent<Transform>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        Ray ray = Camera.main.ScreenPointToRay(center);

        Debug.DrawRay(transform.position, transform.forward * 200f, Color.red);

        y = Input.GetAxis("Mouse Y");

        tr.Rotate(Vector3.right * rotSpeed * Time.deltaTime * -y);

        if (Physics.Raycast(ray, out raycastHit))
        {
            hitPos = raycastHit.point;
        }
    }
}
