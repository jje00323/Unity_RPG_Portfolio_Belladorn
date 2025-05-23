using TMPro;
using UnityEngine;
using System.Collections;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance;

    [SerializeField] private GameObject narrationPanel;
    [SerializeField] private TextMeshProUGUI narrationText;

    private bool isNarrationActive = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("[DialogueSystem] �̱��� �ν��Ͻ� ��� ����");
        }
        else
        {
            Debug.LogWarning("[DialogueSystem] �ߺ� �ν��Ͻ� �߰�, �ı���");
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (isNarrationActive && Input.GetKeyDown(KeyCode.F))
        {
            CloseNarration();
        }
    }

    public void ShowNarration(string text)
    {
        if (narrationPanel == null)
        {
            Debug.LogError("[DialogueSystem] narrationPanel�� ������� �ʾҽ��ϴ�.");
            return;
        }
        if (narrationText == null)
        {
            Debug.LogError("[DialogueSystem] narrationText�� ������� �ʾҽ��ϴ�.");
            return;
        }

        narrationText.text = text;
        narrationPanel.SetActive(true);
        isNarrationActive = true;
        Debug.Log("[DialogueSystem] �����̼� ���۵�");
    }

    public void CloseNarration()
    {
        narrationPanel.SetActive(false);
        isNarrationActive = false;
        Debug.Log("[DialogueSystem] �����̼� ���� (F Ű)");
    }
}