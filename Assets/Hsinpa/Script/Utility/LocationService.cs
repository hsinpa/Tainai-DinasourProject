using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationService
{
    public static LocationInfo cacheInfo;

    public static void GetGPS(MonoBehaviour mono, bool allowCache, System.Action<LocationInfo> callback) {

        if (allowCache && cacheInfo.isSuccess) {
            callback(cacheInfo);
            return;
        }

        mono.StartCoroutine(StartGPS((bool isSuccess) => {
            LocationInfo location = new LocationInfo();
            location.isSuccess = isSuccess;

            if (isSuccess) {
                location.longitude = Input.location.lastData.longitude;
                location.latitude = Input.location.lastData.latitude;

                cacheInfo = location;
            }

            if (callback != null)
                callback(location);
        }));    
    }

    private static IEnumerator StartGPS(System.Action<bool> callback)
    {
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser) {
            callback(false);
            yield break;
        }

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 4;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            Debug.LogError("Timed out");
            callback(false);
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Unable to determine device location");
            callback(false);
            yield break;
        }
        else
        {
            // Access granted and location value could be retrieved
            Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
            callback(true);
        }
    }

    public struct LocationInfo {
        public float latitude;
        public float longitude;

        public bool isSuccess;
    }
    
}
