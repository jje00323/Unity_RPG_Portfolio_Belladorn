using UnityEngine;

public class QuestAreaTrigger : MonoBehaviour
{
    [Header("����")]
    [SerializeField] private string areaID;
    [SerializeField] private GameObject visualEffect;

    private bool hasTriggered = false;

    public void Setup(string id)
    {
        areaID = id;

        // ����Ʈ�� �׻� Ȱ��ȭ�ص� �� (�̹� ���ǿ� ���� ������)
        if (visualEffect != null)
            visualEffect.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered || !other.CompareTag("Player")) return;

        hasTriggered = true;

        // ���� ���� �õ�
        QuestManager.Instance.UpdateCondition("ReachArea", areaID);

        // Ʈ���� ���� ����
        QuestAreaTriggerSpawner.Instance?.RemoveTrigger(areaID);

        if (visualEffect != null)
            visualEffect.SetActive(false);
    }
}