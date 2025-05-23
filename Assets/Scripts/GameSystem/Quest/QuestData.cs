using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/QuestData")]


public class QuestData : ScriptableObject
{
    [Header("�⺻ ����")]
    public int questID;
    public string questTitle;
    [TextArea]
    public string questObjective;
    public string questDescription;
    

    public QuestType questType;
    public int levelRequirement = 1;
    public int nextQuestID = -1; // ���� ����Ʈ ID (������ -1)

    [Header("���� ����")]
    public List<QuestCondition> conditions;

    [Header("����")]
    public QuestReward reward;

    [Header("����Ʈ ���� �����̼�")]
    [TextArea]
    public string startNarration; // ����Ʈ ���� �� ��µ� ȥ�㸻 �Ǵ� ����
}