using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Helmet,
    Chest,
    Legs,
    Gloves,
    Boots,
    Rune
}

public class EquipmentData : ItemData
{
    [Header("���� ��� ����")]
    public EquipmentType equipmentType;

    [Header("���� ����")]
    public List<JobManager.JobType> allowedJobs;

    [Header("���� ��ȭ")]
    public List<StatModifier> statModifiers = new();

    public virtual List<StatModifier> GetAllStatModifiers()
    {
        return new List<StatModifier>(statModifiers); // �⺻���� ��ȯ
    }

}

