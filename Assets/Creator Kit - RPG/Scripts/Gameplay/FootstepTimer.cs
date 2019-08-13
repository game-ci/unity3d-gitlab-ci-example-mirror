using System.Collections;
using System.Collections.Generic;
using RPGM.UI;
using UnityEngine;

namespace RPGM.Gameplay
{
    /// <summary>
    /// Triggers footsteps sounds during playback of an animation state.
    /// </summary>
    public class FootstepTimer : StateMachineBehaviour
    {
        [Range(0, 1)]
        public float leftFoot, rightFoot;
        public AudioClip[] clips;

        float lastNormalizedTime;
        int clipIndex = 0;

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var t = stateInfo.normalizedTime % 1;
            if (lastNormalizedTime < leftFoot && t >= leftFoot)
            {
                UserInterfaceAudio.PlayClip(clips[clipIndex]);
                clipIndex = (clipIndex + 1) % clips.Length;
            }
            if (lastNormalizedTime < rightFoot && t >= rightFoot)
            {
                UserInterfaceAudio.PlayClip(clips[clipIndex]);
                clipIndex = (clipIndex + 1) % clips.Length;
            }
            lastNormalizedTime = t;

        }
    }
}