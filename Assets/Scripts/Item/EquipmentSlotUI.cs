using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class EquipmentSlotUI : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("��� ���� ����")]
    public EquipmentType slotType;

    [Header("UI")]
    public Image iconImage;

    private EquipmentData equippedItem;
    private int currentAmount = 1;

    public static EquipmentSlotUI draggedEquipSlot;

    public string itemName;
    public string itemDescription;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("[EquipmentSlotUI] OnDrop ����");

        if (InventorySlotUI.draggedItem == null || InventorySlotUI.draggedSlotUI == null)
        {
            Debug.LogWarning("[EquipmentSlotUI] �巡�׵� �׸��� null�Դϴ�.");
            return;
        }

        if (!(InventorySlotUI.draggedItem is EquipmentData equipment))
        {
            Debug.LogWarning("[EquipmentSlotUI] �巡�׵� �������� EquipmentData�� �ƴմϴ�.");
            return;
        }

        if (equipment.equipmentType != slotType)
        {
            Debug.LogWarning($"[��� ���� ����] {equipment.equipmentType}�� {slotType} ���Կ� ���� ����");
            return;
        }

        Debug.Log($"[��� ���� �õ�] {equipment.itemName} �� {slotType}");

        var oldItem = equippedItem;

        // ��� ����
        SetItem(equipment);
        EquipmentManager.Instance.EquipItem(equipment);

        if (oldItem != null)
        {
            // ��ȯ: ���� ��� ���� �巡���� ���Կ� �ֱ�
            switch (InventorySlotUI.draggedSlotUI)
            {
                case InventorySlotUI invSlot:
                    invSlot.SetItemToSlot(oldItem, 1);
                    break;
                case QuickSlotUI quickSlot:
                    
                    break;
                case EquipmentSlotUI equipSlot:
                    equipSlot.SetItem(oldItem);
                    break;
            }
        }
        else
        {
            // ���â�� ��� �־��� ��� �� ���� ���� ����
            switch (InventorySlotUI.draggedSlotUI)
            {
                case InventorySlotUI invSlot:
                    invSlot.RemoveItemFromSlot();
                    break;
                case QuickSlotUI quickSlot:
                    quickSlot.RemoveItemFromSlot();
                    break;
                case EquipmentSlotUI equipSlot:
                    equipSlot.RemoveItemFromSlot();
                    break;
            }
        }

        InventorySlotUI.draggedItem = null;
        InventorySlotUI.draggedSlotUI = null;
        draggedEquipSlot = null;
        DragIconUI.Instance.Hide();
    }

    public void SetItem(EquipmentData item)
    {
        equippedItem = item;

        if (item != null)
        {
            iconImage.sprite = item.icon;
            iconImage.enabled = true;

            itemName = item.itemName;
            itemDescription = item.description;
        }
        else
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        equippedItem = null;
        iconImage.sprite = null;
        iconImage.enabled = false;
    }

    public EquipmentData GetEquippedItem() => equippedItem;
    public int GetAmount() => currentAmount;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (equippedItem == null) return;

        InventorySlotUI.draggedItem = equippedItem;
        InventorySlotUI.draggedSlotUI = this;
        draggedEquipSlot = this;
        DragIconUI.Instance.Show(equippedItem.icon);
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        InventorySlotUI.draggedItem = null;
        InventorySlotUI.draggedSlotUI = null;
        draggedEquipSlot = null;
        DragIconUI.Instance.Hide();
    }

    public void RemoveItemFromSlot()
    {
        if (equippedItem != null)
        {
            EquipmentManager.Instance.UnequipItem(equippedItem); // ���� ����
        }

        ClearSlot(); // UI ����
    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        if (equippedItem == null) return; //  �������� ���� �� ���� ����

        if (TooltipUI.Instance != null)
            TooltipUI.Instance.Show(itemName, itemDescription);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipUI.Instance != null)
            TooltipUI.Instance.Hide();
    }
}