using TMPro;
using UnityEngine;
using System.Collections;
public class SkillUIManager : MonoBehaviour
{
    [Header("��ų ������")]
    [SerializeField] private JobSkillData[] allJobSkills;

    [Header("UI ����")]
    [SerializeField] private Transform skillListArea;
    [SerializeField] private GameObject skillSlotPrefab;
    [SerializeField] private TextMeshProUGUI skillPointText;


    public static SkillUIManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (JobManager.Instance != null)
            JobManager.Instance.OnJobChanged += OnJobChanged;
    }

    private void OnDestroy()
    {
        if (JobManager.Instance != null)
            JobManager.Instance.OnJobChanged -= OnJobChanged;
    }

    private void OnEnable()
    {
        if (JobManager.Instance != null && JobManager.Instance.GetSkillData(JobManager.Instance.GetCurrentJob()) != null)
        {
            StartCoroutine(WaitUntilSkillDataReadyThenLoad());
        }
        else
        {
            Debug.LogWarning("[SkillUIManager] JobManager�� �ʱ�ȭ���� �ʾҰų� skillData�� ����. �ε� ����");
        } // ���� ���� & ��ų ����Ʈ �ؽ�Ʈ ����
        
    }

    private void OnJobChanged(JobManager.JobType newJob)
    {
        if (!gameObject.activeInHierarchy)
        {
            Debug.LogWarning("[SkillUIManager] GameObject�� ���� �־� ��ų UI �ε� ������");
            return;
        }

        LoadSkillListForCurrentJob();
    }

    public void LoadSkillListForCurrentJob()
    {
        var currentJob = JobManager.Instance.GetCurrentJob();

        JobSkillData jobData = null;
        foreach (var data in allJobSkills)
        {
            if (data.jobType == currentJob)
            {
                jobData = data;
                break;
            }
        }

        if (jobData == null)
        {
            Debug.LogWarning("[SkillUIManager] �ش� ������ ��ų �����͸� ã�� �� �����ϴ�.");
            return;
        }

        foreach (Transform child in skillListArea)
        {
            Destroy(child.gameObject);
        }

        SkillSlotUI firstSlot = null;

        foreach (var skill in jobData.skills)
        {

            SkillInfo displaySkill = SkillUpgradeManager.Instance.GetCurrentVersionOf(skill);

            GameObject slotObj = Instantiate(skillSlotPrefab, skillListArea);
            SkillSlotUI slotUI = slotObj.GetComponent<SkillSlotUI>();
            
            if (slotUI == null)
            {
                Debug.LogError("[SkillUIManager] SkillSlotUI ������Ʈ�� �����ϴ�.");
                continue;
            }

            slotUI.SetSlot(displaySkill);

            if (firstSlot == null)
                firstSlot = slotUI;
        }

        if (firstSlot != null)
            StartCoroutine(SelectFirstSlotAfterUIReady(firstSlot));

        SkillUpgradeManager.Instance.AutoLinkUpgradeToBase(allJobSkills);
    }

    public void UpdateSkillPointText()
    {
        int current = SkillPointManager.Instance.GetPoints();
        Debug.Log($"[UI] ���� ��ų ����Ʈ: {current}");
        skillPointText.text = $"��ų ����Ʈ: {current}";
    }

    private IEnumerator SelectFirstSlotAfterUIReady(SkillSlotUI firstSlot)
    {
        yield return null;
        yield return null;


        if (firstSlot == null)
        {
            Debug.LogWarning("[SkillUIManager] firstSlot�� null�Դϴ�.");
            yield break;
        }

        if (SkillUpgradeUI.Instance == null)
        {
            Debug.LogWarning("[SkillUIManager] SkillUpgradeUI.Instance�� null�Դϴ�. ù ��ų ���� ����.");
            yield break;
        }

        firstSlot.OnClickSlot();
    }

    private IEnumerator DelayedInitializeUI()
    {
        yield return null;
        yield return null;
        LoadSkillListForCurrentJob();
        UpdateSkillPointText();
    }

    public void InitializeSkillDataOnly()
    {
        var currentJob = JobManager.Instance.GetCurrentJob();

        JobSkillData jobData = null;
        foreach (var data in allJobSkills)
        {
            if (data.jobType == currentJob)
            {
                jobData = data;
                break;
            }
        }

        if (jobData == null)
        {
            Debug.LogWarning("[SkillUIManager] �ش� ������ ��ų �����͸� ã�� �� �����ϴ�.");
            return;
        }

        // UI�� �������� �ʰ�, �����͸� �غ�
        SkillUpgradeManager.Instance.AutoLinkUpgradeToBase(allJobSkills);
    }

    private IEnumerator WaitUntilSkillDataReadyThenLoad()
    {
        // �ִ� 2~3 �����ӱ��� ��� (�ʿ�� �ð� ���ѵ� �߰� ����)
        for (int i = 0; i < 5; i++)
        {
            if (JobManager.Instance != null &&
                SkillUpgradeManager.Instance != null &&
                JobManager.Instance.GetSkillData(JobManager.Instance.GetCurrentJob()) != null)
            {
                LoadSkillListForCurrentJob();
                UpdateSkillPointText();
                yield break;
            }

            yield return null; // �� ������ ���
            yield return null; // �� ������ ���
            yield return null; // �� ������ ���
            yield return null; // �� ������ ���

        }

        Debug.LogWarning("[SkillUIManager] ��ų �����Ͱ� �غ���� �ʾ� �ʱ�ȭ�� �ǳʶݴϴ�.");
    }

}