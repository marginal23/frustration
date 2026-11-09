using UnityEngine;
using UnityEngine.InputSystem;

namespace Frustration
{
    // Луч из глаз игрока: если он смотрит на Interactable, показываем подсказку, по E осматриваем.
    // Пока на экране субтитры, E и пробел листают реплики.
    public class PlayerInteractor : MonoBehaviour
    {
        public Camera viewCamera;
        public float reach = 1.8f;

        void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null || GameUI.Instance == null) return;

            bool pressed = keyboard.eKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame;

            if (GameUI.Instance.IsShowingLines)
            {
                GameUI.Instance.SetPrompt(null);
                if (pressed) GameUI.Instance.NextLine();
                return;
            }

            Interactable target = null;
            var ray = new Ray(viewCamera.transform.position, viewCamera.transform.forward);
            if (Physics.Raycast(ray, out var hit, reach, ~0, QueryTriggerInteraction.Collide))
                target = hit.collider.GetComponentInParent<Interactable>();

            GameUI.Instance.SetPrompt(target != null ? target.prompt : null);
            if (target != null && keyboard.eKey.wasPressedThisFrame) target.Interact();
        }
    }
}
