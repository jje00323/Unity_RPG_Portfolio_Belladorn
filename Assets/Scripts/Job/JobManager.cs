using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobManager : MonoBehaviour
{
    public enum JobType { Basic, Warrior, Mage, Archer }
    public JobType currentJob = JobType.Basic;

    public static JobManager Instance { get; private set; }

    private Dictionary<JobType, DashSettings> dashSettings;
    public List<JobResources> jobResourcesList;
    private Dictionary<JobType, JobResources> jobResourcesDict;

    public JobSkillData[] allJobSkillData;
    private Dictionary<JobType, JobSkillData> skillDataDict;

    public event System.Action<JobType> OnJobChanged;

    [System.Serializable]
    public class JobResources
    {
        public JobManager.JobType jobType;
        public RuntimeAnimatorController animatorController;
        public Avatar avatar;
        public GameObject modelPrefab;
    }

    [Header("���� ���� ��ġ")]
    public Transform modelParent;

    private GameObject currentModel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeJobResources();
            InitializeDashSettings();
            InitializeSkillData();
            InitializeStatData();

            Debug.Log("JobManager �ν��Ͻ� ������!");
        }
        else
        {
            Debug.LogWarning("JobManager �ߺ� ����! ���� �ν��Ͻ��� �����մϴ�.");
            Destroy(gameObject);
        }
    }

    IEnumerator Start()
    {
        yield return null;
        

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null && player.TryGetComponent(out PlayerStatus ps))
        {
            if (ps.stateUI == null)
            {
                ps.stateUI = FindObjectOfType<PlayerStateUI>();
                if (ps.stateUI != null)
                {
                    Debug.Log("[JobManager] Start���� stateUI ���� ���� �Ϸ�");
                    ps.UpdateAllUI();
                }
            }
        }

        ChangeJob(currentJob);
    }

    private void InitializeDashSettings()
    {
        dashSettings = new Dictionary<JobType, DashSettings>
        {
            { JobType.Basic, new DashSettings(5f, 0.2f, 3f, true) },
            { JobType.Warrior, new DashSettings(3f, 0.15f, 4f, false) },
            { JobType.Mage, new DashSettings(7f, 0.3f, 5f, true) },
            { JobType.Archer, new DashSettings(6f, 0.25f, 6f, true) }
        };
    }

    private void InitializeSkillData()
    {
        skillDataDict = new Dictionary<JobType, JobSkillData>();
        foreach (var data in allJobSkillData)
        {
            if (!skillDataDict.ContainsKey(data.jobType))
            {
                skillDataDict[data.jobType] = data;
            }
        }
    }

    public JobSkillData GetSkillData(JobType jobType)
    {
        return skillDataDict.TryGetValue(jobType, out var data) ? data : null;
    }

    private void InitializeJobResources()
    {
        jobResourcesDict = new Dictionary<JobType, JobResources>();
        foreach (var resource in jobResourcesList)
        {
            if (!jobResourcesDict.ContainsKey(resource.jobType))
            {
                jobResourcesDict[resource.jobType] = resource;
            }
        }
    }

    public void ChangeJob(JobType newJob)
    {
        currentJob = newJob;

        if (!jobResourcesDict.ContainsKey(newJob))
        {
            Debug.LogError("�ش� ���� ���ҽ� ����: " + newJob);
            return;
        }

        var res = jobResourcesDict[newJob];
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player ������Ʈ�� ã�� �� �����ϴ�.");
            return;
        }

        if (currentModel != null)
        {
            Destroy(currentModel);
        }

        currentModel = Instantiate(res.modelPrefab, modelParent);
        currentModel.transform.localPosition = Vector3.zero;
        currentModel.transform.localRotation = Quaternion.identity;

        Animator playerAnimator = player.GetComponent<Animator>();
        if (playerAnimator != null)
        {
            playerAnimator.runtimeAnimatorController = res.animatorController;
            playerAnimator.avatar = res.avatar;

            PlayerMovement movement = player.GetComponent<PlayerMovement>();
            if (movement != null)
                movement.UpdateAnimatorReference(playerAnimator);
        }

        PlayerSkillController playerSkill = FindObjectOfType<PlayerSkillController>();
        if (playerSkill != null)
        {
            var skillData = GetSkillData(newJob);
            if (skillData != null)
            {
                playerSkill.LoadSkillsFromData(skillData);
            }
        }

        var stateMachine = player.GetComponent<PlayerStateMachine>();
        if (stateMachine != null)
        {
            stateMachine.ChangeState(PlayerStateMachine.PlayerState.Idle);
        }

        var playerStatus = player.GetComponent<PlayerStatus>();
        var statData = jobStatDict.TryGetValue(newJob, out var data) ? data : null;

        if (playerStatus != null && statData != null)
        {
            if (playerStatus.stateUI == null)
            {
                playerStatus.stateUI = FindObjectOfType<PlayerStateUI>();
                if (playerStatus.stateUI == null)
                {
                    Debug.LogWarning("[JobManager] PlayerStateUI�� ã�� �� �����ϴ�. UI ������ ������ �� �� �ֽ��ϴ�.");
                }
                else
                {
                    Debug.Log("[JobManager] stateUI�� ������ �����߽��ϴ�.");
                }
            }

            playerStatus.ApplyJobStats(statData);
            playerStatus.SetJob(newJob);
            playerStatus.UpdateAllUI();
        }
        else
        {
            Debug.LogWarning("[JobManager] PlayerStatus �Ǵ� ���� �����͸� ã�� �� �����ϴ�: " + newJob);
        }

        var playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
            playerMovement.UpdateAnimatorReference(playerAnimator);

        Debug.Log($"[���� ���� �Ϸ�] {newJob}");

        StartCoroutine(InvokeJobChangedDelayed(newJob));
    }

    private IEnumerator InvokeJobChangedDelayed(JobType newJob)
    {
        yield return null;
        OnJobChanged?.Invoke(newJob);
    }

    public DashSettings GetDashSettings()
    {
        return dashSettings[currentJob];
    }

    public JobType GetCurrentJob()
    {
        return currentJob;
    }

    public void ChangeToBasic() => ChangeJob(JobType.Basic);
    public void ChangeToWarrior() => ChangeJob(JobType.Warrior);
    public void ChangeToMage() => ChangeJob(JobType.Mage);
    public void ChangeToArcher() => ChangeJob(JobType.Archer);

    public JobStatusData[] allJobStatData;
    private Dictionary<JobType, JobStatusData> jobStatDict;

    private void InitializeStatData()
    {
        jobStatDict = new Dictionary<JobType, JobStatusData>();
        foreach (var data in allJobStatData)
        {
            if (!jobStatDict.ContainsKey(data.jobType))
            {
                jobStatDict[data.jobType] = data;
            }
        }
    }

    public JobStatusData GetStatData(JobType jobType)
    {
        return jobStatDict.TryGetValue(jobType, out var data) ? data : null;
    }
}
