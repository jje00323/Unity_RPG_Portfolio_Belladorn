using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Camera))]
public class PreviewCameraSetup : MonoBehaviour
{
    private Camera cam;
    private RectTransform referenceRect; // �߰�

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("[PreviewCameraSetup] ī�޶� ������Ʈ�� �����ϴ�.");
            return;
        }

        var urpData = GetComponent<UniversalAdditionalCameraData>();
        if (urpData != null)
        {
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0, 0, 0, 0); // ���� ����
        }
        else
        {
            Debug.LogWarning("[PreviewCameraSetup] URP �����Ͱ� �����ϴ�. (���� ����)");
        }

    }

    void Update()
    {
        if (referenceRect == null) return;

        float height = referenceRect.rect.height;
        float width = referenceRect.rect.width;
        float aspect = width / height;

        cam.orthographicSize = (height / 2f) / 100f;
        // 100�� Canvas ������ ���Ϳ� ���� ���� �ʿ�
    }

    public void SetReferenceRect(RectTransform rect)
    {
        referenceRect = rect;
    }
}