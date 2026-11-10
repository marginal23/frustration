using UnityEngine;

namespace Frustration.EditorTools
{
    // Шестая часть сборщика: мебель по референсам (фото советских квартир, которые прислал автор).
    // Каждый предмет собирается в своей группе: локальная ось +Z — лицевая сторона предмета,
    // yaw поворачивает его в комнате. Детали без коллайдеров, а у группы один-два простых
    // BoxCollider, чтобы герой не цеплялся за мелочи.
    public static partial class Chapter1Builder
    {
        static Texture2D grainTex, doorPanelTex;

        static void BuildFurniture()
        {
            grainTex = MakeTexture("Grain", 32, (x, y, r) => 0.8f + 0.1f * Mathf.Sin(x * 0.9f + Mathf.Sin(y * 0.2f) * 1.5f) + 0.06f * r);
            doorPanelTex = MakeTexture("Door Panel", 32, (x, y, r) =>
                (x < 2 || y < 2 || x > 29 || y > 29) ? 0.5f : 0.82f + 0.08f * Mathf.Sin(x * 0.9f + Mathf.Sin(y * 0.2f) * 1.5f) + 0.05f * r);

            var kid = GameObject.Find("Kid Room").transform;
            var kitchen = GameObject.Find("Kitchen").transform;
            var hall = GameObject.Find("Hallway").transform;
            var living = GameObject.Find("Living Room").transform;

            // ================= Детская =================
            var desk = WritingDesk(kid, new Vector3(2.93f, 0f, 5.6f), 180f);
            Interact(desk.gameObject, "Письменный стол", "Это был папин стол. Теперь мой.", "В нижнем ящике лежат его письма. Мне туда нельзя.");
            WoodChair(kid, new Vector3(2.8f, 0f, 5.05f), 0f);
            var wardrobe = Wardrobe(kid, new Vector3(3.17f, 0f, 3.48f), -90f);
            Interact(wardrobe.gameObject, "Шкаф", "В шкафу пахнет нафталином.", "Мама повесила туда мой новый костюм. Теперь он на мне.");
            var lamp = DeskLamp(kid, new Vector3(2.58f, 0.75f, 5.78f), 120f);
            Interact(lamp.gameObject, "Настольная лампа", "Мама включила её, чтобы я не одевался в темноте.");
            var bear = TeddyBear(kid, new Vector3(0.45f, 0.5f, 5.35f), 90f);
            Interact(bear.gameObject, "Мишка", "Мишка остаётся дома.", "Я уже большой.");
            var alarm = AlarmClock(kid, new Vector3(3.28f, 0.75f, 5.8f), -100f);
            Interact(alarm.gameObject, "Будильник", "Мама завела его на шесть тридцать.", "Он не прозвенел. Я проснулся сам.");
            var primer = Primer(kid, new Vector3(3.15f, 0.75f, 5.42f), 12f);
            Interact(primer.gameObject, "Букварь", "Я уже знаю все буквы.", "Почти все.");
            KidRoomDoor(kid);
            var bed = KidBed(kid, new Vector3(0.5f, 0f, 4.95f));
            Interact(bed.gameObject, "Кровать", "Ещё темно. А мама сказала, что сегодня праздник.");
            KidCeilingLamp(kid);
            var ball = Primitive(PrimitiveType.Sphere, "Basketball", kid, new Vector3(0.45f, 0.12f, 3.45f), Vector3.one * 0.24f,
                Mat("Basketball", Color.white, MakeColorTexture("Basketball", 16, 8, (x, y, r) => (x % 4 == 0 || y == 4) ? new Color(0.1f, 0.06f, 0.04f) : Noise(new Color(0.75f, 0.35f, 0.1f), r, 0.08f))));
            ball.transform.rotation = Quaternion.Euler(20f, 35f, 0f);
            Interact(ball, "Мяч", "Папа обещал повесить кольцо во дворе.");

            // ================= Кухня =================
            OvalTable(kitchen, new Vector3(5.8f, 0f, 4.8f));
            MetalChair(kitchen, new Vector3(5.0f, 0f, 4.8f), 90f);
            MetalChair(kitchen, new Vector3(6.6f, 0f, 4.8f), -90f);
            var fridge = Fridge(kitchen, new Vector3(6.65f, 0f, 5.62f), -90f);
            Interact(fridge.gameObject, "Холодильник", "Холодильник «Юрюзань».", "Внутри банка огурцов и холодная бутылка.");
            AddSound(fridge.gameObject, ProceduralAmbience.Kind.FridgeHum, 0.2f, true);
            var stove = ElectricStove(kitchen, new Vector3(6.68f, 0f, 3.32f), -90f);
            Interact(stove.gameObject, "Плита", "Чайник холодный. Никто не завтракал.");
            SinkUnit(kitchen, new Vector3(3.83f, 0f, 4.0f), 90f);
            var cupboard = PaintedCupboard(kitchen, new Vector3(6.7f, 0f, 3.99f), -90f);
            Interact(cupboard.gameObject, "Шкафчик", "Здесь мама держит крупу и спички.");
            KitchenTv(kitchen, new Vector3(3.78f, 0f, 5.15f), 90f);
            WallCabinet(kitchen, new Vector3(3.71f, 0f, 4.0f), 90f);
            var father = Father(kitchen, new Vector3(6.6f, 0f, 4.8f), -90f);
            Interact(father.gameObject, "Папа", "Папа спит за столом.", "Лучше его не будить.");

            // ================= Зал =================
            WallUnit(living);
            var sofa = SofaBook(living, new Vector3(9.0f, 0f, 2.08f), 0f);
            Interact(sofa.gameObject, "Диван", "Папа иногда спит здесь, когда они с мамой ругаются.");
            var armchair = Armchair(living, new Vector3(7.75f, 0f, 4.3f), 90f);
            Interact(armchair.gameObject, "Кресло", "Папино кресло.", "Он сидит в нём и смотрит в окно, даже когда там ничего нет.");
            LivingRoomGlassDoor(living);
            CoffeeTable(living, new Vector3(9.0f, 0f, 3.3f));
            LivingChandelier(living);

            // ================= Коридор =================
            ShoeShelf(hall, new Vector3(1.15f, 0f, 1.81f), 0f);

            // ================= Выключатели =================
            var bulbOn = Mat("Bulb", Color.black, plasterTex, 1f, new Color(1f, 0.8f, 0.5f));
            var bulbOff = Mat("Bulb Off", new Color(0.75f, 0.73f, 0.65f), plasterTex);
            LightSwitchOn(kitchen, true, 3f, +1, 6.05f, new[] { kitchen.Find("Kitchen Lamp").GetComponent<Light>() },
                new[] { kitchen.Find("Lamp Glow Kitchen").GetComponent<Renderer>() }, new[] { bulbOn }, new[] { bulbOff });
            LightSwitchOn(hall, true, 3f, -1, 0.85f, new[] { hall.Find("Hall Bulb").GetComponent<Light>() },
                new[] { hall.Find("Lamp Glow Hall").GetComponent<Renderer>() },
                new[] { Mat("Plafond Lit", new Color(0.9f, 0.88f, 0.8f), plasterTex, 1f, new Color(0.45f, 0.4f, 0.3f)) },
                new[] { Mat("Plafond Off", new Color(0.7f, 0.69f, 0.64f), plasterTex) });
        }

        // Выключатель на стене с настоящим действием: включает и выключает перечисленные лампы.
        static LightSwitch LightSwitchOn(Transform parent, bool alongX, float line, int side, float u,
                                         Light[] lights, Renderer[] glows, Material[] on, Material[] off, bool isOn = true)
        {
            var go = WallThing(parent, alongX, line, side, u, 1.3f, Mat("Switch", new Color(0.85f, 0.83f, 0.76f), plasterTex));
            go.name = "Light Switch";
            go.AddComponent<BoxCollider>().size = new Vector3(1.6f, 1.6f, 1.6f);
            var sw = go.AddComponent<LightSwitch>();
            sw.lights = lights;
            sw.glows = glows;
            sw.onMaterials = on;
            sw.offMaterials = off;
            sw.isOn = isOn;
            foreach (var l in lights) l.enabled = isOn;
            for (int i = 0; i < glows.Length; i++) glows[i].sharedMaterial = isOn ? on[i] : off[i];
            return sw;
        }

