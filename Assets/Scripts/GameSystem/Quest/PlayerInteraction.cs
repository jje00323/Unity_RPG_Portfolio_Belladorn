using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject interactUI;

    private NPCInteraction currentNPC;

    private PlayerControls inputActions;

    private void Awake()
    {
        inputActions = new PlayerControls();

        // G Ű�� �ش��ϴ� Talk_NPC �׼� ����
        inputActions.UI.Talk_NPC.performed += ctx =>
        {
            Debug.Log("[Input] G Ű(Talk_NPC) �Է� ������");

            if (currentNPC != null)
            {
                Debug.Log($"[Input] ��ȭ ���� �õ� �� ���: {currentNPC.name}");
                currentNPC.Interact();
            }
            else
            {
                Debug.LogWarning("[Input] ���� ��ȣ�ۿ� ������ NPC ����");
            }
        };
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.UI.Enable(); //  �̰��� ������ Talk_NPC �۵� �� ��
    }
    private void OnDisable()
    {
        inputActions.Player.Disable();
        inputActions.UI.Disable();
    }

    private void Start()
    {
        if (interactUI != null)
            interactUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[TriggerEnter] �浹�� ������Ʈ: {other.name}");

        if (other.TryGetComponent(out NPCInteraction npc))
        {
            currentNPC = npc;
            Debug.Log($"[PlayerInteraction] NPC ������: {npc.name}");

            if (interactUI != null)
                interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out NPCInteraction npc) && npc == currentNPC)
        {
            currentNPC = null;

            if (interactUI != null)
                interactUI.SetActive(false);
        }
    }
}