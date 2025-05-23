using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    public GameObject hpBarGroup;
    public Slider hpSlider;

    private Coroutine hideCoroutine;
    private Transform mainCam;

    void Start()
    {
        if (Camera.main != null)
        {
            mainCam = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning("Main Camera�� ã�� �� �����ϴ�. EnemyUI�� ȸ������ �ʽ��ϴ�.");
        }
    }

    void LateUpdate()
    {
        if (mainCam != null)
        {
            transform.rotation = Quaternion.LookRotation(mainCam.forward, mainCam.up);
        }
    }

    public void ShowHP(float current, float max)
    {
        Debug.Log($"[EnemyUI] ShowHP ȣ���: {current}/{max}");

        if (hpSlider == null)
        {
            Debug.LogWarning("EnemyUI: ������Ʈ ����");
            return;
        }

        hpSlider.value = current / max;
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        hpBarGroup.SetActive(false);
    }
}



