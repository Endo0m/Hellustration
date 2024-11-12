using UnityEngine;
using Cinemachine;

public class MirrorDisplay : MonoBehaviour
{
    public Camera mirrorCamera; // ������, ������� ����� ��������� �����������
    public RenderTexture renderTexture; // RenderTexture, �� ������� ����� ��������� ������
    public Material mirrorMaterial; // ��������, ����������� � ������� �������

    void Start()
    {
        if (mirrorCamera != null && renderTexture != null)
        {
            // ������������� RenderTexture ��� ������
            mirrorCamera.targetTexture = renderTexture;
        }

        if (mirrorMaterial != null && renderTexture != null)
        {
            // ������������� RenderTexture ��� �������� ��������� �������
            mirrorMaterial.mainTexture = renderTexture;
        }
    }
}
