using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class BossEnemyPatternController : MonoBehaviour
{
    private Dictionary<int, float> cooldownTimers = new Dictionary<int, float>();
    private int lastUsedIndex = -1;

    public BossEnemyFSM bossFSM; // �ν����Ϳ��� ���� ����
    private Coroutine lightningRoutine;

    private readonly float comboMinCooldown = 12f;
    private readonly float comboMaxCooldown = 20f;
    private readonly float farMinCooldown = 16f;
    private readonly float farMaxCooldown = 28f;

    private readonly int[] lightAttacks = { 0, 1, 2 };
    private readonly int[] comboAttacks = { 3, 4, 5 };
    private readonly int[] farAttacks = { 6, 7 };
    private const int wallAttackIndex = 8;

    private void Awake()
    {
        foreach (int index in comboAttacks)
            cooldownTimers[index] = -Mathf.Infinity;
        foreach (int index in farAttacks)
            cooldownTimers[index] = -Mathf.Infinity;
    }

    public bool IsFarPatternReady()
    {
        foreach (var index in farAttacks)
        {
            if (Time.time >= cooldownTimers[index])
                return true;
        }
        return false;
    }

    public int GetPattern(float distanceToPlayer, bool isNearWall)
    {
        if (isNearWall)
            return wallAttackIndex;

        List<int> available = new List<int>();

        // 1. Light ���� (0~3m)
        if (distanceToPlayer <= 3f)
        {
            foreach (int i in lightAttacks)
            {
                if (i != lastUsedIndex)
                    available.Add(i);
            }
        }

        // 2. Combo ���� (0~6m)
        if (distanceToPlayer <= 6f)
        {
            foreach (int i in comboAttacks)
            {
                if (i != lastUsedIndex && Time.time >= cooldownTimers[i])
                    available.Add(i);
            }
        }

        // 3. Spin ���� (6~8m)
        if (distanceToPlayer > 6f && distanceToPlayer <= 8f)
        {
            if (Time.time >= cooldownTimers[7])
                available.Add(7);
        }

        // 4. Jump ���� (8~12m)
        if (distanceToPlayer > 8f && distanceToPlayer <= 12f)
        {
            if (Time.time >= cooldownTimers[6])
                available.Add(6);
        }

        if (available.Count > 0)
            return SelectRandomAndRecord(available, useCooldown: true, isCombo: false);
        else
            return -1;
    }

    private int SelectRandomAndRecord(List<int> list, bool useCooldown, bool isCombo)
    {
        if (list.Count == 0) return -1;

        int selected = list[Random.Range(0, list.Count)];
        lastUsedIndex = selected;

        if (useCooldown && selected >= 3)
        {
            float cooldown = (selected >= 6) ?
                Random.Range(farMinCooldown, farMaxCooldown) :
                Random.Range(comboMinCooldown, comboMaxCooldown);

            cooldownTimers[selected] = Time.time + cooldown;
        }

        return selected;
    }

    // ���� �ε����� ��ȯ�ϰ� ����ϴ� ���� �޼���
    private int SelectSingleAndRecord(int index)
    {
        lastUsedIndex = index;

        // ��Ÿ�� ���� ����� ���
        if (index == 6 || index == 7)
        {
            float cooldown = Random.Range(farMinCooldown, farMaxCooldown);
            cooldownTimers[index] = Time.time + cooldown;
        }

        return index;
    }
    public bool HasAvailablePattern(float distanceToPlayer, bool isNearWall)
    {
        if (isNearWall)
            return true; // �� ������ �׻� ��� ����

        // 1. Light ���� (0~3m)
        if (distanceToPlayer <= 3f)
        {
            foreach (int i in lightAttacks)
            {
                if (i != lastUsedIndex)
                    return true;
            }
        }

        // 2. Combo ���� (0~6m)
        if (distanceToPlayer <= 6f)
        {
            foreach (int i in comboAttacks)
            {
                if (i != lastUsedIndex && Time.time >= cooldownTimers[i])
                    return true;
            }
        }

        // 3. Spin ���� (6~8m)
        if (distanceToPlayer > 6f && distanceToPlayer <= 8f)
        {
            if (Time.time >= cooldownTimers[7])
                return true;
        }

        // 4. Jump ���� (8~12m)
        if (distanceToPlayer > 8f && distanceToPlayer <= 12f)
        {
            if (Time.time >= cooldownTimers[6])
                return true;
        }

        return false;
    }
    public void EndBossPattern()
    {
        if (bossFSM != null)
        {
            Debug.Log("[�ִϸ��̼� �̺�Ʈ] EndBossPattern ȣ���");
            bossFSM.Animator.speed = 1f;

            if (bossFSM.cutsceneReserved && !bossFSM.cutsceneTriggered)
            {
                bossFSM.cutsceneReserved = false;
                bossFSM.cutsceneTriggered = true;
                bossFSM.ChangeState(bossFSM.cutsceneState);
                Debug.Log("[�ִϸ��̼� �̺�Ʈ] �ƾ� ���·� ��ȯ��");
            }
            else
            {
                bossFSM.EndBossPattern(); // IdleState�� ����
            }
        }
        else
        {
            Debug.LogWarning("BossEnemyFSM�� ������� �ʾҽ��ϴ�.");
        }
    }

    public void StartPhase2LightningPattern()
    {
        if (lightningRoutine == null)
            Debug.Log("[��������] Phase2 Lightning Pattern ���۵� (�ڷ�ƾ ����)");
            lightningRoutine = StartCoroutine(Phase2LightningRoutine());
    }


    private IEnumerator Phase2LightningRoutine()
    {
        const int lightningIndex = 8;

        while (true)
        {
            float delay = Random.Range(5f, 8f);
            Debug.Log($"[��������] {delay:F2}�� �� ���� {Random.Range(2, 5)}�� ��ȯ ����");

            yield return new WaitForSeconds(delay);

            int count = Random.Range(3, 6);
            List<Vector3> usedPositions = new List<Vector3>();

            for (int i = 0; i < count; i++)
            {
                Vector3 pos = GetValidLightningPosition(usedPositions, minDistance: 2f);
                usedPositions.Add(pos);

                Debug.Log($"[��������] ���� #{i + 1} ��ġ: {pos}");
                bossFSM.attackController.SpawnLightningAttackGroup(pos, lightningIndex);
            }
        }
    }
    private Vector3 GetValidLightningPosition(List<Vector3> existingPositions, float minDistance = 2f, int maxAttempts = 10)
    {
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector2 randCircle = Random.insideUnitCircle * 8f;
            Vector3 candidate = bossFSM.transform.position + new Vector3(randCircle.x, 0f, randCircle.y);

            bool tooClose = false;
            foreach (var existing in existingPositions)
            {
                if (Vector3.Distance(candidate, existing) < minDistance)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
                return candidate;
        }

        // ��õ� ���� �� ������ ���ǰ� ��ȯ
        Vector2 fallback = Random.insideUnitCircle * 6f;
        return bossFSM.transform.position + new Vector3(fallback.x, 0f, fallback.y);
    }

    //private Vector3 GetRandomPositionAroundBoss()
    //{
    //    Vector2 randCircle = Random.insideUnitCircle * 6f;
    //    return bossFSM.transform.position + new Vector3(randCircle.x, 0f, randCircle.y);
    //}
}

