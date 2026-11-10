using UnityEngine;

namespace Frustration
{
    // Копия лампочки в зеркальной комнате берёт материал оригинала (горит или погашена).
    public class RendererSync : MonoBehaviour
    {
        public Renderer source;
        Renderer own;

        void Awake() => own = GetComponent<Renderer>();

        void LateUpdate()
        {
            if (source != null && own != null && own.sharedMaterial != source.sharedMaterial)
                own.sharedMaterial = source.sharedMaterial;
        }
    }
}
