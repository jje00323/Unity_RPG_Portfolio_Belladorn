using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : CharacterStatus
{

    [Header("UI ����")]
    public PlayerStateUI stateUI;


    private JobManager.JobType currentJob;


    public float maxMP;
    public float currentMP;

    public float maxEXP;
    public float currentEXP;

    public int level;

    
    public float critRate;
    public float critDamage;
    public float moveSpeed;

    public PlayerUI playerUI;

    private readonly List<Coroutine> activeBuffs = new();
    private Coroutine hpRegenCoroutine;
    private Coroutine mpRegenCoroutine;
    private Dictionary<StatType, Coroutine> activeBuffDict = new();



    void Start()
    {
        if (stateUI == null)
        {
            // ���� ���� ��ġ (��Ȱ��ȭ �����ؼ� ã��)
            var canvasRoot = GameObject.Find("UIRoot"); // �̸��� ���� UI ��Ʈ GameObject �̸����� �ٲټ���
            if (canvasRoot != null)
                stateUI = canvasRoot.GetComponentInChildren<PlayerStateUI>(true);

            if (stateUI != null)
                Debug.Log("[PlayerStatus] stateUI ���� ���� ����");
            else
                Debug.LogWarning("[PlayerStatus] PlayerStateUI�� ã�� �� �����ϴ�.");
        }

        UpdateAllUI();
    }

    public void ApplyJobStats(JobStatusData data)
    {
        level = 1;

        maxHP = data.baseHP;
        maxMP = data.baseMP;
        currentHP = maxHP;
        currentMP = maxMP;

       
        attack = data.baseAttack;
        defense = data.baseDefense;

        critRate = data.critical;
        critDamage = data.critical_Damage;


        UpdateAllUI();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        playerUI.UpdateHP(currentHP, maxHP);
        Debug.Log($"player ���� ü��: {currentHP}");
    }

    public override void Heal(float amount)
    {
        base.Heal(amount); // ���� Ŭ���� ��� ���
        playerUI?.UpdateHP(currentHP, maxHP); // UI ������Ʈ �� �÷��̾� ���� ó��
    }

    public void UseMana(float amount)
    {
        currentMP = Mathf.Clamp(currentMP - amount, 0, maxMP);
        playerUI.UpdateMP(currentMP, maxMP);
    }

    public void GainEXP(float amount)
    {
        currentEXP += amount;
        var statData = JobManager.Instance.GetStatData(currentJob); // ���� ������ ���� ���� ��������

        while (currentEXP >= maxEXP)
        {
            if (statData != null)
            {
                GainLevel(statData);
            }
            else
            {
                Debug.LogWarning($"[PlayerStatus] {currentJob}�� JobStatusData�� �������� �ʽ��ϴ�.");
                break;
            }
        }

        playerUI.UpdateEXP(currentEXP, maxEXP);
    }

    public void GainLevel(JobStatusData statData)
    {
        int previousLevel = level;

        currentEXP -= maxEXP;
        level++;

        maxHP += statData.hpPerLevel;
        maxMP += statData.mpPerLevel;
        // �ʿ� �� attack, defense � ����
        currentHP = maxHP;
        currentMP = maxMP;

        if (SkillPointManager.Instance != null)
        {
            SkillPointManager.Instance.GainPointByLevel(previousLevel);
            Debug.Log($"[������] Lv.{previousLevel} �� Lv.{level} / ��ų����Ʈ +{previousLevel}");
        }
        else
        {
            Debug.LogWarning("[������] SkillPointManager�� �ʱ�ȭ���� �ʾҽ��ϴ�.");
        }

        UpdateAllUI();
    }

    public void ApplyTemporaryModifier(StatModifier modifier, float duration)
    {
        if (activeBuffDict.TryGetValue(modifier.type, out Coroutine existingBuff))
        {
            Debug.Log($"[���� ��ø ����] �̹� {modifier.type} ������ ����Ǿ� ����");
            return;
        }

        StatusEffectApplier.ApplyStatModifiers(this, new List<StatModifier> { modifier });

        Coroutine newBuff = StartCoroutine(RemoveModifierAfterTime(modifier, duration));
        activeBuffs.Add(newBuff);
        activeBuffDict[modifier.type] = newBuff;
        LogCurrentStats();
    }

    private IEnumerator RemoveModifierAfterTime(StatModifier modifier, float duration)
    {
        yield return new WaitForSeconds(duration);

        var inverse = new StatModifier(modifier.type, -modifier.value, modifier.isFlat);
        StatusEffectApplier.ApplyStatModifiers(this, new List<StatModifier> { inverse });

        activeBuffDict.Remove(modifier.type);
        Debug.Log($"[���� ����] {modifier.type} ���� �����");
    }

    public void ClearAllBuffs()
    {
        foreach (var buff in activeBuffs)
        {
            if (buff != null) StopCoroutine(buff);
        }
        activeBuffs.Clear();
    }

    

    public void StartHPRegen(float amountPerSecond, float duration)
    {
        if (hpRegenCoroutine != null) StopCoroutine(hpRegenCoroutine);
        hpRegenCoroutine = StartCoroutine(RegenCoroutine(amountPerSecond, duration, isHP: true));
    }

    public void StartMPRegen(float amountPerSecond, float duration)
    {
        if (mpRegenCoroutine != null) StopCoroutine(mpRegenCoroutine);
        mpRegenCoroutine = StartCoroutine(RegenCoroutine(amountPerSecond, duration, isHP: false));
    }

    private IEnumerator RegenCoroutine(float amountPerSecond, float duration, bool isHP)
    {
        float elapsed = 0f;
        float interval = 1f;

        while (elapsed < duration)
        {
            if (isHP)
                Heal(amountPerSecond);
            else
                UseMana(-amountPerSecond);

            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }

        Debug.Log($"[���� ȸ�� ����] {(isHP ? "HP" : "MP")} +{amountPerSecond}/��, {duration}��");
    }

    public void UpdateAllUI()
    {
        Debug.Log("[UpdateAllUI] ȣ���");

        playerUI.UpdateHP(currentHP, maxHP);
        playerUI.UpdateMP(currentMP, maxMP);
        playerUI.UpdateEXP(currentEXP, maxEXP);
        playerUI.UpdateLevel(level);

        if (stateUI == null)
        {
            Debug.LogWarning("[UpdateAllUI] stateUI�� null�Դϴ�. UI�� ���ŵ��� �ʽ��ϴ�.");
        }
        else
        {
            Debug.Log("[UpdateAllUI] stateUI ���� Ȯ�ε� �� UpdateStats() ȣ�� �õ�");
            stateUI.UpdateStats();
        }

        Debug.Log($"[���� ����] " +
            $"LV: {level}, " +
            $"HP: {currentHP}/{maxHP}, " +
            $"MP: {currentMP}/{maxMP}, " +
            $"ATK: {attack}, DEF: {defense}, " +
            $"CritRate: {critRate}, CritDmg: {critDamage}");
    }

    protected override void OnDeath()
    {
        Debug.Log("�÷��̾� ��� ó��");
        // �÷��̾� ��� ó�� �߰�
    }

    public void SetJob(JobManager.JobType job)
    {
        currentJob = job;
    }

    private void LogCurrentStats()
    {
        Debug.Log($"[���� ����] HP: {currentHP}/{maxHP}, MP: {currentMP}/{maxMP}");
        // ���⼭ ���ݷ�, ���� � ��� ����
        // ��: Debug.Log($"[���ݷ�] {attack}, [����] {defense}");
    }
}

