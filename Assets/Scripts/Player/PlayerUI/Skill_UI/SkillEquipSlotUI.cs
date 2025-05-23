using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SkillEquipSlotUI : MonoBehaviour, IDropHandler
{
    [Header("���� Ű ���� (��: Q, W, E, R)")]
    [SerializeField] public string slotKey;

    [Header("UI Components")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI keyText;

    private void Start()
    {
        ClearSlot();
        if (keyText != null)
            keyText.text = slotKey;
    }

    public void OnDrop(PointerEventData eventData)
    {
        SkillInfo draggedSkill = SkillSlotDragHandler.draggedSkill;

        if (draggedSkill == null)
        {
            Debug.LogWarning("[SkillEquipSlotUI] draggedSkill�� null�Դϴ�.");
            return;
        }

        if (SkillEquipManager.Instance == null)
        {
            Debug.LogError("[SkillEquipSlotUI] SkillEquipManager.Instance�� null�Դϴ�.");
            return;
        }

        foreach (var pair in SkillEquipManager.Instance.GetAllEquippedSkills())
        {
            var equipped = pair.Value;

            bool isSameLine =
                equipped == draggedSkill ||
                equipped.originalSkill == draggedSkill ||
                draggedSkill.originalSkill == equipped;

            if (isSameLine)
            {
                Debug.LogWarning($"[SkillEquipSlotUI] �迭 �ߺ�: {draggedSkill.skillName}�� �̹� ���� {pair.Key}�� ������");
                return; // ���� ����
            }
        }

        // ���� ���
        SkillEquipManager.Instance.EquipSkill(slotKey, draggedSkill);
        SetSkillIcon(draggedSkill);

        Debug.Log($"[SkillEquipSlotUI] [{slotKey}] ���Կ� {draggedSkill.skillName} ���� �Ϸ�");
    }

    public void SetSkillIcon(SkillInfo skill)
    {
        iconImage.sprite = skill.skillIcon;
        iconImage.enabled = true;
    }

    public void ClearSlot()
    {
        iconImage.sprite = null;
        iconImage.enabled = false;
    }
}