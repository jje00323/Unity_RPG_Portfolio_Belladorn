using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossEnemyHealthUI : MonoBehaviour
{
    [Header("���� ����")]
    public BossEnemyStatus bossStatus;

    [Header("ü�¹� �̹���")]
    [SerializeField] private Image frontFill;
    [SerializeField] private Image backFill;
    [SerializeField] private Image background;

    [Header("���� ü�� ǥ��")]
    [SerializeField] private TextMeshProUGUI hpBarCountText;    // ���� ü�� �� ��
    [SerializeField] private TextMeshProUGUI hpValueText;       // ���� ü�� / �ִ� ü��
    [SerializeField] private TextMeshProUGUI bossLevelText;     // ���� ����

    [Header("���� ����")]
    public Color[] hpColors;

    private float segmentHP;
    private int totalBars;
    private float displayedRatio = 1f;
    private bool initialized = false;

    private void Update()
    {
        if (!initialized)
        {
            Debug.Log("[BossHPUI] �ʱ�ȭ �õ� ��...");
            if (bossStatus == null)
            {
                Debug.LogError("[BossHPUI] bossStatus�� ������� �ʾҽ��ϴ�.");
                return;
            }

            if (bossStatus.bossData == null)
            {
                Debug.LogWarning("[BossHPUI] bossStatus�� ������ bossData�� ���� null�Դϴ�.");
                return;
            }

            totalBars = bossStatus.bossData.hpBarCount;
            segmentHP = bossStatus.maxHP / totalBars;
            initialized = true;

            Debug.Log($"[BossHPUI] �ʱ�ȭ �Ϸ�: segmentHP = {segmentHP}, totalBars = {totalBars}");
        }

        float currentHP = bossStatus.currentHP;
        int currentBarIndex = Mathf.FloorToInt(currentHP / segmentHP);
        float currentBarHP = currentHP % segmentHP;

        if (Mathf.Approximately(currentBarHP, 0) && currentHP > 0)
        {
            currentBarIndex -= 1;
            currentBarHP = segmentHP;
        }

        float targetRatio = currentBarHP / segmentHP;
        displayedRatio = Mathf.Lerp(displayedRatio, targetRatio, Time.deltaTime * 10f);
        frontFill.fillAmount = displayedRatio;
        backFill.fillAmount = (currentBarIndex > 0) ? 1f : 0f;

        // ü�� �� �� ǥ�� (���� �� �� ����)
        int remainingBars = Mathf.CeilToInt(currentHP / segmentHP);
        hpBarCountText.text = $"x{remainingBars}";

        // ü�� ��ġ ǥ��
        hpValueText.text = $"{Mathf.CeilToInt(currentHP)} / {Mathf.CeilToInt(bossStatus.maxHP)}";

        // ���� ���� ǥ��
        if (bossStatus.bossData != null)
        {
            bossLevelText.text = $"Lv.{bossStatus.bossData.bossLevel}";
        }

        // ���� ó��
        if (hpColors != null && hpColors.Length > 0)
        {
            // 1. segmentHP = maxHP / totalBars �� �ʱ�ȭ �� �̹� ����
            // 2. ���� ü�� ���� �� ��° segment�� �ش��ϴ��� (0���� ����)
            int currentSegment = Mathf.FloorToInt((bossStatus.maxHP - currentHP) / segmentHP);

            // 3. ��ȯ�Ǵ� ���� �ε��� ���
            int colorIndex = currentSegment % hpColors.Length;
            int nextColorIndex = (colorIndex + 1) % hpColors.Length;

            // 4. ���� ����
            frontFill.color = hpColors[colorIndex];
            backFill.color = hpColors[nextColorIndex];
        }

        //Debug.Log($"[BossHPUI] FrontFill.fillAmount = {frontFill.fillAmount}, color = {frontFill.color}");

        //Debug.Log($"[BossHPUI] HP: {currentHP} / Segment: {segmentHP}, Bars: {currentBarIndex}");
    }

    public void SetBoss(BossEnemyStatus status)
    {
        bossStatus = status;
        initialized = false; // �ٽ� �ʱ�ȭ �����ϵ���
    }
}
