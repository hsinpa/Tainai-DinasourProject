using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.SpatialAnchors;
using Microsoft.Azure.SpatialAnchors.Unity;
using UnityEngine.XR.ARFoundation;
using System.Threading.Tasks;
using System;
using Microsoft.Azure;

namespace Hsinpa.CloudAnchor {
    public class LightHouseAnchorManager : MonoBehaviour
    {
        [SerializeField]
        private SpatialAnchorManager _spatialAnchorManager;
        public SpatialAnchorManager CloudManager => _spatialAnchorManager;

        [SerializeField]
        private LightHouseAnchorMesh _AnchoredObjectPrefab;
        public LightHouseAnchorMesh AnchoredObjectPrefab => this._AnchoredObjectPrefab;

        protected List<string> anchorIdsToLocate = new List<string>();

        protected CloudSpatialAnchor currentCloudAnchor;
        protected CloudSpatialAnchorWatcher currentWatcher;
        private PlatformLocationProvider platformLocationProvider;
        private float _createProgress;
        public float createProgress {

            get {
                return _createProgress;
            }

            set
            {
                if (OnCreateProgressUpdate != null)
                    OnCreateProgressUpdate(value);

                _createProgress = value;
            }
        }

        #region Public Events
        public System.Action<string> OnLogEvent;
        public System.Action<bool> OnCloudAnchorIsSetUp;
        public System.Action<float> OnCreateProgressUpdate;
        public System.Action<AnchorLocatedEventArgs> OnAnchorIsLocated;

        #endregion

        private void Update()
        {
            if (CloudManager.SessionStatus != null)
            {
                createProgress = CloudManager.SessionStatus.RecommendedForCreateProgress;
            }

            //Debug.Log("_createProgress " + _createProgress);
        }

        public async void SetUp() {

            try
            {
                bool hasPassCheck = SanityCheckAccessConfiguration();

                if (hasPassCheck)
                {
                    CloudManager.SessionUpdated += CloudManager_SessionUpdated;
                    CloudManager.AnchorLocated += CloudManager_AnchorLocated;
                    CloudManager.LocateAnchorsCompleted += CloudManager_LocateAnchorsCompleted;
                    CloudManager.LogDebug += CloudManager_LogDebug;
                    CloudManager.Error += CloudManager_Error;

                    await CloudManager.CreateSessionAsync();

                    //await CloudManager.StartSessionAsync();
                    
                    platformLocationProvider = new PlatformLocationProvider();

                    CloudManager.Session.LocationProvider = platformLocationProvider;

                    SensorPermissionHelper.RequestSensorPermissions();
                    ConfigureSensors();

                    if (OnCloudAnchorIsSetUp != null)
                        OnCloudAnchorIsSetUp(true);

                    return;
                }
                else {
                    if (OnCloudAnchorIsSetUp != null)
                        OnCloudAnchorIsSetUp(false);
                }
            }
            catch {
                Debug.Log("No Azure Spatial allow");

                if (OnCloudAnchorIsSetUp != null)
                    OnCloudAnchorIsSetUp(false);
            }
        }

        public bool SanityCheckAccessConfiguration()
        {
            if (string.IsNullOrWhiteSpace(CloudManager.SpatialAnchorsAccountId)
                || string.IsNullOrWhiteSpace(CloudManager.SpatialAnchorsAccountKey)
                || string.IsNullOrWhiteSpace(CloudManager.SpatialAnchorsAccountDomain))
            {
                return false;
            }

            return true;
        }

        private void ConfigureSensors() {
            //platformLocationProvider.Sensors.GeoLocationEnabled = SensorPermissionHelper.HasGeoLocationPermission();
            platformLocationProvider.Sensors.WifiEnabled = SensorPermissionHelper.HasWifiPermission();
            //platformLocationProvider.Sensors.BluetoothEnabled = SensorPermissionHelper.HasBluetoothPermission();
        }

        #region Watch Cloud Anchor
        public CloudSpatialAnchorWatcher CreateWatcher(AnchorLocateCriteria anchorLocateCriteria)
        {
            if ((CloudManager != null) && (CloudManager.Session != null))
            {
                return CloudManager.Session.CreateWatcher(anchorLocateCriteria);
            }
            else
            {
                return null;
            }
        }

