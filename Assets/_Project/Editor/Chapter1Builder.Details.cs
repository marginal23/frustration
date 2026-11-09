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
            WallThing(kid, true, 3f, +1, 2.05f, 1.3f, plastic);
            WallThing(hall, true, 3f, -1, 0.85f, 1.3f, plastic);
            WallThing(kitchen, true, 3f, +1, 6.05f, 1.3f, plastic);
            WallThing(living, false, 7f, +1, 3.0f, 1.3f, plastic);
            WallThing(kid, false, 3.5f, -1, 5.3f, 0.3f, plastic);
            WallThing(kitchen, true, 6f, -1, 6.2f, 1.1f, plastic);
            WallThing(living, false, 11f, -1, 2.6f, 0.3f, plastic);

            // ================= Детская =================

            // Букет гладиолусов к первому сентября.
            var jar = Primitive(PrimitiveType.Cylinder, "Flower Jar", kid, new Vector3(3.05f, 0.83f, 5.7f), new Vector3(0.1f, 0.08f, 0.1f), glass);
            Interact(jar, "Цветы", "Гладиолусы для учительницы.", "Мама вчера купила их у бабушки возле магазина.");
            var stem = Mat("Stem", new Color(0.2f, 0.4f, 0.15f), plasterTex);
            var petalRed = Mat("Petal Red", new Color(0.75f, 0.12f, 0.15f), plasterTex);
            var petalWhite = Mat("Petal White", new Color(0.9f, 0.85f, 0.85f), plasterTex);
            for (int i = 0; i < 3; i++)
            {
                float ox = (i - 1) * 0.03f;
                var s = Deco(PrimitiveType.Cylinder, "Stem", kid, new Vector3(3.05f + ox, 1.1f, 5.7f + (i % 2) * 0.02f), new Vector3(0.012f, 0.22f, 0.012f), stem);
                s.transform.rotation = Quaternion.Euler(0f, 0f, (i - 1) * 8f);
                for (int k = 0; k < 4; k++)
                    Deco(PrimitiveType.Sphere, "Petal", kid, new Vector3(3.05f + ox * 2.2f, 1.18f + k * 0.06f, 5.7f + (i % 2) * 0.02f), Vector3.one * 0.05f, i == 1 ? petalWhite : petalRed);
            }

            // Мишка на кровати.
            var bearMat = Mat("Bear", new Color(0.5f, 0.33f, 0.18f), plasterTex);
            var bear = Primitive(PrimitiveType.Sphere, "Teddy Bear", kid, new Vector3(0.45f, 0.6f, 5.4f), new Vector3(0.18f, 0.2f, 0.15f), bearMat);
            Interact(bear, "Мишка", "Мишка остаётся дома.", "Я уже большой.");
            Deco(PrimitiveType.Sphere, "Bear Head", kid, new Vector3(0.45f, 0.77f, 5.4f), Vector3.one * 0.13f, bearMat);
            Deco(PrimitiveType.Sphere, "Bear Ear", kid, new Vector3(0.4f, 0.84f, 5.4f), Vector3.one * 0.05f, bearMat);
            Deco(PrimitiveType.Sphere, "Bear Ear", kid, new Vector3(0.5f, 0.84f, 5.4f), Vector3.one * 0.05f, bearMat);

            // Будильник, букварь, тапочки.
            var alarm = Primitive(PrimitiveType.Cylinder, "Alarm Clock", kid, new Vector3(2.75f, 0.81f, 5.75f), new Vector3(0.1f, 0.03f, 0.1f), Mat("Alarm Clock", new Color(0.7f, 0.15f, 0.1f), plasterTex));
            alarm.transform.rotation = Quaternion.Euler(80f, 0f, 0f);
            Interact(alarm, "Будильник", "Мама завела его на шесть тридцать.", "Он не прозвенел. Я проснулся сам.");
            var primer = Box("Primer Book", kid, new Vector3(3.2f, 0.762f, 5.45f), new Vector3(0.17f, 0.02f, 0.24f), Mat("Primer Book", new Color(0.2f, 0.45f, 0.65f), plasterTex));
            Interact(primer, "Букварь", "Я уже знаю все буквы.", "Почти все.");
            var slipper = Mat("Slipper", new Color(0.3f, 0.35f, 0.5f), plasterTex);
            Deco(PrimitiveType.Cube, "Slipper", kid, new Vector3(1.05f, 0.02f, 4.25f), new Vector3(0.09f, 0.04f, 0.22f), slipper);
            Deco(PrimitiveType.Cube, "Slipper", kid, new Vector3(1.17f, 0.02f, 4.28f), new Vector3(0.09f, 0.04f, 0.22f), slipper);

            // ================= Кухня =================

            // Мойка у западной стены и шкафчик над ней.
            Box("Sink Cabinet", kitchen, new Vector3(3.83f, 0.43f, 4.0f), new Vector3(0.55f, 0.86f, 0.9f), white);
            Deco(PrimitiveType.Cube, "Sink", kitchen, new Vector3(3.83f, 0.862f, 4.0f), new Vector3(0.4f, 0.01f, 0.45f), Mat("Sink Metal", new Color(0.45f, 0.46f, 0.47f), plasterTex));
            Deco(PrimitiveType.Cube, "Faucet", kitchen, new Vector3(3.62f, 0.95f, 4.0f), new Vector3(0.03f, 0.16f, 0.03f), metal);
            Deco(PrimitiveType.Cube, "Faucet Spout", kitchen, new Vector3(3.7f, 1.02f, 4.0f), new Vector3(0.16f, 0.025f, 0.025f), metal);
            Box("Wall Cabinet", kitchen, new Vector3(3.73f, 1.8f, 4.0f), new Vector3(0.36f, 0.55f, 0.9f), Mat("Cabinet Light", new Color(0.7f, 0.65f, 0.55f), woodTex));

            // Эмалированный чайник на плите.
            var enamel = Mat("Enamel", new Color(0.75f, 0.2f, 0.15f), plasterTex);
            Deco(PrimitiveType.Sphere, "Kettle", kitchen, new Vector3(6.55f, 0.95f, 3.5f), new Vector3(0.18f, 0.15f, 0.18f), enamel);
            Deco(PrimitiveType.Sphere, "Kettle Lid", kitchen, new Vector3(6.55f, 1.03f, 3.5f), new Vector3(0.07f, 0.03f, 0.07f), petalWhite);
            var spout = Deco(PrimitiveType.Cube, "Kettle Spout", kitchen, new Vector3(6.45f, 0.98f, 3.5f), new Vector3(0.1f, 0.025f, 0.025f), enamel);
            spout.transform.rotation = Quaternion.Euler(0f, 0f, -35f);
            Deco(PrimitiveType.Cube, "Kettle Handle", kitchen, new Vector3(6.55f, 1.07f, 3.5f), new Vector3(0.12f, 0.015f, 0.02f), black);

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
            var calendar = Box("Calendar", kitchen, new Vector3(6.5f, 1.6f, 3.065f), new Vector3(0.3f, 0.4f, 0.01f), Mat("Calendar", new Color(0.88f, 0.86f, 0.8f), plasterTex));
            Deco(PrimitiveType.Cube, "Calendar Top", kitchen, new Vector3(6.5f, 1.77f, 3.072f), new Vector3(0.3f, 0.06f, 0.006f), Mat("Calendar Red", new Color(0.65f, 0.12f, 0.1f), plasterTex));
            Interact(calendar, "Календарь", "Сентябрь 2004.", "Первое число обведено ручкой.");

            // ================= Коридор =================

            var shoeDark = Mat("Boots", new Color(0.1f, 0.08f, 0.07f), plasterTex);
            var shoeRed = Mat("Woman Shoes", new Color(0.45f, 0.12f, 0.1f), plasterTex);
            Deco(PrimitiveType.Cube, "Father Boot", hall, new Vector3(0.95f, 0.06f, 1.82f), new Vector3(0.11f, 0.12f, 0.3f), shoeDark);
            Deco(PrimitiveType.Cube, "Father Boot", hall, new Vector3(1.09f, 0.06f, 1.82f), new Vector3(0.11f, 0.12f, 0.3f), shoeDark);
            Deco(PrimitiveType.Cube, "Mother Shoe", hall, new Vector3(1.3f, 0.03f, 1.8f), new Vector3(0.08f, 0.06f, 0.22f), shoeRed);
            Deco(PrimitiveType.Cube, "Mother Shoe", hall, new Vector3(1.4f, 0.03f, 1.8f), new Vector3(0.08f, 0.06f, 0.22f), shoeRed);
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
            Deco(PrimitiveType.Cube, "Wedding Photo Image", living, new Vector3(8.6f, 1.55f, 1.677f), new Vector3(0.26f, 0.36f, 0.005f), Mat("Photo Sepia", new Color(0.6f, 0.52f, 0.4f), plasterTex));
            Interact(wedding, "Свадебная фотография", "Мама и папа.", "Это ещё до армии.");
            Box("Family Photo", living, new Vector3(9.3f, 1.6f, 1.665f), new Vector3(0.26f, 0.2f, 0.02f), frameDark);
            Deco(PrimitiveType.Cube, "Family Photo Image", living, new Vector3(9.3f, 1.6f, 1.677f), new Vector3(0.21f, 0.15f, 0.005f), Mat("Photo Faded", new Color(0.5f, 0.5f, 0.48f), plasterTex));

            // Радиоприёмник на подоконнике.
            var radio = Box("Radio", living, new Vector3(9.75f, 1.03f, 5.85f), new Vector3(0.3f, 0.16f, 0.14f), Mat("Wood Dark", new Color(0.3f, 0.18f, 0.1f), woodTex));
            Deco(PrimitiveType.Cube, "Radio Grille", living, new Vector3(9.7f, 1.03f, 5.778f), new Vector3(0.15f, 0.11f, 0.005f), Mat("Radio Grille", new Color(0.55f, 0.5f, 0.4f), plasterTex));
            Interact(radio, "Радио", "Папа слушает по нему новости.", "Потом выключает и долго сидит молча.");
        }

        // Декоративная мелочь без коллайдера, чтобы герой не цеплялся за неё ногами.
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
        static void WallThing(Transform parent, bool alongX, float line, int side, float u, float y, Material m)
        {
            float plane = line + side * (WallThickness / 2f + 0.011f);
            var pos = alongX ? new Vector3(u, y, plane) : new Vector3(plane, y, u);
            var size = alongX ? new Vector3(0.07f, 0.09f, 0.014f) : new Vector3(0.014f, 0.09f, 0.07f);
            Deco(PrimitiveType.Cube, "Wall Switch", parent, pos, size, m);
        }
    }
}
