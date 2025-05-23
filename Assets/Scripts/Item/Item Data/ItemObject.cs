using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemObject : MonoBehaviour
{
    [Header("������ ������")]
    public ItemData itemData;
    public int quantity = 1;

    [Header("ȸ�� �� ���� ȿ��")]
    [SerializeField] private bool useFloatingEffect = true;
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 45f, 0f);
    [SerializeField] private float floatAmplitude = 0.25f;
    [SerializeField] private float floatFrequency = 1f;

    [Header("��� ����Ʈ")]
    [SerializeField] private GameObject rarityEffect; // ���� ����Ʈ ������Ʈ

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;

        // ������ ���� (SpriteRenderer��)
        var sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null && itemData != null && itemData.icon != null)
        {
            sr.sprite = itemData.icon;
        }

        // ��� ����Ʈ ���� ����
        if (rarityEffect != null && itemData != null)
        {
            Color color = GetColorByRarity(itemData.rarity);

            // 1) Particle System
            var ps = rarityEffect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                main.startColor = color;
            }

            // 2) Renderer (��: Glow Mesh)
            var renderer = rarityEffect.GetComponent<Renderer>();
            if (renderer != null && renderer.material.HasProperty("_Color"))
            {
                renderer.material.color = color;
            }

            // 3) Light
            var light = rarityEffect.GetComponent<Light>();
            if (light != null)
            {
                light.color = color;
            }
        }
    }

    private void Update()
    {
        if (!useFloatingEffect) return;

        // ȸ��
        transform.Rotate(rotationSpeed * Time.deltaTime);

        // ����
        float newY = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = startPos + new Vector3(0f, newY, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (itemData == null)
        {
            Debug.LogError("[ItemObject] itemData�� null�Դϴ�.");
            return;
        }

        var inventory = InventoryManager.Instance;
        var quickSlots = FindObjectsOfType<QuickSlotUI>();

        // �Һ� �������� ���, �����Կ��� ���� Ȯ��
        if (itemData.itemType == ItemType.Consumable)
        {
            foreach (var slot in quickSlots)
            {
                if (slot.GetItem() == itemData)
                {
                    InventoryManager.Instance.AddItem(itemData, quantity);
                    slot.RefreshSlotUI();
                    Debug.Log($"[ItemObject] ������ ���� ����: {itemData.itemName}");
                    Destroy(gameObject);
                    return;
                }
            }
        }

        // �κ��丮�� �߰�
        if (inventory != null)
        {
            bool added = inventory.AddItem(itemData, quantity);
            if (added)
            {
                Debug.Log($"[ItemObject] �κ��丮�� �߰�: {itemData.itemName} x{quantity}");
                var inventoryUI = FindObjectOfType<InventoryUI>();
                if (inventoryUI != null)
                    inventoryUI.RefreshAllSlots();

                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("[ItemObject] �κ��丮 �߰� ����");
            }
        }
    }

    private Color GetColorByRarity(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Normal: return Color.white;
            case ItemRarity.Rare: return Color.blue;
            case ItemRarity.Epic: return new Color(0.6f, 0f, 0.8f); // �����
            default: return Color.gray;
        }
    }
}