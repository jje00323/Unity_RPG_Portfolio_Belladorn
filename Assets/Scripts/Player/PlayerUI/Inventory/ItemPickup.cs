//using UnityEngine;

//public class ItemPickup : MonoBehaviour
//{
//    public ItemData itemData;
//    public int quantity = 1;

//    private void Start()
//    {
//        var sr = GetComponentInChildren<SpriteRenderer>();
//        if (sr != null && itemData != null && itemData.icon != null)
//        {
//            sr.sprite = itemData.icon;
//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (!other.CompareTag("Player")) return;

//        if (itemData == null)
//        {
//            Debug.LogError("[ItemPickup] itemData�� null�Դϴ�");
//            return;
//        }

//        var inventory = InventoryManager.Instance;
//        var quickSlots = FindObjectsOfType<QuickSlotUI>();

//        //  �����Կ� ������ �Һ� �������� ���� ��� �� ���� ����
//        if (itemData.itemType == ItemType.Consumable)
//        {
//            foreach (var slot in quickSlots)
//            {
//                if (slot.GetItem() == itemData)
//                {
//                    //  �κ��丮���� ���� ������ ���� �� QuickSlotUI�� �ڵ� �ݿ���
//                    InventoryManager.Instance.AddItem(itemData, quantity);

//                    slot.RefreshSlotUI(); // �����Ե� ���� �ݿ�
//                    Debug.Log($"[ItemPickup] ������ ���� ����: {itemData.itemName}");
//                    Destroy(gameObject);
//                    return;
//                }
//            }
//        }

//        //  �κ��丮�� ������ �߰� (�Һ� �������̵� �ƴϵ� �⺻�� �κ��丮)
//        if (inventory != null)
//        {
//            bool added = inventory.AddItem(itemData, quantity);
//            if (added)
//            {
//                Debug.Log($"[ItemPickup] �κ��丮�� �߰�: {itemData.itemName} x{quantity}");
//                var inventoryUI = FindObjectOfType<InventoryUI>();
//                if (inventoryUI != null)
//                    inventoryUI.RefreshAllSlots();

//                Destroy(gameObject);
//            }
//            else
//            {
//                Debug.LogWarning($"[ItemPickup] �߰� ���� - �κ��丮 ����?");
//            }
//        }
//    }
//}