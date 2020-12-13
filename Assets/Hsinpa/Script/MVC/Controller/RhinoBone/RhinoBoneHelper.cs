using Hsinpa.View;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Hsinpa.Other {
    public class RhinoBoneHelper
    {
        private BoneARTemplate _correctTemplatePrefab;
        private DinosaurBoneSRP _dinosaurBoneSRP;
        private Transform _worldContainer;

        public RhinoBoneHelper( BoneARTemplate correctTemplatePrefab, DinosaurBoneSRP dinosaurBoneSRP, Transform worldContainer) {
            this._correctTemplatePrefab = correctTemplatePrefab;
            this._dinosaurBoneSRP = dinosaurBoneSRP;
            this._worldContainer = worldContainer;
        }

        public BoneARTemplate CreateBoneTemplate(Vector3 vector3, Quaternion quat) {
            return UnityEngine.Object.Instantiate(_correctTemplatePrefab, vector3, quat, _worldContainer);
        }

        public BoneARTemplate CreateBoneRandomSet(Vector3 vector3, Quaternion quat) {
            var boneRandomTemp = _dinosaurBoneSRP.GetRandomTemplate();
            return UnityEngine.Object.Instantiate(boneRandomTemp, vector3, quat, _worldContainer);
        }

        public void MoveBoneTemplate(BoneARTemplate template, Vector3 vector3, Quaternion rotation) {
            template.transform.rotation = rotation;
            template.transform.position = vector3;
        }

        public void Clean() {
            UtilityMethod.ClearChildObject(_worldContainer);
        }
    }
}
