using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyIdleState : EnemyState
{
    private float idleTimer = 0f;
    private float patrolCooldown = 3f;

    public EnemyIdleState(EnemyFSM enemy) : base(enemy) { }

    public override void Enter()
    {
        idleTimer = 0f;
        enemy.animator.SetBool("IsMoving", false);

        // ���� �Ϸ� �� �浹 �ٽ� Ȱ��ȭ
        enemy.gameObject.layer = LayerMask.NameToLayer("Enemy");

        // ���� �Ϸ� �� NavMeshObstacle carving �ٽ� �ѱ�
        var obstacle = enemy.GetComponent<NavMeshObstacle>();
        if (obstacle != null)
        {
            obstacle.carving = true;
        }
    }

    public override void Update()
    {
        Transform target = enemy.enemyStatus.target;
        if (target != null)
        {
            float distance = Vector3.Distance(enemy.transform.position, target.position);

            if (!enemy.hasDetectedPlayer && distance <= enemy.enemyData.detectRadius)
            {
                enemy.ChangeState(enemy.detectState);
                return;
            }

            if (enemy.hasDetectedPlayer)
            {
                float distanceFromSpawn = Vector3.Distance(enemy.transform.position, enemy.spawnPosition);

                if (distanceFromSpawn < enemy.enemyData.returnDistance)
                {
                    enemy.ChangeState(enemy.detectState); // ��� ����
                    return;
                }
                else
                {
                    enemy.hasDetectedPlayer = false;
                    enemy.moveState.SetDestination(enemy.spawnPosition, true); // ���Ϳ�
                    enemy.ChangeState(enemy.moveState);
                    return;
                }
            }
        }

        idleTimer += Time.deltaTime;
        if (idleTimer >= patrolCooldown)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-3f, 3f),
                0,
                Random.Range(-3f, 3f)
            );

            Vector3 patrolPoint = enemy.spawnPosition + randomOffset;
            enemy.moveState.SetDestination(patrolPoint, false); // ���� �̵�
            enemy.ChangeState(enemy.moveState);
        }
    }

    public override void Exit() { }
}

