using UnityEngine;

namespace Frustration.EditorTools
{
    // Четвёртая часть сборщика: мелкие детали, которые делают квартиру жилой.
    // Наличники, батареи, выключатели, розетки и бытовые мелочи в каждой комнате.
    public static partial class Chapter1Builder
    {
        static void BuildDetails()
        {
            var kid = GameObject.Find("Kid Room").transform;
            var kitchen = GameObject.Find("Kitchen").transform;
            var hall = GameObject.Find("Hallway").transform;
            var living = GameObject.Find("Living Room").transform;

            var trim = Mat("Door Trim", new Color(0.4f, 0.27f, 0.17f), woodTex);
            var painted = Mat("Radiator", new Color(0.8f, 0.78f, 0.7f), plasterTex);
            var plastic = Mat("Switch", new Color(0.85f, 0.83f, 0.76f), plasterTex);
            var metal = Mat("Metal", new Color(0.7f, 0.7f, 0.65f), plasterTex);
            var glass = Mat("Glass Clear", new Color(0.55f, 0.62f, 0.6f), plasterTex);
            var white = Mat("Appliance", new Color(0.75f, 0.75f, 0.7f), plasterTex);
            var black = Mat("Strap", new Color(0.08f, 0.08f, 0.08f), plasterTex);
            var wood = Mat("Wood", new Color(0.45f, 0.3f, 0.2f), woodTex);

            // --- Наличники вокруг дверных проёмов ---
            DoorTrim(hall, true, 3f, 1.0f, 1.9f, 0, trim);      // детская
            DoorTrim(hall, true, 3f, 5.0f, 5.9f, 0, trim);      // кухня
            DoorTrim(hall, false, 7f, 1.85f, 2.75f, 0, trim);   // зал
            DoorTrim(hall, false, 0f, 1.85f, 2.75f, +1, trim);  // входная дверь
            DoorTrim(living, false, 11f, 3.85f, 4.75f, -1, trim); // спальня

            // --- Батареи под окнами ---
            Radiator(kid, 1.6f, painted);
            Radiator(kitchen, 5.5f, painted);
            Radiator(living, 10.0f, painted);

            // --- Выключатели и розетки ---
            WallThing(kid, false, 3.5f, -1, 5.3f, 0.3f, plastic);
            WallThing(kitchen, true, 6f, -1, 6.2f, 1.1f, plastic);
            WallThing(living, false, 11f, -1, 2.6f, 0.3f, plastic);

            // ================= Детская =================

            // Букет гладиолусов к первому сентября.
            var jar = Primitive(PrimitiveType.Cylinder, "Flower Jar", kid, new Vector3(3.05f, 0.83f, 5.7f), new Vector3(0.1f, 0.08f, 0.1f), glass);
            Interact(jar, "Цветы", "Гладиолусы для учительницы.", "Мама вчера купила их у бабушки возле магазина.");
            // Каждый цветок — две скрещённые плоскости с пиксельной текстурой, как растения в PS1-играх.
            var gladiolusRed = Mat("Gladiolus Red", Color.white, MakeColorTexture("Gladiolus Red", 16, 48, (x, y, r) => GladiolusPixel(x, y, r, false), true));
            var gladiolusWhite = Mat("Gladiolus White", Color.white, MakeColorTexture("Gladiolus White", 16, 48, (x, y, r) => GladiolusPixel(x, y, r, true), true));
            foreach (var m in new[] { gladiolusRed, gladiolusWhite }) { m.SetFloat("_Cutoff", 0.5f); m.SetFloat("_Cull", 0f); }
            var stems = new[] { (-0.025f, 0f, -6f, 0.52f), (0.02f, 0.015f, 5f, 0.6f), (0.0f, -0.02f, 1f, 0.48f) };
            for (int i = 0; i < stems.Length; i++)
            {
                var (ox, oz, tilt, h) = stems[i];
                var material = i == 1 ? gladiolusWhite : gladiolusRed;
                for (int k = 0; k < 2; k++)
                {
                    var q = Deco(PrimitiveType.Quad, "Gladiolus", kid, new Vector3(3.05f + ox, 0.86f + h / 2f, 5.7f + oz), new Vector3(0.17f, h, 1f), material);
                    q.transform.rotation = Quaternion.Euler(0f, 30f + k * 90f + i * 25f, tilt);
                }
            }

            var petalWhite = Mat("Petal White", new Color(0.9f, 0.85f, 0.85f), plasterTex);

            // Мишка, будильник и букварь — в Chapter1Builder.Furniture.cs.
            var slipper = Mat("Slipper", new Color(0.3f, 0.35f, 0.5f), plasterTex);
            Deco(PrimitiveType.Cube, "Slipper", kid, new Vector3(1.05f, 0.02f, 4.25f), new Vector3(0.09f, 0.04f, 0.22f), slipper);
            Deco(PrimitiveType.Cube, "Slipper", kid, new Vector3(1.17f, 0.02f, 4.28f), new Vector3(0.09f, 0.04f, 0.22f), slipper);

            // ================= Кухня =================

            // Мойка у западной стены и шкафчик над ней.
            // Мойка — в Chapter1Builder.Furniture.cs.

            // Эмалированный чайник на плите.
            var enamel = Mat("Enamel", new Color(0.75f, 0.2f, 0.15f), plasterTex);
            Deco(PrimitiveType.Sphere, "Kettle", kitchen, new Vector3(6.56f, 0.95f, 3.2f), new Vector3(0.18f, 0.15f, 0.18f), enamel);
            Deco(PrimitiveType.Sphere, "Kettle Lid", kitchen, new Vector3(6.56f, 1.03f, 3.2f), new Vector3(0.07f, 0.03f, 0.07f), petalWhite);
            var spout = Deco(PrimitiveType.Cube, "Kettle Spout", kitchen, new Vector3(6.46f, 0.98f, 3.2f), new Vector3(0.1f, 0.025f, 0.025f), enamel);
            spout.transform.rotation = Quaternion.Euler(0f, 0f, -35f);
            Deco(PrimitiveType.Cube, "Kettle Handle", kitchen, new Vector3(6.56f, 1.07f, 3.2f), new Vector3(0.12f, 0.015f, 0.02f), black);

            // Пепельница отца, тарелка с хлебом, календарь.
            var ashtray = Primitive(PrimitiveType.Cylinder, "Ashtray", kitchen, new Vector3(5.95f, 0.772f, 4.55f), new Vector3(0.12f, 0.01f, 0.12f), glass);
            Interact(ashtray, "Пепельница", "Полная.", "Папа курил всю ночь.");
            var butt = Mat("Cigarette Butt", new Color(0.75f, 0.6f, 0.4f), plasterTex);
            for (int i = 0; i < 6; i++)
            {
                var b = Deco(PrimitiveType.Cube, "Butt", kitchen, new Vector3(5.92f + (i % 3) * 0.025f, 0.787f, 4.53f + (i / 3) * 0.03f), new Vector3(0.012f, 0.012f, 0.04f), butt);
                b.transform.rotation = Quaternion.Euler(0f, i * 47f, 0f);
            }
            Deco(PrimitiveType.Cylinder, "Plate", kitchen, new Vector3(5.38f, 0.768f, 4.85f), new Vector3(0.2f, 0.004f, 0.2f), petalWhite);
            Deco(PrimitiveType.Cube, "Bread", kitchen, new Vector3(5.38f, 0.785f, 4.85f), new Vector3(0.08f, 0.03f, 0.06f), Mat("Bread", new Color(0.6f, 0.42f, 0.25f), plasterTex));
            // Календарь на весь 2004 год: обведены 1 сентября и 11 ноября.
            Deco(PrimitiveType.Cube, "Calendar Back", kitchen, new Vector3(6.5f, 1.6f, 3.058f), new Vector3(0.44f, 0.62f, 0.006f), Mat("Calendar", new Color(0.88f, 0.86f, 0.8f), plasterTex));
            var calendar = Primitive(PrimitiveType.Quad, "Calendar", kitchen, new Vector3(6.5f, 1.6f, 3.063f), new Vector3(0.42f, 0.6f, 1f), Mat("Calendar 2004", Color.white, MakeColorTexture("Calendar 2004", 48, 72, Calendar2004Pixel)));
            calendar.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            Interact(calendar, "Календарь", "Календарь на 2004 год.", "Первое сентября мама обвела ручкой.");

            // ================= Коридор =================

            var shoeDark = Mat("Boots", new Color(0.1f, 0.08f, 0.07f), plasterTex);
            var shoeRed = Mat("Woman Shoes", new Color(0.45f, 0.12f, 0.1f), plasterTex);
            // Мамины туфли на обувнице, мои сандалии внизу, папины ботинки брошены посреди коридора.
            Deco(PrimitiveType.Cube, "Mother Shoe", hall, new Vector3(1.0f, 0.43f, 1.8f), new Vector3(0.08f, 0.06f, 0.22f), shoeRed);
            Deco(PrimitiveType.Cube, "Mother Shoe", hall, new Vector3(1.1f, 0.43f, 1.8f), new Vector3(0.08f, 0.06f, 0.22f), shoeRed);
            var sandal = Mat("Sandal", new Color(0.4f, 0.26f, 0.15f), plasterTex);
            Deco(PrimitiveType.Cube, "Sandal", hall, new Vector3(1.25f, 0.25f, 1.8f), new Vector3(0.07f, 0.04f, 0.17f), sandal);
            Deco(PrimitiveType.Cube, "Sandal", hall, new Vector3(1.34f, 0.25f, 1.8f), new Vector3(0.07f, 0.04f, 0.17f), sandal);
            var boot = Primitive(PrimitiveType.Cube, "Father Boot", hall, new Vector3(0.95f, 0.06f, 2.15f), new Vector3(0.11f, 0.12f, 0.3f), shoeDark);
            boot.transform.rotation = Quaternion.Euler(0f, 20f, 0f);
            Interact(boot, "Папины ботинки", "Грязные.", "Он пришёл поздно и даже не поставил их на место.");
            var boot2 = Deco(PrimitiveType.Cube, "Father Boot", hall, new Vector3(1.22f, 0.055f, 2.28f), new Vector3(0.11f, 0.12f, 0.3f), shoeDark);
            boot2.transform.rotation = Quaternion.Euler(0f, -35f, 90f);
            Deco(PrimitiveType.Cube, "Doormat", hall, new Vector3(0.35f, 0.006f, 2.3f), new Vector3(0.45f, 0.012f, 0.8f), Mat("Doormat", new Color(0.25f, 0.2f, 0.15f), plasterTex));
            Deco(PrimitiveType.Cube, "Coat Hooks", hall, new Vector3(0.9f, 1.85f, 1.665f), new Vector3(0.6f, 0.06f, 0.03f), wood);
            var keys = Deco(PrimitiveType.Cube, "Keys", hall, new Vector3(1.1f, 1.78f, 1.69f), new Vector3(0.04f, 0.07f, 0.01f), metal);
            keys.transform.rotation = Quaternion.Euler(0f, 0f, 12f);

            // ================= Зал =================

            // Фикус в углу.
            Primitive(PrimitiveType.Cylinder, "Plant Pot", living, new Vector3(10.65f, 0.15f, 5.45f), new Vector3(0.28f, 0.15f, 0.28f), Mat("Clay Pot", new Color(0.55f, 0.3f, 0.18f), plasterTex));
            var leaves = Mat("Leaves", new Color(0.15f, 0.32f, 0.12f), plasterTex);
            Deco(PrimitiveType.Sphere, "Leaves", living, new Vector3(10.65f, 0.65f, 5.45f), new Vector3(0.45f, 0.6f, 0.45f), leaves);
            Deco(PrimitiveType.Sphere, "Leaves", living, new Vector3(10.6f, 1.0f, 5.4f), new Vector3(0.32f, 0.35f, 0.32f), leaves);

            // Фотографии над диваном.
            var frameDark = Mat("Mirror Frame", new Color(0.35f, 0.22f, 0.12f), woodTex);
            var wedding = Box("Wedding Photo", living, new Vector3(8.6f, 1.55f, 1.665f), new Vector3(0.32f, 0.42f, 0.02f), frameDark);
var weddingImage = Deco(PrimitiveType.Quad, "Wedding Photo Image", living, new Vector3(8.6f, 1.55f, 1.678f), new Vector3(0.26f, 0.36f, 1f), Mat("Photo Wedding", Color.white, MakeColorTexture("Photo Wedding", 13, 18, WeddingPhotoPixel)));
            weddingImage.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            Interact(wedding, "Свадебная фотография", "Мама и папа.", "Это ещё до армии.");
var family = Box("Family Photo", living, new Vector3(9.3f, 1.6f, 1.665f), new Vector3(0.26f, 0.2f, 0.02f), frameDark);
            var familyImage = Deco(PrimitiveType.Quad, "Family Photo Image", living, new Vector3(9.3f, 1.6f, 1.678f), new Vector3(0.21f, 0.15f, 1f), Mat("Photo Family", Color.white, MakeColorTexture("Photo Family", 21, 15, FamilyPhotoPixel)));
            familyImage.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            Interact(family, "Фотография", "Мы на озере. Мне тут три года.", "Папа держит меня на плечах. Я этого не помню.");

            // Радиоприёмник на подоконнике.
            var radio = Box("Radio", living, new Vector3(9.75f, 1.03f, 5.85f), new Vector3(0.3f, 0.16f, 0.14f), Mat("Wood Dark", new Color(0.3f, 0.18f, 0.1f), woodTex));
            Deco(PrimitiveType.Cube, "Radio Grille", living, new Vector3(9.7f, 1.03f, 5.778f), new Vector3(0.15f, 0.11f, 0.005f), Mat("Radio Grille", new Color(0.55f, 0.5f, 0.4f), plasterTex));
            Deco(PrimitiveType.Cylinder, "Radio Knob", living, new Vector3(9.84f, 1.03f, 5.776f), new Vector3(0.035f, 0.006f, 0.035f), Mat("Knob Dark", new Color(0.2f, 0.15f, 0.1f), plasterTex)).transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            // Радио играет песню, которую автор положил в Assets/_ThirdParty/Music (папка вне git).
            // Фильтры делают звук «из старого динамика».
            var song = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_ThirdParty/Music/vechno_molodoy.mp3");
            var radioPlayer = radio.AddComponent<RadioPlayer>();
            if (song != null)
            {
                var audio = radio.AddComponent<AudioSource>();
                audio.clip = song;
                audio.loop = true;
                audio.playOnAwake = true;
                audio.volume = 0.55f;
                audio.spatialBlend = 1f;
                audio.rolloffMode = AudioRolloffMode.Linear;
                audio.minDistance = 1.5f;
                audio.maxDistance = 14f;
                radio.AddComponent<AudioHighPassFilter>().cutoffFrequency = 350f;
                radio.AddComponent<AudioLowPassFilter>().cutoffFrequency = 3200f;
                radio.AddComponent<AudioDistortionFilter>().distortionLevel = 0.25f;
                radioPlayer.source = audio;
            }
        }

