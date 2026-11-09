using UnityEngine;

namespace Frustration
{
    // Свет в комнате-отражении повторяет яркость своего оригинала,
    // чтобы мигание лампы в зеркале совпадало с настоящим.
    [RequireComponent(typeof(Light))]
    public class LightSync : MonoBehaviour
    {
        public Light source;
        Light target;

        void Awake() => target = GetComponent<Light>();

        void LateUpdate()
        {
            if (source == null) return;
            target.intensity = source.intensity;
            target.color = source.color;
            target.enabled = source.enabled && source.gameObject.activeInHierarchy;
        }
    }
}
