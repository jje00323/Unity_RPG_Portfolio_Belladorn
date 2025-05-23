using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    public Transform target; // ���� ��� (�÷��̾�)
    public Vector3 offset = new Vector3(0, 9f, -4.8f); // ��ġ ������
    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.fieldOfView = 70f; // FOV ����
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // ��ġ ����
            transform.position = target.position + offset;

            // ���� ����
            transform.rotation = Quaternion.Euler(55f, 0f, 0f);
        }
    }
}
