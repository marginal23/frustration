using UnityEngine;

namespace Frustration
{
    // Шаги героя: каждые stride метров пройденного пути — звук шага.
    // Под ногами ковёр (коллайдер с «Carpet» или «Rug» в имени) — шаг мягкий, иначе — линолеум.
    [RequireComponent(typeof(CharacterController))]
    public class Footsteps : MonoBehaviour
    {
        public float stride = 0.48f;
        public float volume = 0.35f;

        CharacterController controller;
        AudioSource source;
        float distance;
        int variant;

        void Awake()
        {
            controller = GetComponent<CharacterController>();
            source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;
        }

        void Update()
        {
            if (!controller.isGrounded) return;
            var velocity = controller.velocity;
            velocity.y = 0f;
            distance += velocity.magnitude * Time.deltaTime;
            if (distance < stride) return;
            distance = 0f;

            bool soft = false;
            if (Physics.Raycast(transform.position + Vector3.up * 0.3f, Vector3.down, out var hit, 0.6f, ~0, QueryTriggerInteraction.Ignore))
                soft = hit.collider.name.Contains("Carpet") || hit.collider.name.Contains("Rug");

            variant = (variant + 1 + Random.Range(0, 2)) % 4;
            source.pitch = Random.Range(0.95f, 1.05f);
            source.PlayOneShot(ProceduralSfx.Footstep(!soft, variant), soft ? volume * 0.7f : volume);
        }
    }
}
