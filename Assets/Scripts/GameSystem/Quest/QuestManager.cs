using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    private List<PlayerQuestProgress> activeQuests = new List<PlayerQuestProgress>();


    public event System.Action<PlayerQuestProgress> OnQuestUpdated;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // ����Ʈ ����
    public void AcceptQuest(QuestData questData)
    {
        if (IsQuestActive(questData.questID)) return;

        PlayerQuestProgress newProgress = new PlayerQuestProgress(questData);
        activeQuests.Add(newProgress);

        //  ����Ʈ ���� ��ȸ�ϸ鼭 ReachAreaCondition Ȯ��
        foreach (var condition in questData.conditions)
        {
            if (condition is ReachAreaCondition areaCond)
            {
                QuestAreaTriggerSpawner.Instance?.SpawnTrigger(areaCond.areaID);
            }
        }

        if (!string.IsNullOrWhiteSpace(questData.startNarration))
        {
            if (!string.IsNullOrWhiteSpace(questData.startNarration))
            {
                Debug.Log($"[QuestManager] �����̼� ��� �õ�: {questData.startNarration}");
                DialogueSystem.Instance?.ShowNarration(questData.startNarration);
            }
        }
        OnQuestUpdated?.Invoke(newProgress);
        Debug.Log($"[QuestManager] ����Ʈ ����: {questData.questTitle}");
    }

    // ���� ���� �� ��ô�� ����
    public void UpdateCondition(string type, string identifier, int amount = 1)
    {
        foreach (var progress in activeQuests)
        {
            if (progress.state != QuestState.InProgress) continue;

            for (int i = 0; i < progress.questData.conditions.Count; i++)
            {
                var condition = progress.questData.conditions[i];

                if (condition is KillEnemyCondition killCond && type == "KillEnemy")
                {
                    if (killCond.targetEnemyTag == identifier)
                        progress.IncrementProgress(i, amount);
                }
                else if (condition is CollectItemCondition collectCond && type == "CollectItem")
                {
                    if (collectCond.targetItem.itemName == identifier)
                        progress.IncrementProgress(i, amount);
                }
                else if (condition is TalkToNPCCondition talkCond && type == "TalkToNPC")
                {
                    if (talkCond.npcID == identifier)
                        progress.IncrementProgress(i, amount);
                }
                else if (condition is ReachAreaCondition areaCond && type == "ReachArea")
                {
                    if (areaCond.areaID == identifier)
                    {
                        progress.IncrementProgress(i, amount);

                        //  Ʈ���� ��� ����
                        QuestAreaTriggerSpawner.Instance?.RemoveTrigger(identifier);
                    }
                }
                else if (condition is UseItemCondition useCond && type == "UseItem")
                {
                    if (useCond.requiredItem.itemName == identifier)
                        progress.IncrementProgress(i, amount);
                }
            }

            if (progress.IsAllConditionsMet() && progress.state != QuestState.Completed)
            {
                progress.state = QuestState.Completed;
                Debug.Log($"[QuestManager] ����Ʈ �Ϸ� ����: {progress.questData.questTitle}");
            }

            OnQuestUpdated?.Invoke(progress);
        }
    }

    // ����Ʈ ���� ���� ó��
    public void CompleteQuest(QuestData questData)
    {
        PlayerQuestProgress progress = GetProgress(questData.questID);
        if (progress == null || progress.state != QuestState.Completed) return;

        // ���� ���� ó��
        GrantRewards(questData.reward);

        progress.state = QuestState.Rewarded;

        if (questData.nextQuestID != -1)
        {
            QuestData nextQuest = Resources.Load<QuestData>($"Quest/Quest_{questData.nextQuestID}");
            if (nextQuest != null)
            {
                AcceptQuest(nextQuest);
                Debug.Log($"[QuestManager] ���� ����Ʈ �ڵ� ������: {nextQuest.questTitle}");
            }
            else
            {
                Debug.LogWarning($"[QuestManager] ���� ����Ʈ(ID: {questData.nextQuestID})�� ã�� �� �����ϴ�.");
            }
        }
        Debug.Log($"[QuestManager] ���� ���� �Ϸ�: {questData.questTitle}");
    }

    // ���� ����
    private void GrantRewards(QuestReward reward)
    {
        var player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("[QuestManager] �÷��̾� ��ü�� ã�� �� �����ϴ�.");
            return;
        }

        var status = player.GetComponent<PlayerStatus>();
        if (status != null)
        {
            status.GainEXP(reward.experience);
        }
        else
        {
            Debug.LogWarning("[QuestManager] PlayerStatus ������Ʈ�� ã�� �� �����ϴ�.");
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddGold(reward.gold);  // �Ʒ��� ���� �߰� �ʿ�
            foreach (var item in reward.items)
            {
                InventoryManager.Instance.AddItem(item.item, item.amount);
            }
        }
        else
        {
            Debug.LogWarning("[QuestManager] InventoryManager.Instance�� �������� �ʽ��ϴ�.");
        }
    }

    // ���� Ȯ��
    public PlayerQuestProgress GetProgress(int questID)
    {
        return activeQuests.Find(q => q.questData.questID == questID);
    }

    public bool IsQuestActive(int questID)
    {
        return activeQuests.Exists(q => q.questData.questID == questID);
    }

    public List<PlayerQuestProgress> GetActiveQuests()
    {
        return activeQuests;
    }
}