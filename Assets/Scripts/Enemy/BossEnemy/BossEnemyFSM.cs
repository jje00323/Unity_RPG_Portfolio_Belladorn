using UnityEngine;
using UnityEngine.AI; // NavMeshObstacle ����� ���� ���ӽ����̽� �߰�

public class BossEnemyFSM : BaseEnemyFSM
{
    [Header("���� ������")]
    public BossEnemyData bossData;

    [Header("���� ������Ʈ")]
    public BossEnemyStatus bossStatus;
    public BossEnemyPatternController patternController;
    [Header("���� ��Ʈ�ѷ�")]
    public BossEnemyAttackController attackController;
    public NavMeshObstacle navMeshObstacle; // ���� �浹 ���� ó���� ���� �ʵ� �߰�

    [Header("���� ����")]
    public bool lockRotation = false;

    [Header("����")]
    public BossEnemyState currentState;
    public BossEnemyIdleState idleState;
    public BossEnemyMoveState moveState;
    public BossEnemyPatternState patternState;
    public BossEnemyDeadState deadState;
    public BossEnemyCutsceneState cutsceneState;
    public BossEnemyIntroState introState;

    [Header("����׿� �׽�Ʈ ����")]
    public bool useTestPattern = false;
    [Range(0, 7)] public int testPatternIndex = 0;

    [HideInInspector] public bool cutsceneTriggered = false;
    [HideInInspector] public bool cutsceneReserved = false;

    protected override void Awake()
    {
        base.Awake();
        bossStatus = GetComponent<BossEnemyStatus>();
        patternController = GetComponent<BossEnemyPatternController>();
        attackController = GetComponent<BossEnemyAttackController>();
        navMeshObstacle = GetComponent<NavMeshObstacle>(); // NavMeshObstacle ������Ʈ ���� �ʱ�ȭ
    }

    private void Start()
    {
        if (bossData == null)
        {
            Debug.LogError("[BossEnemyFSM] bossData�� �������� �ʾҽ��ϴ�.");
            return;
        }

        // ������ ����
        bossStatus.Setup(bossData);
        Animator.runtimeAnimatorController = bossData.animatorController;

        // ���� �ʱ�ȭ
        idleState = new BossEnemyIdleState(this);
        moveState = new BossEnemyMoveState(this);
        patternState = new BossEnemyPatternState(this);
        deadState = new BossEnemyDeadState(this);
        cutsceneState = new BossEnemyCutsceneState(this);
        introState = new BossEnemyIntroState(this);

        ChangeState(introState);
    }

    private void Update()
    {
        currentState?.Update();
    }

    public void ChangeState(BossEnemyState newState)
    {
        // �ƾ� �߿��� ���� �ٸ� ���·� ��ȯ �Ұ�
        if (currentState == cutsceneState && newState != cutsceneState)
        {
            Debug.LogWarning($"[FSM] �ƾ� �� ���� ��ȯ ���ܵ�: {newState.GetType().Name}");
            return;
        }

        if (currentState == newState) return;

        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public override void Die()
    {
        Debug.Log("[BossEnemyFSM] Die() ȣ��� �� DeadState ��ȯ");
        ChangeState(deadState);
    }

    /// <summary>
    /// ���� �� üũ: ������ �ٶ󺸴� ���⿡ �� �Ǵ� ��ֹ��� ������ �ִ��� Ȯ��
    /// </summary>
    public bool CheckWallNearby()
    {
        float checkDistance = 2.0f;
        Vector3 forward = transform.forward;
        RaycastHit hit;

        //if (Physics.Raycast(transform.position, forward, out hit, checkDistance))
        //{
        //    return hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Obstacle");
        //}

        return false;
    }

    public void EndBossPattern()
    {
        ChangeState(idleState);
    }
}
