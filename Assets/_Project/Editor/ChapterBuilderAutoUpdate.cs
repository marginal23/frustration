using UnityEditor;

namespace Frustration.EditorTools
{
    // Когда сборщик сцены меняется (растёт SceneVersion), Unity один раз спрашивает,
    // пересобрать ли сцену пролога. Так не нужно помнить про меню Frustration > Build Chapter 1.
    [InitializeOnLoad]
    static class ChapterBuilderAutoUpdate
    {
        // Увеличивай это число каждый раз, когда меняется Chapter1Builder.
        const int SceneVersion = 5;
        const string PrefKey = "Frustration.Chapter1.SceneVersion";

        static ChapterBuilderAutoUpdate()
        {
            if (UnityEngine.Application.isBatchMode) return;
            EditorApplication.delayCall += Check;
        }

        static void Check()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (EditorPrefs.GetInt(PrefKey, 0) >= SceneVersion) return;

            bool rebuild = EditorUtility.DisplayDialog(
                "Frustration: сцена обновилась",
                "В сборщике сцены пролога есть изменения (детали: плинтусы, наличники, батареи, подоконники, мелочи в комнатах).\n\n" +
                "Пересобрать сцену Chapter1_Morning сейчас? Ручные правки в этой сцене пропадут.",
                "Пересобрать", "Не сейчас");

            if (!rebuild) return;
            Chapter1Builder.Build();
            EditorPrefs.SetInt(PrefKey, SceneVersion);
        }
    }
}
