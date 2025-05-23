using System.Collections;
using UnityEngine;

public class EnemyAnimationEventReceiver : MonoBehaviour
{
    private EnemyFSM enemyFSM;
    private Animator animator;

    private void Awake()
    {
        enemyFSM = GetComponent<EnemyFSM>();
        animator = GetComponent<Animator>();
    }

    // �ִϸ��̼� �̺�Ʈ�� ȣ��� (���� ���� ��)
    public void OnAttackEnd()
    {
        if (enemyFSM.currentState is EnemyAttackState attackState)
        {
            attackState.OnAttackAnimationComplete();
        }
    }

    // Stunned �ִϸ��̼� �߰��� ȣ���
    public void TriggerStunPause()
    {
        StartCoroutine(StunPauseEffect());
    }

    private IEnumerator StunPauseEffect()
    {
        animator.speed = 0f;
        yield return new WaitForSeconds(0.25f);
        animator.speed = 1f;
    }
}
