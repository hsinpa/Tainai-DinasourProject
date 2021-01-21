using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Hsinpa.Ctrl
{
    public class ARFoundationHelper : MonoBehaviour
    {
        private ARSession _arSession;
        public ARSession arSession => _arSession;

        private ARPlaneManager _arPlaneManager;
        public ARPlaneManager arPlaneManager => _arPlaneManager;

        private ARCameraManager _arCamera;
        public ARCameraManager arCamera => _arCamera;

        private ARRaycastManager _arRaycast;
        public ARRaycastManager arRaycast => _arRaycast;


        private ARPoseDriver _arPoseDriver;
        public ARPoseDriver arPoseDriver {
            get {
                if (_arPoseDriver == null)
                    _arPoseDriver = _arCamera.GetComponent<ARPoseDriver>();

                return _arPoseDriver;
            }
        }

        private List<ARPlane> arplanes = new List<ARPlane>();

        public void Awake()
        {
            _arSession = GameObject.FindObjectOfType<ARSession>();
            _arPlaneManager = GameObject.FindObjectOfType<ARPlaneManager>();
            _arCamera = GameObject.FindObjectOfType<ARCameraManager>();
            _arRaycast = _arPlaneManager.GetComponent<ARRaycastManager>();


            _arPlaneManager.planesChanged += OnARPlaneChange;
        }

        private void OnARPlaneChange(ARPlanesChangedEventArgs p_arPlanesChangedEventArgs)
        {
            if (p_arPlanesChangedEventArgs.added != null)
                arplanes.AddRange(p_arPlanesChangedEventArgs.added);
        }

        public void ActivateAR(bool activate) {
            arPoseDriver.enabled = activate;
            _arCamera.enabled = activate;
            _arSession.enabled = activate;
        }

        public void ActivateFullAR(bool activate) {
            ActivateAR(activate);
            AcitvateARPlane(activate);
        }

        public void AcitvateARPlane(bool p_active) {
            try
            {
                foreach (var plane in arplanes)
                {
                    if (plane != null && plane.gameObject != null)
                        plane.gameObject.SetActive(p_active);
                }
            } catch
            {

            }

            _arPlaneManager.enabled = p_active;
        }

        public void SetARCameraPos(Vector3 position, Quaternion quaternion)
        {
            _arCamera.transform.rotation = quaternion;
            _arCamera.transform.position = position;
        }

    }
}