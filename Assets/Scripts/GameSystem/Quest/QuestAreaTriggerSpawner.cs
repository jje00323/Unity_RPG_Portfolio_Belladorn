using System.Collections.Generic;
using UnityEngine;

public class QuestAreaTriggerSpawner : MonoBehaviour
{
    public static QuestAreaTriggerSpawner Instance { get; private set; }

    [Header("Ʈ���� ������")]
    [SerializeField] private GameObject areaTriggerPrefab;

    [Header("���� ��ġ ����")]
    [SerializeField] private List<AreaMapping> areaMappings = new List<AreaMapping>();

    private Dictionary<string, Vector3> areaPositions = new Dictionary<string, Vector3>();
    private Dictionary<string, GameObject> spawnedTriggers = new Dictionary<string, GameObject>(); //  �߰�

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach (var mapping in areaMappings)
        {
            if (!areaPositions.ContainsKey(mapping.areaID))
            {
                areaPositions.Add(mapping.areaID, mapping.position);
            }
        }
    }

    /// <summary>
    /// ������ ���� ID�� ���� Ʈ���Ÿ� ����
    /// </summary>
    public void SpawnTrigger(string areaID)
    {
        if (spawnedTriggers.ContainsKey(areaID))
        {
            Debug.Log($"[QuestAreaTriggerSpawner] �̹� ������: {areaID}");
            return;
        }

        if (!areaPositions.TryGetValue(areaID, out var spawnPos))
        {
            Debug.LogWarning($"[QuestAreaTriggerSpawner] ��ġ ���� ����: {areaID}");
            return;
        }

        if (areaTriggerPrefab == null)
        {
            Debug.LogError("[QuestAreaTriggerSpawner] areaTriggerPrefab�� ��� ����.");
            return;
        }

        GameObject obj = Instantiate(areaTriggerPrefab, spawnPos, Quaternion.identity);
        obj.name = $"AreaTrigger_{areaID}";

        var trigger = obj.GetComponent<QuestAreaTrigger>();
        if (trigger != null)
        {
            trigger.Setup(areaID);
        }

        spawnedTriggers.Add(areaID, obj); //  ���
        Debug.Log($"[QuestAreaTriggerSpawner] Ʈ���� ���� �Ϸ�: {areaID}");
    }

    /// <summary>
    /// Ʈ���� ����
    /// </summary>
    public void RemoveTrigger(string areaID)
    {
        if (spawnedTriggers.TryGetValue(areaID, out var obj))
        {
            Destroy(obj);
            spawnedTriggers.Remove(areaID);
            Debug.Log($"[QuestAreaTriggerSpawner] Ʈ���� ���ŵ�: {areaID}");
        }
        else
        {
            Debug.LogWarning($"[QuestAreaTriggerSpawner] ���� ��� ����: {areaID}");
        }
    }

    [System.Serializable]
    public class AreaMapping
    {
        public string areaID;
        public Vector3 position;
    }
}