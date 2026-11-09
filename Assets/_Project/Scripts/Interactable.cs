using System.Collections.Generic;
using UnityEngine;

namespace Frustration
{
    // Предмет, который можно осмотреть клавишей E.
    // lines — реплики героя, которые покажутся субтитрами.
    // setsFlag — флаг, который ставится после осмотра (например, "backpack").
    // hideAfterUse — спрятать предмет после осмотра (взять в руки).
    public class Interactable : MonoBehaviour
    {
        public string prompt = "Осмотреть";
        [TextArea] public string[] lines;
        public string setsFlag;
        public bool hideAfterUse;

        public virtual void Interact()
        {
            if (lines != null && lines.Length > 0) GameUI.Instance.ShowLines(lines);
            if (!string.IsNullOrEmpty(setsFlag)) StoryFlags.Set(setsFlag);
            if (hideAfterUse) gameObject.SetActive(false);
        }
    }

    // Простое хранилище флагов сюжета на время одной сцены.
    public static class StoryFlags
    {
        static readonly HashSet<string> flags = new HashSet<string>();
        public static void Set(string flag) => flags.Add(flag);
        public static bool Has(string flag) => flags.Contains(flag);
        public static void Clear() => flags.Clear();
    }
}
