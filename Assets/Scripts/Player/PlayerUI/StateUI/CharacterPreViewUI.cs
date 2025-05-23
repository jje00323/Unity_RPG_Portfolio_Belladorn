using UnityEngine;
using System.Collections;

public class CharacterPreviewUI : MonoBehaviour
{
    [Header("������ ī�޶� ����")]
    public Transform previewCameraTargetParent; // ������ ���� �θ�

    [Header("Animator")]
    public Animator previewAnimator;

    private GameObject currentPreviewModel;
    private JobManager.JobType lastJobType;

    private void OnEnable()
    {
        if (JobManager.Instance != null)
            JobManager.Instance.OnJobChanged += OnJobChanged;

        StartCoroutine(LoadPreviewDelayed());
    }

    private void OnDisable()
    {
        if (JobManager.Instance != null)
            JobManager.Instance.OnJobChanged -= OnJobChanged;
    }

    private void OnJobChanged(JobManager.JobType newJob)
    {
        StartCoroutine(LoadPreviewDelayed());
    }

    private IEnumerator LoadPreviewDelayed()
    {
        yield return null;
        LoadCurrentJobModel();
    }

    public void LoadCurrentJobModel()
    {
        if (previewCameraTargetParent == null)
        {
            Debug.LogError("[CharacterPreviewUI] previewCameraTargetParent ����");
            return;
        }

        var job = JobManager.Instance.GetCurrentJob();
        var resources = JobManager.Instance.jobResourcesList.Find(j => j.jobType == job);
        if (resources == null || resources.modelPrefab == null || resources.avatar == null)
        {
            Debug.LogError($"[CharacterPreviewUI] ���ҽ� ����: {job}");
            return;
        }

        if (currentPreviewModel != null)
            Destroy(currentPreviewModel);

        currentPreviewModel = Instantiate(resources.modelPrefab, previewCameraTargetParent);
        currentPreviewModel.transform.localPosition = new Vector3(0, -2.6f, 10);
        currentPreviewModel.transform.localRotation = Quaternion.Euler(0, 180f, 0);
        currentPreviewModel.transform.localScale = Vector3.one * 2.8f;

        int layer = LayerMask.NameToLayer("PreviewModel");
        currentPreviewModel.layer = layer;
        foreach (Transform t in currentPreviewModel.GetComponentsInChildren<Transform>(true))
            t.gameObject.layer = layer;

        // ���� �߰�!!
        Animator modelAnimator = currentPreviewModel.GetComponent<Animator>();
        if (modelAnimator != null)
        {
            modelAnimator.runtimeAnimatorController = resources.animatorController;
            modelAnimator.avatar = resources.avatar;
            modelAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            modelAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
        else
        {
            Debug.LogWarning("[CharacterPreviewUI] ������ �𵨿� Animator�� �����ϴ�.");
        }

        lastJobType = job;
    }

    private IEnumerator PlayIdleNextFrame(string stateName)
    {
        yield return null;
        yield return null;
        previewAnimator.enabled = true;
        previewAnimator.Play(stateName, 0, 0f);
    }

    private string GetIdleStateName(JobManager.JobType job)
    {
        return job switch
        {
            JobManager.JobType.Basic => "Idle_Basic",
            JobManager.JobType.Warrior => "Idle_Warrior",
            JobManager.JobType.Mage => "Idle_Mage",
            JobManager.JobType.Archer => "Idle_Archer",
            _ => "Idle"
        };
    }
}