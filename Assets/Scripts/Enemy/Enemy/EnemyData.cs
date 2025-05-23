using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("�⺻ ����")]
    public string enemyName;
    public int enemyLevel;

    [Header("����Ʈ ���ǿ� �±�")] //  �߰�
    public string enemyTag;

    [Header("�ִϸ��̼�")]
    public RuntimeAnimatorController animatorController;

    [Header("�ɷ�ġ")]
    public float maxHP;
    public float moveSpeed;
    public float attackPower;

    [Header("���� ����")]
    public float attackCooldown;
    public float attackRange;

    [Header("AI ����")]
    public float detectRadius;
    public float returnDistance;
    public float returnSpeedMultiplier;

    [Header("����")]
    public int expDrop;

    
    [Header("��� ������")]
    public List<DropItemData> dropItems = new List<DropItemData>();
}


[System.Serializable]
public class DropItemData
{
    public GameObject itemPrefab;                  // ������ ������ (ItemObject ����)
    [Range(0f, 100f)] public float dropChance = 100f; // Ȯ�� %
    public GameObject dropEffectPrefab;            // ��� ����Ʈ (����)
}