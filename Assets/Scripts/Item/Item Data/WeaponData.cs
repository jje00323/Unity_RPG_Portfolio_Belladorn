using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Inventory/Weapon Data")]
public class WeaponData : EquipmentData
{
    [Header("���� ���� ����")]
    public int Attack;
    public int MaxMP;
    

    [Header("�ΰ� ȿ��")]
    public float ManaRegen;
    public float CritRate;
    public float CritDamage;

    public override List<StatModifier> GetAllStatModifiers()
    {
        var all = base.GetAllStatModifiers();
        all.Add(new StatModifier(StatType.Attack, Attack, true));
        all.Add(new StatModifier(StatType.MaxMP, MaxMP, true));
        all.Add(new StatModifier(StatType.ManaRegen, ManaRegen, true));
        all.Add(new StatModifier(StatType.CritRate, CritRate, true));
        all.Add(new StatModifier(StatType.CritDamage, CritDamage, true));
        return all;
    }
}