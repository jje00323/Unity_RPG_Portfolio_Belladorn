using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("�� ������ �� ���� ��ġ")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;

    [Header("���� UI ������ �� �θ�")]
    [SerializeField] private GameObject bossHPUIPrefab;

    private Transform bossUIParent;

    [SerializeField] private bool autoSpawnOnStart = true;

    private void Awake()
    {
        // �ƾ��� BossUIGroup(CanvasGroup)�� Transform�� ã�Ƽ� �ڽ����� �ֱ�
        var bossUIGroup = GameObject.Find("BossUIGroup");
        if (bossUIGroup != null)
        {
            bossUIParent = bossUIGroup.transform;
            Debug.Log("[EnemySpawner] BossUIGroup �ڵ� ���� �Ϸ�");
        }
        else
        {
            Debug.LogWarning("[EnemySpawner] BossUIGroup�� ã�� �� �����ϴ�. ���� ü�¹� UI�� ����� ǥ�õ��� ���� �� �ֽ��ϴ�.");
        }
    }

    private void Start()
    {
        if (autoSpawnOnStart)
            SpawnAll();
    }

    public void SpawnAll()
    {
        foreach (Transform spawn in spawnPoints)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawn.position, spawn.rotation);

            if (enemy.CompareTag("Boss"))
            {
                var bossFSM = enemy.GetComponent<BossEnemyFSM>();
                if (bossFSM != null && bossFSM.bossData != null)
                {
                    bossFSM.bossStatus.Setup(bossFSM.bossData);
                    Debug.Log("[EnemySpawner] Boss Setup ȣ�� �Ϸ�");

                    // ���� UI ���� �� ����
                    if (bossHPUIPrefab != null && bossUIParent != null)
                    {
                        GameObject uiGO = Instantiate(bossHPUIPrefab, bossUIParent);
                        var hpUI = uiGO.GetComponent<BossEnemyHealthUI>();

                        if (hpUI != null)
                        {
                            hpUI.SetBoss(bossFSM.bossStatus);
                            Debug.Log("[EnemySpawner] BossHealthUI�� bossStatus ���� �Ϸ�");
                        }
                        else
                        {
                            Debug.LogError("[EnemySpawner] BossHPUIPrefab�� BossEnemyHealthUI ������Ʈ�� �����ϴ�.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("[EnemySpawner] bossHPUIPrefab �Ǵ� bossUIParent�� ����ֽ��ϴ�.");
                    }
                }
                else
                {
                    Debug.LogError("[EnemySpawner] bossFSM �Ǵ� bossData�� �����Ǿ����ϴ�.");
                }
            }
            else
            {
                SetupNavMeshObstacle(enemy);
            }
        }
    }

    private void SetupNavMeshObstacle(GameObject enemy)
    {
        if (enemy.CompareTag("Boss")) return;

        var obstacle = enemy.GetComponent<NavMeshObstacle>();
        if (obstacle == null)
            obstacle = enemy.AddComponent<NavMeshObstacle>();

        obstacle.carving = true;
        obstacle.carveOnlyStationary = false;
        obstacle.shape = NavMeshObstacleShape.Capsule;
        obstacle.radius = 0.3f;
        obstacle.height = 2.0f;
    }
}
