using UnityEngine;

namespace Frustration
{
    // Распашная дверь (одна или две створки). По E открывается или закрывается.
    // Каждая створка — это шарнир (pivot), повёрнутый вокруг вертикальной оси.
    public class SwingDoor : Interactable
    {
        public Transform[] leaves;
        public float[] closedAngles;
        public float[] openAngles;
        public bool isOpen = true;
        public float speed = 160f;
        public string openPrompt = "Закрыть дверь";
        public string closedPrompt = "Открыть дверь";

        void Start() => prompt = isOpen ? openPrompt : closedPrompt;

        public override void Interact()
        {
            isOpen = !isOpen;
            prompt = isOpen ? openPrompt : closedPrompt;
            if (leaves != null && leaves.Length > 0)
                ProceduralSfx.PlayAt(ProceduralSfx.Creak(), leaves[0].position + Vector3.up, 0.45f);
        }

        void Update()
        {
            if (leaves == null) return;
            for (int i = 0; i < leaves.Length; i++)
            {
                var target = Quaternion.Euler(0f, isOpen ? openAngles[i] : closedAngles[i], 0f);
                leaves[i].localRotation = Quaternion.RotateTowards(leaves[i].localRotation, target, speed * Time.deltaTime);
            }
        }
    }
}
