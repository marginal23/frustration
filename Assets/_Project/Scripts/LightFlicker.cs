using UnityEngine;

namespace Frustration
{
    // Мерцание лампы или света от телевизора: яркость скачет по шуму Перлина,
    // иногда лампа ненадолго почти гаснет.
    [RequireComponent(typeof(Light))]
    public class LightFlicker : MonoBehaviour
    {
        public float speed = 8f;
        [Range(0f, 1f)] public float amount = 0.3f;
        [Range(0f, 1f)] public float dropoutChance = 0.002f;

        Light lightSource;
        float baseIntensity;
        float seed;
        float dropout;

        void Awake()
        {
            lightSource = GetComponent<Light>();
            baseIntensity = lightSource.intensity;
            seed = Random.value * 100f;
        }

        void Update()
        {
            if (Random.value < dropoutChance) dropout = 0.15f;
            dropout = Mathf.Max(0f, dropout - Time.deltaTime);
            float n = Mathf.PerlinNoise(seed, Time.time * speed);
            float k = 1f - amount + amount * 2f * n;
            lightSource.intensity = baseIntensity * k * (dropout > 0f ? 0.2f : 1f);
        }
    }
}
