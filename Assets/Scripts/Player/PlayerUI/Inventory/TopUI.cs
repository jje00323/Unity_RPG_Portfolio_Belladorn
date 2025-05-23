using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopUI : MonoBehaviour
{
    void OnEnable()
    {
        // ��� 1: Canvas �켱���� ����
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
            canvas = gameObject.AddComponent<Canvas>();

        canvas.overrideSorting = true;
        canvas.sortingOrder = 1000;

        // ��� 2 (���� �θ� �ȿ���): ���� ���� �ڷ� ������
        transform.SetAsLastSibling();
    }
}