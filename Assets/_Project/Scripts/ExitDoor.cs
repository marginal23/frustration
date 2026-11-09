using System.Collections;
using UnityEngine;

namespace Frustration
{
    // Дверь, которая заканчивает главу. Пока нет нужного флага, герой говорит lockedLines.
    public class ExitDoor : Interactable
    {
        public string requiredFlag = "backpack";
        [TextArea] public string[] lockedLines;
        [TextArea] public string endTitle = "Конец пролога";
        [TextArea] public string endSubtitle = "Продолжение следует";

        bool finished;

        public override void Interact()
        {
            if (finished) return;
            if (!string.IsNullOrEmpty(requiredFlag) && !StoryFlags.Has(requiredFlag))
            {
                GameUI.Instance.ShowLines(lockedLines);
                return;
            }
            finished = true;
            StartCoroutine(EndChapter());
        }

        IEnumerator EndChapter()
        {
            var player = FindAnyObjectByType<FirstPersonController>();
            if (player != null) player.inputLocked = true;
            yield return GameUI.Instance.FadeTo(1f, 2.5f);
            GameUI.Instance.ShowTitle(endTitle, endSubtitle);
        }
    }
}
