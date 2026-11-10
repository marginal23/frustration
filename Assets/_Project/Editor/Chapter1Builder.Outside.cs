using System.IO;
using UnityEditor;
using UnityEngine;

namespace Frustration.EditorTools
{
    // Пятая часть сборщика: двор за окнами.
    // Квартира на третьем этаже, поэтому земля далеко внизу, а за окном видны кроны деревьев.
    // Напротив стоит такая же пятиэтажка, над ней торчат трубы комбината.
    // Дальше всё тонет в тумане и становится силуэтами на сером утреннем небе.
    public static partial class Chapter1Builder
    {
        const float GroundY = -6.4f;

        // Сторонние модели (Retro Tree Pack от Pizza Doggy). По лицензии их нельзя выкладывать
        // отдельными файлами, поэтому папка _ThirdParty не попадает в git (см. .gitignore).
        const string TreePackFolder = "Assets/_ThirdParty/RetroTreePack";

        static void BuildOutside()
        {
            var outside = new GameObject("Outside").transform;

            // Земля: вытоптанная трава с проплешинами и асфальтовая дорожка вдоль дома.
            var ground = Mat("Yard Ground", Color.white, MakeColorTexture("Yard Ground", 32, 32, YardGroundPixel), 20f);
            var asphalt = Mat("Asphalt", new Color(0.25f, 0.25f, 0.26f), plasterTex, 10f);
            Box("Ground", outside, new Vector3(5f, GroundY - 0.1f, 30f), new Vector3(90f, 0.2f, 48f), ground);
            Box("Path", outside, new Vector3(5f, GroundY + 0.01f, 8.5f), new Vector3(90f, 0.02f, 2.2f), asphalt);

            // Пятиэтажка напротив.
            var panel = Mat("Panel Building", Color.white, MakeColorTexture("Panel Building", 32, 32, PanelBuildingPixel), 1f);
            var building = Box("Building Across", outside, new Vector3(5f, GroundY + 7.5f, 27f), new Vector3(60f, 15f, 10f), panel);
            building.GetComponent<Renderer>().sharedMaterial.SetTextureScale("_BaseMap", new Vector2(12f, 5f));
            Box("Building Roof", outside, new Vector3(5f, GroundY + 15.1f, 27f), new Vector3(60.4f, 0.3f, 10.4f), asphalt);

            // Трубы комбината за домами.
            var chimney = Mat("Chimney", new Color(0.35f, 0.33f, 0.32f), plasterTex);
            foreach (var (x, h) in new[] { (-6f, 38f), (0f, 44f), (17f, 34f) })
                Primitive(PrimitiveType.Cylinder, "Chimney", outside, new Vector3(x, GroundY + h / 2f, 46f), new Vector3(2.2f, h / 2f, 2.2f), chimney);

            // Деревья во дворе.
            PlaceTrees(outside);

            // Снаружи туман слабее, а «небо» подсвечивает всё мягким светом: иначе двор почти чёрный.
            foreach (var m in new[] { ground, asphalt, panel, chimney }) MakeOutdoor(m);
            foreach (var r in outside.GetComponentsInChildren<Renderer>(true))
            {
                var mats = r.sharedMaterials;
                for (int i = 0; i < mats.Length; i++) mats[i] = OutdoorCopy(mats[i]);
                r.sharedMaterials = mats;
            }

            // Снаружи ничего не нужно трогать: убираем коллайдеры, кроме земли.
            foreach (var c in outside.GetComponentsInChildren<Collider>(true))
                if (c.gameObject.name != "Ground") Object.DestroyImmediate(c);
        }

        static readonly Color OutdoorLight = new Color(0.26f, 0.28f, 0.32f);
        const float OutdoorFog = 0.35f;

        static void MakeOutdoor(Material m)
        {
            m.SetFloat("_FogScale", OutdoorFog);
            m.SetColor("_ExtraLight", OutdoorLight);
            EditorUtility.SetDirty(m);
        }

        // Материалы деревьев принадлежат паку; делаем их уличные копии рядом с ним (тоже вне git).
        static Material OutdoorCopy(Material m)
        {
            if (m == null) return null;
            string path = AssetDatabase.GetAssetPath(m);
            if (!path.StartsWith(TreePackFolder) && !path.EndsWith(".fbx")) return m;
            string folder = TreePackFolder + "/Outdoor";
            if (!AssetDatabase.IsValidFolder(folder)) AssetDatabase.CreateFolder(TreePackFolder, "Outdoor");
            string copyPath = $"{folder}/{m.name}.mat";
            var copy = AssetDatabase.LoadAssetAtPath<Material>(copyPath);
            if (copy == null)
            {
                copy = new Material(m);
                AssetDatabase.CreateAsset(copy, copyPath);
            }
            copy.CopyPropertiesFromMaterial(m);
            MakeOutdoor(copy);
            return copy;
        }