        // Декоративная мелочь без коллайдера, чтобы герой не цеплялся за неё ногами.
        // Гладиолус 16x48: зелёный стебель с листьями внизу, выше — ярус цветков по обе стороны,
        // на самом верху нераскрытые бутоны. Фон прозрачный.
        static Color GladiolusPixel(int x, int y, float r, bool white)
        {
            var clear = new Color(0f, 0f, 0f, 0f);
            var green = Noise(new Color(0.2f, 0.38f, 0.14f), r, 0.15f);
            var petal = white ? new Color(0.92f, 0.88f, 0.86f) : new Color(0.8f, 0.12f, 0.16f);
            var petalDark = white ? new Color(0.75f, 0.65f, 0.7f) : new Color(0.5f, 0.05f, 0.1f);

            bool stem = x == 7 || x == 8;
            // Узкие листья-мечи у основания.
            if (y < 22 && (Mathf.Abs(x - 7.5f - (22 - y) * 0.12f) < 0.8f || Mathf.Abs(x - 7.5f + (22 - y) * 0.1f) < 0.8f)) return green;
            if (y < 18) return stem ? green : clear;

            // Бутоны сверху.
            if (y >= 42) return (x >= 6 && x <= 9 && y <= 46 - (Mathf.Abs(x - 7.5f) > 1 ? 1 : 0)) ? Noise(new Color(0.3f, 0.45f, 0.18f), r, 0.1f) : clear;

            // Цветки по очереди слева и справа от стебля.
            int tier = (y - 18) / 6;
            int ty = (y - 18) % 6;
            float cx = tier % 2 == 0 ? 5f : 10f;
            float radius = 3.6f - tier * 0.35f;
            float d = Mathf.Sqrt((x - cx) * (x - cx) + (ty - 3f) * (ty - 3f) * 1.6f);
            if (d < radius) return Noise(d < 1.3f ? petalDark : petal, r, 0.12f);
            return stem ? green : clear;
        }

