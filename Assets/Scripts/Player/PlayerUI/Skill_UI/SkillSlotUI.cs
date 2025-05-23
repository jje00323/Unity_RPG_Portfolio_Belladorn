using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SkillSlotUI : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI Components")]
    [SerializeField] private Image skillIcon;
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI featureText;
    [SerializeField] private TextMeshProUGUI currentLevel;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private GameObject lockOverlay;

    public SkillInfo skillData;  // ���� ���� ���� ��ų
    public SkillInfo baseSkill;  // ���� ��ų (���׷��̵� ����)

    public static SkillInfo draggedSkill;
    public void SetSlot(SkillInfo skill)
    {
        skillData = skill;
        baseSkill = skill.originalSkill == null ? skill : skill.originalSkill;

        UpdateUI();

        // Ŭ�� �̺�Ʈ ���� (�ߺ� ����)
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClickSlot);
        }
    }

    public SkillInfo GetCurrentSkill() => skillData;

    public void UpdateUI()
    {
        if (skillData == null) return;

        skillIcon.sprite = skillData.skillIcon;
        skillNameText.text = skillData.skillName;
        featureText.text = skillData.Feature;
        currentLevel.text = $"{skillData.currentLevel}";

        //  ��� ���� �ð�ȭ
        if (lockOverlay != null)
            lockOverlay.SetActive(skillData.currentLevel <= 0);

        //  ��� ǥ��
        int nextLevel = skillData.currentLevel + 1;
        costText.text = (nextLevel > skillData.maxLevel)
            ? "  �ִ뷹��"
            : $"������ ���:{nextLevel}";
    }

    public void LevelUp()
    {
        if (skillData == null || skillData.currentLevel >= skillData.maxLevel)
            return;

        int cost = skillData.currentLevel + 1;

        if (!SkillPointManager.Instance.TrySpend(cost))
        {
            Debug.LogWarning($"��ų ����Ʈ ����: {cost} �ʿ�");
            return;
        }

        skillData.SyncLevelRecursive(skillData.currentLevel + 1);
        UpdateUI();
        SkillUIManager.Instance.UpdateSkillPointText();
    }

    public void LevelDown()
    {
        if (skillData == null || skillData.currentLevel <= 1) return;

        int refund = skillData.currentLevel; // ���緹�� �� ���� �� ����+1�̴ϱ�

        skillData.SyncLevelRecursive(skillData.currentLevel - 1);
        SkillPointManager.Instance.Refund(refund);

        UpdateUI();
        SkillUIManager.Instance.UpdateSkillPointText();
    }

    public void UpgradeSkill(SkillInfo upgraded)
    {
        skillData = upgraded;
        UpdateUI();
        SkillUpgradeManager.Instance.SetCurrentUpgrade(baseSkill, upgraded);

        //  SkillEquipManager���� ���� Ű�� ã�� ���׷��̵� ����
        string slotKey = SkillEquipManager.Instance.FindEquippedSlotKey(baseSkill);
        if (!string.IsNullOrEmpty(slotKey))
        {
            SkillEquipManager.Instance.EquipSkill(slotKey, upgraded);
            Debug.Log($"[SkillSlotUI] ���� {slotKey}�� ��ų�� {upgraded.skillName}���� ���׷��̵� �ݿ���");
        }
    }



    public void ResetToOriginal()
    {
        skillData = baseSkill;
        UpdateUI();
    }

    public void OnClickSlot()
    {
        if (SkillUpgradeUI.Instance == null)
        {
            Debug.LogWarning("[SkillSlotUI] SkillUpgradeUI�� ���� �ʱ�ȭ���� �ʾҽ��ϴ�.");
            return;
        }

        SkillUpgradeUI.Instance.ShowSkillDetail(baseSkill, this);
    }

    //  ������� �巡�� ���� �������̽� ���� 
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (skillData == null || skillData.currentLevel <= 0)
        {
            Debug.Log("[SkillSlotUI] ��ݵ� ��ų�� �巡���� �� �����ϴ�.");
            return;
        }

        draggedSkill = skillData;
        if (draggedSkill != null)
        {
            DragIconUI.Instance.Show(draggedSkill.skillIcon);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // ���콺�� ������� �Ϸ��� DragIconUI�� Update�� ó�� ���̾�� ��
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        draggedSkill = null;
        DragIconUI.Instance.Hide();
    }
}