using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerStateMachine))]
public class PlayerSkill : MonoBehaviour
{
    private PlayerStateMachine stateMachine;
    private PlayerMovement playerMovement;
    private PlayerSkillController skillController;
    private Animator animator;

    private void Awake()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        playerMovement = GetComponent<PlayerMovement>();
        skillController = GetComponent<PlayerSkillController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame) TryUseSkill("Q");
        if (Keyboard.current.wKey.wasPressedThisFrame) TryUseSkill("W");
        if (Keyboard.current.eKey.wasPressedThisFrame) TryUseSkill("E");
        if (Keyboard.current.rKey.wasPressedThisFrame) TryUseSkill("R");
    }

    public void TryUseSkill(string skillKey)
    {
        if (!stateMachine.CanSkill()) return;

        

        var skill = SkillEquipManager.Instance.GetEquippedSkill(skillKey);
        if (skill == null) return;

        if (skill.currentLevel <= 0)
        {
            Debug.Log($"[Skill] {skillKey}�� ��� �����Դϴ�.");
            return;
        }

        float cooldown = skillController.GetSkillCooldown(skillKey);
        float lastUsedTime = skillController.GetSkillLastUsedTime(skillKey);

        float timeSinceLastUse = Time.time - lastUsedTime;
        if (timeSinceLastUse < cooldown)
        {
            float remaining = cooldown - timeSinceLastUse;
            Debug.Log($"[Skill] {skillKey} ��ų ��Ÿ��: {remaining:F1}�� ����");
            return;
        }

        Vector3? mouseTarget = Hitbox.MouseUtility.GetMouseWorldPosition(LayerMask.GetMask("Ground"));

        // ��Ȯ�� ���� ����� ���� ����
        skillController.SetSkillLastUsedTime(skillKey, Time.time);

        Debug.Log($"[Skill] {skillKey} ��ų ���! (��Ÿ�� {cooldown}��)");

        stateMachine.ChangeState(PlayerStateMachine.PlayerState.SkillCasting);
        skillController.ExecuteSkill(skillKey, mouseTarget);
    }
    public void EndSkill()
    {
        var attack = GetComponent<PlayerAttack>();
        if (attack != null)
            attack.EndCombo(); // �޺� ���� �� RootMotion�� ���ʿ��� ����

        //  ���� ���� �ƴ϶�� ���⼭�� ����
        if (attack == null || !IsInAttackState())
            animator.applyRootMotion = false;

        playerMovement.ResumeAgent();
        animator.SetTrigger("EndSkill");
        stateMachine.ChangeState(PlayerStateMachine.PlayerState.Idle);
    }

    private bool IsInAttackState()
    {
        return stateMachine.CurrentState == PlayerStateMachine.PlayerState.Attacking;
    }
}