        // Календарь-плакат на 2004 год 48x72: красная шапка с «2004», ниже 12 месяцев (3 x 4).
        // В каждом месяце точки-дни по неделям, воскресенья красные. Два дня обведены.
        static Color Calendar2004Pixel(int x, int y, float r)
        {
            var paper = new Color(0.92f, 0.9f, 0.85f);
            var ink = new Color(0.25f, 0.25f, 0.28f);
            var red = new Color(0.75f, 0.12f, 0.1f);
            var blue = new Color(0.15f, 0.25f, 0.75f);

            // Шапка с годом: цифры 3x5, увеличенные в 2 раза.
            if (y >= 61)
            {
                string[] digits = { "111001111100111", "111101101101111", "111101101101111", "101101111001001" }; // 2 0 0 4
                int gx = x - 9, gy = 70 - y;
                if (gx >= 0 && gx < 30 && gy >= 0 && gy < 10)
                {
                    int d = gx / 8, cx = (gx % 8) / 2, cy = gy / 2;
                    if (d < 4 && gx % 8 < 6 && digits[d][cy * 3 + cx] == '1') return paper;
                }
                return red;
            }

            int col = x / 16, row = (59 - y) / 15;
            if (row > 3 || y > 59) return paper;
            int month = row * 3 + col;
            int lx = x - col * 16, ly = (59 - y) - row * 15;
            if (ly == 0 && lx >= 2 && lx <= 13) return ink;

            var first = new System.DateTime(2004, month + 1, 1);
            int offset = ((int)first.DayOfWeek + 6) % 7;
            int days = System.DateTime.DaysInMonth(2004, month + 1);

            // Обведённый день: 1 сентября.
            foreach (var (m, day, color) in new[] { (8, 1, blue) })
            {
                if (m != month) continue;
                int cell = day - 1 + offset;
                int dx = 1 + (cell % 7) * 2, dy = 2 + (cell / 7) * 2;
                if (Mathf.Max(Mathf.Abs(lx - dx), Mathf.Abs(ly - dy)) == 1) return color;
            }

            if (ly >= 2 && lx >= 1 && (lx - 1) % 2 == 0 && (ly - 2) % 2 == 0)
            {
                int c = (lx - 1) / 2, rr = (ly - 2) / 2;
                int dayNumber = rr * 7 + c - offset + 1;
                if (c < 7 && rr < 6 && dayNumber >= 1 && dayNumber <= days) return c == 6 ? red : ink;
            }
            return Noise(paper, r, 0.02f);
        }

