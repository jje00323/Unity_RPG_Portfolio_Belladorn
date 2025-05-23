using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkillController : MonoBehaviour
{
    [Header("��ų ��Ʈ�ڽ� ������")]
    public GameObject skillHitboxPrefab;
    public Transform spawnPoint;

    public void SpawnSkillHitbox()
    {
        if (skillHitboxPrefab == null || spawnPoint == null)
        {
            Debug.LogWarning("[EnemySkillController] ��ų ��Ʈ�ڽ� ������ �Ǵ� ���� ��ġ�� �����ϴ�.");
            return;
        }

        GameObject instance = Instantiate(
            skillHitboxPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        Hitbox hitbox = instance.GetComponent<Hitbox>();
        if (hitbox != null)
        {
            hitbox.Initialize(transform, false);
            instance.tag = "EnemyHitbox";
        }
    }
}