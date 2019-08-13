using System;
using UnityEngine;

namespace RPGM.Mechanics
{

    public class AnimationBatchController : MonoBehaviour
    {
        [System.Serializable]
        public class AnimationBatch
        {
            public int frame;
            public Transform sceneParent;
            public Sprite[] animationFrames;
            public SpriteRenderer[] spriteRenderers;
        }

        public float frameRate = 12;
        public AnimationBatch[] batches;

        public float nextFrameTime = 0;

        void OnEnable()
        {
            foreach (var batch in batches)
            {
                if (batch.sceneParent != null)
                {
                    batch.spriteRenderers = batch.sceneParent.GetComponentsInChildren<SpriteRenderer>();
                }
            }
        }

        void Update()
        {
            //if it's time for the next frame...
            if (Time.time - nextFrameTime > (1f / frameRate))
            {
                foreach (var batch in batches)
                {
                    AnimateBatch(batch);
                }
                //calculate the time of the next frame.
                nextFrameTime += 1f / frameRate;
            }
        }

        void AnimateBatch(AnimationBatch batch)
        {
            //update all tokens with the next animation frame.
            var renderers = batch.spriteRenderers;
            var frames = batch.animationFrames;
            var count = frames.Length;
            var frameIndex = batch.frame;
            for (var i = 0; i < batch.spriteRenderers.Length; i++)
            {
                var sr = renderers[i];
                sr.sprite = frames[(frameIndex + i) % count];
            }
            batch.frame += 1;
        }
    }
}
