using UnityEngine;

[CreateAssetMenu(fileName = "JobSkillData", menuName = "Skills/JobSkillData")]
public class JobSkillData : ScriptableObject
{
    public JobManager.JobType jobType;   // ���� ���� (Warrior, Mage ��)
    public SkillInfo[] skills;           // ������ ���� ��� ��ų


}

[System.Serializable]
public class SkillInfo
{
    public string skillKey;
    public string skillName;
    public Sprite skillIcon;
    public string description;
    public float cooldown;
    public GameObject hitboxPrefab;
    public GameObject effectPrefab;
    public float effectDuration;
    public bool followCaster;

    public AnimationClip skillAnimation;

    public int manaCost;
    public int requiredLevel;
    public int currentLevel = 1;
    public int maxLevel = 5;
    public bool IsUnlocked => currentLevel > 0;
    public string Feature;



    [HideInInspector] public SkillInfo originalSkill; // ���׷��̵� �� ����

    public void SyncLevelRecursive(int newLevel)
    {
        currentLevel = Mathf.Clamp(newLevel, 1, maxLevel);

        if (originalSkill != null)
        {
            originalSkill.currentLevel = currentLevel;
            originalSkill.maxLevel = maxLevel;

            originalSkill.PropagateLevelToUpgrades();
        }
        else
        {
            PropagateLevelToUpgrades();
        }

        var upgradeData = SkillUpgradeManager.Instance?.GetUpgradeDataFor(skillName);
        upgradeData?.SyncLevels(this);
    }

    public void PropagateLevelToUpgrades()
    {
        var upgradeData = SkillUpgradeManager.Instance?.GetUpgradeDataFor(skillName);
        if (upgradeData == null || upgradeData.upgradeOptions == null) return;

        foreach (var upgraded in upgradeData.upgradeOptions)
        {
            upgraded.currentLevel = currentLevel;
            upgraded.maxLevel = maxLevel;
        }
    }
}
