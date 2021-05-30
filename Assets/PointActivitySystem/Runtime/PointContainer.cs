using System.Collections.Generic;
using Mapbox.Unity.Location;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using UnityEngine;

namespace PointActivitySystem.Runtime
{
	public class PointContainer : MonoBehaviour
	{
		[SerializeField] private List <PointActivity> availablePoints = new List <PointActivity> ();


		[Space, SerializeField]
		private List <PointActivity> pointsInsideDistanceThreshold = new List <PointActivity> ();


		[Space, SerializeField, Tooltip ("threshold where player can activate point")]
		private float distanceThreshold = 0.000128f;

		[SerializeField] private float pointRefreshRate = 1;
		private float refreshRate;
		public float DistanceThreshold => distanceThreshold;

		private ILocationProvider locationProvider;


		private void Start ()
		{
			locationProvider = InitializeAndUpdateMap.MapInstance.LocationProvider;

			refreshRate = pointRefreshRate;
		}


		private void Update ()
		{

			refreshRate -= Time.deltaTime;

			if (refreshRate > 0)
				return;

			GetClosestPoints ();
			refreshRate = pointRefreshRate;
		}

		/// <summary>
		/// Loop through possible points and check if player is close enough for them to be activated.
		/// </summary>
		/// <returns></returns>
		private void GetClosestPoints ()
		{
			var playerLocation = locationProvider.CurrentLocation.LatitudeLongitude;

			if (availablePoints.Count == 0)
			{
				Debug.LogError ("COULD NOT FOUND ANY POINTS. YOU PROBABLY NEED TO ASSIGN THEM IN EDITOR", this);
				return;
			}

			RemovePointsFromListOutsideThreshold (playerLocation);
			AddPointsInsideThresholdToList (playerLocation);

			if (pointsInsideDistanceThreshold.Count <= 0)
				return;


			foreach (var point in pointsInsideDistanceThreshold)
			{
				point.MakeAvailable ();
			}
		}


		#region HelperFunctions

		/// <summary>
		///  Remove points from list that are too far away from player
		/// </summary>
		/// <param name="playerLocation"></param>
		private void RemovePointsFromListOutsideThreshold (Vector2d playerLocation)
		{
			if (pointsInsideDistanceThreshold.Count <= 0)
			{
				//no point close
				return;
			}

			for (var i = 0; i < pointsInsideDistanceThreshold.Count; i++)
			{
				var point = pointsInsideDistanceThreshold [i];

				//If point has no locations move to next
				if (point.Locations.Count <= 0)
					continue;


				var nearestPoint = Vector2d.zero;
				var nearestLocation = double.MaxValue;

				//Find point location that is closest to the player.
				//Then check if it is outside threshold.
				for (var j = 0; j < point.Locations.Count; j++)
				{
					var coord = Conversions.StringToLatLon(point.Locations[j]);


					if (Vector2d.Distance (playerLocation, coord) < nearestLocation)
					{
						nearestLocation = Vector2d.Distance (playerLocation, coord);
						nearestPoint = coord;
					}
				}

				if (Vector2d.Distance (playerLocation, nearestPoint) > distanceThreshold)
				{
					point.MakeUnavailable ();
					pointsInsideDistanceThreshold.Remove (point);
				}
			}
		}

		/// <summary>
		/// Add point to the list if one of points locations is inside threshold
		/// </summary>
		/// <param name="playerLocation"></param>
		private void AddPointsInsideThresholdToList (Vector2d playerLocation)
		{
			for (var i = 0; i < availablePoints.Count; i++)
			{
				for (var j = 0; j < availablePoints [i].Locations.Count; j++)
				{
					var loc = Conversions.StringToLatLon(availablePoints [i].Locations [j]);

					if (Vector2d.Distance (playerLocation, loc) > distanceThreshold)
						continue;

					if (!pointsInsideDistanceThreshold.Contains (availablePoints [i]))
						pointsInsideDistanceThreshold.Add (availablePoints [i]);
				}
			}
		}

		#endregion
	}
}