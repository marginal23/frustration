using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Frustration.EditorTools
{
    // Главы игры: одна и та же квартира утром 1 сентября в разные годы.
    // decay (0..4) — насколько квартира «постарела»: обои желтеют и отклеиваются, бутылок больше,
    // вещи пропадают. growth — во сколько раз герой выше, чем в первом классе.
    // Всё, что отличается по годам, собирается здесь или проверяет chapter.decay в других частях сборщика.
    public static partial class Chapter1Builder
    {
        public class Chapter
        {
            public int number, grade, year, decay;
            public float growth;
            public string sceneName, gradeName;
            public string[] introLines, noteLines, requiredFlags, missingLines;
        }

        static Chapter chapter;

        static readonly Chapter[] Chapters =
        {
            new Chapter
            {
                number = 1, grade = 1, year = 2004, decay = 0, growth = 1f, sceneName = "Chapter1_Morning", gradeName = "Первый класс",
                introLines = new[] { "Мама меня не разбудила.", "Сегодня первое сентября. Надо собираться." },
                noteLines = new[] { "Записка от мамы.", "«Сынок, я на смене до вечера. Цветы на столе, ранец на стуле.»", "«Папу не буди. Ключ под ковриком. Люблю тебя.»" },
                requiredFlags = new[] { "note", "flowers", "backpack" },
                missingLines = new[] { "Мамы нигде нет. Может, она что-то оставила на кухне?", "Цветы! Мама купила их для учительницы.", "Без ранца в школу нельзя." },
            },
            new Chapter
            {
                number = 2, grade = 5, year = 2008, decay = 1, growth = 1.15f, sceneName = "Chapter2_Grade5_2008", gradeName = "Пятый класс",
                introLines = new[] { "Пятый класс.", "Мама опять ушла на смену затемно." },
                noteLines = new[] { "Записка от мамы.", "«Я на смене. Цветы для классной стоят в банке.»", "«Поешь. Папу не буди.»" },
                requiredFlags = new[] { "note", "flowers", "backpack" },
                missingLines = new[] { "Мама наверняка оставила записку на холодильнике.", "Цветы для классной. Мама просила не забыть.", "Без рюкзака в школу нельзя." },
            },
            new Chapter
            {
                number = 3, grade = 6, year = 2009, decay = 2, growth = 1.2f, sceneName = "Chapter3_Grade6_2009", gradeName = "Шестой класс",
                introLines = new[] { "Шестой класс.", "Папа теперь часто спит в зале." },
                noteLines = new[] { "Записка от мамы.", "«Деньги на проезд на столе. Купи хлеба после школы.»" },
                requiredFlags = new[] { "note", "backpack" },
                missingLines = new[] { "На холодильнике, наверное, опять записка.", "Без рюкзака в школу нельзя." },
            },
            new Chapter
            {
                number = 4, grade = 9, year = 2012, decay = 3, growth = 1.38f, sceneName = "Chapter4_Grade9_2012", gradeName = "Девятый класс",
                introLines = new[] { "Девятый класс.", "Не хочу никуда идти." },
                noteLines = new[] { "Записка от мамы.", "«Опять. Поговорим вечером.»", "«Деньги на проезд на столе.»" },
                requiredFlags = new[] { "note", "backpack" },
                missingLines = new[] { "Мама что-то оставила на холодильнике. Как всегда.", "Рюкзак. Хоть тетрадь положить." },
            },
            new Chapter
            {
                number = 5, grade = 11, year = 2014, decay = 4, growth = 1.45f, sceneName = "Chapter5_Grade11_2014", gradeName = "Одиннадцатый класс",
                introLines = new[] { "Одиннадцатый класс. Последний.", "Мама уехала к бабушке месяц назад." },
                noteLines = new[] { "Старая записка от мамы.", "«Я у бабушки. Не звони мне пока.»", "Она висит здесь уже месяц." },
                requiredFlags = new[] { "note", "backpack" },
                missingLines = new[] { "Записка на холодильнике. Надо ещё раз прочитать.", "Рюкзак. Хоть тетрадь положить." },
            },
        };

        static string ScenePathOf(Chapter c) => $"{Root}/Scenes/{c.sceneName}.unity";

        // Имя ассета, который в каждом году свой (обои, волосы отца и т.п.).
        static string Aged(string name) => chapter.decay == 0 ? name : $"{name} {chapter.year}";

        [MenuItem("Frustration/Build All Chapters")]
        public static void Build()
        {
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            for (int i = 0; i < Chapters.Length; i++)
                BuildChapter(Chapters[i], i + 1 < Chapters.Length ? Chapters[i + 1] : null);
            EditorBuildSettings.scenes = Chapters.Select(c => new EditorBuildSettingsScene(ScenePathOf(c), true)).ToArray();
            AssetDatabase.SaveAssets();
            EditorSceneManager.OpenScene(ScenePathOf(Chapters[0]));
            Debug.Log("[Frustration] All chapters built.");
        }

        static void BuildChapter(Chapter c, Chapter next)
        {
            chapter = c;
            nextChapter = next;

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
            BuildFurniture();
            BuildYearDetails();
            BuildOutside();
            var player = BuildPlayerAndSystems();
            BuildMirror(player);

            EditorSceneManager.SaveScene(scene, ScenePathOf(c));
            Debug.Log($"[Frustration] Chapter {c.number} ({c.year}) built: {ScenePathOf(c)}");
        }

        static Chapter nextChapter;

        // Обои и кафель стареют: желтеют, покрываются пятнами, к девятому классу отклеиваются до штукатурки.
        static System.Func<int, int, float, Color> Wear(System.Func<int, int, float, Color> pixel)
        {
            int d = chapter.decay;
            if (d == 0) return pixel;
            return (x, y, r) =>
            {
                var c = pixel(x, y, r);
                c = Color.Lerp(c, new Color(0.62f, 0.56f, 0.4f) * (c.grayscale + 0.4f), 0.07f * d);
                float stain = Mathf.PerlinNoise(x * 0.21f + 3.1f, y * 0.17f + d * 1.7f);
                if (stain > 0.8f - 0.05f * d) c *= 0.8f;
                if (d >= 3 && Mathf.PerlinNoise(x * 0.33f + 9f, y * 0.33f) > 0.84f) c = new Color(0.62f, 0.62f, 0.6f) * (0.9f + 0.1f * r);
                c.a = 1f;
                return c;
            };
        }

        static Color Lighter(Color c, float k) => Color.Lerp(c, Color.white, k);

        // ================= Вещи, которые появляются и исчезают с годами =================

        static void BuildYearDetails()
        {
            int d = chapter.decay;
            var kid = GameObject.Find("Kid Room").transform;
            var kitchen = GameObject.Find("Kitchen").transform;
            var living = GameObject.Find("Living Room").transform;

            // --- Детская: плакаты ---
            if (d >= 1)
            {
                string first = d >= 3 ? "Poster Band" : "Poster Football";
                var p = Primitive(PrimitiveType.Quad, "Poster", kid, new Vector3(2.95f, 1.6f, 5.943f), new Vector3(0.42f, 0.56f, 1f),
                    Mat(first, Color.white, MakeColorTexture(first, 12, 16, d >= 3 ? (System.Func<int, int, float, Color>)BandPosterPixel : FootballPosterPixel)));
                p.transform.rotation = Quaternion.identity;
                Interact(p, "Плакат", d >= 3 ? new[] { "Вырезал из журнала.", "Папа сказал, что это сатанисты." } : new[] { "Футбол. Сборная.", "Папа раньше смотрел со мной." });
            }
            if (d >= 2)
            {
                string second = d >= 3 ? "Poster Band Grey" : "Poster Rocket";
                var p = Deco(PrimitiveType.Quad, "Poster", kid, new Vector3(3.443f, 1.95f, 4.4f), new Vector3(0.4f, 0.52f, 1f),
                    Mat(second, Color.white, MakeColorTexture(second, 12, 16, d >= 3 ? (System.Func<int, int, float, Color>)GreyPosterPixel : RocketPosterPixel)));
                p.transform.rotation = Quaternion.Euler(0f, 90f, d >= 4 ? 6f : 0f);
            }

            // --- Детская: кассетный плеер, потом сигареты, потом бутылки ---
            if (d >= 2)
            {
                var player = Box("Cassette Player", kid, new Vector3(2.7f, 0.79f, 5.5f), new Vector3(0.14f, 0.08f, 0.1f), Mat("Player Silver", new Color(0.6f, 0.6f, 0.62f), plasterTex));
                Deco(PrimitiveType.Cube, "Cassette Window", kid, new Vector3(2.7f, 0.83f, 5.5f), new Vector3(0.09f, 0.002f, 0.05f), Mat("Oven Glass", new Color(0.06f, 0.06f, 0.07f), plasterTex));
                Deco(PrimitiveType.Cube, "Headphones", kid, new Vector3(2.85f, 0.765f, 5.47f), new Vector3(0.15f, 0.03f, 0.03f), Black);
                Interact(player, "Плеер", d >= 3 ? new[] { "Плеер. Батарейки сели ещё весной." } : new[] { "Плеер. Мама подарила на день рождения." });
            }
            if (d >= 3)
            {
                var pack = Box("Cigarettes", kid, new Vector3(1.38f, 0.995f, 5.84f), new Vector3(0.055f, 0.085f, 0.022f), Mat("Cigarette Pack", new Color(0.75f, 0.12f, 0.1f), plasterTex));
                Interact(pack, "Сигареты", d >= 4 ? new[] { "Уже всё равно, найдёт мама или нет." } : new[] { "Если мама найдёт — убьёт.", "Прячу за шторой." });
                Deco(PrimitiveType.Cylinder, "Jar Lid Ashtray", kid, new Vector3(1.8f, 0.955f, 5.84f), new Vector3(0.08f, 0.005f, 0.08f), Metal);
                for (int i = 0; i < 2 + d; i++)
                {
                    var b = Deco(PrimitiveType.Cube, "Butt", kid, new Vector3(1.78f + (i % 3) * 0.02f, 0.965f, 5.83f + (i / 3) * 0.02f), new Vector3(0.01f, 0.01f, 0.035f), Mat("Cigarette Butt", new Color(0.75f, 0.6f, 0.4f), plasterTex));
                    b.transform.rotation = Quaternion.Euler(0f, i * 53f, 0f);
                }
                // Толстовка на спинке стула.
                Deco(PrimitiveType.Cube, "Hoodie On Chair", kid, new Vector3(2.8f, 0.82f, 4.86f), new Vector3(0.42f, 0.3f, 0.05f), Mat("Hoodie", new Color(0.12f, 0.12f, 0.14f), plasterTex));
            }
            if (d >= 4)
            {
                var kidBottles = new GameObject("Kid Bottles").transform;
                kidBottles.SetParent(kid, false);
                Bottle(kidBottles, new Vector3(1.05f, 0f, 4.15f), Quaternion.identity, vodka: false);
                Bottle(kidBottles, new Vector3(1.15f, 0.033f, 3.9f), Quaternion.Euler(0f, 70f, 90f), vodka: false);
                var it = kidBottles.gameObject.AddComponent<Interactable>();
                it.prompt = "Бутылки";
                it.lines = new[] { "Это не папины." };
                var col = kidBottles.gameObject.AddComponent<BoxCollider>();
                col.center = new Vector3(1.1f, 0.12f, 4.05f);
                col.size = new Vector3(0.3f, 0.25f, 0.4f);
            }

            // --- Кухня: бутылок с каждым годом больше ---
            var bottles = kitchen.Find("Bottles");
            var spots = new[]
            {
                (6.3f, 0f, 4.42f, false), (6.18f, 0f, 4.5f, false), (3.62f, 0f, 4.6f, false), (4.2f, 0.04f, 4.45f, true),
                (6.2f, 0f, 5.85f, false), (6.05f, 0f, 5.88f, false), (5.4f, 0.75f, 4.7f, false), (5.92f, 0.75f, 4.95f, false),
                (4.9f, 0.04f, 5.5f, true), (5.2f, 0f, 5.85f, false), (5.35f, 0f, 5.86f, false), (4.6f, 0.04f, 3.4f, true),
            };
            int extra = new[] { 0, 2, 4, 8, 12 }[d];
            for (int i = 0; i < extra; i++)
            {
                var (x, y, z, lying) = spots[i];
                Bottle(bottles, new Vector3(x, y, z), lying ? Quaternion.Euler(0f, i * 40f, 90f) : Quaternion.Euler(0f, i * 25f, 0f), vodka: true);
            }
            bottles.GetComponent<Interactable>().lines = new[]
            {
                new[] { "Вчера их было меньше." },
                new[] { "Их всё больше." },
                new[] { "Мама перестала их выносить." },
                new[] { "Я уже не считаю." },
                new[] { "Сдаю их в пункт приёма. Хватает на сигареты." },
            }[d];

            // --- Зал: отец спит на диване, его подушка и одеяло ---
            if (d >= 2)
            {
                Deco(PrimitiveType.Sphere, "Father Pillow", living, new Vector3(8.2f, 0.47f, 2.15f), new Vector3(0.45f, 0.12f, 0.35f), Mat("Pillow", new Color(0.88f, 0.87f, 0.83f), plasterTex));
                var blanket = Deco(PrimitiveType.Cube, "Father Blanket", living, new Vector3(9.4f, 0.43f, 2.2f), new Vector3(1.1f, 0.04f, 0.6f), AssetDatabase.LoadAssetAtPath<Material>(Root + "/Materials/Plaid.mat"));
                blanket.transform.rotation = Quaternion.Euler(0f, 8f, 4f);
            }
            if (d == 2) FatherOnSofa(living);

            // --- Зал: свадебная фотография ---
            var wedding = living.Find("Wedding Photo");
            if (d == 3 && wedding != null)
            {
                // Трещина по стеклу.
                var crack = Mat("Crack", new Color(0.05f, 0.05f, 0.05f), plasterTex);
                var c1 = Deco(PrimitiveType.Cube, "Glass Crack", living, new Vector3(8.6f, 1.57f, 1.682f), new Vector3(0.3f, 0.004f, 0.002f), crack);
                c1.transform.rotation = Quaternion.Euler(0f, 0f, 55f);
                var c2 = Deco(PrimitiveType.Cube, "Glass Crack", living, new Vector3(8.64f, 1.5f, 1.682f), new Vector3(0.14f, 0.004f, 0.002f), crack);
                c2.transform.rotation = Quaternion.Euler(0f, 0f, -20f);
                wedding.GetComponent<Interactable>().lines = new[] { "Мама и папа.", "Стекло треснуло, когда они ругались." };
            }
            if (d >= 4 && wedding != null)
            {
                // Фото сняли со стены: на обоях остался светлый след, а сама рамка лежит лицом вниз на столике.
                Object.DestroyImmediate(living.Find("Wedding Photo Image").gameObject);
                Object.DestroyImmediate(wedding.gameObject);
                AddQuad("Photo Mark", living, true, 1.6f + WallThickness / 2f + 0.006f, +1, 8.44f, 8.76f, 1.34f, 1.76f,
                    Mat("Wallpaper Living", Color.white, MakeColorTexture("Wallpaper Living", 24, 32, WallpaperLivingPixel)), 0.5f, 0.5f, 0f, 0f);
                Deco(PrimitiveType.Cube, "Nail", living, new Vector3(8.6f, 1.78f, 1.66f), new Vector3(0.01f, 0.01f, 0.02f), Metal);
                var down = Box("Wedding Photo Face Down", living, new Vector3(8.8f, 0.51f, 3.4f), new Vector3(0.32f, 0.02f, 0.42f), Mat("Mirror Frame", new Color(0.35f, 0.22f, 0.12f), woodTex));
                down.transform.rotation = Quaternion.Euler(0f, 20f, 0f);
                Interact(down, "Фотография", "Свадебная фотография.", "Кто-то положил её лицом вниз.");
            }
        }

        // Отец спит на диване в зале: лежит на боку, рука свисает к полу, рядом бутылка.
        static void FatherOnSofa(Transform living)
        {
            var g = Group("Father", living, new Vector3(9.0f, 0f, 2.15f), 0f);
            var shirt = Mat("Undershirt", new Color(0.78f, 0.76f, 0.7f), plasterTex);
            var pants = Mat("Track Pants", new Color(0.12f, 0.16f, 0.35f), plasterTex);
            var skin = Mat("Skin", new Color(0.6f, 0.45f, 0.38f), plasterTex);
            var hair = FatherHair();
            Part(g, "Torso", new Vector3(-0.35f, 0.55f, 0f), new Vector3(0.3f, 0.32f, 0.42f), shirt, PrimitiveType.Capsule, rz: 90f);
            Part(g, "Belly", new Vector3(-0.25f, 0.52f, 0.06f), new Vector3(0.3f, 0.24f, 0.3f), shirt, PrimitiveType.Sphere);
            Part(g, "Pelvis", new Vector3(0.05f, 0.52f, 0f), new Vector3(0.24f, 0.22f, 0.36f), pants, PrimitiveType.Sphere);
            Part(g, "Legs", new Vector3(0.45f, 0.5f, 0.02f), new Vector3(0.16f, 0.38f, 0.3f), pants, PrimitiveType.Capsule, rz: 90f);
            Part(g, "Head", new Vector3(-0.78f, 0.6f, 0.02f), new Vector3(0.22f, 0.2f, 0.23f), skin, PrimitiveType.Sphere);
            Part(g, "Hair", new Vector3(-0.8f, 0.64f, 0f), new Vector3(0.21f, 0.15f, 0.22f), hair, PrimitiveType.Sphere);
            Part(g, "Arm Hanging", new Vector3(-0.3f, 0.38f, 0.3f), new Vector3(0.09f, 0.2f, 0.09f), skin, PrimitiveType.Capsule, rx: -20f);
            Part(g, "Hand", new Vector3(-0.3f, 0.12f, 0.36f), new Vector3(0.08f, 0.06f, 0.1f), skin, PrimitiveType.Sphere);
            Bottle(g, new Vector3(8.8f, 0.033f, 2.62f), Quaternion.Euler(0f, 30f, 90f), vodka: true);
            Solid(g, new Vector3(-0.1f, 0.55f, 0f), new Vector3(1.6f, 0.4f, 0.5f));
            Interact(g.gameObject, "Папа", "Папа спит в зале.", "Он не дошёл до спальни. Или мама его не пустила.");
        }

        // Волосы отца седеют с годами.
        static Material FatherHair()
        {
            var dark = new Color(0.15f, 0.12f, 0.1f);
            var grey = new Color(0.5f, 0.48f, 0.46f);
            return Mat(Aged("Hair Father"), Color.Lerp(dark, grey, chapter.decay * 0.18f), plasterTex);
        }

        // Рюкзак школьника постарше: тёмный, с лямками и карманом спереди. Перед смотрит в +Z.
        static GameObject Backpack(Transform parent, Vector3 position, Quaternion rotation)
        {
            var bag = new GameObject("Backpack").transform;
            bag.SetParent(parent, false);
            bag.SetPositionAndRotation(position, rotation);
            var cloth = Mat(chapter.decay >= 3 ? "Backpack Black" : "Backpack Blue", chapter.decay >= 3 ? new Color(0.1f, 0.1f, 0.11f) : new Color(0.12f, 0.2f, 0.42f), plasterTex);
            var dark = Mat("Strap", new Color(0.08f, 0.08f, 0.08f), plasterTex);
            LocalPrimitive(PrimitiveType.Capsule, "Body", bag, new Vector3(0f, 0.2f, 0f), new Vector3(0.3f, 0.2f, 0.16f), cloth);
            LocalBox("Pocket", bag, new Vector3(0f, 0.13f, 0.08f), new Vector3(0.22f, 0.14f, 0.05f), cloth);
            LocalBox("Zip", bag, new Vector3(0f, 0.2f, 0.106f), new Vector3(0.2f, 0.008f, 0.006f), Metal);
            LocalBox("Strap L", bag, new Vector3(-0.08f, 0.2f, -0.085f), new Vector3(0.035f, 0.3f, 0.012f), dark);
            LocalBox("Strap R", bag, new Vector3(0.08f, 0.2f, -0.085f), new Vector3(0.035f, 0.3f, 0.012f), dark);
            LocalBox("Handle", bag, new Vector3(0f, 0.41f, 0f), new Vector3(0.08f, 0.02f, 0.02f), dark);
            return bag.gameObject;
        }

        // ---------- Плакаты 12x16 ----------

        static Color FootballPosterPixel(int x, int y, float r)
        {
            var c = (y / 2) % 2 == 0 ? new Color(0.2f, 0.5f, 0.2f) : new Color(0.25f, 0.55f, 0.22f);
            if (y == 7 || x == 0 || x == 11 || y == 0) c = new Color(0.9f, 0.9f, 0.88f);
            if (Ellipse(x, y, 4f, 10f, 1.2f, 1.6f)) c = new Color(0.8f, 0.15f, 0.12f);   // футболист в красном
            if (Ellipse(x, y, 4f, 12.5f, 0.8f, 0.8f)) c = new Color(0.82f, 0.65f, 0.52f);
            if (Ellipse(x, y, 8f, 4f, 1f, 1f)) c = (x + y) % 2 == 0 ? Color.white : Color.black;  // мяч
            if (y >= 14) c = new Color(0.85f, 0.85f, 0.3f);
            return Noise(c, r, 0.05f);
        }

        static Color RocketPosterPixel(int x, int y, float r)
        {
            var c = new Color(0.05f, 0.07f, 0.18f);
            if (r > 0.94f) c = new Color(0.9f, 0.9f, 0.8f);
            if (Mathf.Abs(x - 5.5f) <= 1f && y >= 4 && y <= 12) c = new Color(0.85f, 0.85f, 0.85f);
            if (Mathf.Abs(x - 5.5f) <= (13 - y) * 0.5f && y >= 12 && y <= 14) c = new Color(0.8f, 0.15f, 0.12f);
            if (y >= 1 && y <= 3 && Mathf.Abs(x - 5.5f) <= 2f - (3 - y) * 0.5f) c = new Color(1f, 0.6f, 0.15f);
            return Noise(c, r, 0.04f);
        }

        static Color BandPosterPixel(int x, int y, float r)
        {
            var c = new Color(0.05f, 0.05f, 0.05f);
            if (y >= 12 && y <= 14 && x >= 1 && x <= 10 && (x % 3 != 0 || y == 13)) c = new Color(0.75f, 0.1f, 0.08f);  // буквы
            if (Ellipse(x, y, 5.5f, 6.5f, 3f, 3.5f)) c = new Color(0.82f, 0.8f, 0.75f);                               // лицо-маска
            if ((x == 4 || x == 7) && y == 7) c = Color.black;
            if (y == 4 && x >= 4 && x <= 7) c = Color.black;
            return Noise(c, r, 0.08f);
        }

        static Color GreyPosterPixel(int x, int y, float r)
        {
            var c = new Color(0.35f, 0.35f, 0.36f);
            if (Ellipse(x, y, 5.5f, 8f, 2.5f, 5f)) c = new Color(0.12f, 0.12f, 0.13f);
            if (y >= 14) c = new Color(0.9f, 0.9f, 0.88f);
            return Noise(c, r, 0.1f);
        }
    }
}
