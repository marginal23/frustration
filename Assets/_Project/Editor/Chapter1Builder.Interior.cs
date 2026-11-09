using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Frustration.EditorTools
{
    // Вторая часть сборщика: советский интерьер и зеркало в коридоре.
    public static partial class Chapter1Builder
    {
        // Проём под зеркало в южной стене коридора.
        const float MirrorX0 = 1.85f, MirrorX1 = 2.55f, MirrorY0 = 0.35f, MirrorY1 = 1.95f;
        // Поверхность зеркала: внутренняя сторона южной стены (стена в z = 1.6, толщина 0.1).
        const float MirrorPlaneZ = 1.65f;

        struct Gap
        {
            public float a, b, bottom, top;
            public Gap(float a, float b, float bottom, float top) { this.a = a; this.b = b; this.bottom = bottom; this.top = top; }
        }

        static readonly Gap[] NoGaps = new Gap[0];
        const float DoorTop = 2.05f;

        // ================= Интерьер =================

        static void BuildInterior()
        {
            var kid = GameObject.Find("Kid Room").transform;
            var kitchen = GameObject.Find("Kitchen").transform;
            var hall = GameObject.Find("Hallway").transform;

            var wallpaperKid = Mat("Wallpaper Kid", Color.white, MakeColorTexture("Wallpaper Kid", 32, 32, WallpaperKidPixel));
            var wallpaperHall = Mat("Wallpaper Hall", Color.white, MakeColorTexture("Wallpaper Hall", 32, 32, WallpaperHallPixel));
            var tiles = Mat("Tiles", Color.white, MakeColorTexture("Tiles", 32, 32, TilePixel));
            var paint = Mat("Kitchen Paint", new Color(0.78f, 0.72f, 0.5f), plasterTex);
            var carpet = Mat("Carpet", Color.white, MakeColorTexture("Carpet", 64, 48, CarpetPixel));
            var oilcloth = Mat("Oilcloth", Color.white, MakeColorTexture("Oilcloth", 16, 16, OilclothPixel));
            var curtain = Mat("Curtain", Color.white, MakeColorTexture("Curtain", 16, 32, CurtainPixel));
            var wood = Mat("Wood", new Color(0.45f, 0.3f, 0.2f), woodTex);
            var white = Mat("Appliance", new Color(0.75f, 0.75f, 0.7f), plasterTex);
            var black = Mat("Strap", new Color(0.08f, 0.08f, 0.08f), plasterTex);

            // --- Обои и кафель ---
            var doorKid = new Gap(1.0f, 1.9f, 0f, DoorTop);
            var doorKitchen = new Gap(5.0f, 5.9f, 0f, DoorTop);
            var frontDoor = new Gap(1.85f, 2.75f, 0f, DoorTop);
            var mirrorHole = new Gap(MirrorX0, MirrorX1, MirrorY0, MirrorY1);

            // Детская: x 0..3.5, z 3..6.
            Panel("Wallpaper", kid, true, 6f, -1, 0f, 3.5f, 0f, WallHeight, NoGaps, wallpaperKid, 0.5f);
            Panel("Wallpaper", kid, true, 3f, +1, 0f, 3.5f, 0f, WallHeight, new[] { doorKid }, wallpaperKid, 0.5f);
            Panel("Wallpaper", kid, false, 0f, +1, 3f, 6f, 0f, WallHeight, NoGaps, wallpaperKid, 0.5f);
            Panel("Wallpaper", kid, false, 3.5f, -1, 3f, 6f, 0f, WallHeight, NoGaps, wallpaperKid, 0.5f);

            // Кухня: x 3.5..7, z 3..6. Кафель до 1.5 м, выше крашеная стена.
            foreach (var (y0, y1, m, repeat) in new[] { (0f, 1.5f, tiles, 0.6f), (1.5f, WallHeight, paint, 1f) })
            {
                Panel("Kitchen Wall", kitchen, true, 6f, -1, 3.5f, 7f, y0, y1, NoGaps, m, repeat);
                Panel("Kitchen Wall", kitchen, true, 3f, +1, 3.5f, 7f, y0, y1, new[] { doorKitchen }, m, repeat);
                Panel("Kitchen Wall", kitchen, false, 3.5f, +1, 3f, 6f, y0, y1, NoGaps, m, repeat);
                Panel("Kitchen Wall", kitchen, false, 7f, -1, 3f, 6f, y0, y1, NoGaps, m, repeat);
            }

            // Коридор: x 0..7, z 1.6..3.
            Panel("Wallpaper", hall, true, 3f, -1, 0f, 7f, 0f, WallHeight, new[] { doorKid, doorKitchen }, wallpaperHall, 0.5f);
            Panel("Wallpaper", hall, true, 1.6f, +1, 0f, 7f, 0f, WallHeight, new[] { mirrorHole }, wallpaperHall, 0.5f);
            Panel("Wallpaper", hall, false, 0f, +1, 1.6f, 3f, 0f, WallHeight, new[] { frontDoor }, wallpaperHall, 0.5f);
            Panel("Wallpaper", hall, false, 7f, -1, 1.6f, 3f, 0f, WallHeight, new[] { LivingRoomDoor }, wallpaperHall, 0.5f);

            // --- Детская ---
            // Ковёр на стене над кроватью: классика советской квартиры.
            AddQuad("Wall Carpet", kid, false, 0.05f + 0.008f, +1, 4.05f, 5.85f, 0.7f, 1.95f, carpet, 1.8f, 1.25f, 4.05f, 0.7f);
            Box("Floor Rug", kid, new Vector3(1.75f, 0.006f, 4.4f), new Vector3(1.3f, 0.012f, 0.95f), carpet);
            Primitive(PrimitiveType.Cylinder, "Lampshade", kid, new Vector3(1.75f, 2.47f, 4.5f), new Vector3(0.38f, 0.07f, 0.38f), Mat("Lampshade", new Color(0.85f, 0.75f, 0.5f), plasterTex));
            Box("Lamp Cord", kid, new Vector3(1.75f, 2.56f, 4.5f), new Vector3(0.015f, 0.1f, 0.015f), black);
            Box("Bookshelf", kid, new Vector3(3.33f, 1.45f, 4.4f), new Vector3(0.24f, 0.03f, 0.9f), wood);
            Color[] bookColors = { new Color(0.5f, 0.1f, 0.1f), new Color(0.1f, 0.25f, 0.45f), new Color(0.2f, 0.4f, 0.2f), new Color(0.6f, 0.5f, 0.2f), new Color(0.35f, 0.2f, 0.4f) };
            for (int i = 0; i < 9; i++)
            {
                float h = 0.16f + 0.05f * Mathf.PingPong(i * 0.37f, 1f);
                Box("Book", kid, new Vector3(3.36f, 1.465f + h / 2f, 4.05f + i * 0.075f), new Vector3(0.16f, h, 0.05f), Mat("Book " + (i % bookColors.Length), bookColors[i % bookColors.Length], plasterTex));
            }
            Curtains(kid, 1.6f, curtain);

            // --- Кухня ---
            Box("Oilcloth", kitchen, new Vector3(5.8f, 0.757f, 4.8f), new Vector3(1.06f, 0.012f, 0.86f), oilcloth);
            var stove = Box("Stove", kitchen, new Vector3(6.68f, 0.43f, 3.62f), new Vector3(0.55f, 0.86f, 0.55f), white);
            Interact(stove, "Плита", "Чайник холодный. Никто не завтракал.");
            Box("Stove Top", kitchen, new Vector3(6.68f, 0.865f, 3.62f), new Vector3(0.55f, 0.01f, 0.55f), black);
            Box("Oven Door", kitchen, new Vector3(6.4f, 0.36f, 3.62f), new Vector3(0.01f, 0.4f, 0.45f), Mat("Oven Door", new Color(0.3f, 0.3f, 0.3f), plasterTex));
            for (int i = 0; i < 4; i++)
            {
                var pos = new Vector3(6.55f + (i % 2) * 0.25f, 0.875f, 3.5f + (i / 2) * 0.25f);
                Primitive(PrimitiveType.Cylinder, "Burner", kitchen, pos, new Vector3(0.11f, 0.008f, 0.11f), Mat("Burner", new Color(0.15f, 0.15f, 0.15f), plasterTex));
            }
            Curtains(kitchen, 5.5f, curtain);

            // Настенные часы: тикают.
            var clock = Primitive(PrimitiveType.Cylinder, "Wall Clock", kitchen, new Vector3(6.93f, 1.95f, 4.8f), new Vector3(0.3f, 0.015f, 0.3f), Mat("Clock Face", new Color(0.85f, 0.83f, 0.75f), plasterTex));
            clock.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            var hand1 = Box("Clock Hand", kitchen, new Vector3(6.915f, 2.0f, 4.8f), new Vector3(0.005f, 0.1f, 0.012f), black);
            var hand2 = Box("Clock Hand", kitchen, new Vector3(6.915f, 1.95f, 4.84f), new Vector3(0.005f, 0.012f, 0.08f), black);
            Object.DestroyImmediate(hand1.GetComponent<Collider>());
            Object.DestroyImmediate(hand2.GetComponent<Collider>());
            Interact(clock, "Часы", "Без двадцати семь.");
            AddSound(clock, ProceduralAmbience.Kind.ClockTick, 0.25f, true);

            // --- Коридор ---
            Box("Dresser", hall, new Vector3((MirrorX0 + MirrorX1) / 2f, 0.17f, 1.8f), new Vector3(0.8f, 0.34f, 0.28f), wood);
            var phone = Box("Phone", hall, new Vector3(0.4f, 0.44f, 1.85f), new Vector3(0.18f, 0.08f, 0.2f), Mat("Phone", new Color(0.15f, 0.3f, 0.2f), plasterTex));
            Box("Phone Handset", hall, new Vector3(0.4f, 0.495f, 1.85f), new Vector3(0.05f, 0.04f, 0.22f), Mat("Phone", new Color(0.15f, 0.3f, 0.2f), plasterTex));
            Interact(phone, "Телефон", "Мама просила не звонить ей на работу.");
        }

        static void Curtains(Transform parent, float windowX, Material curtain)
        {
            Box("Curtain", parent, new Vector3(windowX - 0.62f, 1.5f, 5.89f), new Vector3(0.32f, 1.45f, 0.02f), curtain);
            Box("Curtain", parent, new Vector3(windowX + 0.62f, 1.5f, 5.89f), new Vector3(0.32f, 1.45f, 0.02f), curtain);
            Box("Curtain Rod", parent, new Vector3(windowX, 2.25f, 5.88f), new Vector3(1.7f, 0.025f, 0.025f), Mat("Metal", new Color(0.7f, 0.7f, 0.65f), plasterTex));
        }

        // ================= Зеркало =================

        static void BuildMirror(Transform player)
        {
            var hall = GameObject.Find("Hallway").transform;
            var frameMat = Mat("Mirror Frame", new Color(0.35f, 0.22f, 0.12f), woodTex);
            float cx = (MirrorX0 + MirrorX1) / 2f, cy = (MirrorY0 + MirrorY1) / 2f, w = MirrorX1 - MirrorX0, h = MirrorY1 - MirrorY0;
            const float t = 0.05f, z = MirrorPlaneZ + 0.02f;
            Box("Mirror Frame Top", hall, new Vector3(cx, MirrorY1 + t / 2f, z), new Vector3(w + 2f * t, t, 0.04f), frameMat);
            Box("Mirror Frame Bottom", hall, new Vector3(cx, MirrorY0 - t / 2f, z), new Vector3(w + 2f * t, t, 0.04f), frameMat);
            Box("Mirror Frame Left", hall, new Vector3(MirrorX0 - t / 2f, cy, z), new Vector3(t, h, 0.04f), frameMat);
            Box("Mirror Frame Right", hall, new Vector3(MirrorX1 + t / 2f, cy, z), new Vector3(t, h, 0.04f), frameMat);

            // Невидимая стенка, чтобы нельзя было зайти в зазеркалье. Её же можно осмотреть.
            var blocker = new GameObject("Mirror Blocker");
            blocker.transform.SetParent(hall, false);
            blocker.transform.position = new Vector3(cx, cy, MirrorPlaneZ - 0.03f);
            blocker.AddComponent<BoxCollider>().size = new Vector3(w, h, 0.06f);
            var look = blocker.AddComponent<Interactable>();
            look.prompt = "Зеркало";
            look.lines = new[] { "Это я.", "Мама говорит, я похож на папу." };

            // Зеркальная копия квартиры: та же геометрия, отражённая через плоскость зеркала.
            var mirrorRoot = new GameObject("Mirror World").transform;
            mirrorRoot.position = new Vector3(0f, 0f, 2f * MirrorPlaneZ);
            mirrorRoot.localScale = new Vector3(1f, 1f, -1f);
            foreach (var groupName in new[] { "Apartment", "Kid Room", "Kitchen", "Hallway", "Living Room" })
            {
                var original = GameObject.Find(groupName);
                var copy = Object.Instantiate(original, mirrorRoot);
                copy.name = groupName + " (Mirror)";
                StripMirrorCopy(original, copy);
            }

            // Отражение героя: мальчик в школьной форме.
            var reflection = new GameObject("Reflection").transform;
            var uniform = Mat("School Uniform", new Color(0.12f, 0.15f, 0.3f), plasterTex);
            var skin = Mat("Skin", new Color(0.6f, 0.45f, 0.38f), plasterTex);
            var hair = Mat("Hair", new Color(0.22f, 0.15f, 0.08f), plasterTex);
            var shirt = Mat("Shirt", new Color(0.85f, 0.85f, 0.82f), plasterTex);
            LocalPrimitive(PrimitiveType.Capsule, "Body", reflection, new Vector3(0f, 0.62f, 0f), new Vector3(0.34f, 0.38f, 0.24f), uniform);
            LocalBox("Collar", reflection, new Vector3(0f, 0.97f, 0.06f), new Vector3(0.14f, 0.06f, 0.08f), shirt);
            LocalPrimitive(PrimitiveType.Sphere, "Head", reflection, new Vector3(0f, 1.12f, 0f), Vector3.one * 0.22f, skin);
            LocalPrimitive(PrimitiveType.Sphere, "Hair", reflection, new Vector3(0f, 1.18f, -0.02f), new Vector3(0.235f, 0.15f, 0.235f), hair);
            var satchel = Satchel(reflection, Vector3.zero, Quaternion.identity).transform;
            satchel.localPosition = new Vector3(0f, 0.45f, -0.17f);
            satchel.localRotation = Quaternion.Euler(0f, 180f, 0f);
            foreach (var c in reflection.GetComponentsInChildren<Collider>(true)) Object.DestroyImmediate(c);

            var mirror = new GameObject("Mirror").AddComponent<MirrorWorld>();
            mirror.planeZ = MirrorPlaneZ;
            mirror.player = player;
            mirror.reflection = reflection;
            mirror.reflectionSatchel = satchel.gameObject;
        }

        // В копии оставляем только то, что видно: убираем коллайдеры, звуки, взаимодействия
        // и части стены с зеркалом. Свет копии повторяет мигание оригинала.
        static void StripMirrorCopy(GameObject original, GameObject copy)
        {
            var originals = original.GetComponentsInChildren<Transform>(true);
            var copies = copy.GetComponentsInChildren<Transform>(true);
            var toDelete = new List<GameObject>();

            for (int i = 0; i < originals.Length && i < copies.Length; i++)
            {
                var o = originals[i];
                var c = copies[i];

                // "Mirror ..." — сама рама и стена с зеркалом; "(no mirror)" — стены в плоскости зеркала,
                // копия которых вылезла бы в настоящую комнату.
                if (o.name.StartsWith("Mirror") || o.name.Contains("(no mirror)")) { toDelete.Add(c.gameObject); continue; }

                var interactable = o.GetComponent<Interactable>();
                if (interactable != null && interactable.hideAfterUse)
                    c.gameObject.AddComponent<ActiveSync>().source = o.gameObject;

                var originalLight = o.GetComponent<Light>();
                if (originalLight != null)
                {
                    var flicker = c.GetComponent<LightFlicker>();
                    if (flicker != null) Object.DestroyImmediate(flicker);
                    c.gameObject.AddComponent<LightSync>().source = originalLight;
                }

                foreach (var ambience in c.GetComponents<ProceduralAmbience>()) Object.DestroyImmediate(ambience);
                foreach (var audio in c.GetComponents<AudioSource>()) Object.DestroyImmediate(audio);
                foreach (var it in c.GetComponents<Interactable>()) Object.DestroyImmediate(it);
                foreach (var col in c.GetComponents<Collider>()) Object.DestroyImmediate(col);
            }

            foreach (var go in toDelete)
                if (go != null) Object.DestroyImmediate(go);
        }

        // ================= Помощники интерьера =================

        // Обои/кафель на одной стене: плоские полосы с дырками под двери и зеркало.
        // alongX: стена идёт вдоль X в z = line (иначе вдоль Z в x = line).
        // side: с какой стороны стены комната (+1 или -1).
        static void Panel(string name, Transform parent, bool alongX, float line, int side, float a, float b,
                          float y0, float y1, Gap[] gaps, Material material, float repeat)
        {
            float plane = line + side * (WallThickness / 2f + 0.004f);
            var sorted = new List<Gap>(gaps);
            sorted.Sort((g1, g2) => g1.a.CompareTo(g2.a));

            float cursor = a;
            foreach (var g in sorted)
            {
                if (g.a > cursor) AddQuad(name, parent, alongX, plane, side, cursor, g.a, y0, y1, material, repeat, repeat, 0f, 0f);
                float below = Mathf.Min(y1, g.bottom);
                if (below > y0) AddQuad(name, parent, alongX, plane, side, g.a, g.b, y0, below, material, repeat, repeat, 0f, 0f);
                float above = Mathf.Max(y0, g.top);
                if (above < y1) AddQuad(name, parent, alongX, plane, side, g.a, g.b, above, y1, material, repeat, repeat, 0f, 0f);
                cursor = g.b;
            }
            if (b > cursor) AddQuad(name, parent, alongX, plane, side, cursor, b, y0, y1, material, repeat, repeat, 0f, 0f);

            // Плинтус вдоль пола, с разрывами под двери (дырка под зеркало его не прерывает).
            if (y0 <= 0f)
            {
                var skirting = Mat("Skirting", new Color(0.3f, 0.2f, 0.13f), woodTex);
                float start = a;
                foreach (var g in sorted)
                {
                    if (g.bottom > 0f) continue;
                    if (g.a > start) Skirting(parent, alongX, plane, side, start, g.a, skirting);
                    start = g.b;
                }
                if (b > start) Skirting(parent, alongX, plane, side, start, b, skirting);
            }
        }

        static void Skirting(Transform parent, bool alongX, float plane, int side, float u0, float u1, Material m)
        {
            const float height = 0.07f, depth = 0.02f;
            float mid = (u0 + u1) / 2f, len = u1 - u0;
            float offset = plane + side * depth / 2f;
            var pos = alongX ? new Vector3(mid, height / 2f, offset) : new Vector3(offset, height / 2f, mid);
            var size = alongX ? new Vector3(len, height, depth) : new Vector3(depth, height, len);
            var go = Box("Skirting", parent, pos, size, m);
            Object.DestroyImmediate(go.GetComponent<Collider>());
        }

        // Плоский прямоугольник с UV в метрах (текстура повторяется каждые repeatU/repeatV метров).
        static GameObject AddQuad(string name, Transform parent, bool alongX, float plane, int side,
                                  float u0, float u1, float y0, float y1, Material material,
                                  float repeatU, float repeatV, float uOrigin, float vOrigin)
        {
            Vector3 P(float u, float y) => alongX ? new Vector3(u, y, plane) : new Vector3(plane, y, u);
            var vertices = new[] { P(u0, y0), P(u1, y0), P(u1, y1), P(u0, y1) };
            var normal = alongX ? new Vector3(0f, 0f, side) : new Vector3(side, 0f, 0f);
            var uvs = new[]
            {
                new Vector2((u0 - uOrigin) / repeatU, (y0 - vOrigin) / repeatV),
                new Vector2((u1 - uOrigin) / repeatU, (y0 - vOrigin) / repeatV),
                new Vector2((u1 - uOrigin) / repeatU, (y1 - vOrigin) / repeatV),
                new Vector2((u0 - uOrigin) / repeatU, (y1 - vOrigin) / repeatV),
            };
            // Лицевая сторона треугольника должна смотреть в комнату.
            bool facesNormal = Vector3.Dot(Vector3.Cross(vertices[1] - vertices[0], vertices[2] - vertices[0]), normal) > 0f;
            var triangles = facesNormal ? new[] { 0, 1, 2, 0, 2, 3 } : new[] { 0, 2, 1, 0, 3, 2 };

            var mesh = new Mesh { name = name };
            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.normals = new[] { normal, normal, normal, normal };
            mesh.triangles = triangles;
            mesh.RecalculateBounds();

            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            var renderer = go.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            return go;
        }

        static Texture2D MakeColorTexture(string name, int width, int height, System.Func<int, int, float, Color> pixel)
        {
            string path = $"{Root}/Textures/{name}.png";
            var random = new System.Random(name.GetHashCode());
            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                var c = pixel(x, y, (float)random.NextDouble());
                c.a = 1f;
                tex.SetPixel(x, y, c);
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

        static Color Noise(Color c, float r, float amount) => c * (1f - amount + 2f * amount * r);

        // Зеленоватые обои с полосами и ромбиками.
        static Color WallpaperKidPixel(int x, int y, float r)
        {
            var c = new Color(0.62f, 0.68f, 0.52f);
            if (x % 16 == 0) c = new Color(0.5f, 0.56f, 0.42f);
            int dx = Mathf.Abs((x % 16) - 8), dy = Mathf.Abs((y % 16) - 8);
            if (dx + dy == 3) c = new Color(0.78f, 0.58f, 0.42f);
            if (dx + dy == 0) c = new Color(0.7f, 0.3f, 0.25f);
            return Noise(c, r, 0.05f);
        }

        // Коричневые обои коридора: полосы и точки.
        static Color WallpaperHallPixel(int x, int y, float r)
        {
            var c = new Color(0.5f, 0.38f, 0.27f);
            if (x % 8 == 0) c = new Color(0.4f, 0.29f, 0.2f);
            if (x % 8 == 4 && y % 8 == 4) c = new Color(0.68f, 0.56f, 0.38f);
            return Noise(c, r, 0.05f);
        }

        // Белый кафель с серыми швами.
        static Color TilePixel(int x, int y, float r)
        {
            if (x % 8 == 0 || y % 8 == 0) return new Color(0.5f, 0.52f, 0.52f);
            return Noise(new Color(0.8f, 0.85f, 0.86f), r, 0.04f);
        }

        // Красный ковёр: синяя кайма, бежевая полоса, ромб-медальон в центре.
        static Color CarpetPixel(int x, int y, float r)
        {
            const int w = 64, h = 48;
            var red = new Color(0.5f, 0.08f, 0.06f);
            var darkRed = new Color(0.32f, 0.04f, 0.04f);
            var blue = new Color(0.1f, 0.12f, 0.3f);
            var beige = new Color(0.82f, 0.7f, 0.5f);
            int edge = Mathf.Min(Mathf.Min(x, w - 1 - x), Mathf.Min(y, h - 1 - y));
            Color c;
            if (edge < 4) c = blue;
            else if (edge == 5) c = beige;
            else
            {
                float dx = Mathf.Abs(x - (w - 1) / 2f) / (w / 2f);
                float dy = Mathf.Abs(y - (h - 1) / 2f) / (h / 2f);
                float d = dx + dy;
                if (d < 0.75f)
                {
                    int ring = Mathf.FloorToInt(d * 12f);
                    c = ring % 3 == 0 ? beige : (ring % 3 == 1 ? darkRed : blue);
                }
                else c = ((x / 4 + y / 4) % 2 == 0) ? red : darkRed;
            }
            return Noise(c, r, 0.06f);
        }

        // Клеёнка в красно-белую клетку.
        static Color OilclothPixel(int x, int y, float r)
        {
            bool a = (x / 4) % 2 == 0, b = (y / 4) % 2 == 0;
            Color c = a && b ? new Color(0.65f, 0.12f, 0.1f) : (a || b ? new Color(0.82f, 0.6f, 0.55f) : new Color(0.9f, 0.88f, 0.82f));
            return Noise(c, r, 0.03f);
        }

        // Тюль/шторы: бежевые с вертикальными складками и мелкими цветочками.
        static Color CurtainPixel(int x, int y, float r)
        {
            var c = new Color(0.78f, 0.72f, 0.58f) * (0.85f + 0.15f * Mathf.Sin(x * 0.8f));
            if (x % 8 == 3 && y % 8 == 3) c = new Color(0.65f, 0.35f, 0.3f);
            return Noise(c, r, 0.04f);
        }
    }
}
