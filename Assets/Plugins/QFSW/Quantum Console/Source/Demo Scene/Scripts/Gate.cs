using UnityEngine;

namespace QFSW.QC.Demo
{
    [CommandPrefix("demo.gate.")]
    public class Gate : MonoBehaviour
    {
        [Command("opened")]
        private bool IsOpened
        {
            get => GetComponent<Animator>().GetBool("opened");
            set => GetComponent<Animator>().SetBool("opened", value);
        }
    }
}