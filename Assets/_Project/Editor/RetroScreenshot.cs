using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Frustration.EditorTools
{
    // Делает скриншот сцены пролога с камеры игрока в низком разрешении и увеличивает его в 3 раза.
    // Файл кладётся в папку Screenshots рядом с Assets.
    public static class RetroScreenshot
    {
        [MenuItem("Frustration/Take Retro Screenshot")]
        public static void Capture()
        {
            EditorSceneManager.OpenScene("Assets/_Project/Scenes/Chapter1_Morning.unity");
            Directory.CreateDirectory("Screenshots");

            CaptureFrom(new Vector3(1.4f, 1.15f, 4.6f), Quaternion.Euler(5f, 160f, 0f), "Screenshots/01_kid_room.png");
            CaptureFrom(new Vector3(5.2f, 1.15f, 3.4f), Quaternion.Euler(8f, 20f, 0f), "Screenshots/02_kitchen.png");
            CaptureFrom(new Vector3(4.5f, 1.15f, 2.3f), Quaternion.Euler(0f, -90f, 0f), "Screenshots/03_hallway.png");
        }

        static void CaptureFrom(Vector3 position, Quaternion rotation, string path)
        {
            var go = new GameObject("Screenshot Camera");
            var cam = go.AddComponent<Camera>();
            go.transform.SetPositionAndRotation(position, rotation);
            cam.fieldOfView = 70f;
            cam.nearClipPlane = 0.05f;
            cam.farClipPlane = 60f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.3f, 0.33f, 0.38f);

            const int width = 320, height = 240, scale = 3;
            var rt = new RenderTexture(width, height, 24) { filterMode = FilterMode.Point };
            cam.targetTexture = rt;
            cam.Render();

            // Тот же экранный эффект, что и в игре: палитра и дизеринг.
            var final = new RenderTexture(width, height, 0) { filterMode = FilterMode.Point };
            var retro = Resources.Load<Material>("RetroScreen");
            if (retro != null) Graphics.Blit(rt, final, retro);
            else Graphics.Blit(rt, final);

            RenderTexture.active = final;
            var small = new Texture2D(width, height, TextureFormat.RGB24, false);
            small.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            small.Apply();
            RenderTexture.active = null;

            var big = new Texture2D(width * scale, height * scale, TextureFormat.RGB24, false);
            for (int y = 0; y < height * scale; y++)
            for (int x = 0; x < width * scale; x++)
                big.SetPixel(x, y, small.GetPixel(x / scale, y / scale));
            big.Apply();
            File.WriteAllBytes(path, big.EncodeToPNG());

            cam.targetTexture = null;
            Object.DestroyImmediate(rt);
            Object.DestroyImmediate(final);
            Object.DestroyImmediate(small);
            Object.DestroyImmediate(big);
            Object.DestroyImmediate(go);
            Debug.Log("[Frustration] Screenshot saved: " + path);
        }
    }
}
