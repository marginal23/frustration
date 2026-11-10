using UnityEngine;

namespace Frustration
{
    // Копия в зеркальной комнате повторяет поворот оригинала (например, открытую дверь).
    public class TransformSync : MonoBehaviour
    {
        public Transform source;

        void LateUpdate()
        {
            if (source != null) transform.localRotation = source.localRotation;
        }
    }
}
