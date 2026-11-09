using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace Frustration.EditorTools
{
    // Собирает черновую сцену пролога из простых кубов: квартира утром 1 сентября 2004 года.
    // Запуск: меню Frustration > Build Chapter 1. Сцена перезаписывается целиком,
    // поэтому ручные правки лучше делать уже в копии или после того, как сборщик больше не нужен.
    public static class Chapter1Builder
    {
        const string Root = "Assets/_Project";
        const string ScenePath = Root + "/Scenes/Chapter1_Morning.unity";
        const float WallHeight = 2.6f;
        const float WallThickness = 0.1f;

        static Shader ps1Shader;
        static Texture2D plasterTex, floorTex, woodTex;

        [MenuItem("Frustration/Build Chapter 1")]
        public static void Build()
        {
            Directory.CreateDirectory(Root + "/Scenes");
            Directory.CreateDirectory(Root + "/Materials");
            Directory.CreateDirectory(Root + "/Textures");

            ps1Shader = Shader.Find("Frustration/PS1 Lit");
            if (ps1Shader == null) ps1Shader = Shader.Find("Universal Render Pipeline/Lit");

            plasterTex = MakeTexture("Plaster", 32, (x, y, r) => 0.82f + 0.18f * r);
            floorTex = MakeTexture("Linoleum", 32, (x, y, r) => (((x / 8) + (y / 8)) % 2 == 0 ? 0.95f : 0.75f) - 0.12f * r);
            woodTex = MakeTexture("Wood", 32, (x, y, r) => 0.75f + 0.2f * Mathf.Sin(y * 0.9f + r * 2f) * 0.5f + 0.1f * r);

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            SetupLighting();
            BuildApartment();
            BuildKidRoom();
            BuildKitchen();
            BuildHallway();
            BuildPlayerAndSystems();

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.SaveAssets();
            Debug.Log("[Frustration] Chapter 1 scene built: " + ScenePath);
        }

        // ---------- Свет и туман ----------

        static void SetupLighting()
        {
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.07f, 0.07f, 0.09f);
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogColor = new Color(0.015f, 0.015f, 0.02f);
            RenderSettings.fogDensity = 0.14f;
            RenderSettings.skybox = null;

            // Слабый холодный "утренний" свет: на PS1-шейдере он задаёт основное направление освещения.
            var sun = new GameObject("Morning Light").AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.color = new Color(0.55f, 0.62f, 0.8f);
            sun.intensity = 0.12f;
            sun.shadows = LightShadows.None;
            sun.transform.rotation = Quaternion.Euler(35f, 200f, 0f);
        }

        // ---------- Стены, пол, потолок ----------

        static void BuildApartment()
        {
            var parent = new GameObject("Apartment").transform;
            var wall = Mat("Wall", new Color(0.55f, 0.6f, 0.5f), plasterTex);
            var floor = Mat("Floor", new Color(0.45f, 0.3f, 0.22f), floorTex, 6f);
            var ceiling = Mat("Ceiling", new Color(0.6f, 0.6f, 0.58f), plasterTex);

            Box("Floor", parent, new Vector3(3.5f, -0.05f, 3.8f), new Vector3(7f, 0.1f, 4.4f), floor);
            Box("Ceiling", parent, new Vector3(3.5f, WallHeight + 0.05f, 3.8f), new Vector3(7f, 0.1f, 4.4f), ceiling);

            // Наружные стены.
            WallX(parent, 6f, 0f, 7f, wall);       // север
            WallX(parent, 1.6f, 0f, 7f, wall);     // юг
            WallZ(parent, 7f, 1.6f, 6f, wall);     // восток
            WallZ(parent, 0f, 1.6f, 1.85f, wall);  // запад, слева от входной двери
            WallZ(parent, 0f, 2.75f, 6f, wall);    // запад, справа от двери
            Lintel(parent, new Vector3(0f, 0f, 2.3f), new Vector3(WallThickness, 0f, 0.9f), wall);

            // Стена между коридором и комнатами, с двумя проёмами.
            WallX(parent, 3f, 0f, 1.0f, wall);
            WallX(parent, 3f, 1.9f, 5.0f, wall);
            WallX(parent, 3f, 5.9f, 7f, wall);
            Lintel(parent, new Vector3(1.45f, 0f, 3f), new Vector3(0.9f, 0f, WallThickness), wall);
            Lintel(parent, new Vector3(5.45f, 0f, 3f), new Vector3(0.9f, 0f, WallThickness), wall);

            // Стена между детской и кухней.
            WallZ(parent, 3.5f, 3f, 6f, wall);
        }

        // ---------- Детская ----------

        static void BuildKidRoom()
        {
            var room = new GameObject("Kid Room").transform;
            var wood = Mat("Wood", new Color(0.45f, 0.3f, 0.2f), woodTex);
            var blanket = Mat("Blanket", new Color(0.35f, 0.25f, 0.45f), plasterTex);
            var backpackMat = Mat("Backpack", new Color(0.7f, 0.12f, 0.1f), plasterTex);

            var bed = Box("Bed", room, new Vector3(0.5f, 0.25f, 4.95f), new Vector3(0.9f, 0.5f, 1.9f), blanket);
            Box("Pillow", room, new Vector3(0.5f, 0.55f, 5.6f), new Vector3(0.6f, 0.12f, 0.4f), Mat("Pillow", new Color(0.8f, 0.8f, 0.75f), plasterTex));
            Interact(bed, "Кровать", "Ещё темно. А мама сказала, что сегодня праздник.");

            Box("Desk", room, new Vector3(3.0f, 0.375f, 5.6f), new Vector3(0.8f, 0.75f, 0.6f), wood);
            Box("Chair", room, new Vector3(2.6f, 0.225f, 5.0f), new Vector3(0.4f, 0.45f, 0.4f), wood);
            Box("Wardrobe", room, new Vector3(3.0f, 1.0f, 3.4f), new Vector3(0.8f, 2.0f, 0.6f), wood);

            var backpack = Box("Backpack", room, new Vector3(2.6f, 0.63f, 5.0f), new Vector3(0.32f, 0.36f, 0.2f), backpackMat);
            var pickup = Interact(backpack, "Взять рюкзак", "Новый рюкзак. Мама купила его на рынке.", "Он пахнет клеем.");
            pickup.setsFlag = "backpack";
            pickup.hideAfterUse = true;

            var window = Window("Kid Window", room, new Vector3(1.6f, 1.5f, 5.94f));
            Interact(window, "Окно", "Во дворе никого. За домами видно забор.", "Папа говорит, что за забор нельзя.");

            var light = PointLight("Window Glow", room, new Vector3(1.6f, 1.6f, 5.4f), new Color(0.5f, 0.6f, 0.85f), 0.8f, 4f);
            light.shadows = LightShadows.None;
        }

        // ---------- Кухня: отец ----------

        static void BuildKitchen()
        {
            var room = new GameObject("Kitchen").transform;
            var wood = Mat("Wood", new Color(0.45f, 0.3f, 0.2f), woodTex);
            var cloth = Mat("Father Clothes", new Color(0.18f, 0.2f, 0.17f), plasterTex);
            var skin = Mat("Skin", new Color(0.6f, 0.45f, 0.38f), plasterTex);
            var glass = Mat("Bottle", new Color(0.15f, 0.3f, 0.15f), plasterTex);
            var white = Mat("Appliance", new Color(0.75f, 0.75f, 0.7f), plasterTex);

            Box("Table", room, new Vector3(5.8f, 0.375f, 4.8f), new Vector3(1.0f, 0.75f, 0.8f), wood);
            Box("Chair Near", room, new Vector3(5.0f, 0.225f, 4.8f), new Vector3(0.4f, 0.45f, 0.4f), wood);
            Box("Chair Father", room, new Vector3(6.6f, 0.225f, 4.8f), new Vector3(0.4f, 0.45f, 0.4f), wood);

            // Отец спит, уронив голову на стол.
            var father = new GameObject("Father").transform;
            father.SetParent(room, false);
            var body = Primitive(PrimitiveType.Capsule, "Body", father, new Vector3(6.55f, 0.85f, 4.8f), new Vector3(0.45f, 0.42f, 0.32f), cloth);
            body.transform.rotation = Quaternion.Euler(0f, 0f, 35f);
            Primitive(PrimitiveType.Sphere, "Head", father, new Vector3(6.15f, 0.9f, 4.85f), Vector3.one * 0.24f, skin);
            Box("Arm", father, new Vector3(6.05f, 0.8f, 4.62f), new Vector3(0.5f, 0.09f, 0.1f), cloth);
            var fatherInteract = father.gameObject.AddComponent<Interactable>();
            fatherInteract.prompt = "Папа";
            fatherInteract.lines = new[] { "Папа спит за столом.", "Лучше его не будить." };

            var bottles = new GameObject("Bottles").transform;
            bottles.SetParent(room, false);
            Primitive(PrimitiveType.Cylinder, "Bottle", bottles, new Vector3(5.55f, 0.87f, 4.6f), new Vector3(0.07f, 0.12f, 0.07f), glass);
            Primitive(PrimitiveType.Cylinder, "Bottle", bottles, new Vector3(5.7f, 0.87f, 5.0f), new Vector3(0.07f, 0.12f, 0.07f), glass);
            Primitive(PrimitiveType.Cylinder, "Bottle", bottles, new Vector3(5.45f, 0.87f, 4.95f), new Vector3(0.07f, 0.12f, 0.07f), glass);
            var fallen = Primitive(PrimitiveType.Cylinder, "Bottle Fallen", bottles, new Vector3(5.3f, 0.04f, 4.3f), new Vector3(0.07f, 0.12f, 0.07f), glass);
            fallen.transform.rotation = Quaternion.Euler(90f, 30f, 0f);
            var bottlesInteract = bottles.gameObject.AddComponent<Interactable>();
            bottlesInteract.prompt = "Бутылки";
            bottlesInteract.lines = new[] { "Вчера их было меньше." };

            var fridge = Box("Fridge", room, new Vector3(6.65f, 0.9f, 5.6f), new Vector3(0.6f, 1.8f, 0.6f), white);
            AddSound(fridge, ProceduralAmbience.Kind.FridgeHum, 0.2f, true);

            // Телевизор шипит, его синий свет мигает.
            Box("TV Stand", room, new Vector3(4.15f, 0.35f, 5.6f), new Vector3(0.7f, 0.7f, 0.5f), wood);
            var tv = Box("TV", room, new Vector3(4.15f, 0.92f, 5.6f), new Vector3(0.55f, 0.45f, 0.45f), Mat("TV Body", new Color(0.12f, 0.12f, 0.12f), plasterTex));
            var screen = Primitive(PrimitiveType.Quad, "TV Screen", room, new Vector3(4.15f, 0.93f, 5.37f), new Vector3(0.45f, 0.34f, 1f), Mat("TV Screen", Color.black, plasterTex, 1f, new Color(0.35f, 0.4f, 0.5f)));
            screen.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            Object.DestroyImmediate(screen.GetComponent<Collider>());
            Interact(tv, "Телевизор", "Каналов нет. Только шипит.");
            AddSound(tv, ProceduralAmbience.Kind.TvStatic, 0.12f, true);
            var tvLight = PointLight("TV Light", room, new Vector3(4.15f, 0.95f, 5.0f), new Color(0.45f, 0.55f, 0.9f), 0.9f, 3f);
            var tvFlicker = tvLight.gameObject.AddComponent<LightFlicker>();
            tvFlicker.speed = 14f;
            tvFlicker.amount = 0.5f;

            // Лампа над столом мигает и иногда почти гаснет.
            Primitive(PrimitiveType.Sphere, "Bulb", room, new Vector3(5.8f, 2.35f, 4.8f), Vector3.one * 0.12f, Mat("Bulb", Color.black, plasterTex, 1f, new Color(1f, 0.8f, 0.5f)));
            var lamp = PointLight("Kitchen Lamp", room, new Vector3(5.8f, 2.2f, 4.8f), new Color(1f, 0.75f, 0.45f), 1.6f, 5f);
            lamp.gameObject.AddComponent<LightFlicker>().amount = 0.15f;

            var window = Window("Kitchen Window", room, new Vector3(5.5f, 1.5f, 5.94f));
            Interact(window, "Окно", "Вдалеке гудит комбинат. Он гудит всегда.");
        }

        // ---------- Коридор: фото и входная дверь ----------

        static void BuildHallway()
        {
            var hall = new GameObject("Hallway").transform;
            var wood = Mat("Wood", new Color(0.45f, 0.3f, 0.2f), woodTex);
            var doorMat = Mat("Door", new Color(0.35f, 0.2f, 0.12f), woodTex);

            // Первый обрывок прошлого отца.
            Box("Photo Frame", hall, new Vector3(3.0f, 1.5f, 2.93f), new Vector3(0.36f, 0.46f, 0.03f), wood);
            var photo = Primitive(PrimitiveType.Quad, "Photo", hall, new Vector3(3.0f, 1.5f, 2.91f), new Vector3(0.28f, 0.38f, 1f), Mat("Photo", new Color(0.55f, 0.5f, 0.4f), plasterTex));
            photo.transform.rotation = Quaternion.identity;
            Interact(photo, "Фотография", "Папа в форме, совсем молодой.", "Здесь он улыбается.", "Я не помню, чтобы он так улыбался.");

            Box("Shoe Rack", hall, new Vector3(0.4f, 0.2f, 1.85f), new Vector3(0.6f, 0.4f, 0.3f), wood);
            Box("Coat", hall, new Vector3(0.9f, 1.4f, 1.72f), new Vector3(0.45f, 0.8f, 0.12f), Mat("Coat", new Color(0.2f, 0.22f, 0.2f), plasterTex));

            var door = Box("Front Door", hall, new Vector3(0.06f, 1.02f, 2.3f), new Vector3(0.06f, 2.05f, 0.9f), doorMat);
            var exit = door.AddComponent<ExitDoor>();
            exit.prompt = "Выйти в школу";
            exit.lockedLines = new[] { "Без рюкзака в школу нельзя." };
            exit.endTitle = "Глава 1. Первый класс";
            exit.endSubtitle = "Продолжение следует";

            PointLight("Hall Bulb", hall, new Vector3(3.5f, 2.3f, 2.3f), new Color(1f, 0.85f, 0.6f), 0.5f, 3.5f);
        }

        // ---------- Игрок, интерфейс, звук ----------

        static void BuildPlayerAndSystems()
        {
            var player = new GameObject("Player");
            player.transform.position = new Vector3(1.4f, 0.05f, 4.6f);
            player.transform.rotation = Quaternion.Euler(0f, 160f, 0f);
            var controller = player.AddComponent<CharacterController>();
            controller.height = 1.2f;
            controller.radius = 0.22f;
            controller.center = new Vector3(0f, 0.6f, 0f);

            var pivot = new GameObject("Camera Pivot");
            pivot.transform.SetParent(player.transform, false);
            pivot.transform.localPosition = new Vector3(0f, 1.1f, 0f);
            var cam = pivot.AddComponent<Camera>();
            cam.nearClipPlane = 0.05f;
            cam.farClipPlane = 30f;
            cam.fieldOfView = 70f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;
            pivot.AddComponent<AudioListener>();
            pivot.AddComponent<RetroCamera>();
            pivot.tag = "MainCamera";

            var fps = player.AddComponent<FirstPersonController>();
            fps.cameraPivot = pivot.transform;
            player.AddComponent<PlayerInteractor>().viewCamera = cam;

            // Эта камера ничего не рисует, она только выводит интерфейс на экран.
            var screenCam = new GameObject("Screen Camera").AddComponent<Camera>();
            screenCam.cullingMask = 0;
            screenCam.clearFlags = CameraClearFlags.SolidColor;
            screenCam.backgroundColor = Color.black;
            screenCam.depth = 10;

            new GameObject("Game UI").AddComponent<GameUI>();

            var drone = new GameObject("Ambience Drone");
            AddSound(drone, ProceduralAmbience.Kind.Drone, 0.3f, false);
        }

        // ---------- Помощники ----------

        static GameObject Box(string name, Transform parent, Vector3 position, Vector3 size, Material material)
            => Primitive(PrimitiveType.Cube, name, parent, position, size, material);

        static GameObject Primitive(PrimitiveType type, string name, Transform parent, Vector3 position, Vector3 scale, Material material)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.position = position;
            go.transform.localScale = scale;
            var renderer = go.GetComponent<Renderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            return go;
        }

        static void WallX(Transform parent, float z, float x1, float x2, Material m)
            => Box("Wall", parent, new Vector3((x1 + x2) / 2f, WallHeight / 2f, z), new Vector3(x2 - x1, WallHeight, WallThickness), m);

        static void WallZ(Transform parent, float x, float z1, float z2, Material m)
            => Box("Wall", parent, new Vector3(x, WallHeight / 2f, (z1 + z2) / 2f), new Vector3(WallThickness, WallHeight, z2 - z1), m);

        static void Lintel(Transform parent, Vector3 center, Vector3 size, Material m)
        {
            const float doorTop = 2.05f;
            Box("Lintel", parent, new Vector3(center.x, (doorTop + WallHeight) / 2f, center.z), new Vector3(size.x, WallHeight - doorTop, size.z), m);
        }

        static GameObject Window(string name, Transform parent, Vector3 position)
        {
            var frame = Box(name + " Frame", parent, position + new Vector3(0f, 0f, 0.01f), new Vector3(1.0f, 1.1f, 0.04f), Mat("Window Frame", new Color(0.7f, 0.7f, 0.68f), plasterTex));
            var glass = Primitive(PrimitiveType.Quad, name, parent, position - new Vector3(0f, 0f, 0.02f), new Vector3(0.85f, 0.95f, 1f), Mat("Window Glass", Color.black, plasterTex, 1f, new Color(0.25f, 0.3f, 0.42f)));
            glass.transform.rotation = Quaternion.identity;
            Object.DestroyImmediate(glass.GetComponent<Collider>());
            return frame;
        }

        static Light PointLight(string name, Transform parent, Vector3 position, Color color, float intensity, float range)
        {
            var light = new GameObject(name).AddComponent<Light>();
            light.transform.SetParent(parent, false);
            light.transform.position = position;
            light.type = LightType.Point;
            light.color = color;
            light.intensity = intensity;
            light.range = range;
            light.shadows = LightShadows.None;
            return light;
        }

        static Interactable Interact(GameObject go, string prompt, params string[] lines)
        {
            var interactable = go.AddComponent<Interactable>();
            interactable.prompt = prompt;
            interactable.lines = lines;
            return interactable;
        }

        static void AddSound(GameObject go, ProceduralAmbience.Kind kind, float volume, bool spatial)
        {
            var source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = spatial ? 1f : 0f;
            source.minDistance = 0.6f;
            source.maxDistance = 6f;
            source.rolloffMode = AudioRolloffMode.Linear;
            var ambience = go.AddComponent<ProceduralAmbience>();
            ambience.kind = kind;
            ambience.volume = volume;
        }

        static Material Mat(string name, Color color, Texture2D texture, float tiling = 1f, Color? emission = null)
        {
            string path = $"{Root}/Materials/{name}.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(ps1Shader);
                AssetDatabase.CreateAsset(material, path);
            }
            material.shader = ps1Shader;
            material.SetColor("_BaseColor", color);
            material.SetTexture("_BaseMap", texture);
            material.SetTextureScale("_BaseMap", Vector2.one * tiling);
            if (material.HasProperty("_EmissionColor")) material.SetColor("_EmissionColor", emission ?? Color.black);
            EditorUtility.SetDirty(material);
            return material;
        }

        // Маленькая процедурная текстура без сглаживания: на ней хорошо видно "плывущие" PS1-текстуры.
        static Texture2D MakeTexture(string name, int size, System.Func<int, int, float, float> value)
        {
            string path = $"{Root}/Textures/{name}.png";
            var random = new System.Random(name.GetHashCode());
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float v = Mathf.Clamp01(value(x, y, (float)random.NextDouble()));
                tex.SetPixel(x, y, new Color(v, v, v, 1f));
            }
            File.WriteAllBytes(path, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(path);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.wrapMode = TextureWrapMode.Repeat;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }
    }
}
