using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGM.UI
{
    public class TranslationAnimation : MonoBehaviour
    {
        public float duration = 0.5f;
        public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
        public Vector3[] positions;

        public void MoveToOrigin()
        {
            movementCommands.Enqueue(origin);
        }

        public void MoveToPosition(int index)
        {
            if (index < positions.Length)
                movementCommands.Enqueue(positions[index]);
            else
                Debug.LogError("Position Index out of range.");
        }

        void Awake()
        {
            origin = transform.position;
        }

        void Update()
        {

            switch (state)
            {
                case State.Stopped:
                    StoppedState();
                    break;
                case State.Moving:
                    MovingState();
                    break;
            }
        }

        void StoppedState()
        {
            if (movementCommands.Count > 0)
            {
                start = transform.position;
                target = movementCommands.Dequeue();
                state = State.Moving;
                t = 0;
            }
        }

        void MovingState()
        {
            t += Time.deltaTime / duration;
            if (t >= 1)
            {
                t = 1;
                state = State.Stopped;
            }
            transform.position = Vector3.Lerp(start, target, curve.Evaluate(t));

        }

        enum State
        {
            Stopped,
            Moving
        }

        Vector3 origin, start, target;
        Queue<Vector3> movementCommands = new Queue<Vector3>();
        State state;
        float t;
    }
}