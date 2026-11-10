using UnityEngine;

namespace Frustration.EditorTools
{
    // Седьмая часть сборщика: пиксельные «фотографии» в рамках.
    // Каждая картинка рисуется по пикселям простыми фигурами, потом слегка «выцветает».
    public static partial class Chapter1Builder
    {
        static bool Ellipse(float x, float y, float cx, float cy, float rx, float ry)
        {
            float dx = (x - cx) / rx, dy = (y - cy) / ry;
            return dx * dx + dy * dy <= 1f;
        }

        static Color Faded(Color c, float amount, Color tint)
        {
            float gray = c.r * 0.3f + c.g * 0.59f + c.b * 0.11f;
            var old = new Color(gray * tint.r, gray * tint.g, gray * tint.b);
            return Color.Lerp(c, old, amount);
        }

        // Отец в форме, совсем молодой, улыбается. 16x22.
        static Color UniformPhotoPixel(int x, int y, float r)
        {
            var skin = new Color(0.8f, 0.64f, 0.53f);
            var dark = new Color(0.12f, 0.1f, 0.09f);
            Color c = Color.Lerp(new Color(0.5f, 0.55f, 0.58f), new Color(0.68f, 0.7f, 0.7f), y / 21f);

            // Плечи и китель, пятнистый камуфляж.
            if (y <= 8 && Ellipse(x, y, 7.5f, 0f, 9f, 8.5f))
            {
                float n = Mathf.PerlinNoise(x * 0.45f, y * 0.45f);
                c = n > 0.6f ? new Color(0.22f, 0.25f, 0.15f) : (n > 0.4f ? new Color(0.35f, 0.38f, 0.22f) : new Color(0.42f, 0.38f, 0.27f));
            }
            // Вырез: тельняшка.
            if (y >= 5 && y <= 8 && Mathf.Abs(x - 7.5f) <= (y - 4) * 0.5f + 0.5f)
                c = y % 2 == 0 ? new Color(0.2f, 0.25f, 0.5f) : new Color(0.9f, 0.9f, 0.88f);
            // Шея и голова.
            if (y >= 8 && y <= 10 && x >= 6 && x <= 9) c = skin * 0.92f;
            if (Ellipse(x, y, 7.5f, 13.5f, 3.4f, 4.3f)) c = skin;
            // Короткая стрижка.
            if (Ellipse(x, y, 7.5f, 15.5f, 3.6f, 2.6f) && y >= 16) c = dark;
            // Глаза и улыбка.
            if (y == 14 && (x == 6 || x == 9)) c = dark;
            if ((y == 11 && (x == 6 || x == 9)) || (y == 10 && (x == 7 || x == 8))) c = new Color(0.45f, 0.2f, 0.18f);
            if (y == 11 && (x == 7 || x == 8)) c = new Color(0.95f, 0.92f, 0.88f);

            if (x == 0 || x == 15 || y == 0 || y == 21) c = new Color(0.93f, 0.91f, 0.86f);   // белые поля фото
            return Noise(Faded(c, 0.3f, new Color(1f, 0.93f, 0.8f)), r, 0.05f);
        }

        // Свадьба: мама в белом платье и фате, папа в тёмном костюме. Сепия. 13x18.
        static Color WeddingPhotoPixel(int x, int y, float r)
        {
            var skin = new Color(0.8f, 0.66f, 0.55f);
            Color c = Color.Lerp(new Color(0.35f, 0.32f, 0.3f), new Color(0.6f, 0.56f, 0.5f), y / 17f);

            // Невеста слева: платье расширяется книзу, фата за головой.
            if (y <= 9 && Mathf.Abs(x - 3.5f) <= 1.2f + (9 - y) * 0.25f) c = new Color(0.95f, 0.94f, 0.9f);
            if (y >= 7 && y <= 15 && Mathf.Abs(x - 3.5f) <= 2.4f - (y - 7) * 0.15f && !Ellipse(x, y, 3.5f, 12f, 1.4f, 1.8f)) c = new Color(0.88f, 0.88f, 0.86f);
            if (Ellipse(x, y, 3.5f, 12f, 1.4f, 1.8f)) c = skin;
            if (y >= 13 && Ellipse(x, y, 3.5f, 12.8f, 1.6f, 1.4f)) c = new Color(0.3f, 0.2f, 0.12f);

            // Жених справа.
            if (y <= 10 && x >= 7 && x <= 11) c = new Color(0.1f, 0.1f, 0.12f);
            if (y >= 8 && y <= 10 && x == 9) c = new Color(0.92f, 0.92f, 0.9f);
            if (Ellipse(x, y, 9f, 12.5f, 1.5f, 1.9f)) c = skin;
            if (y >= 14 && Ellipse(x, y, 9f, 13.2f, 1.7f, 1.3f)) c = new Color(0.12f, 0.1f, 0.08f);
            if (y == 12 && (x == 8 || x == 10)) c = new Color(0.15f, 0.1f, 0.08f);

            return Noise(Faded(c, 0.85f, new Color(1.08f, 0.9f, 0.68f)), r, 0.06f);
        }

        // Мы на озере: небо, сосны, вода, песок. Папа держит меня на плечах, рядом мама. 21x15.
        static Color FamilyPhotoPixel(int x, int y, float r)
        {
            var skin = new Color(0.82f, 0.62f, 0.5f);
            Color c;
            if (y >= 11) c = new Color(0.62f, 0.75f, 0.85f);
            else if (y >= 8) c = (x + y * 3) % 5 == 0 ? new Color(0.1f, 0.2f, 0.12f) : new Color(0.16f, 0.3f, 0.18f);
            else if (y >= 5) c = new Color(0.3f, 0.45f, 0.58f);
            else c = new Color(0.78f, 0.7f, 0.52f);

            // Папа: тельняшка, на плечах ребёнок.
            if (x >= 6 && x <= 8 && y <= 7) c = y % 2 == 0 ? new Color(0.25f, 0.3f, 0.55f) : new Color(0.9f, 0.9f, 0.88f);
            if (Ellipse(x, y, 7f, 9f, 1.2f, 1.4f)) c = skin;
            if (x >= 6 && x <= 8 && y >= 10 && y <= 11) c = new Color(0.85f, 0.25f, 0.2f);
            if (Ellipse(x, y, 7f, 12.5f, 1f, 1.1f)) c = skin;
            // Мама: летнее платье в горошек.
            if (x >= 11 && x <= 13 && y <= 7) c = (x + y) % 2 == 0 ? new Color(0.75f, 0.35f, 0.4f) : new Color(0.85f, 0.75f, 0.7f);
            if (Ellipse(x, y, 12f, 9f, 1.2f, 1.4f)) c = skin;
            if (y >= 10 && Ellipse(x, y, 12f, 9.6f, 1.5f, 1.2f)) c = new Color(0.4f, 0.25f, 0.15f);

            if (x == 0 || x == 20 || y == 0 || y == 14) c = new Color(0.93f, 0.91f, 0.86f);
            return Noise(Faded(c, 0.35f, new Color(1.05f, 0.9f, 0.75f)), r, 0.05f);
        }
    }
}
