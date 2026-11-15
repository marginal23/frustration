using UnityEngine;

namespace Frustration
{
    // Зеркало старой школы (как в Super Mario 64 или Silent Hill): никакого отражения не считается.
    // За дыркой в стене стоит зеркальная копия квартиры, а в ней ходит "отражение" героя.
    // Этот скрипт:
    // - передаёт шейдеру плоскость зеркала, чтобы свет не проходил с одной стороны на другую;
    // - каждый кадр ставит фигурку-отражение в точку, зеркальную к игроку;
    // - один раз за утро пугает: отражение замирает, смотрит прямо на героя
    //   и стоит уже собранным (с ранцем и цветами), хотя герой их ещё не взял.
    public class MirrorWorld : MonoBehaviour
    {
        [Tooltip("Координата Z поверхности зеркала (зеркало стоит в плоскости XY).")]
        public float planeZ = 1.65f;
        public float mirrorX = 2.2f;
        public Transform player;
        public Transform reflection;
        [Tooltip("Ранец на спине у отражения: появляется, когда герой его взял.")]
        public GameObject reflectionSatchel;
        [Tooltip("Цветы в руке у отражения: появляются, когда герой их взял.")]
        public GameObject reflectionFlowers;

        [Header("Испуг")]
        [Tooltip("Флаг, после которого зеркало может напугать (например, прочитана записка).")]
        public string scareAfterFlag = "note";
        public float scareDistance = 2.6f;
        public float scareSeconds = 1.6f;
        public Light flickerLight;
        [Tooltip("Что показать в отражении во время испуга (например, фигуру отца) и что спрятать.")]
        public GameObject scareShow;
        public GameObject scareHide;

        static readonly int MirrorPlaneId = Shader.PropertyToID("_MirrorPlane");

        bool scareDone;
        float scareTimer;
        float savedIntensity;

        void OnEnable() => Shader.SetGlobalVector(MirrorPlaneId, new Vector4(0f, 0f, 1f, -planeZ));
        void OnDisable() => Shader.SetGlobalVector(MirrorPlaneId, Vector4.zero);

        void LateUpdate()
        {
            if (player == null || reflection == null) return;

            if (scareTimer > 0f)
            {
                scareTimer -= Time.deltaTime;
                if (flickerLight != null) flickerLight.intensity = savedIntensity * (Random.value < 0.3f ? 0.05f : 0.25f);
                if (scareTimer <= 0f)
                {
                    if (flickerLight != null) flickerLight.intensity = savedIntensity;
                    if (scareShow != null) scareShow.SetActive(false);
                    if (scareHide != null) scareHide.SetActive(true);
                }
                return; // отражение стоит на месте и не повторяет движения
            }

            Vector3 p = player.position;
            reflection.position = new Vector3(p.x, p.y, 2f * planeZ - p.z);

            Vector3 forward = player.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude > 0.0001f)
                reflection.rotation = Quaternion.LookRotation(new Vector3(forward.x, 0f, -forward.z));

            if (reflectionSatchel != null) reflectionSatchel.SetActive(StoryFlags.Has("backpack"));
            if (reflectionFlowers != null) reflectionFlowers.SetActive(StoryFlags.Has("flowers"));

            TryScare(forward);
        }

        void TryScare(Vector3 forward)
        {
            if (scareDone || (!string.IsNullOrEmpty(scareAfterFlag) && !StoryFlags.Has(scareAfterFlag))) return;
            var toMirror = new Vector3(mirrorX - player.position.x, 0f, planeZ - player.position.z);
            if (toMirror.magnitude > scareDistance || forward.sqrMagnitude < 0.0001f) return;
            if (Vector3.Dot(forward.normalized, toMirror.normalized) < 0.85f) return;

            scareDone = true;
            scareTimer = scareSeconds;
            // Отражение смотрит прямо из зеркала на героя и уже «собрано» в школу.
            reflection.rotation = Quaternion.LookRotation(Vector3.forward);
            if (reflectionSatchel != null) reflectionSatchel.SetActive(true);
            if (reflectionFlowers != null) reflectionFlowers.SetActive(true);
            if (flickerLight != null) savedIntensity = flickerLight.intensity;
            if (scareShow != null) scareShow.SetActive(true);
            if (scareHide != null) scareHide.SetActive(false);

            var sting = new GameObject("Mirror Sting").AddComponent<AudioSource>();
            sting.clip = ProceduralSfx.Sting();
            sting.volume = 0.7f;
            sting.Play();
            Destroy(sting.gameObject, sting.clip.length + 0.1f);
        }
    }
}
