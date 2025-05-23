using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetectState : EnemyState
{

    public EnemyDetectState(EnemyFSM enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.animator.SetBool("IsMoving", true);
        enemy.hasDetectedPlayer = true; // Ž�� ����
    }

    public override void Update()
    {
        Transform target = enemy.enemyStatus.target;
        if (target == null) return;

        float distanceToTarget = Vector3.Distance(enemy.transform.position, target.position);
        float distanceFromSpawn = Vector3.Distance(enemy.transform.position, enemy.spawnPosition);

        // 1. ���� �Ÿ� �ʰ� �� �� ��� �� �����ϰ� MoveState ����
        if (distanceFromSpawn >= enemy.enemyData.returnDistance)
        {
            enemy.hasDetectedPlayer = false;
            enemy.animator.SetBool("IsMoving", true);
            enemy.moveState.SetDestination(enemy.spawnPosition, true); // ����
            enemy.ChangeState(enemy.moveState);
            return;
        }

        // 2. ���� ���� ���̸� ����
        if (distanceToTarget <= enemy.enemyData.attackRange)
        {
            enemy.animator.SetBool("IsMoving", false);
            enemy.ChangeState(enemy.attackState);
            return;
        }

        // 3. �� �ܿ��� ��� ����
        Vector3 dir = (target.position - enemy.transform.position).normalized;
        enemy.transform.position += dir * enemy.enemyData.moveSpeed * Time.deltaTime;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        enemy.transform.rotation = Quaternion.RotateTowards(enemy.transform.rotation, targetRot, 360f * Time.deltaTime);
    }

    public override void Exit()
    {
        enemy.animator.SetBool("IsMoving", false);
    }
}
