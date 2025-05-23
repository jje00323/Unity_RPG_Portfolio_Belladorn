using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerStateMachine;

[RequireComponent(typeof(PlayerStateMachine))]
public class PlayerDodge : MonoBehaviour
{
    private PlayerStateMachine stateMachine;
    private Animator animator;

    public float dodgeDistance = 5f;
    public float dodgeDuration = 0.2f;

    void Awake()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        animator = GetComponent<Animator>();
    }

    public void HandleDodge()
    {
        if (!stateMachine.CanDodge()) return;

        stateMachine.ChangeState(PlayerState.Dodging);
        animator.SetTrigger("Dodge");

        // RootMotion�� �ִϸ��̼� Ŭ�� ������ ���� ����� (�ڵ忡���� �������� ����)
    }

    // �ִϸ��̼� �̺�Ʈ�� ȣ��� ���� ���� ó�� �Լ�
    public void EndDodge()
    {
        stateMachine.ChangeState(PlayerState.Idle);
    }
}