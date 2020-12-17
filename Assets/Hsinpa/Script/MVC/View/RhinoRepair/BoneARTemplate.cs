using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

namespace Hsinpa.View
{
    public class BoneARTemplate : MonoBehaviour
    {
        [SerializeField]
        private Button confirmBtn;

        private List<BoneARItem> _items;
        private Metric _metric;

        public void SetUp(System.Action OnConfirmBtnClick)
        {
            _metric = new Metric();
            _items = GetComponentsInChildren<BoneARItem>().ToList();

            ShowConfirmBtn(false);

            if (confirmBtn != null && OnConfirmBtnClick != null) {
                confirmBtn.onClick.RemoveAllListeners();
                confirmBtn.onClick.AddListener(() => OnConfirmBtnClick());
            }
        }

        public void ShowConfirmBtn(bool show)
        {
            if (confirmBtn != null)
                confirmBtn.gameObject.SetActive(show);
        }

        public void SetColorAllBones(GeneralFlag.BoneType p_boneType, Color p_color) {
            int itemCount = _items.Count;

            for (int i = 0; i < itemCount; i++) {
                _items[i].boneType = p_boneType;
                _items[i].SetColor(p_color);
            }
        }

        public BoneARItem GetItemByName(string p_name)
        {
            int index = _items.FindIndex(x => x.transform.name == p_name);
            if (index >= 0)
                return _items[index];

            return null;
        }

        public bool IsAllMetricMeet() {
            if (_items == null) return false;

            int boneCount = _items.Count;
            int lockBoneLength = _items.Count(x => x.boneType == GeneralFlag.BoneType.Locked || x.boneType == GeneralFlag.BoneType.TemplateLocked);

            Debug.Log($"BoneCount {boneCount}, LockBone {lockBoneLength}");

            return boneCount == lockBoneLength;
        }

        public Metric GetBoneMetric(BoneARItem targetBone, BoneARItem compareBone) {
            _metric.distance = Vector3.Distance(targetBone.transform.localPosition, compareBone.transform.localPosition);

            // 1=> exactly the same direction, 0 => complete opposite
            _metric.angle = Vector3.Dot(targetBone.transform.forward, compareBone.transform.forward);
            
            return _metric;
        }

        public struct Metric {
            public float angle;
            public float distance;
        }
    }
}