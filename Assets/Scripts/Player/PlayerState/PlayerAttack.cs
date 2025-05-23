using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static PlayerStateMachine;

[RequireComponent(typeof(PlayerStateMachine))]
public class PlayerAttack : MonoBehaviour
{
    private PlayerMovement movement;
    private Animator animator;
    private PlayerStateMachine stateMachine;

    private int comboIndex = 0;
    private bool isAttacking = false;

    private bool inputBuffered = false;
    private bool allowBufferedInput = false;
    private bool canExecuteImmediately = false;
    private bool inputLocked = false; // ��Ŭ ������

    private bool isCombat = false;
    private float combatTimer = 0f;
    private float combatDuration = 6f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        stateMachine = GetComponent<PlayerStateMachine>();
        movement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        CombatSystem();
        if (EventSystem.current.IsPointerOverGameObject()) return;
    }

    public void CombatSystem()
    {
        if (isCombat && !isAttacking)
        {
            combatTimer += Time.deltaTime;

            if (combatTimer >= combatDuration)
            {
                isCombat = false;
                animator.SetBool("IsCombat", false);
                combatTimer = 0f;

               // Debug.Log("���� ���� �����: BasicMove ���·� ���ư�");
            }
        }
    }

    public void HandleAttackInput()
    {
        if (!stateMachine.CanAttack()) return;
        if (inputLocked) return;

        if (isAttacking)
        {
            if (allowBufferedInput && !canExecuteImmediately)
            {
                inputBuffered = true;
                //Debug.Log("�Է� ������ (���� ����)");
            }
            else if (canExecuteImmediately)
            {
                ExecuteNextCombo();
            }
            else
            {
                Debug.Log("�Է� ���õ� (�޺� Ÿ�̹� �ƴ�)");
            }
        }
        else
        {
            if (!isCombat)
            {
                isCombat = true;
                animator.SetBool("IsCombat", isCombat);
            }
            combatTimer = 0f;

            isAttacking = true;
            comboIndex = 1;
            inputLocked = true;

            movement.StopAgent();
            movement.RotateToMouse();

            animator.applyRootMotion = true;

            stateMachine.ChangeState(PlayerState.Attacking);
            animator.SetTrigger("NextCombo");

            //Debug.Log("ù ��° ���� ����");
        }
    }

    private void ExecuteNextCombo()
    {
        if (comboIndex >= 4) return;

        comboIndex++;
        animator.SetTrigger("NextCombo");
        movement.RotateToMouse();

        inputBuffered = false;
        allowBufferedInput = false;
        canExecuteImmediately = false;
        inputLocked = true;

        //Debug.Log($"�޺� {comboIndex}��° �����");
    }

    public void EnableComboInput()
    {
        inputBuffered = false;
        allowBufferedInput = true;
        canExecuteImmediately = false;
        inputLocked = false;
        //Debug.Log("�޺� �Է� ��� ����");
    }

    public void CanAttack()
    {
        canExecuteImmediately = true;

        if (inputBuffered)
        {
            ExecuteNextCombo();
            //Debug.Log("���۵� �Է����� �޺� ����");
        }
        else
        {
           // Debug.Log("CanAttack ȣ��� - ���� �Է� ����");
        }
    }

    public void DisableComboInput()
    {
        allowBufferedInput = false;
        canExecuteImmediately = false;
        //Debug.Log("�޺� �Է� ����");
    }

    public void EndCombo()
    {
        isAttacking = false;
        inputBuffered = false;
        allowBufferedInput = false;
        canExecuteImmediately = false;
        inputLocked = false;
        comboIndex = 0;

        animator.applyRootMotion = false;

        movement.ResumeAgent();

        stateMachine.ChangeState(PlayerState.Idle);

       // Debug.Log("���� �ʱ�ȭ");
    }

    //public void ForceEndCombo()
    //{
    //    isAttacking = false;
    //    inputBuffered = false;
    //    allowBufferedInput = false;
    //    canExecuteImmediately = false;
    //    inputLocked = false;
    //    comboIndex = 0;

    //    movement.ResumeAgent();

    //    Debug.Log("���� �ʱ�ȭ (���� ����)");

    //    stateMachine.ChangeState(PlayerState.Idle);
    //}

    public void ActivateBasicHitbox()
    {
        string key = "�⺻����";

        PlayerSkillController skillSystem = GetComponent<PlayerSkillController>();
        if (skillSystem != null)
        {
            skillSystem.ActivateHitbox(key);
            //Debug.Log($"[�⺻����] ��Ʈ�ڽ� ���� ��û: {key}");
        }
        else
        {
            Debug.LogWarning("PlayerSkillSystem ������Ʈ�� ����!");
        }
    }

    public void EnterCombatMode()
    {
        if (!isCombat)
        {
            isCombat = true;
            animator.SetBool("IsCombat", true);
        }
        combatTimer = 0f;
    }
}
