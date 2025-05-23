//using UnityEngine;

//public class ItemEffectProcessor : MonoBehaviour
//{
//    private PlayerStatus player; // �÷��̾� ���� ���� ������Ʈ ����

//    private void Awake()
//    {
//        player = FindObjectOfType<PlayerStatus>();
//        if (player == null)
//            Debug.LogError("[ItemEffectProcessor] PlayerStatus�� ã�� �� �����ϴ�.");
//    }

//    public void UseItem(ConsumableData consumable)
//    {
//        if (consumable == null)
//        {
//            Debug.LogWarning("����� �������� null�Դϴ�.");
//            return;
//        }

//        switch (consumable.effectType)
//        {
//            case ConsumableEffectType.HealHP:
//                player.Heal(consumable.amount);
//                break;

//            case ConsumableEffectType.RestoreMana:
//                player.RestoreMana(consumable.amount);
//                break;

//            case ConsumableEffectType.BuffAttack:
//                player.ApplyBuff(PlayerBuffType.Attack, consumable.amount, consumable.duration);
//                break;

//            case ConsumableEffectType.BuffDefense:
//                player.ApplyBuff(PlayerBuffType.Defense, consumable.amount, consumable.duration);
//                break;

//            case ConsumableEffectType.CureStatusEffect:
//                player.CureStatusEffects();
//                break;

//            default:
//                Debug.Log("Ư��ȿ���� ���� �������Դϴ�.");
//                break;
//        }
//    }
//}