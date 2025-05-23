using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackController : MonoBehaviour
{
    [Header("��Ʈ�ڽ� ������")]
    public GameObject attackHitboxPrefab;

    public void SpawnAttackHitbox()
    {
        if (attackHitboxPrefab == null) return;

        GameObject instance = Instantiate(attackHitboxPrefab);
        Hitbox hitbox = instance.GetComponent<Hitbox>();

        if (hitbox != null)
        {
            hitbox.Initialize(transform, true); // SpawnPoint�� �ƴ� enemy.transform ����
            instance.tag = "EnemyHitbox";
        }
    }
}
