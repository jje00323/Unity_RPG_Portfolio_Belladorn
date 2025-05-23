using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyState
{
    private bool hasStartedAttack = false;
    private Quaternion targetRotation;

    public EnemyAttackState(EnemyFSM enemy) : base(enemy) { }

    public override void Enter()
    {
        hasStartedAttack = false;

        // �� ������ ������ ����
        Vector3 dir = (enemy.enemyStatus.target.position - enemy.transform.position).normalized;
        if (dir != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(dir);
        }

        enemy.animator.SetBool("IsMoving", false);
    }

    public override void Update()
    {

        // ȸ�� �Ϸ� ���̶�� ��� ȸ��
        if (!hasStartedAttack)
        {
            enemy.transform.rotation = Quaternion.RotateTowards(
                enemy.transform.rotation,
                targetRotation,
                480f * Time.deltaTime
            );

            if (Quaternion.Angle(enemy.transform.rotation, targetRotation) < 1f)
            {
                hasStartedAttack = true;
                Debug.Log("�÷��̾� ����");
                enemy.animator.SetTrigger("Attack");
            }

            return;
        }

    }

    public override void Exit() { }

    public void OnAttackAnimationComplete()
    {
        if (enemy.currentState == this)
        {
            enemy.ChangeState(enemy.idleState);
        }
    }
}
