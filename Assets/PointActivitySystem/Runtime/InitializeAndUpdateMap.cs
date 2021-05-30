using System.Collections;
using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;

/// <summary>
/// This is a modified version of mapboxes Initialize Map With Location Provider-script.
/// It makes it possible to show activity points on top of mapbox map.
/// </summary>
namespace PointActivitySystem.Runtime
{
    public class InitializeAndUpdateMap : MonoBehaviour
    {
        public static InitializeAndUpdateMap MapInstance = null;

        [SerializeField] private AbstractMap map = null;

        private ILocationProvider locationProvider;

        public ILocationProvider LocationProvider => locationProvider ??
                                                     (locationProvider = LocationProviderFactory.Instance
                                                         .DefaultLocationProvider);

        private Vector3 targetPosition = Vector3.zero;
        public bool IsMapInitialized { get; private set; }

        /// <summary>
        /// The time taken to move from the start to finish positions
        /// </summary>
        public float timeTakenDuringLerp = 1f;

        //Whether we are currently interpolating or not
        private bool isLerping = false;

        //The start and finish positions for the interpolation
        private Vector3 startPosition = Vector3.zero;
        private Vector3 endPosition = Vector3.zero;

        private Vector2d startLatLong = Vector2d.zero;
        private Vector2d endLatlong = Vector2d.zero;

        //The Time.time value when we started the interpolation
        private float timeStartedLerping = 0;

        private void Awake()
        {
            if (MapInstance == null)
                MapInstance = this;
            else
                Destroy(this);


            // Prevent double initialization of the map. 
            map.InitializeOnStart = false;
        }

        private IEnumerator Start()
        {
            //wait one frame to make sure that location provider is ready 
            yield return null;
            locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
            locationProvider.OnLocationUpdated += LocationProvider_OnFirstLocationUpdate;
        }

        /// <summary>
        /// Called first time when player location is updated
        /// </summary>
        /// <param name="location"></param>
        private void LocationProvider_OnFirstLocationUpdate(Location location)
        {
            //unsubscribe listener to prevent multiple subscriptions
            locationProvider.OnLocationUpdated -= LocationProvider_OnFirstLocationUpdate;

            map.OnInitialized += () =>
            {
                IsMapInitialized = true;


                //subscribe again
                locationProvider.OnLocationUpdated += LocationProvider_OnLocationUpdated;
            };
            map.Initialize(location.LatitudeLongitude, map.AbsoluteZoom);
        }

        /// <summary>
        /// Called everytime player location is updated
        /// </summary>
        /// <param name="location"></param>
        private void LocationProvider_OnLocationUpdated(Location location)
        {
            if (IsMapInitialized && location.IsLocationUpdated)
            {
                StartLerping(location);
            }
        }

        /// <summary>
        /// Called to begin the linear interpolation
        /// </summary>
        private void StartLerping(Location location)
        {
            isLerping = true;
            timeStartedLerping = Time.time;
            timeTakenDuringLerp = Time.deltaTime;

            //We set the start position to the current position
            startLatLong = map.CenterLatitudeLongitude;
            endLatlong = location.LatitudeLongitude;
            startPosition = map.GeoToWorldPosition(startLatLong, false);
            endPosition = map.GeoToWorldPosition(endLatlong, false);
        }

        //We do the actual interpolation in FixedUpdate(), since we're dealing with a rigidbody
        void LateUpdate()
        {
            if (IsMapInitialized && isLerping)
            {
                //We want percentage = 0.0 when Time.time = _timeStartedLerping
                //and percentage = 1.0 when Time.time = _timeStartedLerping + timeTakenDuringLerp
                //In other words, we want to know what percentage of "timeTakenDuringLerp" the value
                //"Time.time - _timeStartedLerping" is.
                float timeSinceStarted = Time.time - timeStartedLerping;
                float percentageComplete = timeSinceStarted / timeTakenDuringLerp;

                //Perform the actual lerping.  Notice that the first two parameters will always be the same
                //throughout a single lerp-processs (ie. they won't change until we hit the space-bar again
                //to start another lerp)
                startPosition = map.GeoToWorldPosition(startLatLong, false);
                endPosition = map.GeoToWorldPosition(endLatlong, false);
                var position = Vector3.Lerp(startPosition, endPosition, percentageComplete);
                var latLong = map.WorldToGeoPosition(position);
                map.UpdateMap(latLong, map.Zoom);

                //When we've completed the lerp, we set _isLerping to false
                if (percentageComplete >= 1.0f)
                {
                    isLerping = false;
                }
            }
        }
    }
}