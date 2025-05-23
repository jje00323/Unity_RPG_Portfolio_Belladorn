using UnityEditor.Rendering;
using UnityEngine;

public class EnemyDeadState : EnemyState
{
    

    private Material dissolveMat;

    public EnemyDeadState(EnemyFSM enemy) : base(enemy) { }

    public override void Enter()
    {
       

        // Rigidbody ���� ���� (���� ������ ���� ����)
        if (enemy.Rigidbody != null)
        {
            enemy.Rigidbody.useGravity = false;
            enemy.Rigidbody.velocity = Vector3.zero;
            enemy.Rigidbody.isKinematic = true;
        }

        // �ִϸ��̼� ���
        if (enemy.Animator != null)
        {
            enemy.Animator.SetTrigger("Die");
        }

        // �浹 ���� �ð� Ȯ��
        if (enemy.Collider != null)
        {
            enemy.Collider.enabled = true;
        }

        // Dissolve ��Ƽ���� �Ҵ�
        Renderer rend = enemy.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            dissolveMat = rend.material; // �ݵ�� �ν��Ͻ� ��Ƽ���� ���
            if (dissolveMat.HasProperty("_DissolveAmount"))
                dissolveMat.SetFloat("_DissolveAmount", 0f);
        }

        if (!string.IsNullOrEmpty(enemy.enemyData.enemyTag))
        {
            QuestManager.Instance.UpdateCondition("KillEnemy", enemy.enemyData.enemyTag);
            Debug.Log($"[EnemyDeadState] ����Ʈ ���� ����: {enemy.enemyData.enemyTag}");
        }

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            PlayerStatus playerStatus = playerObj.GetComponent<PlayerStatus>();
            if (playerStatus != null)
            {
                playerStatus.GainEXP(enemy.enemyData.expDrop);
                Debug.Log($"[EnemyDeadState] ����ġ {enemy.enemyData.expDrop} ���޵�");
            }
            else
            {
                Debug.LogWarning("[EnemyDeadState] PlayerStatus ������Ʈ�� ã�� �� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogWarning("[EnemyDeadState] Player �±׸� ���� ������Ʈ�� ã�� �� �����ϴ�.");
        }
        TryDropItems();
    }

    private void TryDropItems()
    {
        var dropList = enemy.enemyData.dropItems;
        if (dropList == null || dropList.Count == 0) return;

        foreach (var drop in dropList)
        {
            if (drop.itemPrefab == null) continue;

            float roll = Random.Range(0f, 100f);
            if (roll <= drop.dropChance)
            {
                Vector2 offset2D = Random.insideUnitCircle * 1.5f;
                Vector3 dropPos = enemy.transform.position + new Vector3(offset2D.x, 0.5f, offset2D.y);

                GameObject item = GameObject.Instantiate(drop.itemPrefab, dropPos, Quaternion.identity);

                if (drop.dropEffectPrefab != null)
                    GameObject.Instantiate(drop.dropEffectPrefab, dropPos, Quaternion.identity);

                

                Debug.Log($"[Drop] {item.name} ��ӵ� (Ȯ��: {drop.dropChance}%)");
            }
        }
    }

    

    private Color GetColorByRarity(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Normal: return Color.white;
            case ItemRarity.Rare: return Color.blue;
            case ItemRarity.Epic: return Color.magenta;
            default: return Color.gray;
        }
    }

    public override void Update()
    {
        
        
    }

    public override void Exit()
    {
        
    }
}
