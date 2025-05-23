using System.Collections;
using UnityEngine;
using TMPro;

public class NPCInteraction : MonoBehaviour
{
    [Header("NPC ����")]
    [SerializeField] private string npcID = "TUTORIAL_NPC";
    [TextArea][SerializeField] private string[] dialogueLines;
    [SerializeField] private string npcName = "???";  // Inspector���� ����
    [SerializeField] private TextMeshProUGUI npcNameText;

    [Header("UI ����")]
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private TextMeshProUGUI dialogueText;

    private int currentLine = 0;
    private bool isTalking = false;
    private bool questConditionUpdated = false;

    private void Start()
    {
        if (dialogueUI != null)
            dialogueUI.SetActive(false);
    }

    // ��ȣ�ۿ� (��: ��ư, ���� �� Ű �Է�)
    public void Interact()
    {
        Debug.Log($"[NPCInteraction] Interact() ȣ��� �� npcID: {npcID}");

        if (isTalking) return;

        isTalking = true;
        currentLine = 0;
        questConditionUpdated = false;

        if (npcNameText != null)
            npcNameText.text = npcName;

        dialogueUI.SetActive(true);
        ShowNextLine();
    }

    private void Update()
    {
        if (isTalking && Input.GetKeyDown(KeyCode.F))
        {
            ShowNextLine();
        }
    }

    private void ShowNextLine()
    {
        Debug.Log($"[NPCInteraction] ��ȭ {currentLine + 1}��° �� ǥ�� �õ�");

        if (currentLine < dialogueLines.Length)
        {
            dialogueText.text = dialogueLines[currentLine];
            currentLine++;

            if (!questConditionUpdated)
            {
                Debug.Log($"[NPCInteraction] ����Ʈ ���� ����: {npcID}");
                QuestManager.Instance.UpdateCondition("TalkToNPC", npcID);
                questConditionUpdated = true;
            }
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        isTalking = false;
        dialogueUI.SetActive(false);
        dialogueText.text = "";
    }
}