        static GameObject Deco(PrimitiveType type, string name, Transform parent, Vector3 position, Vector3 scale, Material material)
        {
            var go = Primitive(type, name, parent, position, scale, material);
            Object.DestroyImmediate(go.GetComponent<Collider>());
            return go;
        }

        // Наличник: две вертикальные планки и верхняя. side = 0 — с обеих сторон стены.
        static void DoorTrim(Transform parent, bool alongX, float line, float a, float b, int side, Material m)
        {
            const float width = 0.06f, depth = 0.02f;
            float height = DoorTop + width;
            foreach (int s in side == 0 ? new[] { 1, -1 } : new[] { side })
            {
                float plane = line + s * (WallThickness / 2f + depth / 2f + 0.004f);
                Vector3 P(float u, float y) => alongX ? new Vector3(u, y, plane) : new Vector3(plane, y, u);
                Vector3 S(float su, float sy) => alongX ? new Vector3(su, sy, depth) : new Vector3(depth, sy, su);
                Deco(PrimitiveType.Cube, "Door Trim", parent, P(a - width / 2f, height / 2f), S(width, height), m);
                Deco(PrimitiveType.Cube, "Door Trim", parent, P(b + width / 2f, height / 2f), S(width, height), m);
                Deco(PrimitiveType.Cube, "Door Trim", parent, P((a + b) / 2f, DoorTop + width / 2f), S(b - a + 2f * width, width), m);
            }
        }

