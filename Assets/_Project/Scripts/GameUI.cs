using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Frustration
{
    // Весь интерфейс создаётся из кода: подсказка, субтитры, затемнение, заголовок главы.
    // Плюс здесь же выводится картинка из RetroCamera (игра рендерится в маленькую текстуру).
    public class GameUI : MonoBehaviour
    {
        public static GameUI Instance { get; private set; }

        [TextArea] public string introTitle = "МопсГрад";
        [TextArea] public string introSubtitle = "1 сентября 2004 года";

        // Свой шрифт можно перетащить сюда в инспекторе (например, пиксельный .ttf).
        // Если пусто, берётся системный моноширинный шрифт в духе печатной машинки.
        public Font customFont;
        static readonly string[] TypewriterFonts = { "Courier New", "Lucida Console", "Consolas" };

        Text promptText;
        Text subtitleText;
        Text titleText;
        Text titleSubText;
        Image fade;
        RawImage gameView;

        string[] currentLines;
        int lineIndex;
        public bool IsShowingLines => currentLines != null;

        void Awake()
        {
            Instance = this;
            StoryFlags.Clear();
            BuildCanvas();
        }

        IEnumerator Start()
        {
            // Вступление: чёрный экран, название города и дата, потом картинка проявляется.
            var player = FindAnyObjectByType<FirstPersonController>();
            if (player != null) player.inputLocked = true;
            SetFade(1f);
            ShowTitle(introTitle, introSubtitle);
            yield return new WaitForSeconds(3.5f);
            ShowTitle(null, null);
            yield return FadeTo(0f, 3f);
            if (player != null) player.inputLocked = false;
        }

        public void SetGameTexture(Texture texture)
        {
            gameView.texture = texture;
            gameView.enabled = texture != null;
        }

        public void SetPrompt(string prompt)
        {
            promptText.text = string.IsNullOrEmpty(prompt) ? "" : "[E] " + prompt;
        }

        public void ShowLines(string[] lines)
        {
            if (lines == null || lines.Length == 0) return;
            currentLines = lines;
            lineIndex = 0;
            subtitleText.text = currentLines[0];
        }

        public void NextLine()
        {
            if (currentLines == null) return;
            lineIndex++;
            if (lineIndex >= currentLines.Length)
            {
                currentLines = null;
                subtitleText.text = "";
                return;
            }
            subtitleText.text = currentLines[lineIndex];
        }

        public void ShowTitle(string title, string subtitle)
        {
            titleText.text = title ?? "";
            titleSubText.text = subtitle ?? "";
        }

        public void SetFade(float alpha) => fade.color = new Color(0f, 0f, 0f, alpha);

        public IEnumerator FadeTo(float target, float duration)
        {
            float start = fade.color.a;
            for (float t = 0f; t < duration; t += Time.deltaTime)
            {
                SetFade(Mathf.Lerp(start, target, t / duration));
                yield return null;
            }
            SetFade(target);
        }

        void BuildCanvas()
        {
            var canvasGo = new GameObject("UI Canvas");
            canvasGo.transform.SetParent(transform, false);
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);
            scaler.matchWidthOrHeight = 1f;

            gameView = new GameObject("Game View").AddComponent<RawImage>();
            gameView.transform.SetParent(canvasGo.transform, false);
            Stretch(gameView.rectTransform);
            gameView.enabled = false;
            gameView.raycastTarget = false;

            // Урезанная палитра и дизеринг. Настройки — в материале
            // Assets/_Project/Resources/RetroScreen (можно крутить прямо во время игры).
            var retroMaterial = Resources.Load<Material>("RetroScreen");
            if (retroMaterial != null) gameView.material = retroMaterial;

            var font = customFont != null ? customFont : Font.CreateDynamicFontFromOSFont(TypewriterFonts, 32);
            if (font == null) font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            promptText = MakeText(canvasGo.transform, font, 26, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.42f), new Color(0.85f, 0.85f, 0.8f, 0.9f));
            subtitleText = MakeText(canvasGo.transform, font, 30, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.12f), new Color(0.95f, 0.93f, 0.85f));

            fade = new GameObject("Fade").AddComponent<Image>();
            fade.transform.SetParent(canvasGo.transform, false);
            Stretch(fade.rectTransform);
            fade.raycastTarget = false;

            titleText = MakeText(canvasGo.transform, font, 64, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.55f), new Color(0.8f, 0.78f, 0.72f));
            titleSubText = MakeText(canvasGo.transform, font, 28, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.45f), new Color(0.6f, 0.58f, 0.55f));
        }

        static Text MakeText(Transform parent, Font font, int size, TextAnchor anchor, Vector2 pivotPos, Color color)
        {
            var text = new GameObject("Text").AddComponent<Text>();
            text.transform.SetParent(parent, false);
            text.font = font;
            text.fontSize = size;
            text.fontStyle = FontStyle.Bold;
            text.alignment = anchor;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            var shadow = text.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.8f);
            shadow.effectDistance = new Vector2(2f, -2f);
            var rect = text.rectTransform;
            rect.anchorMin = rect.anchorMax = pivotPos;
            rect.sizeDelta = new Vector2(1100f, 120f);
            return text;
        }

        static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
        }
    }
}
