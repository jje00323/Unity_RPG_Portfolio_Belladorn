using UnityEngine;

[CreateAssetMenu(fileName = "SkillUpgradeData", menuName = "Skills/SkillUpgradeData")]
public class SkillUpgradeData : ScriptableObject
{
    public string skillName;                    // ���� ��ų
    public SkillInfo[] upgradeOptions;        // �ִ� 3���� ���׷��̵� �ɼ�

    public bool HasUpgradeOptions() => upgradeOptions != null && upgradeOptions.Length > 0;

    public void SyncLevels(SkillInfo source)
    {
        foreach (var skill in upgradeOptions)
        {
            if (skill != source)
            {
                skill.currentLevel = source.currentLevel;
                skill.maxLevel = source.maxLevel;
            }
        }

        if (source.originalSkill != null)
        {
            source.originalSkill.currentLevel = source.currentLevel;
            source.originalSkill.maxLevel = source.maxLevel;
        }
        else
        {
            foreach (var skill in upgradeOptions)
            {
                if (skill.originalSkill == null)
                    skill.originalSkill = source;
            }
        }
    }
}