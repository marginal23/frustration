using System.Collections.Generic;
using UnityEngine;

namespace Frustration
{
    // Короткие звуки без файлов: шаги, скрип двери, щелчок выключателя, тревожный «удар».
    // Клипы собираются один раз из шума и синусоид и потом переиспользуются.
    // Это заглушки: когда появятся настоящие записи, их можно подставить вместо этих клипов.
    public static class ProceduralSfx
    {
        const int Rate = 44100;
        static readonly Dictionary<string, AudioClip> cache = new Dictionary<string, AudioClip>();

        // Шаг. hard = линолеум (щелчок каблука и глухой стук), иначе ковёр (мягкий шорох).
        public static AudioClip Footstep(bool hard, int variant)
        {
            string key = (hard ? "step_hard_" : "step_soft_") + variant;
            if (cache.TryGetValue(key, out var clip)) return clip;
            var rng = new System.Random(100 + variant * 7 + (hard ? 0 : 50));
            float pitch = 1f + (variant - 1.5f) * 0.06f;
            return cache[key] = Build(key, 0.16f, (t, noise, state) =>
            {
                state[0] = Mathf.Lerp(state[0], noise, hard ? 0.5f : 0.08f);   // фильтр шума: мягче на ковре
                float thump = Mathf.Sin(2f * Mathf.PI * 85f * pitch * t) * Mathf.Exp(-t * 35f);
                if (hard)
                {
                    float click = state[0] * Mathf.Exp(-t * 90f);
                    float scuff = state[0] * 0.25f * Mathf.Exp(-(t - 0.04f) * (t - 0.04f) * 4000f);
                    return 0.55f * click + 0.45f * thump + scuff;
                }
                return 0.7f * state[0] * Mathf.Exp(-t * 28f) + 0.25f * thump;
            }, rng);
        }

        // Скрип петель: «прилипание и срыв» — частые щелчки с плавающей частотой через резонанс.
        public static AudioClip Creak()
        {
            const string key = "creak";
            if (cache.TryGetValue(key, out var clip)) return clip;
            var rng = new System.Random(77);
            float phase = 0f;
            return cache[key] = Build(key, 0.9f, (t, noise, state) =>
            {
                float rate = 70f + 90f * Mathf.Sin(t * 3.2f) * Mathf.Sin(t * 1.3f + 0.5f) + 60f * t;
                phase += rate / Rate;
                float pulse = Mathf.Exp(-(phase % 1f) * 9f);
                float body = Mathf.Sin(2f * Mathf.PI * (650f + 200f * Mathf.Sin(t * 2f)) * t) + 0.4f * Mathf.Sin(2f * Mathf.PI * 1430f * t);
                float envelope = Mathf.Clamp01(t * 8f) * Mathf.Clamp01((0.9f - t) * 5f);
                return envelope * (0.6f * pulse * body + 0.08f * noise * pulse);
            }, rng);
        }

        // Щелчок выключателя или ручки радио.
        public static AudioClip Click()
        {
            const string key = "click";
            if (cache.TryGetValue(key, out var clip)) return clip;
            var rng = new System.Random(5);
            return cache[key] = Build(key, 0.06f, (t, noise, state) =>
                noise * Mathf.Exp(-t * 400f) * 0.6f + Mathf.Sin(2f * Mathf.PI * 2600f * t) * Mathf.Exp(-t * 300f) * 0.5f, rng);
        }

        // Тревожный низкий удар с гудящим хвостом — для момента у зеркала.
        public static AudioClip Sting()
        {
            const string key = "sting";
            if (cache.TryGetValue(key, out var clip)) return clip;
            var rng = new System.Random(13);
            return cache[key] = Build(key, 2.6f, (t, noise, state) =>
            {
                state[0] = Mathf.Lerp(state[0], noise, 0.03f);
                float hit = Mathf.Sin(2f * Mathf.PI * 46f * t) * Mathf.Exp(-t * 2.5f);
                float drone = (Mathf.Sin(2f * Mathf.PI * 49f * t) + Mathf.Sin(2f * Mathf.PI * 51.9f * t) + 0.3f * Mathf.Sin(2f * Mathf.PI * 1244f * t))
                              * Mathf.Clamp01(t * 3f) * Mathf.Exp(-t * 0.9f);
                return 0.6f * hit + 0.3f * drone + 0.4f * state[0] * Mathf.Exp(-t * 1.5f);
            }, rng);
        }

        // Проиграть звук в точке пространства (3D, тише с расстоянием).
        public static void PlayAt(AudioClip clip, Vector3 position, float volume)
        {
            var go = new GameObject("Sfx " + clip.name);
            go.transform.position = position;
            var source = go.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = volume;
            source.spatialBlend = 1f;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.minDistance = 0.8f;
            source.maxDistance = 9f;
            source.pitch = Random.Range(0.94f, 1.06f);
            source.Play();
            Object.Destroy(go, clip.length / source.pitch + 0.1f);
        }

        delegate float Sample(float t, float noise, float[] state);

        static AudioClip Build(string name, float seconds, Sample sample, System.Random rng)
        {
            int count = Mathf.RoundToInt(seconds * Rate);
            var data = new float[count];
            var state = new float[2];
            for (int i = 0; i < count; i++)
            {
                float noise = (float)(rng.NextDouble() * 2.0 - 1.0);
                data[i] = Mathf.Clamp(sample(i / (float)Rate, noise, state), -1f, 1f) * 0.8f;
            }
            var clip = AudioClip.Create(name, count, 1, Rate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
