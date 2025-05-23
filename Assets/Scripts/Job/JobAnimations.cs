using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobAnimations : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator animator;
    private JobManager.JobType currentJob;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (JobManager.Instance == null)
        {
            Debug.LogError("JobManager.Instance�� null�Դϴ�! JobManager�� ���� �ִ��� Ȯ���ϼ���.");
            return;
        }

        currentJob = JobManager.Instance.GetCurrentJob(); //  Awake()���� ����
        Debug.Log($"JobAnimations Awake - ���� ����: {currentJob}");
    }

    //�̵� �ִϸ��̼� ����
    public void PlayMovementAnimation(float speed)
    {
        //Debug.Log("PlayMovementAnimation() ȣ���. Speed: " + speed);

        if (JobManager.Instance == null) return;

        if (currentJob == JobManager.JobType.Basic)
        {
            animator.SetFloat("Speed_Basic", speed);
            //Debug.Log($"Basic ���� - Speed_Basic �� ����: {speed}");
        }
        else if (currentJob == JobManager.JobType.Warrior)
        {
            animator.SetFloat("Speed_Warrior", speed);
            Debug.Log($"Warrior ���� - Speed_Warrior �� ����: {speed}");
        }
        else if (JobManager.Instance.GetCurrentJob() == JobManager.JobType.Mage)
        {
            animator.SetFloat("Speed_Mage", speed);
            Debug.Log($"Mage ���� - Speed_Mage �� ����: {speed}");
        }
        else if (JobManager.Instance.GetCurrentJob() == JobManager.JobType.Archer)
        {
            animator.SetFloat("Speed_Archer", speed);
            Debug.Log($"Archer ���� - Speed_Archer �� ����: {speed}");
        }
        else
        {
            Debug.LogError($"�� �� ���� ����: {JobManager.Instance.GetCurrentJob()}");
        }
    }

    // Idle ���·� ����
    public void StopMovementAnimation()
    {
        PlayMovementAnimation(0); // �̵� �ӵ��� 0���� �����Ͽ� Idle �ִϸ��̼� ����
    }
}
