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

        [SerializeField]
        private BoneARItem selectedARItem;

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
                        OnDoubleTap(inputStruct.gameObject);
                    }
                    break;
            }
        }

        private void OnSingleTap(GameObject gameObject) { 
        
        }

        private void OnDoubleTap(GameObject gameObject)
        {
            _raycastInputHandler.ResetRaycaster();
        }

        private void Update()
        {
            if (!isActivate || selectedARItem == null) return;

            var targetBone = correctBoneTemplate.GetItemByName(selectedARItem.name);

            if (targetBone == null) return;

            bool thresHoldMeet = CheckTargetThreshold(targetBone, selectedARItem);

            if (thresHoldMeet) {
                selectedARItem.inputTouchable.touchable = false;
                selectedARItem.SetColor(colorLookupTable.GetColor(GeneralFlag.BoneType.Locked).color);
                selectedARItem.transform.localPosition = targetBone.transform.localPosition;
                selectedARItem.transform.rotation = targetBone.transform.localRotation;
            }
        }

        private bool CheckTargetThreshold(BoneARItem p_targetBone, BoneARItem p_selectedBone) {
            BoneARTemplate.Metric m = correctBoneTemplate.GetBoneMetric(p_targetBone, p_selectedBone);

            Debug.Log($"Angle {m.angle}, Dist {m.distance}");

            return (m.angle > _angleThreshold && m.distance < _distThreshold);
        }

    }
}