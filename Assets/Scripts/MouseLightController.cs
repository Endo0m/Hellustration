using UnityEngine;
using UnityEngine.Rendering.Universal; 

public class MouseLightController : MonoBehaviour
{
    [SerializeField] private Light2D spotlight; // Ссылка на Light2D объект

    private void Update()
    {
        // Получаем позицию мыши в мировых координатах
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Устанавливаем свету позицию мыши (Z остаётся 0, так как это 2D)
        spotlight.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0f);
    }
}
