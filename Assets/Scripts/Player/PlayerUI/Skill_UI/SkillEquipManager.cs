using UnityEngine;
using System.Collections.Generic;

public class SkillEquipManager : MonoBehaviour
{
    public static SkillEquipManager Instance { get; private set; }

    private Dictionary<string, SkillInfo> equippedSkills = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void EquipSkill(string key, SkillInfo skill)
    {
        //  �ߺ� �迭 �˻�: �ڱ� ������ ����
        foreach (var pair in equippedSkills)
        {
            if (pair.Key == key) continue; // �ڱ� ������ ����

            bool isSameLine =
                pair.Value == skill ||
                pair.Value.originalSkill == skill ||
                skill.originalSkill == pair.Value;

            if (isSameLine)
            {
                Debug.LogWarning($"[SkillEquipManager] {skill.skillName}�� �̹� ����({pair.Key})�� �����Ǿ� �ֽ��ϴ�. �ߺ� �Ұ�.");
                return;
            }
        }

        //  ���� ����
        equippedSkills[key] = skill;
        skill.skillKey = key;
    }

    public Dictionary<string, SkillInfo> GetAllEquippedSkills()
    {
        return equippedSkills; // ���� Ű �� ��ų
    }


    public SkillInfo GetEquippedSkill(string key)
    {
        if (equippedSkills.TryGetValue(key, out var skill))
            return skill;
        return null;
    }

    public string FindEquippedSlotKey(SkillInfo skill)
    {
        foreach (var kvp in equippedSkills)
        {
            if (kvp.Value == skill || kvp.Value.originalSkill == skill || skill.originalSkill == kvp.Value)
                return kvp.Key;
        }
        return null;
    }


}