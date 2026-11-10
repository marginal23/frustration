using UnityEngine;

namespace Frustration.EditorTools
{
    // Третья часть сборщика: зал (x 7..11, z 1.6..6) и закрытая дверь в спальню родителей.
    // Вход в зал — из восточного конца коридора.
    public static partial class Chapter1Builder
    {
        static readonly Gap LivingRoomDoor = new Gap(1.85f, 2.75f, 0f, DoorTop);

        static void BuildLivingRoom()
        {
            var room = new GameObject("Living Room").transform;
            var wall = Mat("Wall", new Color(0.55f, 0.6f, 0.5f), plasterTex);
            var floor = Mat("Floor", new Color(0.45f, 0.3f, 0.22f), floorTex, 6f);
            var ceiling = Mat("Ceiling", new Color(0.6f, 0.6f, 0.58f), plasterTex);
            var wood = Mat("Wood Dark", new Color(0.3f, 0.18f, 0.1f), woodTex);
            var wallpaper = Mat("Wallpaper Living", Color.white, MakeColorTexture("Wallpaper Living", 24, 32, WallpaperLivingPixel));
            var carpet = Mat("Carpet", Color.white, MakeColorTexture("Carpet", 64, 48, CarpetPixel));
            var curtain = Mat("Curtain", Color.white, MakeColorTexture("Curtain", 16, 32, CurtainPixel));
            var crystal = Mat("Crystal", new Color(0.75f, 0.8f, 0.85f), plasterTex, 1f, new Color(0.1f, 0.1f, 0.12f));

            // --- Пол, потолок, стены ---
            var floorBox = Box("Floor", room, new Vector3(9f, -0.05f, 3.8f), new Vector3(4f, 0.1f, 4.4f), floor);
            Object.DestroyImmediate(floorBox.GetComponent<MeshRenderer>());
            FlatQuad("Floor Surface", room, 7f, 11f, 1.6f, 6f, 0f, floor, 7f, 7f);
            Box("Ceiling", room, new Vector3(9f, WallHeight + 0.05f, 3.8f), new Vector3(4f, 0.1f, 4.4f), ceiling);
            WallXWithWindows(room, 6f, 7f, 11f, new[] { 10.0f }, wall);
            WallZ(room, 11f, 1.6f, 6f, wall);
            // Южная стена лежит в плоскости зеркала: её копию в отражение не берём.
            Box("Wall South (no mirror)", room, new Vector3(9f, WallHeight / 2f, 1.6f), new Vector3(4f, WallHeight, WallThickness), wall);

            Panel("Wallpaper", room, true, 6f, -1, 7f, 11f, 0f, WallHeight, new[] { WindowGap(10.0f) }, wallpaper, 0.5f);
            Panel("Wallpaper", room, true, 1.6f, +1, 7f, 11f, 0f, WallHeight, NoGaps, wallpaper, 0.5f);
            Panel("Wallpaper", room, false, 7f, +1, 1.6f, 6f, 0f, WallHeight, new[] { LivingRoomDoor }, wallpaper, 0.5f);
            Panel("Wallpaper", room, false, 11f, -1, 1.6f, 6f, 0f, WallHeight, NoGaps, wallpaper, 0.5f);

            // Стенка, диван, кресло и дверь со стеклом — в Chapter1Builder.Furniture.cs.
            // Журнальный столик — в Chapter1Builder.Furniture.cs.
            FlatQuad("Floor Carpet", room, 7.7f, 10.3f, 2.5f, 4.3f, 0.006f, carpet, 2.6f, 1.8f);

            // --- Торшер: единственный включённый свет в зале ---
            Primitive(PrimitiveType.Cylinder, "Floor Lamp Pole", room, new Vector3(10.6f, 0.75f, 2.1f), new Vector3(0.03f, 0.75f, 0.03f), Mat("Strap", new Color(0.08f, 0.08f, 0.08f), plasterTex));
            Primitive(PrimitiveType.Cylinder, "Floor Lamp Shade", room, new Vector3(10.6f, 1.55f, 2.1f), new Vector3(0.36f, 0.14f, 0.36f), Mat("Lamp Shade Lit", new Color(0.8f, 0.65f, 0.4f), plasterTex, 1f, new Color(0.35f, 0.25f, 0.12f)));
            var lamp = PointLight("Floor Lamp", room, new Vector3(10.6f, 1.35f, 2.1f), new Color(1f, 0.75f, 0.45f), 0.7f, 3.5f);
            lamp.gameObject.AddComponent<LightFlicker>().amount = 0.08f;

            // Люстра (выключена) и выключатель — в Chapter1Builder.Furniture.cs.

            // --- Окно ---
            var window = Window("Living Window", room, new Vector3(10.0f, 1.5f, 5.94f));
            Interact(window, "Окно", "Отсюда видно трубы комбината.");
            Curtains(room, 10.0f, curtain);
            PointLight("Living Window Glow", room, new Vector3(10.0f, 1.6f, 5.4f), new Color(0.5f, 0.6f, 0.85f), 0.7f, 4f);

            // --- Закрытая дверь в спальню родителей (комнаты за ней нет) ---
            var bedroomDoor = Box("Bedroom Door", room, new Vector3(10.94f, 1.02f, 4.3f), new Vector3(0.06f, 2.05f, 0.9f), Mat("Door", new Color(0.35f, 0.2f, 0.12f), woodTex));
            Box("Bedroom Door Handle", room, new Vector3(10.9f, 1.0f, 3.95f), new Vector3(0.03f, 0.03f, 0.1f), Mat("Metal", new Color(0.7f, 0.7f, 0.65f), plasterTex));
            Interact(bedroomDoor, "Спальня родителей", "Закрыто.", "Туда без стука нельзя.");
        }

        // Бежево-розовые обои зала с вертикальными цветочными дорожками.
        static Color WallpaperLivingPixel(int x, int y, float r)
        {
            var c = new Color(0.72f, 0.6f, 0.52f);
            int cx = x % 12;
            if (cx == 0 || cx == 11) c = new Color(0.62f, 0.5f, 0.43f);
            int dx = Mathf.Abs(cx - 6), dy = Mathf.Abs((y % 8) - 4);
            if (dx + dy <= 1) c = new Color(0.55f, 0.3f, 0.3f);
            else if (dx + dy == 2 && y % 8 > 4) c = new Color(0.4f, 0.48f, 0.32f);
            return Noise(c, r, 0.05f);
        }
    }
}
