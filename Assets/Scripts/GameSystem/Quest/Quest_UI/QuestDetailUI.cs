using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;

public class QuestDetailUI : MonoBehaviour
{
    [Header("UI ���")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI conditionText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private Button completeButton;

    [Header("���� ����")]
    [SerializeField] private RewardSlotUI[] rewardItemSlots;  // ������ �̸� ��ġ�ؼ� �迭�� ����

    private PlayerQuestProgress currentProgress;

    private void OnEnable()
    {
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestUpdated += OnQuestProgressUpdated;
    }

    private void OnDisable()
    {
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestUpdated -= OnQuestProgressUpdated;
    }

    private void OnQuestProgressUpdated(PlayerQuestProgress updated)
    {
        // ���� ���� �ִ� ����Ʈ�� ���ŵǾ����� �ٽ� ǥ��
        if (currentProgress != null && updated.questData.questID == currentProgress.questData.questID)
        {
            ShowDetails(updated);
        }
    }
    public void ShowDetails(PlayerQuestProgress progress)
    {
        currentProgress = progress;
        var data = progress.questData;

        titleText.text = data.questTitle;

        // ���൵ ���
        int total = progress.conditionProgress.Count;
        int complete = 0;
        for (int i = 0; i < total; i++)
        {
            if (data.conditions[i].IsMet(progress))
                complete++;
        }

        objectiveText.text = $"{data.questObjective} (<color=green>{complete} / {total}</color>)";
        descriptionText.text = data.questDescription;

        // ���� ����
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < data.conditions.Count; i++)
        {
            var condition = data.conditions[i];
            int cur = progress.conditionProgress[i];
            string condName = condition.conditionName;
            string detail = condition switch
            {
                KillEnemyCondition k => $"{cur}/{k.requiredKillCount}",
                CollectItemCondition c => $"{cur}/{c.requiredAmount}",
                TalkToNPCCondition => cur >= 1 ? "�Ϸ�" : "�̿Ϸ�",
                ReachAreaCondition => cur >= 1 ? "�Ϸ�" : "�̿Ϸ�",
                UseItemCondition u => $"{cur}/{u.requiredUseCount}",
                _ => $"{cur} / ?"
            };
            sb.AppendLine($"- {condName}: {detail}");
        }
        conditionText.text = sb.ToString();

        // EXP, Gold �ؽ�Ʈ
        rewardText.text = $"EXP: {data.reward.experience}\nGold: {data.reward.gold}";

        // ���� �ʱ�ȭ �� ���� ����
        for (int i = 0; i < rewardItemSlots.Length; i++)
        {
            if (i < data.reward.items.Count)
            {
                rewardItemSlots[i].Setup(data.reward.items[i]);
            }
            else
            {
                rewardItemSlots[i].Clear();
            }
        }

        // �Ϸ� ��ư
        completeButton.interactable = progress.state == QuestState.Completed;
        completeButton.onClick.RemoveAllListeners();
        completeButton.onClick.AddListener(() =>
        {
            QuestManager.Instance.CompleteQuest(data);
            completeButton.interactable = false;
            conditionText.text += "\n�� �Ϸ��!";
        });

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void RefreshCurrent()
    {
        if (currentProgress != null)
        {
            ShowDetails(currentProgress); // ������ �ٽ� ����
        }
    }
}