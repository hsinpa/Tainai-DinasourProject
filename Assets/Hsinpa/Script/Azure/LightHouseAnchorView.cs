using Microsoft.Azure.SpatialAnchors;
using Microsoft.Azure.SpatialAnchors.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;

namespace Hsinpa.CloudAnchor
{
    public class LightHouseAnchorView : ObserverPattern.Observer
    {
        [SerializeField]
        private LightHouseAnchorManager lightHouseAnchorManager;

        [SerializeField]
        private TempAnchorDataSRP tempAnchorDataSRP;

        [SerializeField, Range(1, 10)]
        private int numToMake = 5;

        [SerializeField, Range(1, 30)]
        private int rangeToSearch = 15;

        [SerializeField]
        private Transform anchorWorldHolder;

        private List<LightHouseAnchorMesh> anchorMeshList = new List<LightHouseAnchorMesh>();
        private List<TempAnchorDataSRP.AnchorStruct> anchorStructs = new List<TempAnchorDataSRP.AnchorStruct>();

        public System.Action<Vector3, Quaternion> OnMainAnchorDetectEvent;

        private AnchorLocateCriteria _anchorLocateCriteria;
        private CloudSpatialAnchorWatcher _cloudWatcher;
        private int anchorFoundLength;

        public override void OnNotify(string p_event, params object[] p_objects)
        {
            switch (p_event)
            {
                case EventFlag.Event.GameStart:
                    {
                        if (p_objects.Length == 1 && (bool)p_objects[0])
                            SetUp();
                    }
                    break;
            }
        }

        private void SetUp() {
            lightHouseAnchorManager.CloudManager.AnchorLocated += CloudManager_AnchorLocated;
            lightHouseAnchorManager.CloudManager.LocateAnchorsCompleted += CloudManager_LocateAnchorsCompleted;
        }

        public async Task StartWatcher(string mission_id)
        {
            List<TempAnchorDataSRP.AnchorStruct> anchorStructs = tempAnchorDataSRP.FindAnchorsByMissionID(mission_id);
            string[] criteriaIDs = anchorStructs.Select(x => x._id).ToArray();

            foreach (string cid in criteriaIDs)
                Debug.Log("Anchor " + cid);

            _anchorLocateCriteria = lightHouseAnchorManager.GetAnchorCriteria(criteriaIDs, LocateStrategy.AnyStrategy);

            if (!lightHouseAnchorManager.CloudManager.IsSessionStarted) {
                await Task.Delay(2000);
                await lightHouseAnchorManager.CloudManager.StartSessionAsync();
            }

            _cloudWatcher = lightHouseAnchorManager.CreateWatcher(_anchorLocateCriteria);  
        }

        private void CloudManager_AnchorLocated(object sender, AnchorLocatedEventArgs args)
        {
            Debug.LogFormat("Anchor recognized as a possible anchor {0} {1}", args.Identifier, args.Status);
            if (args.Status == LocateAnchorStatus.Located && CheckAnchorDuplication(args.Identifier) && anchorStructs != null)
            {
                UnityDispatcher.InvokeOnAppThread(() =>
                {
                    var currentCloudAnchor = args.Anchor;
                    Pose anchorPose = Pose.identity;

                    var anchorStruct = anchorStructs.Find(x => x._id == currentCloudAnchor.Identifier);

                    if (anchorStruct.anchorType == TempAnchorDataSRP.AnchorType.Main && OnMainAnchorDetectEvent != null) {
                        OnMainAnchorDetectEvent(anchorPose.position, anchorPose.rotation);
                    }

                    anchorFoundLength++;

#if UNITY_ANDROID || UNITY_IOS
                    anchorPose = currentCloudAnchor.GetPose();
#endif
                    //var spawnObject = lightHouseAnchorManager.SpawnNewAnchoredObject(anchorPose.position, anchorPose.rotation);
                    //LightHouseAnchorMesh anchorMesh = spawnObject.GetComponent<LightHouseAnchorMesh>();

                    //RegisterNewAnchorMesh(anchorMesh);
                });
            }
        }


        private void CloudManager_LocateAnchorsCompleted(object sender, LocateAnchorsCompletedEventArgs args)
        {
            // OnCloudLocateAnchorsCompleted(args);
        }

        public bool CheckAnchorDuplication(string p_id) {
            int i = anchorMeshList.FindIndex(x => x.name == p_id);
            return (i < 0);
        }

        public int RegisterNewAnchorMesh(LightHouseAnchorMesh anchorMesh) {
            anchorMeshList.Add(anchorMesh);
            anchorMesh.transform.SetParent(anchorWorldHolder);

            return anchorMeshList.Count;
        }

        public bool RemoveAnchorMesh(string anchorID) {
            int i = anchorMeshList.FindIndex(x => x.name == anchorID);

            if (i >= 0) {
                var deleteMesh = anchorMeshList[i];
                anchorMeshList.RemoveAt(i);
                Utility.UtilityMethod.SafeDestroy(deleteMesh.gameObject);
            }

            return i >= 0;
        }

    }
}