using Mapbox.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PointActivitySystem.Runtime
{
    [CreateAssetMenu (menuName = "Point Activity/Activity", fileName = "Point Activity")]
    public class PointActivity : ScriptableObject
    {

        [Space] [SerializeField, Tooltip("Prefab which will open when point is activated")] private GameObject pointPrefab;
        public GameObject PointPrefab => pointPrefab;



        
        [SerializeField, Tooltip("Add point locations here and mapbox map. Currently it has to be added twice")] private List<string> locations = new List<string>();
        public List<string> Locations => locations;


        [SerializeField] private Sprite pointIconWithBackground;
        public Sprite PointIconWithBackground => pointIconWithBackground;
        
        public event Action <bool> PointStatusChange;
        public event Action QuizCompleted;


        public bool IsInUse { get; private set; }


        [SerializeField] private bool isCompleted;
        public bool IsCompleted => isCompleted;


        [TextArea, SerializeField] private string pointInstructionText;

        public string PointInstructionText => pointInstructionText;

        public void MakeAvailable ()
        {
            OnPointBecameAvailable ();
        }

        private void OnPointBecameAvailable ()
        {
            if (IsInUse)
                return;

            IsInUse = true;
            PointStatusChange?.Invoke (true);
        }

        public void MakeUnavailable ()
        {
            OnPointBecameUnavailable ();
        }

        private void OnPointBecameUnavailable ()
        {
            if (!IsInUse)
                return;

            IsInUse = false;
            PointStatusChange?.Invoke (false);
        }

        public void SetComplete () {}


        private void Reset ()
        {
            //Since scriptable objs won't reset automatically set this to false so next startup won't cause any problems.
            IsInUse = false;
        }
    }
}