using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillUpgradeUI : MonoBehaviour
{
    public static SkillUpgradeUI Instance;

    [Header("���� ��ų ����")]
    public Image skillImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI featureText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI cooldownText;
    public TextMeshProUGUI manaText;

    [Header("���׷��̵� ��ư��")]
    public Button originalButton;
    public Button upgrade1Button;
    public Button upgrade2Button;
    public Button upgrade3Button;

    private SkillInfo baseSkill;
    private SkillSlotUI currentSlot;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowSkillDetail(SkillInfo skill, SkillSlotUI slot)
    {
        baseSkill = skill;
        currentSlot = slot;

        UpdateDescription(slot.GetCurrentSkill());

        var upgradeData = SkillUpgradeManager.Instance.GetUpgradeDataFor(skill);

        if (upgradeData == null)
        {
            Debug.LogWarning($"[���׷��̵� ������ ����] {skill.skillName}");
            AssignUpgradeButton(originalButton, skill, true);
            AssignUpgradeButton(upgrade1Button, null);
            AssignUpgradeButton(upgrade2Button, null);
            AssignUpgradeButton(upgrade3Button, null);
            return;
        }

        AssignUpgradeButton(originalButton, skill, true);
        AssignUpgradeButton(upgrade1Button, GetUpgradeOption(upgradeData, 0));
        AssignUpgradeButton(upgrade2Button, GetUpgradeOption(upgradeData, 1));
        AssignUpgradeButton(upgrade3Button, GetUpgradeOption(upgradeData, 2));
    }

    private SkillInfo GetUpgradeOption(SkillUpgradeData data, int index)
    {
        if (data.upgradeOptions != null && data.upgradeOptions.Length > index)
            return data.upgradeOptions[index];
        return null;
    }

    private void AssignUpgradeButton(Button btn, SkillInfo skill, bool isOriginal = false)
    {
        btn.onClick.RemoveAllListeners();

        if (skill == null)
        {
            btn.interactable = false;
            btn.GetComponentInChildren<TextMeshProUGUI>().text = "����";
        }
        else
        {
            btn.interactable = true;
            btn.GetComponentInChildren<TextMeshProUGUI>().text = skill.skillName;

            btn.onClick.AddListener(() =>
            {
                if (currentSlot != null)
                {
                    if (isOriginal)
                    {
                        currentSlot.ResetToOriginal();
                        UpdateDescription(baseSkill);
                    }
                    else
                    {
                        currentSlot.UpgradeSkill(skill);
                        UpdateDescription(skill);

                        //  ���⿡ SkillEquipSlotUI �ݿ� ���� �߰�
                        string equippedSlotKey = SkillEquipManager.Instance.FindEquippedSlotKey(baseSkill);
                        if (!string.IsNullOrEmpty(equippedSlotKey))
                        {
                            SkillEquipManager.Instance.EquipSkill(equippedSlotKey, skill);

                            // ���� UI ����
                            SkillEquipSlotUI[] equipSlots = GameObject.FindObjectsOfType<SkillEquipSlotUI>();
                            foreach (var slot in equipSlots)
                            {
                                if (slot.name.Contains(equippedSlotKey) || slot.slotKey == equippedSlotKey)
                                {
                                    slot.SetSkillIcon(skill);
                                    Debug.Log($"[SkillUpgradeUI] {baseSkill.skillName} �� {skill.skillName} ���� {slot.slotKey} ��ü �Ϸ�");
                                    break;
                                }
                            }
                        }
                    }
                }
            });
        }
    }

    private void UpdateDescription(SkillInfo skill)
    {
        skillImage.sprite = skill.skillIcon;
        nameText.text = skill.skillName;
        featureText.text = skill.Feature;
        descriptionText.text = skill.description;
        cooldownText.text = $"���� ���ð�: {skill.cooldown}��";
        manaText.text = $"MP: {skill.manaCost}";
    }

    public void SetCurrentSlot(SkillSlotUI slot)
    {
        currentSlot = slot;
    }
}