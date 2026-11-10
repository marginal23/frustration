using UnityEngine;

namespace Frustration
{
    // Радиоприёмник: играет музыку с хрипом старого динамика. По E включается и выключается.
    public class RadioPlayer : Interactable
    {
        public AudioSource source;

        void Start() => UpdatePrompt();

        public override void Interact()
        {
            if (source == null) return;
            if (source.isPlaying) source.Pause();
            else source.UnPause();
            if (!source.isPlaying && source.time == 0f) source.Play();
            UpdatePrompt();
        }

        void UpdatePrompt() => prompt = source != null && source.isPlaying ? "Выключить радио" : "Включить радио";
    }
}
