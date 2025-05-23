using UnityEngine;

public enum ConsumableEffectType
{
    None,
    HealHP,
    RestoreMana,
    BuffAttack,
    BuffDefense,
    RegenHP,       // �ʴ� ü�� ȸ��
    RegenMP        // �ʴ� ���� ȸ��
}

[CreateAssetMenu(fileName = "NewConsumable", menuName = "Inventory/Consumable Data")]
public class ConsumableData : ItemData
{
    [Header("�Ҹ�ǰ ȿ��")]
    public ConsumableEffectType effectType;
    public int amount;
    public float duration;

    /// <summary>
    /// ȿ�� ����
    /// </summary>
    public void ApplyEffect(PlayerStatus playerStatus)
    {
        switch (effectType)
        {
            case ConsumableEffectType.HealHP:
                playerStatus.Heal(amount);
                break;

            case ConsumableEffectType.RestoreMana:
                playerStatus.UseMana(-amount);
                break;

            case ConsumableEffectType.BuffAttack:
                playerStatus.ApplyTemporaryModifier(new StatModifier(StatType.Attack, amount, true), duration);
                break;

            case ConsumableEffectType.BuffDefense:
                playerStatus.ApplyTemporaryModifier(new StatModifier(StatType.Defense, amount, true), duration);
                break;

            case ConsumableEffectType.RegenHP:
                playerStatus.StartHPRegen(amount, duration);
                break;

            case ConsumableEffectType.RegenMP:
                playerStatus.StartMPRegen(amount, duration);
                break;

            default:
                Debug.LogWarning($"[Consumable] {effectType} ȿ�� �̱���");
                break;
        }
    }


}