using Microsoft.Azure.SpatialAnchors.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsinpa.CloudAnchor
{
    public class LightHouseAnchorMesh : MonoBehaviour
    {
        [SerializeField]
        private MeshFilter meshFilter;

        public Bounds meshBounds => meshFilter.sharedMesh.bounds;

        //public CloudAnchorFireData CloudAnchorFireData = CloudAnchorFireData.GetDefault();

        //public bool dataIsSet => (CloudAnchorFireData != null && CloudAnchorFireData.project_id != GeneralFlag.FirestoreFake.ProjectID_EDITOR);

        private CloudNativeAnchor _CloudNativeAnchor;

        public CloudNativeAnchor CloudNativeAnchor {
            get {
                if (_CloudNativeAnchor == null)
                    _CloudNativeAnchor = this.GetComponent<CloudNativeAnchor>();

                if (_CloudNativeAnchor.CloudAnchor == null) {
                    _CloudNativeAnchor.NativeToCloud();
                }

                return _CloudNativeAnchor;
            }
        }
    }
}
