//  SkillUpgradeManager.cs ����
using UnityEngine;
using System.Collections.Generic;

public class SkillUpgradeManager : MonoBehaviour
{
    public static SkillUpgradeManager Instance;

    [SerializeField] private List<SkillUpgradeData> allUpgradeData;

    //  ���� ���׷��̵� ���� ������ ��
    private Dictionary<SkillInfo, SkillInfo> upgradeMap = new();

    private void Awake()
    {
        Instance = this;
    }

    public SkillUpgradeData GetUpgradeDataFor(SkillInfo baseSkill)
    {
        if (baseSkill == null) return null;
        return allUpgradeData.Find(data => data.skillName == baseSkill.skillName);
    }

    public SkillUpgradeData GetUpgradeDataFor(string baseSkillName)
    {
        return allUpgradeData.Find(data => data.skillName == baseSkillName);
    }

    public void AutoLinkUpgradeToBase(JobSkillData[] allJobSkills)
    {
        foreach (var jobData in allJobSkills)
        {
            foreach (var baseSkill in jobData.skills)
            {
                SkillUpgradeData upgradeData = GetUpgradeDataFor(baseSkill);
                if (upgradeData == null || upgradeData.upgradeOptions == null) continue;

                foreach (var upgraded in upgradeData.upgradeOptions)
                {
                    upgraded.originalSkill = baseSkill;
                }
            }
        }
    }

    //  ���׷��̵� ���� ���
    public void SetCurrentUpgrade(SkillInfo original, SkillInfo upgraded)
    {
        if (upgradeMap.ContainsKey(original))
            upgradeMap[original] = upgraded;
        else
            upgradeMap.Add(original, upgraded);
    }

    //  ���� ���õ� ���� ��ȯ
    public SkillInfo GetCurrentVersionOf(SkillInfo baseSkill)
    {
        if (upgradeMap.TryGetValue(baseSkill, out var upgraded))
            return upgraded;
        return baseSkill;
    }

    //  ���׷��̵� �ʱ�ȭ (��: ���� ��)
    public void ClearUpgrades()
    {
        upgradeMap.Clear();
    }


}
