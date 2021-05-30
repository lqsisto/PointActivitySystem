using System.Collections.Generic;
using PointActivitySystem.Runtime;
using UnityEngine;
using UnityEngine.Serialization;

namespace PointActivitySystem
{


[CreateAssetMenu(fileName = "Point Database", menuName = "Point Database/New Point Database")]
    public class PointDatabase : ScriptableObject
    {
        [SerializeField] private List<PointActivity> activityPoints = new List<PointActivity>();
        public PointActivity GetPoint(int pointIndex)
        {
            return activityPoints[pointIndex];
        }
    }
}
