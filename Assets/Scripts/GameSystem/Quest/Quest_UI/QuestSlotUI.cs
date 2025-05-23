using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class QuestSlotUI : MonoBehaviour
{
    [Header("UI ���")]
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI progressText;

    [SerializeField] public QuestDetailUI detailUI;

    private PlayerQuestProgress questProgress;

    public void Setup(PlayerQuestProgress progress)
    {
        questProgress = progress;
        QuestData data = progress.questData;

        // ����Ʈ Ÿ�� ǥ��
        typeText.text = data.questType switch
        {
            QuestType.Main => "<color=#FFA500>����</color>",     // ��Ȳ��
            QuestType.Side => "<color=#00BFFF>����</color>",     // �ϴû�
            QuestType.Repeatable => "<color=#AAAAAA>�ݺ�</color>",
            QuestType.Event => "<color=#FFD700>�̺�Ʈ</color>",
            _ => "��Ÿ"
        };

        // ����
        titleText.text = data.questTitle;

        // ���� ���൵ ���
        int total = progress.conditionProgress.Count;
        int complete = 0;
        for (int i = 0; i < total; i++)
        {
            if (data.conditions[i].IsMet(progress))
                complete++;
        }

        progressText.text = $"<color=green>({complete} / {total})</color>";

        // Ŭ�� �� �� ���� ����
        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() =>
        {
            detailUI.ShowDetails(progress);
        });
    }
}