using System;
using UnityEngine;
using UnityEngine.UI;

namespace PointActivitySystem.Runtime
{
    /// <summary>
    /// Point class works as base for all points.
    /// </summary>
    public abstract class Point : MonoBehaviour
    {
        [SerializeField] protected PointActivity pointActivity;

        [SerializeField] protected Canvas pointCanvas;
        [SerializeField] protected bool canvasEnabledWhenPointIsActivated;

        public static Action <bool> PointEnabled;

        protected static bool PointIsActive;

        protected virtual void StartPointActivity ()
        {
            if (PointIsActive)
                return;

            PointIsActive = true;
            PointEnabled?.Invoke (PointIsActive);


            if (canvasEnabledWhenPointIsActivated)
                pointCanvas.enabled = true;
        }

        protected virtual void Update () {}

        protected virtual void EndPointActivity ()
        {
            PointIsActive = false;
            PointEnabled?.Invoke (PointIsActive);

            pointCanvas.enabled = false;

            Destroy (gameObject);
        }

        protected virtual void SetComplete()
        {
            pointActivity.SetComplete();
        }
    }
}