using UnityEngine;
using System.Collections;
using System;

public class BossEnemyPatternState : BossEnemyState
{
    private Transform target;

    // �浹 ���ÿ� �ݶ��̴� ����
    private Collider bossCollider;
    private Collider playerCollider;

    private bool isPhase2Started = false;
    //private Coroutine lightningRoutine = null;

    public BossEnemyPatternState(BossEnemyFSM boss) : base(boss) { }

    public override void Enter()
    {
        target = boss.bossStatus.target;
        boss.Animator.SetBool("IsMoving", false);

        if (bossCollider == null)
            bossCollider = boss.GetComponent<CapsuleCollider>();

        if (playerCollider == null)
            playerCollider = GameObject.FindWithTag("Player")?.GetComponent<Collider>();

        if (bossCollider != null && playerCollider != null)
            Physics.IgnoreCollision(bossCollider, playerCollider, true);

        // NavMeshObstacle ��Ȱ��ȭ
        if (boss.navMeshObstacle != null)
        {
            boss.navMeshObstacle.enabled = false;
            Debug.Log("[PatternState] NavMeshObstacle �� Disabled"); // ����� ���
        }
        else
        {
            Debug.LogWarning("[PatternState] NavMeshObstacle ������Ʈ�� �����ϴ�.");
        }

        boss.StartCoroutine(HandlePattern());
    }

    public override void Exit()
    {
        boss.lockRotation = false;

        if (bossCollider != null && playerCollider != null)
            Physics.IgnoreCollision(bossCollider, playerCollider, false);

        // NavMeshObstacle �ٽ� Ȱ��ȭ
        if (boss.navMeshObstacle != null)
            boss.navMeshObstacle.enabled = true;

        // �ƾ� ���� Ȯ��
        if (!boss.cutsceneTriggered && boss.cutsceneReserved)
        {
            boss.cutsceneTriggered = true;
            boss.cutsceneReserved = false;
            boss.ChangeState(boss.cutsceneState);
            Debug.Log("[PatternState] �ƾ� ���� ���� �� CutsceneState ��ȯ");
        }
    }
    public override void Update()
    {
        if (boss.cutsceneTriggered && !isPhase2Started)
        {
            isPhase2Started = true;
            boss.patternController.StartPhase2LightningPattern();
        }


    }
    private IEnumerator HandlePattern()
    {
        if (target == null)
        {
            boss.ChangeState(boss.idleState);
            yield break;
        }

        float distance = Vector3.Distance(boss.transform.position, target.position);
        bool isNearWall = boss.CheckWallNearby();

        //int selectedIndex = boss.patternController.GetPattern(distance, isNearWall);

        int selectedIndex;

        if (boss.useTestPattern)
        {
            selectedIndex = boss.testPatternIndex;
            Debug.Log($"[TEST] ���� �׽�Ʈ ���� ����: {selectedIndex}");
        }
        else
        {
            selectedIndex = boss.patternController.GetPattern(distance, isNearWall);
        }

        // ^^^^ ���� �׽�Ʈ ���� ���� ���� ^^^^

        if (selectedIndex == -1)
        {
            boss.ChangeState(boss.moveState);
            yield break;
        }

        boss.bossStatus.lastAttackIndex = selectedIndex; // ������ ���� ����
        boss.Animator.SetInteger("AttackIndex", selectedIndex);

        // �ӵ� ���� �޼��� ȣ��
        ApplyAnimationSpeedForIndex(selectedIndex);

        if (selectedIndex >= 0 && selectedIndex <= 5) // Close ����
        {
            yield return SmoothLookAtTarget(); // ȸ�� �Ϸ���� ��ٸ�
            boss.Animator.SetTrigger("Attack");
        }
        else if (selectedIndex == 6) // Jump Attack
        {
            yield return ExecuteJumpAttack();
        }
        else if (selectedIndex == 7) // Spin Attack
        {
            ExecuteSpinAttack();
        }
        else if (selectedIndex == 8) // Wall Escape
        {
            LookAwayFromWall();
            boss.Animator.SetTrigger("Attack");
        }
    }

    private void LookAwayFromWall()
    {
        Vector3 forward = boss.transform.forward;
        Vector3 opposite = -forward;
        opposite.y = 0f;
        boss.transform.rotation = Quaternion.LookRotation(opposite);
    }

    private IEnumerator ExecuteJumpAttack()
    {
        if (target == null)
        {
            boss.ChangeState(boss.idleState);
            yield break;
        }

        Vector3 targetPosition = target.position;
        Vector3 bossPosition = boss.transform.position;
        Vector3 direction = (targetPosition - bossPosition);
        direction.y = 0f;

        if (direction == Vector3.zero)
        {
            direction = boss.transform.forward;
        }

        direction.Normalize();

        float fullDistance = Vector3.Distance(bossPosition, targetPosition);
        float adjustedDistance = Mathf.Max(fullDistance - 5.0f, 0f);
        Vector3 destination = bossPosition + direction * adjustedDistance;

        boss.transform.rotation = Quaternion.LookRotation(direction);

        boss.Animator.SetInteger("AttackIndex", 6);
        boss.Animator.SetTrigger("Attack");

        yield return new WaitUntil(() =>
            boss.Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack_G")); // �ִϸ��̼� �̸� Ȯ�� �ʿ�

        float jumpTime = 0.85f;
        float elapsed = 0f;
        Vector3 startPos = boss.transform.position;

        while (elapsed < jumpTime)
        {
            float t = elapsed / jumpTime;
            Vector3 movePos = Vector3.Lerp(startPos, destination, t);
            movePos.y += Mathf.Sin(t * Mathf.PI) * 0.2f; // ������ ȿ��

            boss.transform.position = movePos;

            elapsed += Time.deltaTime;
            yield return null;
        }

        boss.transform.position = new Vector3(destination.x, boss.transform.position.y, destination.z);
    }

    private void ExecuteSpinAttack()
    {
        if (target == null) return;

        Vector3 direction = (target.position - boss.transform.position).normalized;
        direction.y = 0f;
        if (direction == Vector3.zero) return;

        boss.transform.rotation = Quaternion.LookRotation(direction);
        boss.Animator.SetTrigger("Attack");
    }

    private IEnumerator SmoothLookAtTarget(float duration = 0.3f)
    {
        if (target == null) yield break;

        Vector3 dir = (target.position - boss.transform.position).normalized;
        dir.y = 0f;

        if (dir == Vector3.zero) dir = boss.transform.forward;

        Quaternion startRot = boss.transform.rotation;
        Quaternion targetRot = Quaternion.LookRotation(dir);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            boss.transform.rotation = Quaternion.Slerp(startRot, targetRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        boss.transform.rotation = targetRot;
        boss.lockRotation = true;
    }
 
    private void ApplyAnimationSpeedForIndex(int index)
    {
        switch (index)
        {
            case 1:
                boss.Animator.speed = 0.65f;
                break;
            case 2:
                boss.Animator.speed = 0.65f;
                break;
            case 3:
                boss.Animator.speed = 0.75f;
                break;
            case 4:
                boss.Animator.speed = 0.75f;
                break;
            case 5:
                boss.Animator.speed = 0.75f;
                break;
            case 7:
                boss.Animator.speed = 0.75f;
                break;
            default:
                boss.Animator.speed = 1f;
                break;
        }
    }
    
}