        // Чугунная батарея-гармошка под окном на северной стене.
        static void Radiator(Transform parent, float x, Material m)
        {
            const float z = 5.86f;
            for (int i = 0; i < 8; i++)
                Deco(PrimitiveType.Cube, "Radiator Fin", parent, new Vector3(x - 0.315f + i * 0.09f, 0.55f, z), new Vector3(0.06f, 0.5f, 0.08f), m);
            Deco(PrimitiveType.Cube, "Radiator Pipe", parent, new Vector3(x, 0.33f, z), new Vector3(0.75f, 0.03f, 0.03f), m);
            Deco(PrimitiveType.Cube, "Radiator Pipe", parent, new Vector3(x, 0.77f, z), new Vector3(0.75f, 0.03f, 0.03f), m);
            Deco(PrimitiveType.Cube, "Radiator Riser", parent, new Vector3(x + 0.42f, 0.55f, z + 0.02f), new Vector3(0.03f, 1.1f, 0.03f), m);
        }

        // Выключатель или розетка на стене.
        static GameObject WallThing(Transform parent, bool alongX, float line, int side, float u, float y, Material m)
        {
            float plane = line + side * (WallThickness / 2f + 0.011f);
            var pos = alongX ? new Vector3(u, y, plane) : new Vector3(plane, y, u);
            var size = alongX ? new Vector3(0.07f, 0.09f, 0.014f) : new Vector3(0.014f, 0.09f, 0.07f);
            return Deco(PrimitiveType.Cube, "Wall Switch", parent, pos, size, m);
        }
    }
}
