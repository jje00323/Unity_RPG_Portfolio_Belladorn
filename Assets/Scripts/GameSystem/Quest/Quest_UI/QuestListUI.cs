using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestListUI : MonoBehaviour
{
    [Header("UI ����")]
    [SerializeField] private GameObject questSlotPrefab;
    [SerializeField] private Transform questSlotParent;

    [SerializeField] private Button inProgressButton;
    [SerializeField] private Button completedButton;

    [SerializeField] private QuestDetailUI questDetailUI;

    private enum QuestFilter { InProgress, Completed }
    private QuestFilter currentFilter = QuestFilter.InProgress;

    private void Start()
    {
        inProgressButton.onClick.AddListener(() => ChangeFilter(QuestFilter.InProgress));
        completedButton.onClick.AddListener(() => ChangeFilter(QuestFilter.Completed));

        RefreshUI();
    }

    private void OnEnable()
    {
        if (QuestManager.Instance == null)
        {
            Debug.LogWarning("[QuestListUI] QuestManager.Instance�� ���� �غ���� �ʾҽ��ϴ�.");
            return;
        }

        RefreshUI();

        questDetailUI.RefreshCurrent();
    }

    private void ChangeFilter(QuestFilter filter)
    {
        if (currentFilter == filter) return;

        currentFilter = filter;
        RefreshUI();
    }

    public void RefreshUI()
    {
        // ���� ���� ����
        foreach (Transform child in questSlotParent)
        {
            Destroy(child.gameObject);
        }

        List<PlayerQuestProgress> quests = QuestManager.Instance.GetActiveQuests();

        foreach (var progress in quests)
        {
            bool show = currentFilter switch
            {
                QuestFilter.InProgress => progress.state == QuestState.InProgress || progress.state == QuestState.Completed,
                QuestFilter.Completed => progress.state == QuestState.Rewarded,
                _ => false
            };

            if (!show) continue;

            GameObject slotGO = Instantiate(questSlotPrefab, questSlotParent);
            QuestSlotUI slot = slotGO.GetComponent<QuestSlotUI>();

            //  QuestDetailUI ����
            slot.detailUI = questDetailUI;

            slot.Setup(progress);
        }
    }
}