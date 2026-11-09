using UnityEngine;
using UnityEngine.InputSystem;

namespace Frustration
{
    // Управление от первого лица: WASD, мышь, лёгкое покачивание камеры при ходьбе.
    // Камера стоит низко (около 1.1 м), потому что герой в начале игры ребёнок.
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        public Transform cameraPivot;
        public float walkSpeed = 1.6f;
        public float mouseSensitivity = 0.1f;
        public float gravity = -9.81f;

        [Header("Покачивание головы")]
        public float bobAmount = 0.025f;
        public float bobFrequency = 7f;

        // Когда true, игрок не может двигаться (например, пока идёт затемнение).
        public bool inputLocked;

        CharacterController controller;
        float pitch;
        float verticalVelocity;
        float bobTime;
        Vector3 cameraBasePosition;

        void Awake()
        {
            controller = GetComponent<CharacterController>();
            cameraBasePosition = cameraPivot.localPosition;
            LockCursor(true);
        }

        void Update()
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            if (keyboard == null || mouse == null) return;

            if (keyboard.escapeKey.wasPressedThisFrame) LockCursor(false);
            if (mouse.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked) LockCursor(true);

            Vector2 move = Vector2.zero;
            if (!inputLocked && Cursor.lockState == CursorLockMode.Locked)
            {
                Vector2 look = mouse.delta.ReadValue() * mouseSensitivity;
                transform.Rotate(0f, look.x, 0f);
                pitch = Mathf.Clamp(pitch - look.y, -80f, 80f);
                cameraPivot.localEulerAngles = new Vector3(pitch, 0f, 0f);

                if (keyboard.wKey.isPressed) move.y += 1f;
                if (keyboard.sKey.isPressed) move.y -= 1f;
                if (keyboard.dKey.isPressed) move.x += 1f;
                if (keyboard.aKey.isPressed) move.x -= 1f;
            }

            Vector3 direction = transform.forward * move.y + transform.right * move.x;
            if (direction.sqrMagnitude > 1f) direction.Normalize();

            if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -1f;
            verticalVelocity += gravity * Time.deltaTime;
            controller.Move((direction * walkSpeed + Vector3.up * verticalVelocity) * Time.deltaTime);

            // Покачивание: при ходьбе синус крутится, при остановке камера плавно возвращается.
            if (direction.sqrMagnitude > 0.01f) bobTime += Time.deltaTime * bobFrequency;
            else bobTime = Mathf.Lerp(bobTime, Mathf.Round(bobTime / Mathf.PI) * Mathf.PI, Time.deltaTime * 6f);
            cameraPivot.localPosition = cameraBasePosition + Vector3.up * (Mathf.Sin(bobTime) * bobAmount);
        }

        static void LockCursor(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }
}
