using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class QuestHUDUI : MonoBehaviour
{
    [SerializeField] private GameObject questEntryPrefab;
    [SerializeField] private Transform entryParent; // Content
    [SerializeField] private Button completeAllButton;


    private void Start()
    {
        completeAllButton.onClick.AddListener(OnClickCompleteAll);
    }
    private void OnEnable()
    {
        QuestManager.Instance.OnQuestUpdated += HandleQuestUpdate;
        Refresh();
    }

    private void OnDisable()
    {
        QuestManager.Instance.OnQuestUpdated -= HandleQuestUpdate;
    }

    public void Refresh()
    {
        foreach (Transform child in entryParent)
            Destroy(child.gameObject);

        bool anyComplete = false;

        var quests = QuestManager.Instance.GetActiveQuests();
        foreach (var progress in QuestManager.Instance.GetActiveQuests())
        {
            //  ���°� ���� �Ϸ�� ����
            if (progress.state == QuestState.Rewarded)
                continue;
            GameObject go = Instantiate(questEntryPrefab, entryParent);
            go.GetComponent<QuestHUDElement>().Setup(progress);

            if (progress.state == QuestState.Completed)
                anyComplete = true;
        }

        completeAllButton.gameObject.SetActive(anyComplete);
    }

    public void OnClickCompleteAll()
    {
        var quests = QuestManager.Instance.GetActiveQuests();
        foreach (var q in quests)
        {
            if (q.state == QuestState.Completed)
            {
                QuestManager.Instance.CompleteQuest(q.questData);
            }
        }

        Refresh(); // UI �ٽ� �׸���
    }

    private void HandleQuestUpdate(PlayerQuestProgress _) => Refresh();
}