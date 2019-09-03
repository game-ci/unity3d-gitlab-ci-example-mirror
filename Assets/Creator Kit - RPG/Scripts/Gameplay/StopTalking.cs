using RPGM.Core;
using UnityEngine;

namespace RPGM.Events
{
    public class StopTalking : Event<StopTalking>
    {
        public Animator animator;

        public override void Execute()
        {
            animator.SetBool("Talk", false);
        }
    }
}