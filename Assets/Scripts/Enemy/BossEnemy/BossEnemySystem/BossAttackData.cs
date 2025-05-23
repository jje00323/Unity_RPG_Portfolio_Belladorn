using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boss/Attack Pattern Data")]
public class BossAttackData : ScriptableObject
{
    public int attackIndex;

    [System.Serializable]
    public class HitboxInfo
    {
        public GameObject hitboxPrefab;
        public bool followCaster = false;
    }

    [Header("��Ʈ�ڽ� ����")]
    public List<HitboxInfo> hitboxInfos;

    [System.Serializable]
    public class TelegraphInfo
    {
        public GameObject telegraphPrefab;
        public BossHitbox.ShapeType shapeType; // ���� ShapeType �� BossHitbox.ShapeType �� ����
    }

    [Header("��� ���� ������")]
    public List<TelegraphInfo> telegraphInfos;
}