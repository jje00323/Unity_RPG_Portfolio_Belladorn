using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        InitializeSystems();
    }

    private void InitializeSystems()
    {
        if (SkillUIManager.Instance != null)
            SkillUIManager.Instance.InitializeSkillDataOnly();  // UI�� ���� �־ ����

        // �ٸ� �ý��۵� ���⿡ �߰� ����
        // InventoryManager.Instance.Initialize();
    }
}