using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hsinpa.GameInput;
using Hsinpa.View;

namespace Hsinpa.Ctrl {

    public class RhinoBoneRepairCtrl : ObserverPattern.Observer
    {
        [SerializeField]
        private RaycastInputHandler _raycastInputHandler;

        [SerializeField]
        private BoneARTemplate correctBoneTemplate;

        [SerializeField]
        private ColorSRP colorLookupTable;

        [SerializeField, Range(-1, 1)]
        private float _angleThreshold;

        [SerializeField, Range(0, 3)]
        private float _distThreshold;

        private BoneARItem selectedARItem;
        private BoneARItem targetARItem;

        public bool isActivate = false;

        public override void OnNotify(string p_event, params object[] p_objects)
        {
            switch (p_event)
            {

            }
        }

        public void Start()
        {
            Initialization();
            Activate(true);
        }

        public void Initialization() {
            correctBoneTemplate.SetUp();
            correctBoneTemplate.SetColorAllBones(GeneralFlag.BoneType.TemplateIdle, colorLookupTable.GetColor(GeneralFlag.BoneType.TemplateIdle).color);
            _raycastInputHandler.OnInputEvent += OnRaycastInputEvent;
        }

        public void Activate(bool isActivate) {
            this.isActivate = isActivate;
        }

        private void OnRaycastInputEvent(RaycastInputHandler.InputStruct inputStruct) {
            if (!isActivate) return;
            
            switch (inputStruct.inputType) {
                case RaycastInputHandler.InputType.SingleTap: {
                        OnSingleTap(inputStruct.gameObject);
                    }
                    break;
                case RaycastInputHandler.InputType.DoubleTap:
                    {
                        OnDoubleTap();
                    }
                    break;
            }
        }

        private void OnSingleTap(GameObject gameObject) {
            selectedARItem = gameObject.GetComponent<BoneARItem>();

            if (selectedARItem != null) {
                selectedARItem.boneType = GeneralFlag.BoneType.Selected;
                selectedARItem.SetColor(colorLookupTable.GetColor(GeneralFlag.BoneType.Selected).color);

                targetARItem = correctBoneTemplate.GetItemByName(selectedARItem.name);
                targetARItem.SetColor(colorLookupTable.GetColor(GeneralFlag.BoneType.TemplateHint).color);
            }
        }

        private void OnDoubleTap()
        {
            if (selectedARItem == null) return;

            _raycastInputHandler.ResetRaycaster();

            if (selectedARItem.boneType != GeneralFlag.BoneType.Locked)
                selectedARItem.SetColor(colorLookupTable.GetColor(GeneralFlag.BoneType.Idle).color);

            targetARItem.SetColor(colorLookupTable.GetColor(GeneralFlag.BoneType.TemplateIdle).color);

            selectedARItem = null;
        }

        private void Update()
        {
            if (!isActivate || selectedARItem == null) return;

            var targetBone = correctBoneTemplate.GetItemByName(selectedARItem.name);

            if (targetBone == null) return;

            bool thresHoldMeet = CheckTargetThreshold(targetBone, selectedARItem);

            if (thresHoldMeet) {
                selectedARItem.inputTouchable.touchable = false;
                selectedARItem.boneType = GeneralFlag.BoneType.Locked;
                selectedARItem.SetColor(colorLookupTable.GetColor(GeneralFlag.BoneType.Locked).color);
                selectedARItem.transform.localPosition = targetBone.transform.localPosition;
                selectedARItem.transform.rotation = targetBone.transform.localRotation;

                OnDoubleTap();
            }
        }

        private bool CheckTargetThreshold(BoneARItem p_targetBone, BoneARItem p_selectedBone) {
            BoneARTemplate.Metric m = correctBoneTemplate.GetBoneMetric(p_targetBone, p_selectedBone);

            Debug.Log($"Angle {m.angle}, Dist {m.distance}");

            return (m.angle > _angleThreshold && m.distance < _distThreshold);
        }

    }
}