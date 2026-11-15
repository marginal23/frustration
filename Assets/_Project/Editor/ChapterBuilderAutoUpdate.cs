using UnityEditor;

namespace Frustration.EditorTools
{
    // Когда сборщик сцены меняется (растёт SceneVersion), Unity один раз спрашивает,
    // пересобрать ли сцену пролога. Так не нужно помнить про меню Frustration > Build Chapter 1.
    [InitializeOnLoad]
    static class ChapterBuilderAutoUpdate
    {
        // Увеличивай это число каждый раз, когда меняется Chapter1Builder.
        const int SceneVersion = 13;
        const string PrefKey = "Frustration.Chapter1.SceneVersion";

        static ChapterBuilderAutoUpdate()
        {
            if (UnityEngine.Application.isBatchMode) return;
            EditorApplication.delayCall += Check;
            // Если код обновился, пока шла игра (Play), спросим сразу после выхода из неё.
            EditorApplication.playModeStateChanged += state =>
            {
                if (state == PlayModeStateChange.EnteredEditMode) EditorApplication.delayCall += Check;
            };
        }

        static void Check()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (EditorPrefs.GetInt(PrefKey, 0) >= SceneVersion) return;

            bool rebuild = EditorUtility.DisplayDialog(
                "Frustration: сцена обновилась",
                "В сборщике сцены пролога есть изменения (пять глав: 2004, 2008, 2009, 2012, 2014 — квартира меняется с годами).\n\n" +
                "Пересобрать сцены всех глав сейчас? Ручные правки в них пропадут. Сборка займёт пару минут.",
                "Пересобрать", "Не сейчас");

            if (!rebuild) return;
            Chapter1Builder.Build();
            EditorPrefs.SetInt(PrefKey, SceneVersion);
        }
    }
}
