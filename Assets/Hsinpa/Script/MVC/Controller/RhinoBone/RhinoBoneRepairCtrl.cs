using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hsinpa.GameInput;
using Hsinpa.View;
using UnityEngine.XR.ARFoundation;
using System.Linq;
using Hsinpa.Other;
using UnityEngine.Rendering;
using Hsinpa.CloudAnchor;
using System.Threading.Tasks;

namespace Hsinpa.Ctrl {

    public class RhinoBoneRepairCtrl : ObserverPattern.Observer
    {

        #region Parameter
        [SerializeField]
        private Transform _worldContainer;

        [SerializeField]
        private ARFoundationHelper _arHelper;

        [SerializeField]
        private LightHouseAnchorView _lighthouseAnchorView;

        [SerializeField]
        private RaycastInputHandler _raycastInputHandler;

        [SerializeField]
        private BoneARTemplate correctBoneTemplate;

        [SerializeField]
        private DinosaurBoneSRP boneRandomSetSRP;

        [SerializeField]
        private ColorSRP colorLookupTable;

        [SerializeField, Range(-1, 1)]
        private float _angleThreshold;

        [SerializeField, Range(0, 3)]
        private float _distThreshold;

        private BoneARTemplate spawnCorrectBoneTemplate;
        private BoneARTemplate spawnRandomBoneTemplate;
        private BoneARItem selectedARItem;
        private BoneARItem targetARItem;
        private RhinoBoneHelper _rhinoBoneHelper;

        public GeneralFlag.GeneralState _state = GeneralFlag.GeneralState.Idle;
        private int waitForSecondPlaneARActivate = 15000;
        #endregion

        public override void OnNotify(string p_event, params object[] p_objects)
        {
            switch (p_event)
            {
                case EventFlag.Event.OnARMode_NoAR:
                {
                    PerformNoARAction();
                }
                break;

                case EventFlag.Event.OnARMode_PlaneAR:
                {
                    PerformPlaneARAction();
                }
                break;
            }
        }

        public void Start()
        {
            _rhinoBoneHelper = new RhinoBoneHelper(correctBoneTemplate, boneRandomSetSRP, _worldContainer);
            _rhinoBoneHelper.Clean();
            _raycastInputHandler.OnInputEvent += OnRaycastInputEvent;
            _lighthouseAnchorView.OnMainAnchorDetectEvent += OnAzureAnchorIsFind;
        }

#region State => Preparation Mode
        private void PerformNoARAction() {
            _arHelper.ActivateFullAR(false);
            _arHelper.SetARCameraPos(new Vector3(0, 0.8f, 0), Quaternion.Euler(90, 0, 0));

            _rhinoBoneHelper.Clean();
            spawnCorrectBoneTemplate = _rhinoBoneHelper.CreateBoneTemplate(Vector3.zero, Quaternion.identity);
            spawnRandomBoneTemplate = _rhinoBoneHelper.CreateBoneRandomSet(spawnCorrectBoneTemplate.transform.position, spawnCorrectBoneTemplate.transform.rotation);

            Initialization();
            _state = GeneralFlag.GeneralState.UnderGoing;
        }

        private void PerformPlaneARAction()
        {
            _arHelper.AcitvateARPlane(false);
            _arHelper.ActivateAR(true);
            _ = _lighthouseAnchorView.StartWatcher(GeneralFlag.MissionID.BoneRepairHome);
            _rhinoBoneHelper.Clean();
            
            spawnCorrectBoneTemplate = _rhinoBoneHelper.CreateBoneTemplate(new Vector3(1000, 500, 0), Quaternion.identity);
            spawnRandomBoneTemplate = _rhinoBoneHelper.CreateBoneRandomSet(spawnCorrectBoneTemplate.transform.position, spawnCorrectBoneTemplate.transform.rotation);

            Initialization();
            _ = StartPlaneARIfAnchorNotFound(waitForSecondPlaneARActivate);
            _state = GeneralFlag.GeneralState.Preparation;
        }

        private void OnPlaneARReadyClick() {
            _arHelper.AcitvateARPlane(false);
            spawnCorrectBoneTemplate.ShowConfirmBtn(false);
            _rhinoBoneHelper.MoveBoneTemplate(spawnRandomBoneTemplate, spawnCorrectBoneTemplate.transform.position, spawnCorrectBoneTemplate.transform.rotation);
            _state = GeneralFlag.GeneralState.UnderGoing;
        }

        private void OnAzureAnchorIsFind(Vector3 anchorPos, Quaternion anchorRot) {
            if (_state != GeneralFlag.GeneralState.Preparation) return;

            _rhinoBoneHelper.MoveBoneTemplate(spawnCorrectBoneTemplate, anchorPos, anchorRot);
            _rhinoBoneHelper.MoveBoneTemplate(spawnRandomBoneTemplate, anchorPos, anchorRot);

            _state = GeneralFlag.GeneralState.UnderGoing;
        }

