//using UnityEngine;

//public class ItemRotator : MonoBehaviour
//{
//    [Header("ȸ�� ����")]
//    [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 45f, 0f);

//    [Header("���� ����")]
//    [SerializeField] private float floatAmplitude = 0.25f; // ���Ʒ� �̵� ��
//    [SerializeField] private float floatFrequency = 1f;    // �ʴ� ���� Ƚ��

//    private Vector3 startPos;

//    void Start()
//    {
//        startPos = transform.position;
//    }

//    void Update()
//    {
//        // ȸ��
//        transform.Rotate(rotationSpeed * Time.deltaTime);

//        // ���� (���Ʒ� ������)
//        float newY = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
//        transform.position = startPos + new Vector3(0f, newY, 0f);
//    }
//}