        public AnchorLocateCriteria GetAnchorCriteria(string[] defaultAnchorIds, LocateStrategy locateStrategy)
        {
            AnchorLocateCriteria anchorLocateCriteria = new AnchorLocateCriteria();

            anchorLocateCriteria.Strategy = locateStrategy;

            anchorLocateCriteria.Identifiers = defaultAnchorIds;

            return anchorLocateCriteria;
        }

        public AnchorLocateCriteria SetNearbyAnchor(AnchorLocateCriteria anchorLocateCriteria, CloudSpatialAnchor nearbyAnchor, float DistanceInMeters, int MaxNearAnchorsToFind)
        {
            NearAnchorCriteria nac = new NearAnchorCriteria();
            nac.SourceAnchor = nearbyAnchor;
            nac.DistanceInMeters = DistanceInMeters;
            nac.MaxResultCount = MaxNearAnchorsToFind;

            anchorLocateCriteria.NearAnchor = nac;

            return anchorLocateCriteria;
        }

        public AnchorLocateCriteria SetAnchorCriteriaIDs(AnchorLocateCriteria anchorLocateCriteria, string[] anchorIds)
        {
            anchorLocateCriteria.Identifiers = anchorIds;

            return anchorLocateCriteria;
        }

        #endregion

        #region Save, Move&Edit, Anchor point
        /// <summary>
        /// Spawns a new anchored object.
        /// </summary>
        /// <param name="worldPos">The world position.</param>
        /// <param name="worldRot">The world rotation.</param>
        /// <returns><see cref="GameObject"/>.</returns>
        public GameObject SpawnNewAnchoredObject(Vector3 worldPos, Quaternion worldRot)
        {
            // Create the prefab
            GameObject newGameObject = GameObject.Instantiate(_AnchoredObjectPrefab.gameObject, worldPos, worldRot);

            // Attach a cloud-native anchor behavior to help keep cloud
            // and native anchors in sync.
            newGameObject.AddComponent<CloudNativeAnchor>();

            // Return created object
            return newGameObject;
        }

        /// <summary>
        /// Spawns a new object.
        /// </summary>
        /// <param name="worldPos">The world position.</param>
        /// <param name="worldRot">The world rotation.</param>
        /// <param name="cloudSpatialAnchor">The cloud spatial anchor.</param>
        /// <returns><see cref="GameObject"/>.</returns>
        public GameObject SpawnNewAnchoredObject(Vector3 worldPos, Quaternion worldRot, CloudSpatialAnchor cloudSpatialAnchor)
        {
            // Create the object like usual
            GameObject newGameObject = SpawnNewAnchoredObject(worldPos, worldRot);

            // If a cloud anchor is passed, apply it to the native anchor
            if (cloudSpatialAnchor != null)
            {
                CloudNativeAnchor cloudNativeAnchor = newGameObject.GetComponent<CloudNativeAnchor>();
                cloudNativeAnchor.CloudToNative(cloudSpatialAnchor);
            }

            // Return newly created object
            return newGameObject;
        }

        /// <summary>
        /// Moves the specified anchored object.
        /// </summary>
        /// <param name="objectToMove">The anchored object to move.</param>
        /// <param name="worldPos">The world position.</param>
        /// <param name="worldRot">The world rotation.</param>
        /// <param name="cloudSpatialAnchor">The cloud spatial anchor.</param>
        public void MoveAnchoredObject(GameObject objectToMove, Vector3 worldPos, Quaternion worldRot, CloudSpatialAnchor cloudSpatialAnchor = null)
        {
            // Get the cloud-native anchor behavior
            CloudNativeAnchor cna = objectToMove.GetComponent<CloudNativeAnchor>();

            if (cloudSpatialAnchor == null) {
                cloudSpatialAnchor = cna.CloudAnchor;
            }

            // Warn and exit if the behavior is missing
            if (cna == null)
            {
                Debug.LogWarning($"The object {objectToMove.name} is missing the {nameof(CloudNativeAnchor)} behavior.");
                return;
            }

            // Is there a cloud anchor to apply
            if (cloudSpatialAnchor != null)
            {
                // Yes. Apply the cloud anchor, which also sets the pose.
                cna.CloudToNative(cloudSpatialAnchor);
            }
            else
            {
                // No. Just set the pose.
                cna.SetPose(worldPos, worldRot);
            }
        }

