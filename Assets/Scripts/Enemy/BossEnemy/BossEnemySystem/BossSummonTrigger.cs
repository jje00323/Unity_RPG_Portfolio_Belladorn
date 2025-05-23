using System.Collections;
using UnityEngine;

public class BossSummonTrigger : MonoBehaviour
{
    [Header("���� ������ ������")]
    public GameObject bossSpawnerPrefab;

    [Header("�ƾ� �� ���̵� ������")]
    public GameObject cutsceneTrackPrefab;

    [Header("��Ʈ�� ī�޶� ������")]
    public GameObject introTrackPrefab;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        triggered = true; // ù �ٿ��� �ٷ� true ����

        if (!other.CompareTag("Player"))
        {
            Debug.Log("[BossSummonTrigger] �±� ����ġ - ����");
            return;
        }

        Debug.Log("[BossSummonTrigger] �÷��̾� ���� ������ �� ���� ��ȯ ����");
        StartCoroutine(SummonBossSequence());
    }

    private IEnumerator SummonBossSequence()
    {
        yield return new WaitForSeconds(1f);

        if (bossSpawnerPrefab != null)
        {
            var bossObj = Instantiate(bossSpawnerPrefab, transform.position, transform.rotation);
            Debug.Log("[BossSummonTrigger] ���� ������ ������");

            // FSM ���� ������ ���� ��ũ��Ʈ ���ο��� ó���ϰų� ���⼭�� ����
            var fsm = bossObj.GetComponentInChildren<BossEnemyFSM>();
            if (fsm != null)
                fsm.ChangeState(fsm.introState);
        }

        if (introTrackPrefab != null)
            Instantiate(introTrackPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject); // ������ ����
    }
}

