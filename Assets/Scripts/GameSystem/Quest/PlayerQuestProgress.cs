using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum QuestState
{
    NotStarted,
    InProgress,
    Completed,
    Rewarded
}
[System.Serializable]
public class PlayerQuestProgress
{
    public QuestData questData;
    public QuestState state;

    // �� ���ǿ� ���� ���� ��ġ (���� ����Ʈ�� �ε����� ��Ī)
    public List<int> conditionProgress = new List<int>();

    public PlayerQuestProgress(QuestData data)
    {
        questData = data;
        state = QuestState.InProgress;
        // ���� ����ŭ �ʱ�ȭ (��� 0���� ����)
        for (int i = 0; i < data.conditions.Count; i++)
        {
            conditionProgress.Add(0);
        }
    }

    // ���� ��ġ ������Ʈ (��: ������ óġ �� 1 ����)
    public void IncrementProgress(int conditionIndex, int amount = 1)
    {
        if (conditionIndex >= 0 && conditionIndex < conditionProgress.Count)
        {
            conditionProgress[conditionIndex] += amount;
        }
    }

    // ����Ʈ�� ��� ������ �����Ǿ����� Ȯ��
    public bool IsAllConditionsMet()
    {
        for (int i = 0; i < questData.conditions.Count; i++)
        {
            if (!questData.conditions[i].IsMet(this))
                return false;
        }
        return true;
    }
}

