using UnityEngine;

namespace Frustration
{
    // Звук без аудиофайлов: клип собирается из синусоид и шума прямо в коде.
    // Это временная заглушка и заодно первый урок звука: низкий гул из пары близких частот
    // даёт "биения" (медленное колыхание громкости), а шум делает звук живым.
    [RequireComponent(typeof(AudioSource))]
    public class ProceduralAmbience : MonoBehaviour
    {
        public enum Kind { Drone, FridgeHum, TvStatic, ClockTick }

        public Kind kind = Kind.Drone;
        [Range(0f, 1f)] public float volume = 0.4f;
        public float clipSeconds = 8f;

        void Start()
        {
            const int rate = 44100;
            if (kind == Kind.ClockTick) clipSeconds = 2f; // "тик" и "так" по секунде
            int samples = Mathf.RoundToInt(rate * clipSeconds);
            var data = new float[samples];
            var rng = new System.Random(1957);
            float smooth = 0f;

            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)rate;
                float noise = (float)(rng.NextDouble() * 2.0 - 1.0);
                float s;
                switch (kind)
                {
                    case Kind.FridgeHum:
                        // Сетевой гул 50 Гц и его гармоника.
                        s = 0.5f * Mathf.Sin(2f * Mathf.PI * 50f * t) + 0.25f * Mathf.Sin(2f * Mathf.PI * 100f * t) + 0.05f * noise;
                        break;
                    case Kind.ClockTick:
                    {
                        // Короткий щелчок в начале каждой секунды: шум под быстро гаснущей огибающей.
                        // "Тик" выше, "так" ниже.
                        float local = t % 1f;
                        float pitch = t < 1f ? 2400f : 1700f;
                        float envelope = Mathf.Exp(-local * 700f);
                        s = envelope * (0.6f * noise + 0.6f * Mathf.Sin(2f * Mathf.PI * pitch * t));
                        break;
                    }
                    case Kind.TvStatic:
                        // Шипение: шум, слегка сглаженный, с дрожанием громкости.
                        smooth = Mathf.Lerp(smooth, noise, 0.6f);
                        s = smooth * (0.7f + 0.3f * Mathf.Sin(2f * Mathf.PI * 0.5f * t));
                        break;
                    default:
                        // Низкий гул: 55 и 55.5 Гц дают биения раз в две секунды, 82.4 Гц добавляет квинту.
                        smooth = Mathf.Lerp(smooth, noise, 0.02f);
                        float lfo = 0.75f + 0.25f * Mathf.Sin(2f * Mathf.PI * t / clipSeconds);
                        s = lfo * (0.4f * Mathf.Sin(2f * Mathf.PI * 55f * t)
                                 + 0.35f * Mathf.Sin(2f * Mathf.PI * 55.5f * t)
                                 + 0.15f * Mathf.Sin(2f * Mathf.PI * 82.4f * t))
                            + 0.6f * smooth;
                        break;
                }
                data[i] = s * 0.5f;
            }

            // Короткий кроссфейд конца в начало, чтобы при зацикливании не было щелчка.
            int fadeLength = rate / 10;
            for (int i = 0; i < fadeLength; i++)
            {
                float k = i / (float)fadeLength;
                data[samples - fadeLength + i] = Mathf.Lerp(data[samples - fadeLength + i], data[i], k);
            }

            var clip = AudioClip.Create(kind.ToString(), samples, 1, rate, false);
            clip.SetData(data, 0);
            var source = GetComponent<AudioSource>();
            source.clip = clip;
            source.loop = true;
            source.volume = volume;
            source.Play();
        }
    }
}
