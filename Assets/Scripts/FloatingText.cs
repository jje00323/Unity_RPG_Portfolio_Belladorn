using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public float duration = 2f;

    private float timer = 0f;
    private int accumulatedDamage = 0;

    public System.Action onDestroyed; // �ݹ� ��Ͽ�

    void Awake()
    {
        if (textMesh == null)
            textMesh = GetComponent<TextMeshProUGUI>();
    }

    public void SetDamage(int amount, Color color)
    {
        accumulatedDamage = amount;
        textMesh.text = accumulatedDamage.ToString();
        textMesh.color = color;
        timer = 0f;
    }

    public void AddDamage(int amount)
    {
        accumulatedDamage += amount;
        textMesh.text = accumulatedDamage.ToString();
        timer = 0f; // ���� �ð� ����
    }
    public void SetText(string message, Color color)
    {
        textMesh.text = message;
        textMesh.color = color;
        timer = 0f; // ���� �ð� ����
    }


    void Update()
    {
        if (Camera.main != null)
        {
            transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }

        timer += Time.deltaTime;
        if (timer >= duration)
        {
            onDestroyed?.Invoke(); // ������� ������ �˸�
            Destroy(gameObject);
        }
    }
}
