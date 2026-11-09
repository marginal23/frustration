using UnityEngine;

namespace Frustration
{
    // Копия предмета в зеркальной комнате исчезает вместе с оригиналом
    // (например, когда герой берёт ранец со стула).
    public class ActiveSync : MonoBehaviour
    {
        public GameObject source;

        void LateUpdate()
        {
            if (source != null && !source.activeInHierarchy) gameObject.SetActive(false);
        }
    }
}
