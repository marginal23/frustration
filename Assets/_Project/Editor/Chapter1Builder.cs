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
    public static partial class Chapter1Builder
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
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

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
            BuildLivingRoom();
            BuildInterior();
            BuildDetails();
            BuildOutside();
            var player = BuildPlayerAndSystems();
            BuildMirror(player);

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
            WallXWithWindows(parent, 6f, 0f, 7f, new[] { 1.6f, 5.5f }, wall);  // север, с проёмами окон
            // Южная стена коридора с проёмом под зеркало. Части названы "Mirror ...",
            // чтобы они не копировались в комнату-отражение.
            Box("Mirror Wall Left", parent, new Vector3(MirrorX0 / 2f, WallHeight / 2f, 1.6f), new Vector3(MirrorX0, WallHeight, WallThickness), wall);
            Box("Mirror Wall Right", parent, new Vector3((MirrorX1 + 7f) / 2f, WallHeight / 2f, 1.6f), new Vector3(7f - MirrorX1, WallHeight, WallThickness), wall);
            Box("Mirror Wall Below", parent, new Vector3((MirrorX0 + MirrorX1) / 2f, MirrorY0 / 2f, 1.6f), new Vector3(MirrorX1 - MirrorX0, MirrorY0, WallThickness), wall);
            Box("Mirror Wall Above", parent, new Vector3((MirrorX0 + MirrorX1) / 2f, (MirrorY1 + WallHeight) / 2f, 1.6f), new Vector3(MirrorX1 - MirrorX0, WallHeight - MirrorY1, WallThickness), wall);
            // Восточная стена: проём из коридора в зал.
            WallZ(parent, 7f, 1.6f, 1.85f, wall);
            WallZ(parent, 7f, 2.75f, 6f, wall);
            Lintel(parent, new Vector3(7f, 0f, 2.3f), new Vector3(WallThickness, 0f, 0.9f), wall);
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

            var bed = Box("Bed", room, new Vector3(0.5f, 0.25f, 4.95f), new Vector3(0.9f, 0.5f, 1.9f), blanket);
            Box("Pillow", room, new Vector3(0.5f, 0.55f, 5.6f), new Vector3(0.6f, 0.12f, 0.4f), Mat("Pillow", new Color(0.8f, 0.8f, 0.75f), plasterTex));
            Interact(bed, "Кровать", "Ещё темно. А мама сказала, что сегодня праздник.");

            Box("Desk", room, new Vector3(3.0f, 0.375f, 5.6f), new Vector3(0.8f, 0.75f, 0.6f), wood);
            Box("Chair", room, new Vector3(2.6f, 0.225f, 5.0f), new Vector3(0.4f, 0.45f, 0.4f), wood);
            Box("Wardrobe", room, new Vector3(3.0f, 1.0f, 3.4f), new Vector3(0.8f, 2.0f, 0.6f), wood);

            var backpack = Satchel(room, new Vector3(2.6f, 0.45f, 5.0f), Quaternion.Euler(0f, -90f, 0f));
            var pickup = Interact(backpack, "Взять ранец", "Новый ранец. Мама купила его на рынке.", "Он пахнет клеем.");
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
            Bottle(bottles, new Vector3(5.55f, 0.75f, 4.6f), Quaternion.identity, vodka: true);
            Bottle(bottles, new Vector3(5.7f, 0.75f, 5.0f), Quaternion.identity, vodka: false);
            Bottle(bottles, new Vector3(5.45f, 0.75f, 4.95f), Quaternion.identity, vodka: true);
            Bottle(bottles, new Vector3(5.3f, 0.033f, 4.3f), Quaternion.Euler(0f, 30f, 90f), vodka: false);
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
            exit.lockedLines = new[] { "Без ранца в школу нельзя." };
            exit.endTitle = "Глава 1. Первый класс";
            exit.endSubtitle = "Продолжение следует";

            PointLight("Hall Bulb", hall, new Vector3(3.5f, 2.3f, 2.3f), new Color(1f, 0.85f, 0.6f), 0.5f, 3.5f);
        }

        // ---------- Игрок, интерфейс, звук ----------

        static Transform BuildPlayerAndSystems()
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
            cam.farClipPlane = 60f;
            cam.fieldOfView = 70f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = SkyColor;
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
            return player.transform;
        }

        // ---------- Предметы из нескольких частей ----------

        // Советский школьный ранец: корпус, клапан с двумя застёжками, светоотражатель, ручка, лямки сзади.
        // Перед ранца смотрит вдоль его локальной оси +Z.
        static GameObject Satchel(Transform parent, Vector3 position, Quaternion rotation)
        {
            var satchel = new GameObject("Satchel").transform;
            satchel.SetParent(parent, false);
            satchel.SetPositionAndRotation(position, rotation);

            var red = Mat("Satchel", new Color(0.62f, 0.1f, 0.08f), plasterTex);
            var darkRed = Mat("Satchel Flap", new Color(0.42f, 0.06f, 0.05f), plasterTex);
            var metal = Mat("Metal", new Color(0.7f, 0.7f, 0.65f), plasterTex);
            var black = Mat("Strap", new Color(0.08f, 0.08f, 0.08f), plasterTex);
            var reflector = Mat("Reflector", new Color(0.9f, 0.75f, 0.1f), plasterTex, 1f, new Color(0.12f, 0.1f, 0.02f));

            LocalBox("Body", satchel, new Vector3(0f, 0.16f, 0f), new Vector3(0.30f, 0.32f, 0.12f), red);
            LocalBox("Lid", satchel, new Vector3(0f, 0.325f, 0f), new Vector3(0.31f, 0.015f, 0.13f), darkRed);
            LocalBox("Flap", satchel, new Vector3(0f, 0.25f, 0.066f), new Vector3(0.31f, 0.16f, 0.015f), darkRed);
            LocalBox("Buckle L", satchel, new Vector3(-0.08f, 0.175f, 0.076f), new Vector3(0.035f, 0.03f, 0.008f), metal);
            LocalBox("Buckle R", satchel, new Vector3(0.08f, 0.175f, 0.076f), new Vector3(0.035f, 0.03f, 0.008f), metal);
            LocalBox("Reflector", satchel, new Vector3(0f, 0.27f, 0.076f), new Vector3(0.06f, 0.06f, 0.006f), reflector);
            LocalBox("Handle", satchel, new Vector3(0f, 0.345f, 0f), new Vector3(0.1f, 0.02f, 0.025f), black);
            LocalBox("Strap L", satchel, new Vector3(-0.08f, 0.16f, -0.066f), new Vector3(0.035f, 0.3f, 0.012f), black);
            LocalBox("Strap R", satchel, new Vector3(0.08f, 0.16f, -0.066f), new Vector3(0.035f, 0.3f, 0.012f), black);
            return satchel.gameObject;
        }

        // Бутылка: корпус, плечики, горлышко, крышка и этикетка. Водочная прозрачная, пивная коричневая.
        // Точка position — центр донышка.
        static GameObject Bottle(Transform parent, Vector3 position, Quaternion rotation, bool vodka)
        {
            var bottle = new GameObject(vodka ? "Vodka Bottle" : "Beer Bottle").transform;
            bottle.SetParent(parent, false);
            bottle.SetPositionAndRotation(position, rotation);

            var glass = vodka ? Mat("Glass Clear", new Color(0.55f, 0.62f, 0.6f), plasterTex)
                              : Mat("Glass Brown", new Color(0.28f, 0.13f, 0.04f), plasterTex);
            var label = vodka ? Mat("Label Vodka", new Color(0.85f, 0.82f, 0.75f), plasterTex)
                              : Mat("Label Beer", new Color(0.8f, 0.62f, 0.15f), plasterTex);
            var cap = vodka ? Mat("Cap Silver", new Color(0.65f, 0.65f, 0.62f), plasterTex)
                            : Mat("Cap Gold", new Color(0.7f, 0.55f, 0.2f), plasterTex);

            float r = vodka ? 0.075f : 0.065f;      // диаметр корпуса
            float h = vodka ? 0.16f : 0.14f;        // высота корпуса
            LocalPrimitive(PrimitiveType.Cylinder, "Body", bottle, new Vector3(0f, h / 2f, 0f), new Vector3(r, h / 2f, r), glass);
            LocalPrimitive(PrimitiveType.Cylinder, "Label", bottle, new Vector3(0f, h * 0.45f, 0f), new Vector3(r + 0.004f, h * 0.22f, r + 0.004f), label);
            LocalPrimitive(PrimitiveType.Sphere, "Shoulder", bottle, new Vector3(0f, h, 0f), new Vector3(r, 0.045f, r), glass);
            LocalPrimitive(PrimitiveType.Cylinder, "Neck", bottle, new Vector3(0f, h + 0.05f, 0f), new Vector3(0.028f, 0.045f, 0.028f), glass);
            LocalPrimitive(PrimitiveType.Cylinder, "Cap", bottle, new Vector3(0f, h + 0.1f, 0f), new Vector3(0.032f, 0.01f, 0.032f), cap);
            return bottle.gameObject;
        }

        static GameObject LocalBox(string name, Transform parent, Vector3 localPosition, Vector3 size, Material material)
            => LocalPrimitive(PrimitiveType.Cube, name, parent, localPosition, size, material);

        static GameObject LocalPrimitive(PrimitiveType type, string name, Transform parent, Vector3 localPosition, Vector3 scale, Material material)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = scale;
            var renderer = go.GetComponent<Renderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            return go;
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

        // Проём окна: 0.85 x 0.95 м, низ на высоте 1.025 м. Центр окна по высоте — 1.5 м.
        const float WindowHalfWidth = 0.425f, WindowBottom = 1.025f, WindowTop = 1.975f;

        // Утреннее небо: цвет фона камеры. Туман чёрный, поэтому дальние дома и трубы
        // превращаются в тёмные силуэты на сером небе.
        static readonly Color SkyColor = new Color(0.3f, 0.33f, 0.38f);

        // Стена вдоль X с проёмами под окна (центры окон по X).
        static void WallXWithWindows(Transform parent, float z, float x1, float x2, float[] windows, Material m)
        {
            float cursor = x1;
            foreach (float wx in windows)
            {
                float a = wx - WindowHalfWidth, b = wx + WindowHalfWidth;
                WallX(parent, z, cursor, a, m);
                Box("Wall", parent, new Vector3(wx, WindowBottom / 2f, z), new Vector3(b - a, WindowBottom, WallThickness), m);
                Box("Wall", parent, new Vector3(wx, (WindowTop + WallHeight) / 2f, z), new Vector3(b - a, WallHeight - WindowTop, WallThickness), m);
                cursor = b;
            }
            WallX(parent, z, cursor, x2, m);
        }

        static GameObject Window(string name, Transform parent, Vector3 position)
        {
            // Рама вокруг настоящего проёма в стене.
            var frameMat = Mat("Window Frame", new Color(0.7f, 0.7f, 0.68f), plasterTex);
            var z = position.z + 0.01f;
            Box(name + " Frame", parent, new Vector3(position.x, position.y + 0.5125f, z), new Vector3(1.0f, 0.075f, 0.04f), frameMat);
            Box(name + " Frame", parent, new Vector3(position.x, position.y - 0.5125f, z), new Vector3(1.0f, 0.075f, 0.04f), frameMat);
            Box(name + " Frame", parent, new Vector3(position.x - 0.4625f, position.y, z), new Vector3(0.075f, 0.95f, 0.04f), frameMat);
            Box(name + " Frame", parent, new Vector3(position.x + 0.4625f, position.y, z), new Vector3(0.075f, 0.95f, 0.04f), frameMat);

            // Стекла нет (как в старых играх), но есть невидимая преграда: через неё нельзя
            // выпасть, и на неё можно навести взгляд, чтобы посмотреть в окно.
            var pane = new GameObject(name);
            pane.transform.SetParent(parent, false);
            pane.transform.position = new Vector3(position.x, position.y, position.z + 0.06f);
            pane.AddComponent<BoxCollider>().size = new Vector3(0.85f, 0.95f, 0.05f);

            // Переплёт (крестовина рамы) и подоконник.
            var bar1 = Box(name + " Bar", parent, position + new Vector3(0f, 0.12f, -0.03f), new Vector3(0.85f, 0.04f, 0.02f), frameMat);
            var bar2 = Box(name + " Bar", parent, position + new Vector3(0f, 0f, -0.03f), new Vector3(0.04f, 0.95f, 0.02f), frameMat);
            Object.DestroyImmediate(bar1.GetComponent<Collider>());
            Object.DestroyImmediate(bar2.GetComponent<Collider>());
            Box(name + " Sill", parent, new Vector3(position.x, position.y - 0.565f, position.z - 0.1f), new Vector3(1.15f, 0.03f, 0.22f), frameMat);
            return pane;
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
