using System;
using UnityEngine;

namespace PointActivitySystem.Runtime
{
    public abstract class PointActiveListener : MonoBehaviour
    {
        protected virtual void OnEnable ()
        {
            Point.PointEnabled += OnPointVisibilityChange;
        }

        protected virtual void OnDisable ()
        {
            Point.PointEnabled -= OnPointVisibilityChange;
        }

        protected abstract void OnPointVisibilityChange (bool visibility);
    }
}
