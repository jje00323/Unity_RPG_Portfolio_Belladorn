using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyAttackController : MonoBehaviour
{
    [Header("ScriptableObject ��� ���� ���� ������")]
    [SerializeField]
    private BossAttackData[] attackDataList = new BossAttackData[8];

    private int hitboxInvokeCount = 0;
    private int telegraphInvokeCount = 0;

    private BossEnemyFSM bossFSM;
    private Coroutine animSpeedRoutine = null;
    private float animStartTime = -1f;

    [System.Serializable]
    public struct AnimSpeedProfile
    {
        public int attackIndex;
        public float startSpeed;
        public float targetSpeed;
        public float duration;
        public float finalSpeed;
    }

    [SerializeField]
    private List<AnimSpeedProfile> animSpeedProfiles = new List<AnimSpeedProfile>();
    private Dictionary<int, AnimSpeedProfile> profileLookup;

    private void Awake()
    {
        bossFSM = GetComponent<BossEnemyFSM>();
        if (bossFSM == null)
            Debug.LogError("[AttackController] BossEnemyFSM ������Ʈ�� �����ϴ�.");

        profileLookup = new Dictionary<int, AnimSpeedProfile>();
        foreach (var profile in animSpeedProfiles)
            profileLookup[profile.attackIndex] = profile;
    }

    public void SpawnAttackBySubtable()
    {
        if (bossFSM == null || bossFSM.bossStatus == null)
            return;

        int index = bossFSM.bossStatus.lastAttackIndex;
        if (index < 0 || index >= attackDataList.Length)
            return;

        var data = attackDataList[index];
        if (data == null || data.hitboxInfos == null)
            return;

        if (hitboxInvokeCount >= data.hitboxInfos.Count)
            return;

        var info = data.hitboxInfos[hitboxInvokeCount];
        if (info == null || info.hitboxPrefab == null)
            return;

        GameObject instance = Instantiate(info.hitboxPrefab, transform.position, transform.rotation);
        instance.tag = "EnemyHitbox";

        BossHitbox hitbox = instance.GetComponent<BossHitbox>();
        if (hitbox != null)
        {
            hitbox.Initialize(transform, info.followCaster);
        }

        hitboxInvokeCount++;
    }

    //public void SpawnTelegraphBySubtable()
    //{
    //    if (bossFSM == null || bossFSM.bossStatus == null)
    //        return;

    //    int index = bossFSM.bossStatus.lastAttackIndex;
    //    if (index < 0 || index >= attackDataList.Length)
    //        return;

    //    var data = attackDataList[index];
    //    if (data == null || data.telegraphInfos == null)
    //        return;

    //    if (telegraphInvokeCount >= data.telegraphInfos.Count)
    //        return;

    //    var info = data.telegraphInfos[telegraphInvokeCount];
    //    if (info == null || info.telegraphPrefab == null)
    //        return;

    //    Transform target = bossFSM.bossStatus.target;
    //    if (target == null) return;

    //    //Vector3 bossPos = transform.position;
    //    //Vector3 dir = (target.position - bossPos);
    //    //dir.y = 0f;
    //    //if (dir == Vector3.zero) dir = transform.forward;
    //    //dir.Normalize();

    //    //bool isJumpAttack = (index == 6);
    //    //Vector3 spawnPos = isJumpAttack ? target.position : bossPos + dir * Mathf.Max(Vector3.Distance(bossPos, target.position) - 1.5f, 0f);
    //    //Quaternion rotation = Quaternion.LookRotation(dir);
    //    bool isJumpAttack = (index == 6);

    //    // ������ ���� ȸ������ �״�� ���
    //    Quaternion rotation = transform.rotation;

    //    // ���� ���� �������� ���� �Ÿ� ����
    //    float spawnDistance = 3.5f; // ���� ������ ���� ���� ����
    //    Vector3 spawnPos = isJumpAttack
    //        ? target.position
    //        : transform.position + rotation * Vector3.forward * spawnDistance;

    //    GameObject instance = Instantiate(info.telegraphPrefab, spawnPos, rotation);
    //    TelegraphArea telegraph = instance.GetComponent<TelegraphArea>();
    //    if (telegraph != null)
    //    {
    //        telegraph.shape = info.shapeType;
    //        telegraph.Initialize(spawnPos, rotation, transform, isJumpAttack);
    //    }

    //    telegraphInvokeCount++;
    //}
    public void SpawnTelegraphBySubtable()
    {
        if (bossFSM == null || bossFSM.bossStatus == null)
            return;

        int index = bossFSM.bossStatus.lastAttackIndex;
        if (index < 0 || index >= attackDataList.Length)
        {
            Debug.LogWarning($"[Telegraph] �߸��� index ����: {index}");
            return;
        }

        var data = attackDataList[index];
        if (data == null || data.telegraphInfos == null)
        {
            Debug.LogWarning($"[Telegraph] attackDataList[{index}]�� telegraphInfos�� null");
            return;
        }

        if (telegraphInvokeCount >= data.telegraphInfos.Count)
        {
            Debug.LogWarning($"[Telegraph] telegraphInvokeCount({telegraphInvokeCount}) >= telegraphInfos.Count({data.telegraphInfos.Count})");
            return;
        }

        var info = data.telegraphInfos[telegraphInvokeCount];
        if (info == null || info.telegraphPrefab == null)
        {
            Debug.LogWarning($"[Telegraph] telegraphInfos[{telegraphInvokeCount}]�� null �Ǵ� ������ ����");
            return;
        }

        Transform target = bossFSM.bossStatus.target;
        if (target == null)
        {
            Debug.LogWarning("[Telegraph] Ÿ���� null�Դϴ�");
            return;
        }

        // ����� �α�: ���� ��ġ �� Ÿ�� �Ÿ�
        Vector3 bossPos = transform.position;
        Vector3 targetPos = target.position;
        Vector3 dir = targetPos - bossPos;
        dir.y = 0f;
        if (dir == Vector3.zero) dir = transform.forward;
        dir.Normalize();

        bool isJumpAttack = (index == 6);
        Quaternion rotation = transform.rotation; // ���� ����
        float spawnDistance = 3.5f;
        Vector3 spawnPos = isJumpAttack ? targetPos : bossPos + rotation * Vector3.forward * spawnDistance;

        // ����� �α� ���
        Debug.Log($"[Telegraph] ȣ��� - ����Index: {index}, InvokeCount: {telegraphInvokeCount}, isJump: {isJumpAttack}");
        Debug.Log($"[Telegraph] BossPos: {bossPos}, TargetPos: {targetPos}, SpawnPos: {spawnPos}");
        Debug.Log($"[Telegraph] ���� ȸ����: {transform.rotation.eulerAngles}, ���� ȸ����: {rotation.eulerAngles}");
        Debug.Log($"[Telegraph] Prefab: {info.telegraphPrefab.name}, Shape: {info.shapeType}");

        GameObject instance = Instantiate(info.telegraphPrefab, spawnPos, rotation);
        TelegraphArea telegraph = instance.GetComponent<TelegraphArea>();
        if (telegraph != null)
        {
            telegraph.shape = info.shapeType;
            telegraph.Initialize(spawnPos, rotation, transform, isJumpAttack);
        }
        else
        {
            Debug.LogWarning("[Telegraph] TelegraphArea ������Ʈ�� ã�� �� �����ϴ�.");
        }

        telegraphInvokeCount++;
    }
    private IEnumerator SmoothAnimSpeed(float from, float to, float duration)
    {
        float elapsed = 0f;
        bossFSM.Animator.speed = from;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            bossFSM.Animator.speed = Mathf.Lerp(from, to, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        bossFSM.Animator.speed = to;
        animSpeedRoutine = null;
        Debug.Log("[AnimSpeed] ��� ���� �Ϸ�");
    }

    public void OnAnimSpeedStart()
    {
        if (bossFSM == null || bossFSM.Animator == null) return;

        int index = bossFSM.bossStatus.lastAttackIndex;
        if (profileLookup.TryGetValue(index, out AnimSpeedProfile profile))
        {
            if (animSpeedRoutine != null)
                StopCoroutine(animSpeedRoutine);

            animSpeedRoutine = StartCoroutine(SmoothAnimSpeed(profile.startSpeed, profile.targetSpeed, profile.duration));
        }
        else
        {
            Debug.Log($"[AnimSpeed] �ε��� {index}�� ���� ��� �������� �����ϴ�.");
        }
    }

    public void OnAnimSpeedEnd()
    {
        if (bossFSM == null || bossFSM.Animator == null) return;

        int index = bossFSM.bossStatus.lastAttackIndex;
        if (animSpeedRoutine != null)
        {
            StopCoroutine(animSpeedRoutine);
            animSpeedRoutine = null;
        }

        if (profileLookup.TryGetValue(index, out AnimSpeedProfile profile))
            bossFSM.Animator.speed = profile.finalSpeed;
        else
            bossFSM.Animator.speed = 1f;

        Debug.Log($"[AnimSpeed] ��� ���� �Ϸ� (Index: {index})");
    }

    public void BeginAttack()
    {
        hitboxInvokeCount = 0;
        telegraphInvokeCount = 0;
    }

    public void AnimTimeStart()
    {
        animStartTime = Time.time;
        Debug.Log($"[AnimTimer] ���� �ð� ��ϵ�: {animStartTime:F4}��");
    }

    public void AnimTimeEnd()
    {
        if (animStartTime < 0f)
        {
            Debug.LogWarning("[AnimTimer] ���� �ð��� �������� �ʾҽ��ϴ�.");
            return;
        }

        float elapsed = Time.time - animStartTime;
        Debug.Log($"[AnimTimer] �� ��� �ð�: {elapsed:F4}�� (���ۡ����� ������)");
        animStartTime = -1f;
    }
    public GameObject GetTelegraphPrefab(int index)
    {
        if (index < 0 || index >= attackDataList.Length) return null;
        var data = attackDataList[index];
        return (data != null && data.telegraphInfos.Count > 0) ? data.telegraphInfos[0].telegraphPrefab : null;
    }

    public GameObject GetHitboxPrefab(int index)
    {
        if (index < 0 || index >= attackDataList.Length) return null;
        var data = attackDataList[index];
        return (data != null && data.hitboxInfos.Count > 0) ? data.hitboxInfos[0].hitboxPrefab : null;
    }

    public void SpawnLightningAttackGroup(Vector3 pos, int index)
    {
        var telegraphPrefab = GetTelegraphPrefab(index);
        var hitboxPrefab = GetHitboxPrefab(index);

        if (telegraphPrefab != null)
        {
            Debug.Log("[��������] Telegraph Prefab �ν��Ͻ� ������");
            GameObject t = Instantiate(telegraphPrefab, pos, Quaternion.identity);
            var ta = t.GetComponent<TelegraphArea>();
            ta?.Initialize(pos, Quaternion.identity, transform, false);
        }
        else
        {
            Debug.LogWarning("[��������] Telegraph Prefab�� null�Դϴ�.");
        }

        StartCoroutine(SpawnHitboxDelayed(pos, hitboxPrefab, 1.5f));
    }

    private IEnumerator SpawnHitboxDelayed(Vector3 pos, GameObject prefab, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (prefab != null)
        {
            Debug.Log("[��������] ��Ʈ�ڽ� ������ ������");
            GameObject h = Instantiate(prefab, pos, Quaternion.identity);
            var bh = h.GetComponent<BossHitbox>();
            bh?.Initialize(pos, Quaternion.identity, transform, false);
        }
        else
        {
            Debug.LogWarning("[��������] ��Ʈ�ڽ� �������� null�Դϴ�.");
        }
    }
}