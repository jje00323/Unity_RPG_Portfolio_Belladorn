using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillSlotDragHandler : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static SkillInfo draggedSkill;

    public void OnBeginDrag(PointerEventData eventData)
    {
        SkillSlotUI slotUI = GetComponent<SkillSlotUI>();
        if (slotUI == null) return;

        draggedSkill = slotUI.GetCurrentSkill();

        //  �巡�� ���� �� ���׷��̵� UI�� ���� ���� ���
        SkillUpgradeUI.Instance.SetCurrentSlot(slotUI);

        SkillUpgradeUI.Instance.ShowSkillDetail(draggedSkill.originalSkill ?? draggedSkill, slotUI);

        if (draggedSkill != null)
        {
            DragIconUI.Instance.Show(draggedSkill.skillIcon);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Optional: ������ �������� DragIconUI���� ó�� ��
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        draggedSkill = null;
        DragIconUI.Instance.Hide();
    }
}