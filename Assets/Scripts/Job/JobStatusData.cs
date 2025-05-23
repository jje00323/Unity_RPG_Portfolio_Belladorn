using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewJobStat", menuName = "Job/Job Stat Data")]
public class JobStatusData : ScriptableObject
{
    public JobManager.JobType jobType;

    [Header("�⺻ ����")]
    public float baseHP;
    public float baseMP;
    public float baseAttack;
    public float baseDefense;

    [Header("Ư�� ����")]
    public float critical;
    public float critical_Damage;
    public float Someting_Else;


    [Header("���� �� ������")]
    public float hpPerLevel;
    public float mpPerLevel;
    public float attackPerLevel;
    public float defensePerLevel;


   
}