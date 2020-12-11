using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using LightHouse.Edit;
using Utility;
using UnityEngine.InputSystem.EnhancedTouch;

namespace Hsinpa.GameInput
{
    public class RaycastInputHandler : MonoBehaviour
    {
        [SerializeField]
        private ARRaycastManager arRaycastManager;

        [SerializeField]
        private ARCameraManager arCamera;
        private Camera _camera;

        [SerializeField]
        private InputEditMode _lightHouseEditMode;
        public InputEditMode lightHouseEditMode => _lightHouseEditMode;

        [SerializeField, Range(0.05f, 1f)]
        private float doubleTapTreshold = 0.2f;
        private float timeRecord;
        private int tapCount = 0;

        private PointerEventData eventData;
        private List<RaycastResult> raycastResults = new List<RaycastResult>();
        private List<ARRaycastHit> arRaycastResults = new List<ARRaycastHit>();
        private RaycastHit[] anchorHits = new RaycastHit[1];
        private GameObject selectedAnchor;

        public enum InputType { SingleTap, DoubleTap, None };
        public System.Action<InputStruct> OnInputEvent;

        private InputStruct _inputStruct;

        private void Start()
        {
            this._camera = arCamera.GetComponent<Camera>();
            eventData = new PointerEventData(EventSystem.current);
            _inputStruct = new InputStruct();
            lightHouseEditMode.SetUp(_camera);

            EnhancedTouchSupport.Enable();
            TouchSimulation.Enable();
        }

        public Quaternion GetFrontQuaternion(Vector3 hitPoint, Vector3 offset)
        {
            var cameraDir = (arCamera.transform.position - hitPoint).normalized;
            cameraDir.y = 0;

            var quaRot = Quaternion.LookRotation(cameraDir);
            return Quaternion.Euler(quaRot.eulerAngles + offset);
        }

        private void Update()
        {
            if (selectedAnchor != null) {

                var touches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;
                int touchCount = touches.Count;

                //UI AND ARPLANE Detection
                if ((UnityEngine.Input.GetMouseButtonDown(0) || 
                    (touchCount == 1 && touches[0].phase == UnityEngine.InputSystem.TouchPhase.Began)) && 
                    CheckIsDoubleTabActivate())
                {

                    Debug.Log("Double click");
                    _inputStruct.raycastPosition = Vector3.zero;
                    _inputStruct.inputType = InputType.DoubleTap;

                    if (OnInputEvent != null)
                        OnInputEvent(_inputStruct);
                    return;
                }

                _lightHouseEditMode.OnUpdate(touches, touchCount);

                return;
            }

            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                //EXIST AR ANCHOR Detection
                _inputStruct = CheckCastOnExistAnchor();
                if (_inputStruct.inputType == InputType.SingleTap) {

                    Debug.Log("Hit on anchor");
                    _lightHouseEditMode.SetTargetAnchor(_inputStruct.gameObject);

                    if (OnInputEvent != null)
                    {
                        OnInputEvent(_inputStruct);
                    }

                    selectedAnchor = _inputStruct.gameObject;
                    return;
                }

                if (CheckNoUIIsActivate())
                {
                    return;
                }

                _inputStruct = CheckPlaneIsCast();
                if (_inputStruct.inputType == InputType.SingleTap && OnInputEvent != null)
                {
                    OnInputEvent(_inputStruct);
                }
            }
        }

        private bool CheckIsDoubleTabActivate()
        {
            if (Time.time > timeRecord)
            {
                tapCount = 1;
                timeRecord = doubleTapTreshold + Time.time;
            }
            else
            {
                tapCount++;
            }

            if (tapCount >= 2)
            {
                tapCount = 0;
                timeRecord = 0;
                return true;
            }

            return false;
        }

        private bool CheckNoUIIsActivate()
        {
            raycastResults.Clear();

            eventData.position = UnityEngine.Input.mousePosition;
            EventSystem.current.RaycastAll(eventData, raycastResults);
            return raycastResults.Count > 0;
        }

        private InputStruct CheckPlaneIsCast() {
            _inputStruct.inputType = InputType.None;

#if UNITY_EDITOR
            Ray ray = _camera.ScreenPointToRay(UnityEngine.Input.mousePosition);

            int hitCount = Physics.RaycastNonAlloc(ray, anchorHits, 100, GeneralFlag.Layer.Plane);
            if (hitCount > 0) {
                _inputStruct.inputType = InputType.SingleTap;
                _inputStruct.raycastPosition = anchorHits[0].point;
                _inputStruct.gameObject = anchorHits[0].collider.gameObject;
            }
#else
            arRaycastResults.Clear();
            bool hasHit = arRaycastManager.Raycast(UnityEngine.Input.mousePosition, arRaycastResults, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon);
            if (hasHit) {
                _inputStruct.inputType = InputType.SingleTap;
                _inputStruct.raycastPosition = arRaycastResults[0].pose.position;
                _inputStruct.gameObject = arRaycastResults[0].trackable.gameObject;
            }
#endif

            return _inputStruct;
        }

        private InputStruct CheckCastOnExistAnchor()
        {
            Ray ray = _camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
            _inputStruct.inputType = InputType.None;

            int hitCount = Physics.RaycastNonAlloc(ray, anchorHits, 100, GeneralFlag.Layer.ARDetectable);

            if (hitCount > 0)
            {
                _inputStruct.inputType = InputType.SingleTap;
                _inputStruct.raycastPosition = anchorHits[0].point;
                _inputStruct.gameObject = anchorHits[0].collider.gameObject;
            }

            return _inputStruct;
        }

        public void ResetRaycaster() {
            selectedAnchor = null;
        }

        public struct InputStruct
        {
            public Vector3 raycastPosition;
            public RaycastInputHandler.InputType inputType;
            public GameObject gameObject;

            public InputStruct(Vector3 p_pos, RaycastInputHandler.InputType p_inputType, GameObject p_gameObject)
            {
                this.raycastPosition = p_pos;
                this.inputType = p_inputType;
                this.gameObject = p_gameObject;
            }
        }
    }
}