        private async Task StartPlaneARIfAnchorNotFound(int millisecond) {
            await Task.Delay(millisecond);

            if (_state == GeneralFlag.GeneralState.Preparation)
            {
                _arHelper.AcitvateARPlane(true);
            }
        }

        public void Initialization() {
            spawnCorrectBoneTemplate.SetUp(OnPlaneARReadyClick);
            spawnCorrectBoneTemplate.SetColorAllBones(GeneralFlag.BoneType.TemplateIdle, colorLookupTable.GetColor(GeneralFlag.BoneType.TemplateIdle).color);

            spawnRandomBoneTemplate.SetUp(null);
            spawnRandomBoneTemplate.SetColorAllBones(GeneralFlag.BoneType.Idle, colorLookupTable.GetColor(GeneralFlag.BoneType.Idle).color);
        }

#endregion
        private void OnRaycastInputEvent(RaycastInputHandler.InputStruct inputStruct) {
            
            switch (inputStruct.inputType) {
                case RaycastInputHandler.InputType.SingleTap: {
                        OnSingleTap(inputStruct);
                    }
                    break;
                case RaycastInputHandler.InputType.DoubleTap:
                    {
                        OnDoubleTap();
                    }
                    break;
            }
        }

        private void OnSingleTap(RaycastInputHandler.InputStruct inputStruct) {
            if (spawnCorrectBoneTemplate == null) return;

            selectedARItem = inputStruct.gameObject.GetComponent<BoneARItem>();

            if (selectedARItem != null) {
                selectedARItem.boneType = GeneralFlag.BoneType.Selected;
                selectedARItem.SetColor(colorLookupTable.GetColor(GeneralFlag.BoneType.Selected).color);

                targetARItem = spawnCorrectBoneTemplate.GetItemByName(selectedARItem.name);
                targetARItem.SetColor(colorLookupTable.GetColor(GeneralFlag.BoneType.TemplateHint).color);
                return;
            }

            if (inputStruct.gameObject.layer == GeneralFlag.Layer.PlaneInt && _state == GeneralFlag.GeneralState.Preparation)
            {
                Vector3 dir = (inputStruct.raycastPosition - _arHelper.arCamera.transform.position).normalized;
                dir.y = 0;
                Quaternion faceQuat = Quaternion.LookRotation(dir);

                _rhinoBoneHelper.MoveBoneTemplate(spawnCorrectBoneTemplate, inputStruct.raycastPosition, faceQuat);
                spawnCorrectBoneTemplate.ShowConfirmBtn(true);
            }
        }

        private void OnDoubleTap()
        {
            if (selectedARItem == null) return;

            _raycastInputHandler.ResetRaycaster();

            if (selectedARItem.boneType != GeneralFlag.BoneType.Locked) {
                selectedARItem.SetColor(colorLookupTable.GetColor(GeneralFlag.BoneType.Idle).color);

                targetARItem.SetColor(colorLookupTable.GetColor(GeneralFlag.BoneType.TemplateIdle).color);
            }

            selectedARItem = null;
        }

        private void Update()
        {
            if (_state != GeneralFlag.GeneralState.UnderGoing || selectedARItem == null) return;

            var targetBone = spawnCorrectBoneTemplate.GetItemByName(selectedARItem.name);

            if (targetBone == null) return;

            bool thresHoldMeet = CheckTargetThreshold(targetBone, selectedARItem);

            if (thresHoldMeet) {
                selectedARItem.inputTouchable.touchable = false;
                selectedARItem.boneType = GeneralFlag.BoneType.Locked;
                selectedARItem.SetColor(colorLookupTable.GetColor(GeneralFlag.BoneType.Locked).color);
                selectedARItem.gameObject.layer = GeneralFlag.Layer.IgnoreRaycastInt;

                targetBone.SetColor(colorLookupTable.GetColor(GeneralFlag.BoneType.TemplateLocked).color);
                selectedARItem.transform.localPosition = targetBone.transform.localPosition;
                selectedARItem.transform.rotation = targetBone.transform.localRotation;

                //Release object and make it lock state
                OnDoubleTap();

                if (spawnRandomBoneTemplate.IsAllMetricMeet())
                    DoEndGameAction();
                
            }
        }

        private void DoEndGameAction() {
            _rhinoBoneHelper.Clean();
            _state = GeneralFlag.GeneralState.Idle;
            DinosaurApp.Instance.Notify(EventFlag.Event.GameStart);
        }

        private bool CheckTargetThreshold(BoneARItem p_targetBone, BoneARItem p_selectedBone) {
            BoneARTemplate.Metric m = spawnCorrectBoneTemplate.GetBoneMetric(p_targetBone, p_selectedBone);

            //Debug.Log($"Angle {m.angle}, Dist {m.distance}");

            return (m.angle > _angleThreshold && m.distance < _distThreshold);
        }

    }
}