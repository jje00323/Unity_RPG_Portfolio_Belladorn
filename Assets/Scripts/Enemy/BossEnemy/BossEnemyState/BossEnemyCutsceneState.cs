using System.Collections;
using UnityEngine;
using Cinemachine;

public class BossEnemyCutsceneState : BossEnemyState
{
    private CinemachineVirtualCamera cutsceneCam;
    private CinemachineDollyCart dollyCart;
    private CinemachineVirtualCamera mainCam;

    public BossEnemyCutsceneState(BossEnemyFSM boss) : base(boss) { }

    public override void Enter()
    {
        Debug.Log("[CutsceneState] �ƾ� ����");

        InitializeCutsceneObjects(); // �� ���� ���� ����

        if (cutsceneCam != null)
            cutsceneCam.gameObject.SetActive(true); // �� ��Ȱ��ȭ ���

        BossEnemyCutsceneController.Instance.FadeOut(0.5f, () =>
        {
            TeleportBossToTransitionPoint();
            HideAllUI();

            SetCameraPriority(10, 100);  // MainVCam = 10, CutsceneVCam = 100
            SetLookAtTarget();

            if (dollyCart != null)
            {
                dollyCart.m_Position = 0f;
                dollyCart.m_Speed = 3f; // �ʿ� �� 1.5f�� ����
            }

            BossEnemyCutsceneController.Instance.FadeIn(0.5f, () =>
            {
                boss.Animator.speed = 0.6f;
                boss.Animator.SetTrigger("CutScene");

                boss.StartCoroutine(PlayCutscene());
            });
        });
    }

    private void InitializeCutsceneObjects()
    {
        // ������ �������� ������ ������Ʈ �̸� ����
        Transform trackRoot = GameObject.Find("BossCutSceneTrack(Clone)")?.transform;
        if (trackRoot != null)
        {
            cutsceneCam = trackRoot.Find("BossCutsceneVCam")?.GetComponent<CinemachineVirtualCamera>();
            dollyCart = trackRoot.Find("Dolly Cart")?.GetComponent<CinemachineDollyCart>();
        }

        mainCam = GameObject.Find("MainVCam")?.GetComponent<CinemachineVirtualCamera>();
    }

    private IEnumerator PlayCutscene()
    {
        // �ƾ� �ִϸ��̼� ���۱��� ���
        yield return new WaitUntil(() =>
            boss.Animator.GetCurrentAnimatorStateInfo(0).IsName("CutScene"));
        Debug.Log("[CutsceneState] �ִϸ��̼� CutScene ���۵�");

        // �ƾ� �ִϸ��̼� ��� �ð� ���
        yield return new WaitForSeconds(boss.bossData.cutsceneDuration);

        // ���̵� �ƿ� �� ī�޶� ��ȯ
        BossEnemyCutsceneController.Instance.FadeOut(0.5f, () =>
        {
            SetCameraPriority(100, 10); // CutsceneCam �� MainCam
            if (dollyCart != null) dollyCart.m_Speed = 0f;

            ShowAllUI();

            // ���̵� �� �Ϸ� �� 1�� ����ϰ� Idle ���� ����
            BossEnemyCutsceneController.Instance.FadeIn(0.5f, () =>
            {
                boss.StartCoroutine(WaitAndEnterIdle());
            });
        });
    }
    private IEnumerator WaitAndEnterIdle()
    {
        yield return new WaitForSeconds(1f); // ī�޶� ��ȯ �Ϸ� �� �߰� ��� �ð�

        boss.cutsceneTriggered = false;
        boss.Animator.speed = 1.0f;
        boss.ChangeState(boss.idleState);
    }

    private void TeleportBossToTransitionPoint()
    {
        Transform trans = GameObject.Find("BossTransition")?.transform;
        if (trans != null)
        {
            boss.transform.position = trans.position;
            boss.transform.rotation = trans.rotation;
            Debug.Log("[CutsceneState] ���� ��ġ ��ȯ �Ϸ�");
        }
        else
        {
            Debug.LogWarning("[CutsceneState] BossTransition ������Ʈ�� ã�� �� �����ϴ�.");
        }
    }

    private void SetCameraPriority(int mainPriority, int cutscenePriority)
    {
        if (mainCam != null) mainCam.Priority = mainPriority;
        if (cutsceneCam != null) cutsceneCam.Priority = cutscenePriority;
    }

    private void SetLookAtTarget()
    {
        Transform lookAt = FindDeepChild(boss.transform, "LookAtTarget");
        if (cutsceneCam != null && lookAt != null)
            cutsceneCam.LookAt = lookAt;
    }

    private Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == name)
                return child;
        }
        return null;
    }

    private void HideAllUI()
    {
        BossEnemyCutsceneController.Instance.HideAll();
    }

    private void ShowAllUI()
    {
        BossEnemyCutsceneController.Instance.ShowAll();
    }

    public override void Exit()
    {
        if (dollyCart != null)
            dollyCart.m_Speed = 0f;
    }

    public override void Update() { }
}
