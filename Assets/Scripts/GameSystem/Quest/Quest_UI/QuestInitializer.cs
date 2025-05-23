using UnityEngine;

public class QuestInitializer : MonoBehaviour
{
    [Header("�ʱ� ����Ʈ ����")]
    [SerializeField] private QuestData[] defaultQuests;

    void Start()
    {
        foreach (var quest in defaultQuests)
        {
            if (!QuestManager.Instance.IsQuestActive(quest.questID))
            {
                QuestManager.Instance.AcceptQuest(quest);
                Debug.Log($"[�ʱ�ȭ] �⺻ ����Ʈ ������: {quest.questTitle}");
            }
        }
    }
}