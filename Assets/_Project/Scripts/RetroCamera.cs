using UnityEngine;

namespace Frustration
{
    // Главный приём стиля PS1: игра рендерится в маленькую текстуру (по умолчанию 240 пикселей
    // в высоту) без сглаживания, а потом растягивается на весь экран. Отсюда крупные пиксели.
    [RequireComponent(typeof(Camera))]
    public class RetroCamera : MonoBehaviour
    {
        public int verticalResolution = 240;

        Camera cam;
        RenderTexture target;
        int lastWidth, lastHeight;

        void Awake() => cam = GetComponent<Camera>();

        void LateUpdate()
        {
            if (Screen.width == lastWidth && Screen.height == lastHeight && target != null) return;
            lastWidth = Screen.width;
            lastHeight = Screen.height;

            int height = Mathf.Max(64, verticalResolution);
            int width = Mathf.Max(64, Mathf.RoundToInt(height * (float)Screen.width / Mathf.Max(1, Screen.height)));

            if (target != null) { cam.targetTexture = null; target.Release(); Destroy(target); }
            target = new RenderTexture(width, height, 24) { filterMode = FilterMode.Point, name = "RetroTarget" };
            target.Create();
            cam.targetTexture = target;
            if (GameUI.Instance != null) GameUI.Instance.SetGameTexture(target);
        }

        void OnDestroy()
        {
            if (target != null) { target.Release(); Destroy(target); }
        }
    }
}