        // Усечённый конус без крышек (абажур, плафон-тюльпан). Низ в точке position.
        static GameObject Frustum(string name, Transform parent, Vector3 position, float rBottom, float rTop, float height, Material m, int segments = 8)
        {
            var vertices = new System.Collections.Generic.List<Vector3>();
            var normals = new System.Collections.Generic.List<Vector3>();
            var uvs = new System.Collections.Generic.List<Vector2>();
            var triangles = new System.Collections.Generic.List<int>();
            float slope = (rBottom - rTop) / height;
            for (int i = 0; i <= segments; i++)
            {
                float a = i * Mathf.PI * 2f / segments;
                var dir = new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a));
                var n = (dir + Vector3.up * slope).normalized;
                vertices.Add(dir * rBottom); normals.Add(n); uvs.Add(new Vector2((float)i / segments, 0f));
                vertices.Add(dir * rTop + Vector3.up * height); normals.Add(n); uvs.Add(new Vector2((float)i / segments, 1f));
            }
            for (int i = 0; i < segments; i++)
            {
                int b0 = i * 2, t0 = b0 + 1, b1 = b0 + 2, t1 = b0 + 3;
                triangles.AddRange(new[] { b0, t0, t1, b0, t1, b1 });
            }
            var mesh = new Mesh { name = name };
            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateBounds();
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = position;
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            var renderer = go.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = m;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            return go;
        }

        static Material DoubleSided(Material m)
        {
            m.SetFloat("_Cull", 0f);
            return m;
        }

        // Абажур на шнуре под потолком детской (включён) и выключатель у двери.
        static void KidCeilingLamp(Transform room)
        {
            var center = new Vector3(1.75f, 0f, 4.5f);
            Box("Lamp Cord", room, center + new Vector3(0f, 2.45f, 0f), new Vector3(0.012f, 0.3f, 0.012f), Black);
            var shadeOn = DoubleSided(Mat("Lampshade Lit", new Color(0.85f, 0.72f, 0.48f), plasterTex, 1f, new Color(0.35f, 0.25f, 0.1f)));
            var shadeOff = DoubleSided(Mat("Lampshade Off", new Color(0.7f, 0.6f, 0.42f), plasterTex));
            var bulbOn = Mat("Bulb", Color.black, plasterTex, 1f, new Color(1f, 0.8f, 0.5f));
            var bulbOff = Mat("Bulb Off", new Color(0.75f, 0.73f, 0.65f), plasterTex);
            var shade = Frustum("Lamp Glow Kid Shade", room, center + new Vector3(0f, 2.1f, 0f), 0.22f, 0.06f, 0.2f, shadeOn);
            var bulb = Deco(PrimitiveType.Sphere, "Lamp Glow Kid Bulb", room, center + new Vector3(0f, 2.16f, 0f), Vector3.one * 0.07f, bulbOn);
            var light = PointLight("Kid Ceiling Lamp", room, center + new Vector3(0f, 2.0f, 0f), new Color(1f, 0.8f, 0.55f), 0.9f, 4.5f);
            light.gameObject.AddComponent<LightFlicker>().amount = 0.04f;
            LightSwitchOn(room, true, 3f, +1, 2.05f, new[] { light },
                new[] { shade.GetComponent<Renderer>(), bulb.GetComponent<Renderer>() },
                new[] { shadeOn, bulbOn }, new[] { shadeOff, bulbOff });
        }

        // Люстра в зале: латунный центр и пять плафонов-тюльпанов. Выключена, пока не нажать выключатель.
        static void LivingChandelier(Transform room)
        {
            var center = new Vector3(9.0f, 0f, 3.6f);
            var brass = Mat("Brass", new Color(0.7f, 0.55f, 0.25f), plasterTex, 1f, new Color(0.04f, 0.03f, 0.01f));
            var tulipOn = DoubleSided(Mat("Tulip Lit", new Color(0.92f, 0.86f, 0.75f), plasterTex, 1f, new Color(0.5f, 0.42f, 0.28f)));
            var tulipOff = DoubleSided(Mat("Tulip Off", new Color(0.78f, 0.76f, 0.7f), plasterTex));
            Deco(PrimitiveType.Cylinder, "Chandelier Rod", room, center + new Vector3(0f, 2.47f, 0f), new Vector3(0.02f, 0.13f, 0.02f), brass);
            Deco(PrimitiveType.Sphere, "Chandelier Body", room, center + new Vector3(0f, 2.32f, 0f), new Vector3(0.12f, 0.1f, 0.12f), brass);
            var glows = new Renderer[5];
            var on = new Material[5];
            var off = new Material[5];
            for (int i = 0; i < 5; i++)
            {
                float a = i * Mathf.PI * 2f / 5f;
                var dir = new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a));
                var arm = Deco(PrimitiveType.Cube, "Chandelier Arm", room, center + new Vector3(0f, 2.29f, 0f) + dir * 0.12f, new Vector3(0.015f, 0.015f, 0.24f), brass);
                arm.transform.rotation = Quaternion.LookRotation(dir);
                glows[i] = Frustum("Lamp Glow Living Tulip", room, center + new Vector3(0f, 2.26f, 0f) + dir * 0.24f, 0.03f, 0.075f, 0.11f, tulipOff).GetComponent<Renderer>();
                on[i] = tulipOn;
                off[i] = tulipOff;
            }
            var light = PointLight("Chandelier Light", room, center + new Vector3(0f, 2.15f, 0f), new Color(1f, 0.82f, 0.55f), 1.3f, 5.5f);
            LightSwitchOn(room, false, 7f, +1, 3.0f, new[] { light }, glows, on, off, false);
        }

        // Советская деревянная кровать: полированные спинки, клетчатое одеяло, подушка уголком.
        static Transform KidBed(Transform parent, Vector3 position)
        {
            var g = Group("Bed", parent, position, 0f);
            var veneer = Mat("Veneer", new Color(0.55f, 0.22f, 0.12f), grainTex);
            var sheet = Mat("Sheet", new Color(0.85f, 0.84f, 0.8f), plasterTex);
            var plaid = Mat("Plaid", Color.white, MakeColorTexture("Plaid", 16, 16, PlaidPixel), 3f);
            var pillow = Mat("Pillow", new Color(0.88f, 0.87f, 0.83f), plasterTex);
            Part(g, "Headboard", new Vector3(0f, 0.45f, 0.93f), new Vector3(0.92f, 0.9f, 0.04f), veneer);
            Part(g, "Headboard Rail", new Vector3(0f, 0.91f, 0.93f), new Vector3(0.96f, 0.04f, 0.06f), veneer);
            Part(g, "Footboard", new Vector3(0f, 0.32f, -0.93f), new Vector3(0.92f, 0.64f, 0.04f), veneer);
            Part(g, "Footboard Rail", new Vector3(0f, 0.65f, -0.93f), new Vector3(0.96f, 0.04f, 0.06f), veneer);
            Part(g, "Side Rail", new Vector3(-0.44f, 0.3f, 0f), new Vector3(0.03f, 0.16f, 1.82f), veneer);
            Part(g, "Side Rail", new Vector3(0.44f, 0.3f, 0f), new Vector3(0.03f, 0.16f, 1.82f), veneer);
            Part(g, "Mattress", new Vector3(0f, 0.43f, 0f), new Vector3(0.84f, 0.14f, 1.82f), sheet);
            Part(g, "Blanket", new Vector3(0f, 0.515f, -0.3f), new Vector3(0.88f, 0.03f, 1.25f), plaid);
            Part(g, "Blanket Side", new Vector3(-0.445f, 0.43f, -0.3f), new Vector3(0.01f, 0.2f, 1.25f), plaid);
            Part(g, "Blanket Side", new Vector3(0.445f, 0.43f, -0.3f), new Vector3(0.01f, 0.2f, 1.25f), plaid);
            Part(g, "Pillow", new Vector3(0f, 0.56f, 0.66f), new Vector3(0.62f, 0.15f, 0.42f), pillow, PrimitiveType.Sphere);
            Part(g, "Pillow Dent", new Vector3(0.02f, 0.6f, 0.64f), new Vector3(0.3f, 0.06f, 0.22f), Mat("Pillow Shade", new Color(0.78f, 0.77f, 0.73f), plasterTex), PrimitiveType.Sphere);
            Solid(g, new Vector3(0f, 0.45f, 0f), new Vector3(0.92f, 0.9f, 1.9f));
            return g;
        }

        // Отец спит за столом: майка, треники, тапки. Голова лежит на скрещённых руках. Лицом к +Z.
        static Transform Father(Transform parent, Vector3 position, float yaw)
        {
            var g = Group("Father", parent, position, yaw);
            var shirt = Mat("Undershirt", new Color(0.78f, 0.76f, 0.7f), plasterTex);
            var pants = Mat("Track Pants", new Color(0.12f, 0.16f, 0.35f), plasterTex);
            var skin = Mat("Skin", new Color(0.6f, 0.45f, 0.38f), plasterTex);
            var hair = Mat("Hair Father", new Color(0.15f, 0.12f, 0.1f), plasterTex);
            var slipper = Mat("Slipper Father", new Color(0.3f, 0.2f, 0.12f), plasterTex);
            Part(g, "Pelvis", new Vector3(0f, 0.58f, -0.02f), new Vector3(0.38f, 0.2f, 0.32f), pants, PrimitiveType.Sphere);
            foreach (int s in new[] { -1, 1 })
            {
                Part(g, "Thigh", new Vector3(s * 0.1f, 0.56f, 0.19f), new Vector3(0.15f, 0.23f, 0.15f), pants, PrimitiveType.Capsule, rx: 90f);
                Part(g, "Knee", new Vector3(s * 0.1f, 0.54f, 0.4f), Vector3.one * 0.13f, pants, PrimitiveType.Sphere);
                Part(g, "Shin", new Vector3(s * 0.1f, 0.28f, 0.42f), new Vector3(0.12f, 0.24f, 0.12f), pants, PrimitiveType.Capsule);
                Part(g, "Slipper", new Vector3(s * 0.1f, 0.03f, 0.49f), new Vector3(0.11f, 0.07f, 0.25f), slipper, PrimitiveType.Sphere);
                Part(g, "Shoulder", new Vector3(s * 0.17f, 1.0f, 0.24f), Vector3.one * 0.12f, skin, PrimitiveType.Sphere);
                Part(g, "Upper Arm", new Vector3(s * 0.21f, 0.9f, 0.34f), new Vector3(0.095f, 0.16f, 0.095f), skin, PrimitiveType.Capsule, rx: 50f, rz: s * -8f);
                Part(g, "Elbow", new Vector3(s * 0.23f, 0.81f, 0.43f), Vector3.one * 0.09f, skin, PrimitiveType.Sphere);
            }
            Part(g, "Torso", new Vector3(0f, 0.84f, 0.1f), new Vector3(0.42f, 0.3f, 0.26f), shirt, PrimitiveType.Capsule, rx: 35f);
            Part(g, "Belly", new Vector3(0f, 0.72f, 0.12f), new Vector3(0.36f, 0.2f, 0.26f), shirt, PrimitiveType.Sphere);
            Part(g, "Forearm", new Vector3(0f, 0.8f, 0.45f), new Vector3(0.085f, 0.25f, 0.085f), skin, PrimitiveType.Capsule, ry: 12f, rz: 90f);
            Part(g, "Forearm", new Vector3(0f, 0.825f, 0.5f), new Vector3(0.085f, 0.25f, 0.085f), skin, PrimitiveType.Capsule, ry: -12f, rz: 90f);
            Part(g, "Hand", new Vector3(0.22f, 0.81f, 0.53f), new Vector3(0.08f, 0.05f, 0.1f), skin, PrimitiveType.Sphere);
            Part(g, "Hand", new Vector3(-0.22f, 0.8f, 0.47f), new Vector3(0.08f, 0.05f, 0.1f), skin, PrimitiveType.Sphere);
            Part(g, "Neck", new Vector3(0f, 1.0f, 0.32f), new Vector3(0.1f, 0.06f, 0.1f), skin, PrimitiveType.Cylinder, rx: 60f);
            Part(g, "Head", new Vector3(0.04f, 0.93f, 0.47f), new Vector3(0.2f, 0.22f, 0.23f), skin, PrimitiveType.Sphere, rz: 60f);
            Part(g, "Hair", new Vector3(0.02f, 0.96f, 0.45f), new Vector3(0.21f, 0.17f, 0.21f), hair, PrimitiveType.Sphere, rz: 60f);
            Part(g, "Ear", new Vector3(0.1f, 0.99f, 0.48f), new Vector3(0.03f, 0.05f, 0.04f), skin, PrimitiveType.Sphere, rz: 60f);
            Solid(g, new Vector3(0f, 0.6f, 0.15f), new Vector3(0.5f, 1.2f, 0.75f));
            return g;
        }

        // Журнальный столик: полированная столешница, полка снизу, ножки чуть врозь. На нём газета.
        static void CoffeeTable(Transform room, Vector3 position)
        {
            var g = Group("Coffee Table", room, position, 0f);
            var wood = Mat("Stenka", new Color(0.6f, 0.38f, 0.2f), grainTex);
            var legs = Mat("Wood Dark", new Color(0.3f, 0.18f, 0.1f), woodTex);
            Part(g, "Top", new Vector3(0f, 0.485f, 0f), new Vector3(1.0f, 0.03f, 0.6f), wood);
            Part(g, "Apron", new Vector3(0f, 0.44f, 0f), new Vector3(0.92f, 0.06f, 0.52f), wood);
            Part(g, "Shelf", new Vector3(0f, 0.12f, 0f), new Vector3(0.9f, 0.02f, 0.5f), wood);
            foreach (var (x, z) in new[] { (-0.42f, -0.22f), (0.42f, -0.22f), (-0.42f, 0.22f), (0.42f, 0.22f) })
                Part(g, "Leg", new Vector3(x, 0.235f, z), new Vector3(0.035f, 0.235f, 0.035f), legs,
                     PrimitiveType.Cylinder, Mathf.Sign(z) * 6f, 0f, -Mathf.Sign(x) * 6f);
            Part(g, "Magazines", new Vector3(-0.2f, 0.14f, 0f), new Vector3(0.25f, 0.02f, 0.32f), Mat("Magazine", new Color(0.1f, 0.25f, 0.45f), plasterTex));
            Solid(g, new Vector3(0f, 0.25f, 0f), new Vector3(1.0f, 0.5f, 0.6f));
            var paper = Box("Newspaper", room, position + new Vector3(0.15f, 0.505f, -0.05f), new Vector3(0.3f, 0.01f, 0.4f), Mat("Newspaper", new Color(0.78f, 0.76f, 0.7f), plasterTex));
            Interact(paper, "Газета", "Вчерашняя газета.", "На первой странице — Чечня. Папа обвёл что-то ручкой.");
        }

        // Навесной шкафчик над мойкой, белый, с двумя дверцами.
        static Transform WallCabinet(Transform parent, Vector3 position, float yaw)
        {
            var g = Group("Wall Cabinet", parent, position, yaw);
            var paint = Mat("Cupboard Paint", new Color(0.86f, 0.85f, 0.8f), plasterTex);
            var doors = Mat("Old White Paint", new Color(0.84f, 0.83f, 0.77f), plasterTex);
            Part(g, "Body", new Vector3(0f, 1.82f, 0f), new Vector3(0.9f, 0.55f, 0.32f), paint);
            Part(g, "Cornice", new Vector3(0f, 2.11f, 0.01f), new Vector3(0.94f, 0.03f, 0.35f), paint);
            foreach (int s in new[] { -1, 1 })
            {
                Part(g, "Door", new Vector3(s * 0.222f, 1.82f, 0.166f), new Vector3(0.43f, 0.51f, 0.012f), doors);
                Part(g, "Knob", new Vector3(s * 0.04f, 1.65f, 0.178f), Vector3.one * 0.025f, Metal, PrimitiveType.Sphere);
            }
            Solid(g, new Vector3(0f, 1.82f, 0f), new Vector3(0.9f, 0.55f, 0.32f));
            return g;
        }

        // Клетчатое шерстяное одеяло.
        static Color PlaidPixel(int x, int y, float r)
        {
            bool a = (x / 4) % 2 == 0, b = (y / 4) % 2 == 0;
            Color c = a && b ? new Color(0.45f, 0.12f, 0.1f) : (a || b ? new Color(0.6f, 0.35f, 0.22f) : new Color(0.75f, 0.65f, 0.45f));
            if (x % 8 == 0 || y % 8 == 0) c = new Color(0.15f, 0.12f, 0.1f);
            return Noise(c, r, 0.05f);
        }

        // ---------- Помощники ----------

        static Transform Group(string name, Transform parent, Vector3 position, float yaw)
        {
            var g = new GameObject(name).transform;
            g.SetParent(parent, false);
            g.SetPositionAndRotation(position, Quaternion.Euler(0f, yaw, 0f));
            return g;
        }

        // Деталь предмета без коллайдера.
        static GameObject Part(Transform g, string name, Vector3 localPosition, Vector3 size, Material m,
                               PrimitiveType type = PrimitiveType.Cube, float rx = 0f, float ry = 0f, float rz = 0f)
        {
            var go = LocalPrimitive(type, name, g, localPosition, size, m);
            go.transform.localRotation = Quaternion.Euler(rx, ry, rz);
            Object.DestroyImmediate(go.GetComponent<Collider>());
            return go;
        }

        static void Solid(Transform g, Vector3 center, Vector3 size)
        {
            var c = g.gameObject.AddComponent<BoxCollider>();
            c.center = center;
            c.size = size;
        }

        static Material Metal => Mat("Metal", new Color(0.7f, 0.7f, 0.65f), plasterTex);
        static Material Black => Mat("Strap", new Color(0.08f, 0.08f, 0.08f), plasterTex);
        static Material Chrome => Mat("Chrome", new Color(0.78f, 0.78f, 0.76f), plasterTex, 1f, new Color(0.05f, 0.05f, 0.05f));

        // ================= Детская =================

        // Письменный стол: столешница, боковина-щит слева, тумба с тремя ящиками справа на чёрных ножках.
        static Transform WritingDesk(Transform parent, Vector3 position, float yaw)
        {
            var g = Group("Desk", parent, position, yaw);
            var wood = Mat("Desk Wood", new Color(0.42f, 0.27f, 0.16f), grainTex);
            var front = Mat("Desk Drawer", new Color(0.5f, 0.33f, 0.2f), grainTex);
            Part(g, "Top", new Vector3(0f, 0.735f, 0f), new Vector3(1.0f, 0.03f, 0.6f), wood);
            Part(g, "Side", new Vector3(0.47f, 0.36f, 0f), new Vector3(0.03f, 0.72f, 0.56f), wood);
            Part(g, "Back Panel", new Vector3(0f, 0.45f, -0.26f), new Vector3(0.92f, 0.45f, 0.02f), wood);
            Part(g, "Pedestal", new Vector3(-0.29f, 0.51f, 0f), new Vector3(0.38f, 0.42f, 0.55f), wood);
            foreach (float y in new[] { 0.655f, 0.515f, 0.375f })
            {
                Part(g, "Drawer", new Vector3(-0.29f, y, 0.281f), new Vector3(0.36f, 0.125f, 0.012f), front);
                Part(g, "Knob", new Vector3(-0.29f, y, 0.292f), Vector3.one * 0.022f, Metal, PrimitiveType.Sphere);
            }
            Part(g, "Leg", new Vector3(-0.45f, 0.15f, 0.22f), new Vector3(0.03f, 0.15f, 0.03f), Black, PrimitiveType.Cylinder);
            Part(g, "Leg", new Vector3(-0.45f, 0.15f, -0.22f), new Vector3(0.03f, 0.15f, 0.03f), Black, PrimitiveType.Cylinder);
            Solid(g, new Vector3(0f, 0.375f, 0f), new Vector3(1.0f, 0.75f, 0.6f));
            return g;
        }

        // Простой деревянный стул.
        static Transform WoodChair(Transform parent, Vector3 position, float yaw)
        {
            var g = Group("Chair", parent, position, yaw);
            var wood = Mat("Chair Wood", new Color(0.45f, 0.3f, 0.18f), grainTex);
            Part(g, "Seat", new Vector3(0f, 0.45f, 0f), new Vector3(0.4f, 0.035f, 0.4f), wood);
            foreach (var (x, z) in new[] { (-0.17f, -0.17f), (0.17f, -0.17f), (-0.17f, 0.17f), (0.17f, 0.17f) })
                Part(g, "Leg", new Vector3(x, 0.215f, z), new Vector3(0.035f, 0.43f, 0.035f), wood);
            Part(g, "Back Post", new Vector3(-0.17f, 0.7f, -0.17f), new Vector3(0.035f, 0.5f, 0.035f), wood);
            Part(g, "Back Post", new Vector3(0.17f, 0.7f, -0.17f), new Vector3(0.035f, 0.5f, 0.035f), wood);
            Part(g, "Back Slat", new Vector3(0f, 0.88f, -0.17f), new Vector3(0.37f, 0.09f, 0.02f), wood);
            Part(g, "Back Slat", new Vector3(0f, 0.7f, -0.17f), new Vector3(0.37f, 0.05f, 0.02f), wood);
            Solid(g, new Vector3(0f, 0.45f, 0f), new Vector3(0.42f, 0.9f, 0.42f));
            return g;
        }

        // Полированный шкаф из шпона с золотыми полосками-ручками.
        static Transform Wardrobe(Transform parent, Vector3 position, float yaw)
        {
            var g = Group("Wardrobe", parent, position, yaw);
            var veneer = Mat("Veneer", new Color(0.55f, 0.22f, 0.12f), grainTex);
            var gold = Mat("Gold Strip", new Color(0.8f, 0.62f, 0.25f), plasterTex, 1f, new Color(0.06f, 0.045f, 0.01f));
            Part(g, "Body", new Vector3(0f, 1.0f, 0f), new Vector3(0.85f, 1.9f, 0.55f), veneer);
            Part(g, "Cornice", new Vector3(0f, 1.97f, 0.01f), new Vector3(0.87f, 0.04f, 0.57f), veneer);
            Part(g, "Plinth", new Vector3(0f, 0.04f, 0f), new Vector3(0.83f, 0.08f, 0.52f), Mat("Wood Dark", new Color(0.3f, 0.18f, 0.1f), woodTex));
            foreach (int s in new[] { -1, 1 })
            {
                Part(g, "Door", new Vector3(s * 0.212f, 1.0f, 0.281f), new Vector3(0.415f, 1.84f, 0.012f), veneer);
                Part(g, "Gold Strip", new Vector3(s * 0.022f, 1.0f, 0.29f), new Vector3(0.01f, 1.7f, 0.006f), gold);
                Part(g, "Knob", new Vector3(s * 0.045f, 1.05f, 0.295f), Vector3.one * 0.025f, gold, PrimitiveType.Sphere);
            }
            Solid(g, new Vector3(0f, 1.0f, 0f), new Vector3(0.87f, 2.0f, 0.57f));
            return g;
        }

        // Советская настольная лампа на шарнирной ножке. Светит вперёд (+Z).
        static Transform DeskLamp(Transform parent, Vector3 position, float yaw)
        {
            var g = Group("Desk Lamp", parent, position, yaw);
            var green = Mat("Lamp Green", new Color(0.12f, 0.25f, 0.18f), plasterTex);
            Part(g, "Base", new Vector3(0f, 0.012f, 0f), new Vector3(0.13f, 0.012f, 0.13f), green, PrimitiveType.Cylinder);
            Part(g, "Arm Low", new Vector3(0f, 0.17f, -0.03f), new Vector3(0.015f, 0.16f, 0.015f), Metal, PrimitiveType.Cylinder, -10f);
            Part(g, "Arm High", new Vector3(0f, 0.34f, 0.04f), new Vector3(0.015f, 0.1f, 0.015f), Metal, PrimitiveType.Cylinder, 75f);
            Part(g, "Shade", new Vector3(0f, 0.32f, 0.16f), new Vector3(0.12f, 0.06f, 0.12f), green, PrimitiveType.Cylinder, 35f);
            Part(g, "Bulb", new Vector3(0f, 0.29f, 0.18f), Vector3.one * 0.045f, Mat("Bulb", Color.black, plasterTex, 1f, new Color(1f, 0.8f, 0.5f)), PrimitiveType.Sphere);
            var light = PointLight("Desk Lamp Light", g, g.TransformPoint(new Vector3(0f, 0.22f, 0.24f)), new Color(1f, 0.82f, 0.55f), 0.9f, 2.5f);
            light.gameObject.AddComponent<LightFlicker>().amount = 0.03f;
            Solid(g, new Vector3(0f, 0.2f, 0.05f), new Vector3(0.14f, 0.4f, 0.3f));
            return g;
        }

        // Старый плюшевый мишка, тёмный, с голубым бантом. Сидит лицом к +Z.
        static Transform TeddyBear(Transform parent, Vector3 position, float yaw)
        {
            var g = Group("Teddy Bear", parent, position, yaw);
            var fur = Mat("Bear Old", new Color(0.16f, 0.11f, 0.08f), plasterTex);
            var bow = Mat("Bear Ribbon", new Color(0.55f, 0.7f, 0.85f), plasterTex);
            Part(g, "Body", new Vector3(0f, 0.11f, 0f), new Vector3(0.17f, 0.2f, 0.14f), fur, PrimitiveType.Sphere);
            Part(g, "Head", new Vector3(0f, 0.27f, 0.01f), Vector3.one * 0.14f, fur, PrimitiveType.Sphere);
            Part(g, "Ear", new Vector3(-0.055f, 0.33f, 0f), new Vector3(0.05f, 0.05f, 0.025f), fur, PrimitiveType.Sphere);
            Part(g, "Ear", new Vector3(0.055f, 0.33f, 0f), new Vector3(0.05f, 0.05f, 0.025f), fur, PrimitiveType.Sphere);
            Part(g, "Muzzle", new Vector3(0f, 0.255f, 0.065f), new Vector3(0.06f, 0.045f, 0.05f), fur, PrimitiveType.Sphere);
            Part(g, "Nose", new Vector3(0f, 0.265f, 0.09f), Vector3.one * 0.018f, Black, PrimitiveType.Sphere);
            Part(g, "Eye", new Vector3(-0.03f, 0.29f, 0.07f), Vector3.one * 0.014f, Black, PrimitiveType.Sphere);
            Part(g, "Eye", new Vector3(0.03f, 0.29f, 0.07f), Vector3.one * 0.014f, Black, PrimitiveType.Sphere);
            Part(g, "Arm", new Vector3(-0.09f, 0.14f, 0.02f), new Vector3(0.06f, 0.13f, 0.06f), fur, PrimitiveType.Sphere);
            Part(g, "Arm", new Vector3(0.09f, 0.14f, 0.02f), new Vector3(0.06f, 0.13f, 0.06f), fur, PrimitiveType.Sphere);
            Part(g, "Leg", new Vector3(-0.05f, 0.03f, 0.07f), new Vector3(0.07f, 0.06f, 0.11f), fur, PrimitiveType.Sphere);
            Part(g, "Leg", new Vector3(0.05f, 0.03f, 0.07f), new Vector3(0.07f, 0.06f, 0.11f), fur, PrimitiveType.Sphere);
            Part(g, "Ribbon", new Vector3(0f, 0.2f, 0f), new Vector3(0.13f, 0.02f, 0.115f), bow, PrimitiveType.Cylinder);
            Part(g, "Bow", new Vector3(-0.02f, 0.2f, 0.065f), new Vector3(0.035f, 0.025f, 0.01f), bow, rz: 20f);
            Part(g, "Bow", new Vector3(0.02f, 0.2f, 0.065f), new Vector3(0.035f, 0.025f, 0.01f), bow, rz: -20f);
            Solid(g, new Vector3(0f, 0.17f, 0f), new Vector3(0.2f, 0.36f, 0.2f));
            return g;
        }

        // Будильник «Слава»: красный корпус, белый циферблат, два звонка сверху. Циферблат смотрит в +Z.
        static Transform AlarmClock(Transform parent, Vector3 position, float yaw)
        {
            var g = Group("Alarm Clock", parent, position, yaw);
            var red = Mat("Alarm Clock", new Color(0.7f, 0.15f, 0.1f), plasterTex);
            Part(g, "Body", new Vector3(0f, 0.065f, 0f), new Vector3(0.09f, 0.025f, 0.09f), red, PrimitiveType.Cylinder, 90f);
            Part(g, "Face", new Vector3(0f, 0.065f, 0.026f), new Vector3(0.075f, 0.002f, 0.075f), Mat("Clock Face", new Color(0.85f, 0.83f, 0.75f), plasterTex), PrimitiveType.Cylinder, 90f);
            Part(g, "Hand", new Vector3(0f, 0.08f, 0.029f), new Vector3(0.004f, 0.03f, 0.002f), Black);
            Part(g, "Hand", new Vector3(0.01f, 0.065f, 0.029f), new Vector3(0.022f, 0.004f, 0.002f), Black);
            Part(g, "Bell", new Vector3(-0.03f, 0.112f, 0f), Vector3.one * 0.038f, Metal, PrimitiveType.Sphere);
            Part(g, "Bell", new Vector3(0.03f, 0.112f, 0f), Vector3.one * 0.038f, Metal, PrimitiveType.Sphere);
            Part(g, "Hammer", new Vector3(0f, 0.115f, -0.01f), new Vector3(0.006f, 0.02f, 0.006f), Metal);
            Part(g, "Foot", new Vector3(-0.028f, 0.01f, 0f), new Vector3(0.012f, 0.01f, 0.012f), Metal, PrimitiveType.Cylinder);
            Part(g, "Foot", new Vector3(0.028f, 0.01f, 0f), new Vector3(0.012f, 0.01f, 0.012f), Metal, PrimitiveType.Cylinder);
            Solid(g, new Vector3(0f, 0.07f, 0f), new Vector3(0.1f, 0.14f, 0.06f));
            return g;
        }

        // Букварь: страницы и обложка с картинкой.
        static Transform Primer(Transform parent, Vector3 position, float yaw)
        {
            var g = Group("Primer Book", parent, position, yaw);
            Part(g, "Pages", new Vector3(0f, 0.011f, 0f), new Vector3(0.165f, 0.018f, 0.235f), Mat("Pages", new Color(0.88f, 0.86f, 0.78f), plasterTex));
            var cover = Mat("Primer Cover", Color.white, MakeColorTexture("Primer Cover", 16, 24, PrimerCoverPixel));
            var top = Part(g, "Cover", new Vector3(0f, 0.0215f, 0f), new Vector3(0.17f, 0.24f, 1f), cover, PrimitiveType.Quad, 90f);
            top.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            Part(g, "Back Cover", new Vector3(0f, 0.001f, 0f), new Vector3(0.17f, 0.002f, 0.24f), Mat("Primer Back", new Color(0.7f, 0.2f, 0.15f), plasterTex));
            Solid(g, new Vector3(0f, 0.012f, 0f), new Vector3(0.17f, 0.03f, 0.24f));
            return g;
        }

        // Белая крашеная дверь в детскую, приоткрыта внутрь комнаты.
        static void KidRoomDoor(Transform room)
        {
            var root = new GameObject("Kid Room Door").transform;
            root.SetParent(room, false);
            var pivot = new GameObject("Door Pivot Kid").transform;
            pivot.SetParent(root, false);
            pivot.position = new Vector3(1.0f, 0f, 3.0f);

            var paint = Mat("Door Paint", new Color(0.8f, 0.78f, 0.7f), plasterTex);
            var inset = Mat("Door Paint Inset", new Color(0.72f, 0.7f, 0.62f), plasterTex);
            LocalBox("Leaf", pivot, new Vector3(0.45f, 1.02f, 0f), new Vector3(0.88f, 2.03f, 0.04f), paint);
            foreach (int s in new[] { -1, 1 })
            {
                Part(pivot, "Inset", new Vector3(0.45f, 1.45f, s * 0.022f), new Vector3(0.66f, 0.75f, 0.006f), inset);
                Part(pivot, "Inset", new Vector3(0.45f, 0.5f, s * 0.022f), new Vector3(0.66f, 0.6f, 0.006f), inset);
                Part(pivot, "Handle", new Vector3(0.78f, 1.0f, s * 0.04f), new Vector3(0.1f, 0.022f, 0.022f), Metal);
            }

            var door = root.gameObject.AddComponent<SwingDoor>();
            door.leaves = new[] { pivot };
            door.closedAngles = new[] { 0f };
            door.openAngles = new[] { -80f };
            door.isOpen = true;
            pivot.localRotation = Quaternion.Euler(0f, -80f, 0f);
        }

        // ================= Кухня =================

        // Овальный стол «под мрамор» на белых металлических ножках.
        static Transform OvalTable(Transform parent, Vector3 position)
        {
            var g = Group("Table", parent, position, 0f);
            var marble = Mat("Marble", Color.white, MakeColorTexture("Marble", 32, 32, MarblePixel));
            var white = Mat("White Metal", new Color(0.85f, 0.85f, 0.82f), plasterTex);
            Part(g, "Top", new Vector3(0f, 0.735f, 0f), new Vector3(1.1f, 0.015f, 0.75f), marble, PrimitiveType.Cylinder);
            Part(g, "Apron", new Vector3(0f, 0.7f, 0f), new Vector3(0.8f, 0.05f, 0.45f), white);
            foreach (var (x, z) in new[] { (-0.38f, -0.2f), (0.38f, -0.2f), (-0.38f, 0.2f), (0.38f, 0.2f) })
                Part(g, "Leg", new Vector3(x, 0.35f, z), new Vector3(0.04f, 0.35f, 0.04f), white, PrimitiveType.Cylinder);
            Solid(g, new Vector3(0f, 0.375f, 0f), new Vector3(1.05f, 0.75f, 0.7f));
            return g;
        }

        // Стул на белом металлическом каркасе с серо-голубой обивкой и высокой спинкой.
        static Transform MetalChair(Transform parent, Vector3 position, float yaw)
        {
            var g = Group("Chair", parent, position, yaw);
            var white = Mat("White Metal", new Color(0.85f, 0.85f, 0.82f), plasterTex);
            var cloth = Mat("Chair Upholstery", new Color(0.38f, 0.43f, 0.5f), plasterTex);
            foreach (var (x, z) in new[] { (-0.18f, -0.18f), (0.18f, -0.18f), (-0.18f, 0.18f), (0.18f, 0.18f) })
                Part(g, "Leg", new Vector3(x, 0.225f, z), new Vector3(0.025f, 0.225f, 0.025f), white, PrimitiveType.Cylinder);
            Part(g, "Seat", new Vector3(0f, 0.47f, 0.01f), new Vector3(0.42f, 0.06f, 0.42f), cloth);
            Part(g, "Back Tube", new Vector3(-0.18f, 0.75f, -0.2f), new Vector3(0.025f, 0.28f, 0.025f), white, PrimitiveType.Cylinder);
            Part(g, "Back Tube", new Vector3(0.18f, 0.75f, -0.2f), new Vector3(0.025f, 0.28f, 0.025f), white, PrimitiveType.Cylinder);
            Part(g, "Top Tube", new Vector3(0f, 1.03f, -0.2f), new Vector3(0.025f, 0.18f, 0.025f), white, PrimitiveType.Cylinder, rz: 90f);
            Part(g, "Back", new Vector3(0f, 0.78f, -0.2f), new Vector3(0.3f, 0.44f, 0.03f), cloth);
            Solid(g, new Vector3(0f, 0.5f, 0f), new Vector3(0.44f, 1.0f, 0.44f));
            return g;
        }

        // Холодильник со скруглённым верхом, хромированной ручкой-рычагом и табличкой.
        static Transform Fridge(Transform parent, Vector3 position, float yaw)
        {
            var g = Group("Fridge", parent, position, yaw);
            var enamel = Mat("Fridge Enamel", new Color(0.82f, 0.81f, 0.76f), plasterTex);
            Part(g, "Body", new Vector3(0f, 0.66f, 0f), new Vector3(0.56f, 1.24f, 0.55f), enamel);
            Part(g, "Round Top", new Vector3(0f, 1.28f, 0f), new Vector3(0.16f, 0.28f, 0.55f), enamel, PrimitiveType.Cylinder, rz: 90f);
            Part(g, "Plinth", new Vector3(0f, 0.03f, 0f), new Vector3(0.52f, 0.06f, 0.5f), Black);
            Part(g, "Door Seam", new Vector3(0f, 1.17f, 0.276f), new Vector3(0.56f, 0.006f, 0.004f), Black);
            Part(g, "Chrome Strip", new Vector3(0f, 1.2f, 0.278f), new Vector3(0.5f, 0.012f, 0.006f), Chrome);
            Part(g, "Name Plate", new Vector3(0f, 1.24f, 0.279f), new Vector3(0.2f, 0.04f, 0.008f), Chrome);
            Part(g, "Name Plate Red", new Vector3(0f, 1.24f, 0.284f), new Vector3(0.06f, 0.022f, 0.004f), Mat("Calendar Red", new Color(0.65f, 0.12f, 0.1f), plasterTex));
            Part(g, "Handle Base", new Vector3(-0.21f, 0.95f, 0.29f), new Vector3(0.04f, 0.08f, 0.03f), Chrome);
            Part(g, "Handle Lever", new Vector3(-0.15f, 0.935f, 0.302f), new Vector3(0.12f, 0.025f, 0.02f), Chrome);
            Solid(g, new Vector3(0f, 0.68f, 0f), new Vector3(0.58f, 1.36f, 0.57f));
            return g;
        }

        // Старая электроплита: две маленькие и одна большая конфорка с нагаром, ручки, духовка.
        static Transform ElectricStove(Transform parent, Vector3 position, float yaw)
        {
            var g = Group("Stove", parent, position, yaw);
            var enamel = Mat("Stove Enamel", new Color(0.85f, 0.84f, 0.8f), plasterTex);
            var burner = Mat("Burner", new Color(0.15f, 0.15f, 0.15f), plasterTex);
            var burnt = Mat("Burnt", new Color(0.32f, 0.22f, 0.16f), plasterTex);
            Part(g, "Body", new Vector3(0f, 0.43f, 0f), new Vector3(0.5f, 0.84f, 0.55f), enamel);
            Part(g, "Back Splash", new Vector3(0f, 0.92f, -0.265f), new Vector3(0.5f, 0.12f, 0.02f), enamel);
            foreach (var (x, z, d) in new[] { (-0.12f, 0.12f, 0.15f), (-0.12f, -0.12f, 0.15f), (0.12f, -0.02f, 0.2f) })
            {
                Part(g, "Burnt Ring", new Vector3(x, 0.851f, z), new Vector3(d + 0.06f, 0.001f, d + 0.06f), burnt, PrimitiveType.Cylinder);
                Part(g, "Burner", new Vector3(x, 0.857f, z), new Vector3(d, 0.006f, d), burner, PrimitiveType.Cylinder);
            }
            for (int i = 0; i < 4; i++)
                Part(g, "Knob", new Vector3(-0.18f + i * 0.12f, 0.79f, 0.285f), new Vector3(0.04f, 0.012f, 0.04f), Black, PrimitiveType.Cylinder, 90f);
            Part(g, "Oven Door", new Vector3(0f, 0.48f, 0.278f), new Vector3(0.44f, 0.4f, 0.01f), Mat("Stove Door", new Color(0.8f, 0.79f, 0.75f), plasterTex));
            Part(g, "Oven Window", new Vector3(0f, 0.5f, 0.284f), new Vector3(0.28f, 0.16f, 0.004f), Mat("Oven Glass", new Color(0.06f, 0.06f, 0.07f), plasterTex));
            Part(g, "Oven Handle", new Vector3(0f, 0.66f, 0.3f), new Vector3(0.3f, 0.025f, 0.025f), Black);
            Part(g, "Drawer", new Vector3(0f, 0.15f, 0.278f), new Vector3(0.44f, 0.14f, 0.008f), Mat("Stove Door", new Color(0.8f, 0.79f, 0.75f), plasterTex));
            Part(g, "Drawer Handle", new Vector3(0f, 0.17f, 0.29f), new Vector3(0.2f, 0.02f, 0.02f), Black);
            Solid(g, new Vector3(0f, 0.43f, 0f), new Vector3(0.5f, 0.86f, 0.55f));
            return g;
        }

        // Мойка из нержавейки (круглая чаша и рифлёное крыло) на белой тумбе, кран-гусак.
        static Transform SinkUnit(Transform parent, Vector3 position, float yaw)
        {
            var g = Group("Sink", parent, position, yaw);
            var paint = Mat("Cupboard Paint", new Color(0.86f, 0.85f, 0.8f), plasterTex);
            var steel = Mat("Stainless", new Color(0.62f, 0.63f, 0.64f), plasterTex, 1f, new Color(0.04f, 0.04f, 0.04f));
            var bowl = Mat("Sink Bowl", new Color(0.32f, 0.33f, 0.34f), plasterTex);
            Part(g, "Cabinet", new Vector3(0f, 0.43f, 0f), new Vector3(0.9f, 0.82f, 0.55f), paint);
            foreach (int s in new[] { -1, 1 })
            {
                Part(g, "Door", new Vector3(s * 0.222f, 0.44f, 0.281f), new Vector3(0.43f, 0.7f, 0.012f), paint);
                Part(g, "Knob", new Vector3(s * 0.04f, 0.6f, 0.292f), Vector3.one * 0.025f, Metal, PrimitiveType.Sphere);
            }
            Part(g, "Steel Top", new Vector3(0f, 0.855f, 0f), new Vector3(0.92f, 0.03f, 0.57f), steel);
            Part(g, "Bowl", new Vector3(-0.2f, 0.871f, 0.02f), new Vector3(0.38f, 0.002f, 0.38f), bowl, PrimitiveType.Cylinder);
            Part(g, "Drain", new Vector3(-0.2f, 0.873f, 0.02f), new Vector3(0.05f, 0.001f, 0.05f), Black, PrimitiveType.Cylinder);
            for (int i = 0; i < 6; i++)
                Part(g, "Rib", new Vector3(0.08f + i * 0.06f, 0.872f, 0f), new Vector3(0.02f, 0.004f, 0.4f), bowl);
            Part(g, "Faucet Base", new Vector3(-0.2f, 0.9f, -0.24f), new Vector3(0.04f, 0.03f, 0.04f), steel, PrimitiveType.Cylinder);
            Part(g, "Faucet Pipe", new Vector3(-0.2f, 1.04f, -0.24f), new Vector3(0.025f, 0.14f, 0.025f), steel, PrimitiveType.Cylinder);
            Part(g, "Faucet Spout", new Vector3(-0.2f, 1.18f, -0.14f), new Vector3(0.025f, 0.1f, 0.025f), steel, PrimitiveType.Cylinder, 90f);
            Part(g, "Faucet Tip", new Vector3(-0.2f, 1.15f, -0.04f), new Vector3(0.025f, 0.03f, 0.025f), steel, PrimitiveType.Cylinder);
            Part(g, "Faucet Lever", new Vector3(-0.2f, 1.0f, -0.21f), new Vector3(0.015f, 0.015f, 0.06f), steel);
            Solid(g, new Vector3(0f, 0.43f, 0f), new Vector3(0.92f, 0.87f, 0.57f));
            return g;
        }

        // Старая крашеная белая тумба: ящик сверху, две дверцы.
        static Transform PaintedCupboard(Transform parent, Vector3 position, float yaw)
        {
            var g = Group("Cupboard", parent, position, yaw);
            var paint = Mat("Old White Paint", new Color(0.84f, 0.83f, 0.77f), plasterTex);
            Part(g, "Body", new Vector3(0f, 0.46f, 0f), new Vector3(0.72f, 0.78f, 0.46f), paint);
            Part(g, "Top", new Vector3(0f, 0.86f, 0.01f), new Vector3(0.78f, 0.03f, 0.5f), paint);
            Part(g, "Skirt", new Vector3(0f, 0.05f, 0.225f), new Vector3(0.72f, 0.1f, 0.02f), paint);
            Part(g, "Drawer", new Vector3(0f, 0.74f, 0.236f), new Vector3(0.4f, 0.1f, 0.015f), paint);
            Part(g, "Drawer Knob", new Vector3(0f, 0.74f, 0.25f), Vector3.one * 0.025f, Metal, PrimitiveType.Sphere);
            foreach (int s in new[] { -1, 1 })
                Part(g, "Door", new Vector3(s * 0.175f, 0.4f, 0.236f), new Vector3(0.34f, 0.5f, 0.012f), paint);
            Part(g, "Door Knob", new Vector3(0.04f, 0.42f, 0.25f), Vector3.one * 0.025f, Metal, PrimitiveType.Sphere);
            Part(g, "Latch", new Vector3(0f, 0.66f, 0.246f), new Vector3(0.02f, 0.03f, 0.006f), Metal);
            Solid(g, new Vector3(0f, 0.44f, 0f), new Vector3(0.78f, 0.88f, 0.5f));
            return g;
        }

        // Тумба на тонких чёрных ножках из светлого полированного шпона (под телевизор).
        static Transform LowCabinet(Transform parent, Vector3 position, float yaw)
        {
            var g = Group("TV Stand", parent, position, yaw);
            var veneer = Mat("Light Veneer", new Color(0.68f, 0.45f, 0.25f), grainTex);
            var door = Mat("Light Veneer Door", new Color(0.62f, 0.4f, 0.22f), grainTex);
            Part(g, "Body", new Vector3(0f, 0.32f, 0f), new Vector3(0.8f, 0.4f, 0.45f), veneer);
            foreach (int s in new[] { -1, 1 })
            {
                Part(g, "Door", new Vector3(s * 0.2f, 0.32f, 0.228f), new Vector3(0.39f, 0.36f, 0.01f), door);
                Part(g, "Handle", new Vector3(s * 0.05f, 0.38f, 0.24f), new Vector3(0.02f, 0.04f, 0.02f), Metal);
            }
            foreach (var (x, z) in new[] { (-0.36f, -0.18f), (0.36f, -0.18f), (-0.36f, 0.18f), (0.36f, 0.18f) })
                Part(g, "Leg", new Vector3(x, 0.06f, z), new Vector3(0.025f, 0.06f, 0.025f), Black, PrimitiveType.Cylinder);
            Solid(g, new Vector3(0f, 0.26f, 0f), new Vector3(0.8f, 0.52f, 0.45f));
            return g;
        }

        // Ламповый телевизор на тумбе, сверху салфетка и красная ваза. Шипит, экран мигает.
        // Экран смотрит в локальную +Z: на кухне — на стол, где сидит отец.
        static void KitchenTv(Transform room, Vector3 position, float yaw)
        {
            LowCabinet(room, position, yaw);
            var g = Group("TV Set", room, position, yaw);
            var tv = LocalBox("TV", g, new Vector3(0f, 0.74f, 0.02f), new Vector3(0.6f, 0.44f, 0.42f), Mat("TV Body", new Color(0.12f, 0.12f, 0.12f), plasterTex));
            Part(g, "TV Screen", new Vector3(0.06f, 0.75f, 0.235f), new Vector3(0.4f, 0.32f, 1f), Mat("TV Screen", Color.black, plasterTex, 1f, new Color(0.35f, 0.4f, 0.5f)), PrimitiveType.Quad, ry: 180f);
            Part(g, "TV Panel", new Vector3(-0.22f, 0.74f, 0.234f), new Vector3(0.11f, 0.38f, 0.006f), Mat("TV Panel", new Color(0.22f, 0.22f, 0.23f), plasterTex));
            for (int i = 0; i < 6; i++)
                Part(g, "TV Button", new Vector3(-0.22f, 0.86f - i * 0.035f, 0.239f), new Vector3(0.06f, 0.014f, 0.006f), Mat("TV Button", new Color(0.6f, 0.6f, 0.58f), plasterTex));
            Part(g, "TV Power", new Vector3(-0.25f, 0.92f, 0.239f), new Vector3(0.012f, 0.012f, 0.004f), Mat("LED Red", Color.black, plasterTex, 1f, new Color(0.9f, 0.1f, 0.05f)));
            Part(g, "TV Logo", new Vector3(0.2f, 0.56f, 0.236f), new Vector3(0.08f, 0.012f, 0.004f), Chrome);
            Part(g, "Doily", new Vector3(0.03f, 0.962f, 0.02f), new Vector3(0.28f, 0.002f, 0.28f), Mat("Doily", new Color(0.9f, 0.9f, 0.86f), plasterTex), PrimitiveType.Cylinder);
            var redGlass = Mat("Red Glass", new Color(0.75f, 0.08f, 0.06f), plasterTex, 1f, new Color(0.15f, 0.01f, 0.01f));
            Part(g, "Vase Foot", new Vector3(0.1f, 0.97f, 0.02f), new Vector3(0.07f, 0.008f, 0.07f), redGlass, PrimitiveType.Cylinder);
            Part(g, "Vase", new Vector3(0.1f, 1.07f, 0.02f), new Vector3(0.045f, 0.1f, 0.045f), redGlass, PrimitiveType.Cylinder);
            Part(g, "Vase Lip", new Vector3(0.1f, 1.17f, 0.02f), new Vector3(0.075f, 0.01f, 0.075f), redGlass, PrimitiveType.Cylinder);

            Interact(tv, "Телевизор", "Каналов нет. Только шипит.");
            AddSound(tv, ProceduralAmbience.Kind.TvStatic, 0.06f, true);
            var tvLight = PointLight("TV Light", room, g.TransformPoint(new Vector3(0f, 0.9f, 0.6f)), new Color(0.45f, 0.55f, 0.9f), 0.9f, 3f);
            var flicker = tvLight.gameObject.AddComponent<LightFlicker>();
            flicker.speed = 14f;
            flicker.amount = 0.5f;
        }

        // ================= Зал =================

        // Стенка: четыре секции. Крайние закрыты дверцами, в средних — открытые витрины с хрусталём и книгами.
        static void WallUnit(Transform room)
        {
            var g = Group("Wall Unit", room, Vector3.zero, 0f);
            var body = Mat("Stenka", new Color(0.6f, 0.38f, 0.2f), grainTex);
            var door = Mat("Stenka Door", new Color(0.62f, 0.4f, 0.21f), doorPanelTex);
            var inner = Mat("Stenka Inside", new Color(0.5f, 0.32f, 0.17f), grainTex);
            var knob = Mat("Knob Dark", new Color(0.2f, 0.15f, 0.1f), plasterTex);
            var crystal = Mat("Crystal", new Color(0.75f, 0.8f, 0.85f), plasterTex, 1f, new Color(0.1f, 0.1f, 0.12f));
            const float x0 = 7.07f, w = 0.52f, zFront = 5.5f, zMid = 5.725f;
            float xMid = x0 + 2f * w;

            Part(g, "Back", new Vector3(xMid, 1.15f, 5.94f), new Vector3(4f * w, 2.3f, 0.02f), inner);
            Part(g, "Top", new Vector3(xMid, 2.29f, zMid), new Vector3(4f * w, 0.02f, 0.45f), body);
            Part(g, "Plinth", new Vector3(xMid, 0.04f, 5.72f), new Vector3(4f * w - 0.02f, 0.08f, 0.42f), Mat("Wood Dark", new Color(0.3f, 0.18f, 0.1f), woodTex));
            for (int k = 0; k <= 4; k++)
                Part(g, "Divider", new Vector3(x0 + k * w, 1.15f, zMid), new Vector3(0.02f, 2.3f, 0.45f), body);

            for (int k = 0; k < 4; k++)
            {
                float cx = x0 + k * w + w / 2f;
                Part(g, "Top Door", new Vector3(cx, 2.085f, zFront - 0.01f), new Vector3(w - 0.02f, 0.39f, 0.02f), door);
                Part(g, "Knob", new Vector3(cx, 1.93f, zFront - 0.025f), Vector3.one * 0.025f, knob, PrimitiveType.Sphere);
                bool closed = k == 0 || k == 3;
                if (closed)
                {
                    Part(g, "Tall Door", new Vector3(cx, 0.98f, zFront - 0.01f), new Vector3(w - 0.02f, 1.79f, 0.02f), door);
                    Part(g, "Knob", new Vector3(cx + (k == 0 ? 0.2f : -0.2f), 1.0f, zFront - 0.025f), Vector3.one * 0.025f, knob, PrimitiveType.Sphere);
                    continue;
                }
                Part(g, "Bottom Door", new Vector3(cx, 0.43f, zFront - 0.01f), new Vector3(w - 0.02f, 0.69f, 0.02f), door);
                Part(g, "Knob", new Vector3(cx, 0.7f, zFront - 0.025f), Vector3.one * 0.025f, knob, PrimitiveType.Sphere);
                foreach (float y in new[] { 0.79f, 1.33f, 1.88f })
                    Part(g, "Shelf", new Vector3(cx, y, 5.72f), new Vector3(w - 0.02f, 0.02f, 0.44f), body);

                if (k == 1)
                {
                    // Нижняя полка витрины: хрустальные рюмки и бокалы в два ряда.
                    var crystalTwo = DoubleSided(Mat("Crystal Cut", new Color(0.78f, 0.82f, 0.88f), plasterTex, 1f, new Color(0.14f, 0.14f, 0.16f)));
                    for (int i = 0; i < 4; i++)
                    {
                        var basePos = new Vector3(cx - 0.17f + i * 0.11f, 0.8f, 5.82f);
                        Frustum("Crystal Glass", g, basePos, 0.022f, 0.035f, 0.11f, crystalTwo, 6);
                        Frustum("Crystal Shot", g, basePos + new Vector3(0.05f, 0f, -0.1f), 0.015f, 0.024f, 0.06f, crystalTwo, 6);
                    }
                    // Верхняя полка: чайный сервиз — чайник, чашки на блюдцах, сахарница.
                    var china = Mat("China", new Color(0.92f, 0.9f, 0.86f), plasterTex);
                    var gilt = Mat("Gold Strip", new Color(0.8f, 0.62f, 0.25f), plasterTex, 1f, new Color(0.06f, 0.045f, 0.01f));
                    Part(g, "Teapot", new Vector3(cx - 0.12f, 1.41f, 5.78f), new Vector3(0.13f, 0.11f, 0.13f), china, PrimitiveType.Sphere);
                    Part(g, "Teapot Lid", new Vector3(cx - 0.12f, 1.47f, 5.78f), new Vector3(0.05f, 0.02f, 0.05f), gilt, PrimitiveType.Sphere);
                    Part(g, "Teapot Spout", new Vector3(cx - 0.05f, 1.42f, 5.78f), new Vector3(0.07f, 0.018f, 0.018f), china, rz: 35f);
                    for (int i = 0; i < 3; i++)
                    {
                        var p = new Vector3(cx + 0.03f + i * 0.08f, 1.34f, 5.7f + (i % 2) * 0.1f);
                        Part(g, "Saucer", p + new Vector3(0f, 0.005f, 0f), new Vector3(0.08f, 0.004f, 0.08f), china, PrimitiveType.Cylinder);
                        Part(g, "Cup", p + new Vector3(0f, 0.035f, 0f), new Vector3(0.05f, 0.025f, 0.05f), china, PrimitiveType.Cylinder);
                        Part(g, "Cup Rim", p + new Vector3(0f, 0.061f, 0f), new Vector3(0.052f, 0.002f, 0.052f), gilt, PrimitiveType.Cylinder);
                    }
                    Part(g, "Plate Standing", new Vector3(cx + 0.05f, 1.46f, 5.9f), new Vector3(0.22f, 0.008f, 0.22f), china, PrimitiveType.Cylinder, 80f);
                }
                else
                {
                    Color[] books = { new Color(0.45f, 0.1f, 0.1f), new Color(0.12f, 0.2f, 0.4f), new Color(0.2f, 0.35f, 0.2f), new Color(0.5f, 0.42f, 0.2f) };
                    for (int i = 0; i < 7; i++)
                    {
                        float h = 0.2f + 0.04f * Mathf.PingPong(i * 0.53f, 1f);
                        Part(g, "Book", new Vector3(cx - 0.2f + i * 0.055f, 0.8f + h / 2f, 5.8f), new Vector3(0.045f, h, 0.16f), Mat("Stenka Book " + (i % books.Length), books[i % books.Length], plasterTex));
                    }
                    Part(g, "Plate", new Vector3(cx - 0.08f, 1.47f, 5.88f), new Vector3(0.22f, 0.01f, 0.22f), Mat("Petal White", new Color(0.9f, 0.85f, 0.85f), plasterTex), PrimitiveType.Cylinder, 80f);
                    Part(g, "Plate", new Vector3(cx + 0.12f, 1.45f, 5.88f), new Vector3(0.18f, 0.01f, 0.18f), Mat("Petal White", new Color(0.9f, 0.85f, 0.85f), plasterTex), PrimitiveType.Cylinder, 80f);
                }
            }

            // Коллайдеры: низ целиком, верх витрин только у задней стенки (чтобы можно было осмотреть медаль).
            Solid(g, new Vector3(xMid, 0.39f, zMid), new Vector3(4f * w, 0.78f, 0.45f));
            Solid(g, new Vector3(xMid, 1.54f, 5.85f), new Vector3(4f * w, 1.52f, 0.2f));
            Solid(g, new Vector3(x0 + w / 2f, 1.15f, zMid), new Vector3(w, 2.3f, 0.45f));
            Solid(g, new Vector3(x0 + 3.5f * w, 1.15f, zMid), new Vector3(w, 2.3f, 0.45f));
            Interact(g.gameObject, "Стенка", "Мамин хрусталь.", "Его достают только на праздники.");

            // Обрывок прошлого отца: медаль в витрине.
            // Открытая коробочка: бордовый бархат, крышка откинута, внутри медаль на колодке.
            var medalPos = new Vector3(x0 + 1.5f * w + 0.12f, 0.8f, 5.6f);
            var velvet = Mat("Medal Box", new Color(0.45f, 0.08f, 0.08f), plasterTex);
            var medal = Box("Medal Box", room, medalPos + new Vector3(0f, 0.02f, 0f), new Vector3(0.12f, 0.04f, 0.09f), velvet);
            Deco(PrimitiveType.Cube, "Medal Velvet", room, medalPos + new Vector3(0f, 0.041f, 0f), new Vector3(0.105f, 0.002f, 0.075f), Mat("Velvet Blue", new Color(0.12f, 0.12f, 0.3f), plasterTex));
            var lid = Deco(PrimitiveType.Cube, "Medal Lid", room, medalPos + new Vector3(0f, 0.08f, 0.05f), new Vector3(0.12f, 0.08f, 0.012f), velvet);
            lid.transform.rotation = Quaternion.Euler(-15f, 0f, 0f);
            Deco(PrimitiveType.Cube, "Medal Ribbon", room, medalPos + new Vector3(0f, 0.044f, 0.012f), new Vector3(0.026f, 0.003f, 0.03f), Mat("Medal Ribbon", new Color(0.55f, 0.56f, 0.58f), plasterTex));
            Deco(PrimitiveType.Cube, "Medal Ribbon Stripe", room, medalPos + new Vector3(0f, 0.0455f, 0.012f), new Vector3(0.006f, 0.002f, 0.03f), Mat("Velvet Blue", new Color(0.12f, 0.12f, 0.3f), plasterTex));
            Deco(PrimitiveType.Cylinder, "Medal", room, medalPos + new Vector3(0f, 0.044f, -0.02f), new Vector3(0.034f, 0.002f, 0.034f), Mat("Medal Silver", new Color(0.75f, 0.75f, 0.72f), plasterTex, 1f, new Color(0.08f, 0.08f, 0.08f)));
            Interact(medal, "Коробочка", "Папина медаль.", "Мама сказала её не трогать.");
        }

        // Красный диван-книжка с тёмными деревянными боковинами.
        static Transform SofaBook(Transform parent, Vector3 position, float yaw)
        {
            var g = Group("Sofa", parent, position, yaw);
            var red = Mat("Sofa Red", new Color(0.6f, 0.13f, 0.1f), plasterTex);
            var wood = Mat("Wood Dark", new Color(0.3f, 0.18f, 0.1f), woodTex);
            Part(g, "Frame", new Vector3(0f, 0.17f, 0.02f), new Vector3(1.92f, 0.2f, 0.78f), wood);
            foreach (var (x, z) in new[] { (-0.9f, -0.35f), (0.9f, -0.35f), (-0.9f, 0.35f), (0.9f, 0.35f) })
                Part(g, "Leg", new Vector3(x, 0.035f, z), new Vector3(0.05f, 0.07f, 0.05f), wood);
            Part(g, "Side", new Vector3(-0.985f, 0.27f, 0f), new Vector3(0.04f, 0.5f, 0.84f), wood);
            Part(g, "Side", new Vector3(0.985f, 0.27f, 0f), new Vector3(0.04f, 0.5f, 0.84f), wood);
            foreach (float x in new[] { -0.63f, 0f, 0.63f })
            {
                Part(g, "Seat Cushion", new Vector3(x, 0.34f, 0.06f), new Vector3(0.62f, 0.14f, 0.66f), red);
                Part(g, "Back Cushion", new Vector3(x, 0.64f, -0.3f), new Vector3(0.62f, 0.48f, 0.14f), red, rx: -8f);
            }
            Solid(g, new Vector3(0f, 0.4f, 0f), new Vector3(1.98f, 0.8f, 0.84f));
            return g;
        }

        // Кресло на деревянном каркасе с подлокотниками и высокой рыже-оранжевой спинкой.
        static Transform Armchair(Transform parent, Vector3 position, float yaw)
        {
            var g = Group("Armchair", parent, position, yaw);
            var fabric = Mat("Armchair Fabric", new Color(0.72f, 0.3f, 0.13f), plasterTex);
            var wood = Mat("Armchair Wood", new Color(0.4f, 0.24f, 0.13f), grainTex);
            foreach (int s in new[] { -1, 1 })
            {
                Part(g, "Front Leg", new Vector3(s * 0.29f, 0.28f, 0.24f), new Vector3(0.04f, 0.56f, 0.04f), wood);
                Part(g, "Back Leg", new Vector3(s * 0.29f, 0.22f, -0.25f), new Vector3(0.04f, 0.44f, 0.04f), wood);
                Part(g, "Armrest", new Vector3(s * 0.29f, 0.57f, 0f), new Vector3(0.06f, 0.03f, 0.62f), wood);
                Part(g, "Side Rail", new Vector3(s * 0.29f, 0.3f, 0f), new Vector3(0.03f, 0.05f, 0.5f), wood);
                Part(g, "Back Post", new Vector3(s * 0.27f, 0.75f, -0.31f), new Vector3(0.035f, 0.75f, 0.035f), wood, rx: -12f);
            }
            Part(g, "Seat", new Vector3(0f, 0.38f, 0.02f), new Vector3(0.52f, 0.12f, 0.52f), fabric);
            Part(g, "Back", new Vector3(0f, 0.8f, -0.27f), new Vector3(0.52f, 0.75f, 0.12f), fabric, rx: -12f);
            Solid(g, new Vector3(0f, 0.5f, 0f), new Vector3(0.62f, 1.0f, 0.62f));
            return g;
        }

        // Двустворчатая дверь в зал: белая рама и рифлёное стекло. Створки открываются внутрь зала.
        static void LivingRoomGlassDoor(Transform room)
        {
            var root = new GameObject("Living Room Door").transform;
            root.SetParent(room, false);
            var paint = Mat("Door Paint", new Color(0.8f, 0.78f, 0.7f), plasterTex);
            var glass = Mat("Ribbed Glass", Color.white, MakeColorTexture("Ribbed Glass", 8, 8, (x, y, r) => x % 4 < 2 ? new Color(0.78f, 0.82f, 0.8f) : new Color(0.6f, 0.66f, 0.66f)), 6f, new Color(0.1f, 0.1f, 0.1f));

            var pivots = new Transform[2];
            for (int i = 0; i < 2; i++)
            {
                float s = i == 0 ? 1f : -1f;
                var pivot = new GameObject(i == 0 ? "Door Pivot Living A" : "Door Pivot Living B").transform;
                pivot.SetParent(root, false);
                pivot.position = new Vector3(7f, 0f, i == 0 ? 1.85f : 2.75f);
                pivots[i] = pivot;

                var leaf = pivot.gameObject.AddComponent<BoxCollider>();
                leaf.center = new Vector3(0f, 1.015f, s * 0.2225f);
                leaf.size = new Vector3(0.04f, 2.03f, 0.445f);
                Part(pivot, "Stile", new Vector3(0f, 1.015f, s * 0.035f), new Vector3(0.04f, 2.03f, 0.07f), paint);
                Part(pivot, "Stile", new Vector3(0f, 1.015f, s * 0.41f), new Vector3(0.04f, 2.03f, 0.07f), paint);
                Part(pivot, "Top Rail", new Vector3(0f, 1.99f, s * 0.2225f), new Vector3(0.04f, 0.08f, 0.445f), paint);
                Part(pivot, "Bottom Panel", new Vector3(0f, 0.25f, s * 0.2225f), new Vector3(0.04f, 0.5f, 0.445f), paint);
                Part(pivot, "Glass", new Vector3(0f, 1.245f, s * 0.2225f), new Vector3(0.012f, 1.41f, 0.31f), glass);
                Part(pivot, "Handle", new Vector3(0.035f, 1.0f, s * 0.4f), new Vector3(0.025f, 0.025f, 0.1f), Metal);
                Part(pivot, "Handle", new Vector3(-0.035f, 1.0f, s * 0.4f), new Vector3(0.025f, 0.025f, 0.1f), Metal);
            }

            var door = root.gameObject.AddComponent<SwingDoor>();
            door.leaves = pivots;
            door.closedAngles = new[] { 0f, 0f };
            door.openAngles = new[] { 85f, -85f };
            door.isOpen = true;
            pivots[0].localRotation = Quaternion.Euler(0f, 85f, 0f);
            pivots[1].localRotation = Quaternion.Euler(0f, -85f, 0f);
        }

        // ================= Коридор =================

        // Открытая обувница с решётчатыми полками.
        static Transform ShoeShelf(Transform parent, Vector3 position, float yaw)
        {
            var g = Group("Shoe Shelf", parent, position, yaw);
            var wood = Mat("Shoe Shelf", new Color(0.5f, 0.35f, 0.22f), grainTex);
            Part(g, "Side", new Vector3(-0.34f, 0.2f, 0f), new Vector3(0.02f, 0.4f, 0.28f), wood);
            Part(g, "Side", new Vector3(0.34f, 0.2f, 0f), new Vector3(0.02f, 0.4f, 0.28f), wood);
            Part(g, "Top", new Vector3(0f, 0.39f, 0f), new Vector3(0.7f, 0.02f, 0.28f), wood);
            foreach (float y in new[] { 0.06f, 0.22f })
                foreach (float z in new[] { -0.08f, 0f, 0.08f })
                    Part(g, "Slat", new Vector3(0f, y, z), new Vector3(0.66f, 0.015f, 0.05f), wood);
            Solid(g, new Vector3(0f, 0.2f, 0f), new Vector3(0.7f, 0.4f, 0.28f));
            return g;
        }

        // ================= Текстуры =================

        // Обложка букваря: заголовок, картинка с небом, травой, солнцем и двумя детьми.
        static Color PrimerCoverPixel(int x, int y, float r)
        {
            if (x == 0 || x == 15 || y == 0 || y == 23) return new Color(0.6f, 0.15f, 0.12f);
            if (y >= 17)
            {
                bool letter = y >= 18 && y <= 21 && x >= 2 && x <= 13 && ((x % 2 == 0) || y == 18 || y == 21);
                return letter ? new Color(0.95f, 0.92f, 0.85f) : new Color(0.72f, 0.16f, 0.12f);
            }
            if (x < 2 || x > 13 || y < 3) return new Color(0.88f, 0.84f, 0.7f);
            if ((x == 11 || x == 12) && (y == 14 || y == 15)) return new Color(0.95f, 0.8f, 0.2f);
            if (x == 6 && y >= 6 && y <= 9) return y == 9 ? new Color(0.85f, 0.65f, 0.5f) : new Color(0.75f, 0.15f, 0.12f);
            if (x == 9 && y >= 6 && y <= 9) return y == 9 ? new Color(0.85f, 0.65f, 0.5f) : new Color(0.15f, 0.25f, 0.6f);
            return y >= 9 ? Noise(new Color(0.5f, 0.68f, 0.88f), r, 0.04f) : Noise(new Color(0.35f, 0.6f, 0.3f), r, 0.08f);
        }

        // Светлый «мрамор» с серыми прожилками (столешница на кухне).
        static Color MarblePixel(int x, int y, float r)
        {
            float n = Mathf.PerlinNoise(x * 0.12f, y * 0.12f);
            float vein = Mathf.Abs(Mathf.Sin(x * 0.25f + y * 0.1f + n * 6f));
            if (vein < 0.07f) return new Color(0.5f, 0.5f, 0.52f);
            if (vein < 0.14f) return new Color(0.7f, 0.7f, 0.71f);
            return Noise(new Color(0.88f, 0.87f, 0.84f), r, 0.03f);
        }
    }
}
