using System.Collections;
using UnityEngine;

public class EnemyStunnedState : EnemyState
{
    private float stunDuration = 1.5f;
    private float elapsed = 0f;

    public EnemyStunnedState(EnemyFSM enemy) : base(enemy) { }

    public override void Enter()
    {
        elapsed = 0f;

        // �ִϸ����� �ʱ�ȭ
        enemy.animator.ResetTrigger("Attack");
        enemy.animator.ResetTrigger("Die");
        enemy.animator.SetBool("IsMoving", false);

        // ���� �ε��� ����
        int index = Random.Range(0, 2); // 0 �Ǵ� 1
        enemy.animator.SetInteger("StunnedIndex", index);

        // Stunned Ʈ���� ����
        enemy.animator.SetTrigger("Stunned");
    }

    public override void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= stunDuration)
        {
            enemy.animator.ResetTrigger("Stunned");
            enemy.ChangeState(enemy.idleState);
        }
    }

    public override void Exit()
    {
        // �� ���¿����� animator.speed ���� �� ��
    }
}
