using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Frustration
{
    // Дверь, которая заканчивает главу. Пока не выполнены все нужные дела (флаги),
    // герой говорит реплику про первое невыполненное.
    public class ExitDoor : Interactable
    {
        public string[] requiredFlags = { "backpack" };
        [TextArea] public string[] missingLines = { "Без ранца в школу нельзя." };
        [TextArea] public string endTitle = "Конец пролога";
        [TextArea] public string endSubtitle = "Продолжение следует";
        [Tooltip("Сцена следующей главы. Пусто — игра на этом заканчивается.")]
        public string nextScene;

        bool finished;

        public override void Interact()
        {
            if (finished) return;
            for (int i = 0; i < requiredFlags.Length; i++)
            {
                if (StoryFlags.Has(requiredFlags[i])) continue;
                GameUI.Instance.ShowLines(new[] { i < missingLines.Length ? missingLines[i] : "Я ещё не готов." });
                return;
            }
            finished = true;
            ProceduralSfx.PlayAt(ProceduralSfx.Creak(), transform.position + Vector3.up, 0.6f);
            StartCoroutine(EndChapter());
        }

        IEnumerator EndChapter()
        {
            var player = FindAnyObjectByType<FirstPersonController>();
            if (player != null) player.inputLocked = true;
            yield return GameUI.Instance.FadeTo(1f, 2.5f);
            GameUI.Instance.ShowTitle(endTitle, endSubtitle);
            if (string.IsNullOrEmpty(nextScene)) yield break;
            yield return new WaitForSeconds(4f);
            SceneManager.LoadScene(nextScene);
        }
    }
}
