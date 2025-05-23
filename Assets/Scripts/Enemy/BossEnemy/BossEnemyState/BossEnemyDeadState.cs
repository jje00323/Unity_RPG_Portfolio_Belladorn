using UnityEngine;
using System.Collections;

public class BossEnemyDeadState : BossEnemyState
{
    public BossEnemyDeadState(BossEnemyFSM boss) : base(boss) { }

    public override void Enter()
    {
        boss.Animator.SetTrigger("Die");
        boss.Animator.SetBool("IsMoving", false);

        if (boss.TryGetComponent<Collider>(out var col))
            col.enabled = false;

        boss.StartCoroutine(HandleDeathSequence());

        boss.Animator.speed = 0.5f;
    }

    public override void Update() { }

    public override void Exit() { }

    private IEnumerator HandleDeathSequence()
    {
        // ��� �ִϸ��̼� ��ٸ��� (2�ʶ�� ����)
        yield return new WaitForSeconds(4f);

        // ü�� UI ����
        BossEnemyHealthUI bossUI = GameObject.FindObjectOfType<BossEnemyHealthUI>();
        if (bossUI != null)
        {
            GameObject.Destroy(bossUI.gameObject);
            Debug.Log("[DeadState] ���� ü�� UI ���ŵ�");
        }


        // �������� ��ȯ
        if (boss.bossData != null && boss.bossData.itemDrops != null && boss.bossData.itemDrops.Length > 0)
        {
            Vector3 spawnPosition = new Vector3(0f, 0f, 0f); // �Ǵ� ���� ������ ��ġ
            Quaternion spawnRotation = Quaternion.identity;

            foreach (GameObject drop in boss.bossData.itemDrops)
            {
                GameObject.Instantiate(drop, spawnPosition, spawnRotation);
                Debug.Log($"[DeadState] �������� �Ǵ� ��� ������ ������: {drop.name}");
            }
        }

        // 4. ���� ������Ʈ ����
        GameObject.Destroy(boss.gameObject);
    }
}
