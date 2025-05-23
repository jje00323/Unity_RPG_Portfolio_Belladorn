using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyCutsceneController : MonoBehaviour
{
    public static BossEnemyCutsceneController Instance;

    [Header("���ƿ� ��������")]
    [SerializeField] private CanvasGroup overlayGroup;

    [Header("�÷��̾� ����")]
    [SerializeField] private PlayerInputHandler playerInputHandler;

    [Header("UI ��ҵ�")]
    [SerializeField] private CanvasGroup playerUIGroup;
    [SerializeField] private CanvasGroup bossUIGroup;
    [SerializeField] private List<GameObject> uiToBackupAndToggleDuringCutscene;

    private Dictionary<GameObject, bool> uiActiveStates = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {

    }

    // ���ƿ�
    public void FadeOut(float duration, System.Action onComplete = null)
    {
        StartCoroutine(Fade(1f, duration, onComplete));
    }

    // ���� ����
    public void FadeIn(float duration, System.Action onComplete = null)
    {
        StartCoroutine(Fade(0f, duration, onComplete));
    }

    private IEnumerator Fade(float targetAlpha, float duration, System.Action onComplete)
    {
        float startAlpha = overlayGroup.alpha;
        float time = 0f;

        while (time < duration)
        {
            overlayGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        overlayGroup.alpha = targetAlpha;
        onComplete?.Invoke();
    }

    public void HideAll()
    {
        uiActiveStates.Clear();

        SetCanvasGroup(playerUIGroup, false);
        SetCanvasGroup(bossUIGroup, false);

        foreach (var ui in uiToBackupAndToggleDuringCutscene)
        {
            if (ui != null)
            {
                uiActiveStates[ui] = ui.activeSelf;  // ���� ����
                ui.SetActive(false);                 // ��Ȱ��ȭ
            }
        }

        if (playerInputHandler != null)
            playerInputHandler.enabled = false;
    }

    public void ShowAll()
    {
        SetCanvasGroup(playerUIGroup, true);
        SetCanvasGroup(bossUIGroup, true);

        foreach (var kvp in uiActiveStates)
        {
            if (kvp.Key != null)
                kvp.Key.SetActive(kvp.Value); // ����� ���·� ����
        }

        if (playerInputHandler != null)
            playerInputHandler.enabled = true;
    }

    private void SetCanvasGroup(CanvasGroup group, bool active)
    {
        if (group == null) return;

        group.alpha = active ? 1f : 0f;
        group.interactable = active;
        group.blocksRaycasts = active;
    }
}
