using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("�κ��丮 ����")]
    public int maxSlotCount = 48;
    public List<InventorySlot> slots = new();
    private int gold;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeSlots();
    }

    private void InitializeSlots()
    {
        slots.Clear();
        for (int i = 0; i < maxSlotCount; i++)
        {
            slots.Add(new InventorySlot());
        }
    }

    /// <summary>
    /// �������� �κ��丮�� �߰�
    /// </summary>
    public bool AddItem(ItemData item, int amount)
    {
        Debug.Log($"[AddItem] {item.itemName} x{amount} �߰� �õ�");

        int totalAdded = 0;

        // 1. ���� ������ ���Կ� �켱 �߰�
        foreach (var slot in slots)
        {
            if (slot.item != null && slot.item == item && item.isStackable && slot.quantity < item.maxStack)
            {
                int space = item.maxStack - slot.quantity;
                int addAmount = Mathf.Min(space, amount);
                slot.quantity += addAmount;
                amount -= addAmount;
                totalAdded += addAmount;

                Debug.Log($"[AddItem] ���� ���� ���Կ� {addAmount} �߰��� �� ����: {slot.quantity}");

                if (amount <= 0)
                {
                    InventoryUI.Instance?.RefreshAllSlots(); //  �κ��丮 UI ����
                    RefreshQuickSlots();

                    // ����Ʈ ���� �ݿ�
                    QuestManager.Instance?.UpdateCondition("CollectItem", item.itemName, totalAdded);
                    return true;
                }
            }
        }

        // 2. �� ���Կ� ���� �߰�
        foreach (var slot in slots)
        {
            if (slot.item == null)
            {
                int addAmount = item.isStackable ? Mathf.Min(item.maxStack, amount) : 1;

                slot.item = item;
                slot.quantity = addAmount;
                amount -= addAmount;
                totalAdded += addAmount;

                Debug.Log($"[AddItem] �� ���Կ� {item.itemName} x{addAmount} �߰���");

                if (amount <= 0)
                {
                    InventoryUI.Instance?.RefreshAllSlots(); //  �κ��丮 UI ����
                    RefreshQuickSlots();

                    // ����Ʈ ���� �ݿ�
                    QuestManager.Instance?.UpdateCondition("CollectItem", item.itemName, totalAdded);
                    return true;
                }
            }
        }

        // 3. ������ ���� ������ �ִٸ� ����
        Debug.LogWarning($"[AddItem] ����: {item.itemName} x{amount} ���� �� �κ��丮�� �߰� �Ұ�");

        // �׷��� �Ϻ� �߰��� ��� ���� �ݿ�
        if (totalAdded > 0)
        {
            QuestManager.Instance?.UpdateCondition("CollectItem", item.itemName, totalAdded);
        }

        return false;
    }

    /// <summary>
    /// ������ ����
    /// </summary>
    public void RemoveItem(ItemData targetItem, int amount = 1)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            if (slot.item == targetItem)
            {
                slot.quantity -= amount;

                if (slot.quantity <= 0)
                    slot.Clear();

                InventoryUI.Instance?.RefreshAllSlots(); //  ������ ���� �Ŀ��� UI ����
                RefreshQuickSlots();
                return;
            }
        }
    }

    private void RefreshQuickSlots()
    {
        QuickSlotUI[] quickSlots = GameObject.FindObjectsOfType<QuickSlotUI>();
        foreach (var qs in quickSlots)
        {
            qs.RefreshSlotUI();
        }
    }

    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log($"[Inventory] ��� +{amount} �� �� {gold}");
        // �ʿ� �� UI ���� ���� �߰�
    }

    public int GetGold() => gold;
}