        static void PlaceTrees(Transform outside)
        {
            if (!Directory.Exists(TreePackFolder))
            {
                Debug.LogWarning($"[Frustration] Нет папки {TreePackFolder}: двор будет без деревьев.");
                return;
            }

            var models = Directory.GetFiles(TreePackFolder, "*.fbx");
            if (models.Length == 0) return;
            System.Array.Sort(models);

            // Если пак импортировался раньше, чем скомпилировался RetroAssetImporter,
            // у моделей стандартные материалы. Тогда переимпортируем папку один раз.
            var probe = AssetDatabase.LoadAssetAtPath<GameObject>(models[0].Replace('\\', '/'));
            var probeRenderer = probe != null ? probe.GetComponentInChildren<Renderer>() : null;
            if (probeRenderer != null && probeRenderer.sharedMaterial != null && probeRenderer.sharedMaterial.shader != ps1Shader)
                AssetDatabase.ImportAsset(TreePackFolder, ImportAssetOptions.ImportRecursive | ImportAssetOptions.ForceUpdate);

            // (x, z, высота в метрах). Ближние деревья почти касаются окон, дальние уходят в туман.
            var spots = new[]
            {
                (1.0f, 8.6f, 10.5f), (4.2f, 9.4f, 9f), (8.6f, 8.8f, 11f), (11.5f, 9.8f, 9.5f),
                (-2.5f, 12f, 12f), (2.8f, 13.5f, 12.5f), (6.5f, 12.5f, 10f), (10f, 14f, 13f),
                (13.5f, 12.5f, 11.5f), (-0.5f, 17f, 11f), (5f, 18f, 12f), (9.5f, 19f, 10.5f),
            };
            var random = new System.Random(1);
            var parent = new GameObject("Trees").transform;
            parent.SetParent(outside, false);
            foreach (var (x, z, height) in spots)
            {
                string path = models[random.Next(models.Length)].Replace('\\', '/');
                PlaceModel(path, parent, new Vector3(x, GroundY, z), height, (float)random.NextDouble() * 360f);
            }
        }

        // Ставит модель так, чтобы её низ стоял в точке position, а высота была ровно height метров.
        static GameObject PlaceModel(string path, Transform parent, Vector3 position, float height, float yaw)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) return null;

            var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            go.transform.SetParent(parent, false);
            go.transform.rotation = Quaternion.Euler(0f, yaw, 0f);

            var bounds = ModelBounds(go);
            if (bounds.size.y > 0.001f) go.transform.localScale *= height / bounds.size.y;
            bounds = ModelBounds(go);
            go.transform.position += position - new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
            return go;
        }

        static Bounds ModelBounds(GameObject go)
        {
            var renderers = go.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return new Bounds(go.transform.position, Vector3.zero);
            var bounds = renderers[0].bounds;
            foreach (var r in renderers) bounds.Encapsulate(r.bounds);
            return bounds;
        }

        // Трава, вытоптанная до земли.
        static Color YardGroundPixel(int x, int y, float r)
        {
            var grass = new Color(0.22f, 0.27f, 0.14f);
            var dirt = new Color(0.3f, 0.25f, 0.18f);
            float patch = Mathf.PerlinNoise(x * 0.18f, y * 0.18f);
            return Noise(patch > 0.55f ? dirt : grass, r, 0.15f);
        }

        // Серые панели с тёмными окнами; в некоторых окнах уже горит свет.
        static Color PanelBuildingPixel(int x, int y, float r)
        {
            var wall = new Color(0.5f, 0.5f, 0.48f);
            if (y % 32 == 0 || x % 16 == 0) return new Color(0.4f, 0.4f, 0.38f);
            bool window = (x % 16) >= 4 && (x % 16) <= 11 && (y % 32) >= 10 && (y % 32) <= 24;
            if (!window) return Noise(wall, r, 0.06f);
            return x / 16 == 1 && y / 32 == 0 ? new Color(0.75f, 0.6f, 0.3f) : new Color(0.12f, 0.13f, 0.15f);
        }
    }
}
