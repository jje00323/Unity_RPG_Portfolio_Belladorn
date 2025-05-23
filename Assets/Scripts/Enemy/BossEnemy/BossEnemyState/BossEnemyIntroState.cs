using System.Collections;
using UnityEngine;
using Cinemachine;

public class BossEnemyIntroState : BossEnemyState
{
    private CinemachineVirtualCamera introCam;
    private CinemachineVirtualCamera mainCam;

    public BossEnemyIntroState(BossEnemyFSM boss) : base(boss) { }

    public override void Enter()
    {
        Debug.Log("[IntroState] ���� ��Ʈ�� ����");

        InitializeIntroCameras();

        if (introCam == null || mainCam == null)
        {
            Debug.LogWarning("[IntroState] ��Ʈ��/���� ī�޶� �����ϴ�.");
            boss.ChangeState(boss.idleState);
            return;
        }

        // Step 1: Fade Out �� UI ����� ī�޶� ��ȯ
        BossEnemyIntroController.Instance.FadeOut(0.5f, () =>
        {
            SetCameraPriority(introCam, 100);
            SetCameraPriority(mainCam, 10);

            BossEnemyIntroController.Instance.HideAll();

            // Step 2: Fade In �� �ִϸ��̼� ����
            BossEnemyIntroController.Instance.FadeIn(0.5f, () =>
            {
                boss.Animator.SetTrigger("Intro");

                // Step 3: ���� ���� �� ���� ��ȯ
                boss.StartCoroutine(WaitAndEnterIdle());
            });
        });
    }

    private void InitializeIntroCameras()
    {
        Transform trackRoot = GameObject.Find("BossIntroTrack(Clone)")?.transform;
        if (trackRoot != null)
            introCam = trackRoot.Find("BossIntroVCam")?.GetComponent<CinemachineVirtualCamera>();

        mainCam = GameObject.Find("MainVCam")?.GetComponent<CinemachineVirtualCamera>();
    }

    private IEnumerator WaitAndEnterIdle()
    {
        yield return new WaitForSeconds(4f); // ��Ʈ�� ���� �ð�

        // Step 4: Fade Out �� ���� ī�޶� ����
        BossEnemyIntroController.Instance.FadeOut(0.5f, () =>
        {
            SetCameraPriority(introCam, 10);
            SetCameraPriority(mainCam, 100);

            // Step 5: Fade In �� UI ���� �� Idle ���� ����
            BossEnemyIntroController.Instance.FadeIn(0.5f, () =>
            {
                BossEnemyIntroController.Instance.ShowAll();
                boss.ChangeState(boss.idleState);
            });
        });
    }

    private void SetCameraPriority(CinemachineVirtualCamera cam, int priority)
    {
        if (cam != null) cam.Priority = priority;
    }

    public override void Update() { }

    public override void Exit() { }
}
