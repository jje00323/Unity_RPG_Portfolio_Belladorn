using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBossEnemyData", menuName = "Enemy/BossEnemyData")]
public class BossEnemyData : ScriptableObject
{
    [Header("�⺻ ����")]
    public string bossName;
    public int bossLevel;
    public Sprite portrait;

    [Header("����")]
    public float maxHP;
    public int hpBarCount;
    public float moveSpeed;
    public float attackPower;

    [Header("�ִϸ��̼�")]
    public RuntimeAnimatorController animatorController;

    [Header("���� ��ȯ ����")]
    public float[] phaseThresholds; // ex) [0.7f, 0.4f] �� ü�� 70%, 40%�� �� �ƾ� �߻�

    [Header("�ó׸�ƽ �ƾ� ������")]
    public string[] cutsceneDialogues;
    public AudioClip[] cutsceneVoiceClips;
    public GameObject cutsceneVFXPrefab;
    public float cutsceneDuration = 4f;

    [Header("����")]
    public int expReward;
    public int goldReward;
    public GameObject[] itemDrops; // ��� ������ ������

    [Header("��Ÿ")]
    public AudioClip bossThemeMusic;
}
