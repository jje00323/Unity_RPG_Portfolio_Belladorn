using UnityEngine;

public class BossEnemyStatus : CharacterStatus
{
    [Header("���� ����")]
    public Transform target;
    [Header("���� �ɷ�ġ")]
    public float moveSpeed;

    [Header("���� ����")]
    public int lastAttackIndex = -1; // ���Ϻ� Idle ������ ������

    //private bool cutsceneTriggered = false;


    public BossEnemyData bossData { get; private set; }

    public void Setup(BossEnemyData data)
    {
        bossData = data;
        maxHP = data.maxHP;
        currentHP = maxHP;
        moveSpeed = data.moveSpeed;
        // attackPower �� �߰� �ʿ�� ��������
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
    }

    public override void TakeDamage(float damage)
    {
        currentHP = Mathf.Clamp(currentHP - damage, 0, maxHP);
        Debug.Log($"[BossEnemyStatus] {damage} ������ �� ���� ü��: {currentHP}");

        int totalBars = bossData.hpBarCount;
        float segmentHP = maxHP / totalBars;
        int currentBarIndex = Mathf.FloorToInt(currentHP / segmentHP);

        // �ƾ� ���� (ü�� 50% ���� + �̹ߵ�)
        if (!GetComponent<BossEnemyFSM>().cutsceneTriggered &&
            currentHP / maxHP <= 0.5f)
        {
            var fsm = GetComponent<BossEnemyFSM>();

            if (!fsm.cutsceneTriggered && !fsm.cutsceneReserved && currentHP / maxHP <= 0.5f)
            {
                fsm.cutsceneReserved = true;
                Debug.Log("[BossEnemyStatus] �ƾ� ����� (ü�� 50% ����)");
            }
        }



        if (IsDead)
        {
            OnDeath();
            return;
        }
    }

    protected override void OnDeath()
    {
        GetComponent<BossEnemyFSM>()?.Die();
    }

}