        public async Task RemoveCloudAnchor(CloudSpatialAnchor cloudSpatialAnchor) {
            await CloudManager.DeleteAnchorAsync(cloudSpatialAnchor);
        }

        /// <summary>
        /// Saves the current object anchor to the cloud.
        /// </summary>
        public async Task SaveCurrentObjectAnchorToCloudAsync(CloudNativeAnchor nativeAnchor)
        {
            if (!CloudManager.IsSessionStarted) {
                await CloudManager.StartSessionAsync();
            }

            //// Get the cloud-native anchor behavior
            //CloudNativeAnchor cna = spawnedObject.GetComponent<CloudNativeAnchor>();

            //// If the cloud portion of the anchor hasn't been created yet, create it
            if (nativeAnchor.CloudAnchor == null) { nativeAnchor.NativeToCloud(); }
            
            // Get the cloud portion of the anchor
            CloudSpatialAnchor cloudAnchor = nativeAnchor.CloudAnchor;

            // In this sample app we delete the cloud anchor explicitly, but here we show how to set an anchor to expire automatically
            cloudAnchor.Expiration = System.DateTimeOffset.Now.AddDays(7);

            while (!CloudManager.IsReadyForCreate)
            {
                await Task.Delay(330);
                float createProgress = CloudManager.SessionStatus.RecommendedForCreateProgress;
                Debug.Log($"Move your device to capture more environment data: {createProgress:0%}");
            }

            bool success = false;

            Debug.Log("Saving...");

            try
            {
                // Actually save
                await CloudManager.CreateAnchorAsync(cloudAnchor);

                // Store
                currentCloudAnchor = cloudAnchor;

                // Success?
                success = currentCloudAnchor != null;

                if (success)
                {
                    // Await override, which may perform additional tasks
                    // such as storing the key in the AnchorExchanger
                    OnSaveCloudAnchorSuccessfulAsync();
                }
                else
                {
                    OnSaveCloudAnchorFailed(new Exception("Failed to save, but no exception was thrown."));
                }
            }
            catch (Exception ex)
            {
                OnSaveCloudAnchorFailed(ex);
            }
        }
        #endregion

        #region Listen SaveCloudAnchor Events
        /// <summary>
        /// Called when a cloud anchor is not saved successfully.
        /// </summary>
        /// <param name="exception">The exception.</param>
        protected void OnSaveCloudAnchorFailed(Exception exception)
        {
            // we will block the next step to show the exception message in the UI.
            Debug.LogException(exception);
            Debug.Log("Failed to save anchor " + exception.ToString());

            UnityDispatcher.InvokeOnAppThread(() => {

            });
        }

        /// <summary>
        /// Called when a cloud anchor is saved successfully.
        /// </summary>
        protected void OnSaveCloudAnchorSuccessfulAsync()
        {

        }
        #endregion

        #region Listen CloudManager Events

        private void CloudManager_LocateAnchorsCompleted(object sender, LocateAnchorsCompletedEventArgs args)
        {
            // OnCloudLocateAnchorsCompleted(args);
        }

        private void CloudManager_SessionUpdated(object sender, SessionUpdatedEventArgs args)
        {
            //OnCloudSessionUpdated();

            var status = args.Status;
            if (status.UserFeedback == SessionUserFeedback.None) return;
            Debug.Log($"Feedback: {System.Enum.GetName(typeof(SessionUserFeedback), status.UserFeedback)} -" +
                $" Recommend Create={status.RecommendedForCreateProgress: 0.#%}");
        }

        private void CloudManager_Error(object sender, SessionErrorEventArgs args)
        {
            Debug.Log(args.ErrorMessage);
        }

        private void CloudManager_LogDebug(object sender, OnLogDebugEventArgs args)
        {
            //Debug.Log(args.Message);
        }

        #endregion

        #region Listen Watch Events
        private void CloudManager_AnchorLocated(object sender, AnchorLocatedEventArgs args)
        {
            Debug.LogFormat("Anchor recognized as a possible anchor {0} {1}", args.Identifier, args.Status);

            if (OnAnchorIsLocated != null)
                OnAnchorIsLocated(args);
        }
        #endregion
    }
}