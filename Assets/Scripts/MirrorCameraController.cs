using UnityEngine;
using Cinemachine;

public class MirrorDisplay : MonoBehaviour
{
    public Camera mirrorCamera; //  амера, котора€ будет рендерить изображение
    public RenderTexture renderTexture; // RenderTexture, на который будет рендерить камера
    public Material mirrorMaterial; // ћатериал, примененный к спрайту зеркала

    void Start()
    {
        if (mirrorCamera != null && renderTexture != null)
        {
            // ”станавливаем RenderTexture дл€ камеры
            mirrorCamera.targetTexture = renderTexture;
        }

        if (mirrorMaterial != null && renderTexture != null)
        {
            // ”станавливаем RenderTexture как текстуру материала зеркала
            mirrorMaterial.mainTexture = renderTexture;
        }
    }
}
