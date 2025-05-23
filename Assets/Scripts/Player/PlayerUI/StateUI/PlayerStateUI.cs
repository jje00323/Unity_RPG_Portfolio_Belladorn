using UnityEngine;
using TMPro;

public class PlayerStateUI : MonoBehaviour
{
    [Header("���� UI ����")]
    public TextMeshProUGUI maxHPText;
    public TextMeshProUGUI maxMPText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI critRateText;
    public TextMeshProUGUI critDamageText;
    public TextMeshProUGUI healthRegenText;
    public TextMeshProUGUI manaRegenText;

    private PlayerStatus player;


    private PlayerStatus Player => GameObject.FindWithTag("Player")?.GetComponent<PlayerStatus>();
    public static PlayerStateUI Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateStats();
    }

    public void UpdateStats()
    {
        Debug.Log("[PlayerStateUI] UpdateStats() ȣ���");

        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogWarning("[PlayerStateUI] Player ������Ʈ�� ã�� �� �����ϴ�.");
            return;
        }

        var status = playerObj.GetComponent<PlayerStatus>();
        if (status == null)
        {
            Debug.LogWarning("[PlayerStateUI] PlayerStatus ������Ʈ�� �����ϴ�.");
            return;
        }

        Debug.Log($"[PlayerStateUI] ���� ���� �� �� maxHP: {status.maxHP}, ATK: {status.attack}, CritRate: {status.critRate}");

        if (maxHPText == null || attackText == null)
        {
            Debug.LogError("[PlayerStateUI] UI �ؽ�Ʈ ������Ʈ ���� �ȵ�");
            return;
        }

        maxHPText.text = $"{status.maxHP:F0}";
        maxMPText.text = $"{status.maxMP:F0}";
        attackText.text = $"{status.attack:F0}";
        defenseText.text = $"{status.defense:F0}";
        critRateText.text = $"{status.critRate:F0}%";
        critDamageText.text = $"{status.critDamage:F0}%";
        healthRegenText.text = "-";
        manaRegenText.text = "-";
    }
}