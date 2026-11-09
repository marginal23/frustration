using UnityEngine;

namespace Frustration
{
    // Зеркало старой школы (как в Super Mario 64 или Silent Hill): никакого отражения не считается.
    // За дыркой в стене стоит зеркальная копия квартиры, а в ней ходит "отражение" героя.
    // Этот скрипт:
    // - передаёт шейдеру плоскость зеркала, чтобы свет не проходил с одной стороны на другую;
    // - каждый кадр ставит фигурку-отражение в точку, зеркальную к игроку.
    // Плюс трюка: отражение может вести себя иначе, чем игрок. Для хоррора это золото.
    public class MirrorWorld : MonoBehaviour
    {
        [Tooltip("Координата Z поверхности зеркала (зеркало стоит в плоскости XY).")]
        public float planeZ = 1.65f;
        public Transform player;
        public Transform reflection;
        [Tooltip("Ранец на спине у отражения: появляется, когда герой его взял.")]
        public GameObject reflectionSatchel;

        static readonly int MirrorPlaneId = Shader.PropertyToID("_MirrorPlane");

        void OnEnable() => Shader.SetGlobalVector(MirrorPlaneId, new Vector4(0f, 0f, 1f, -planeZ));
        void OnDisable() => Shader.SetGlobalVector(MirrorPlaneId, Vector4.zero);

        void LateUpdate()
        {
            if (player == null || reflection == null) return;

            Vector3 p = player.position;
            reflection.position = new Vector3(p.x, p.y, 2f * planeZ - p.z);

            Vector3 forward = player.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude > 0.0001f)
                reflection.rotation = Quaternion.LookRotation(new Vector3(forward.x, 0f, -forward.z));

            if (reflectionSatchel != null)
                reflectionSatchel.SetActive(StoryFlags.Has("backpack"));
        }
    }
}
