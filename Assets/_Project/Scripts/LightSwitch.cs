using UnityEngine;

namespace Frustration
{
    // Выключатель: по E включает и выключает лампы в комнате.
    // glows — лампочки и плафоны, у которых меняется материал (светится или нет).
    public class LightSwitch : Interactable
    {
        public Light[] lights;
        public Renderer[] glows;
        public Material[] onMaterials;
        public Material[] offMaterials;
        public bool isOn = true;

        void Start() => Apply();

        public override void Interact()
        {
            isOn = !isOn;
            Apply();
        }

        void Apply()
        {
            prompt = isOn ? "Выключить свет" : "Включить свет";
            foreach (var l in lights) if (l != null) l.enabled = isOn;
            for (int i = 0; i < glows.Length; i++)
                if (glows[i] != null) glows[i].sharedMaterial = isOn ? onMaterials[i] : offMaterials[i];
        }
    }
}
