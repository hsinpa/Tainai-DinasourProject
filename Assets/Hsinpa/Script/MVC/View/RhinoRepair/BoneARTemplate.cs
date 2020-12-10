using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Hsinpa.View
{
    public class BoneARTemplate : MonoBehaviour
    {
        private List<BoneARItem> _items;
        private Metric _metric;

        public void SetUp()
        {
            _metric = new Metric();
            _items = GetComponentsInChildren<BoneARItem>().ToList();
        }

        public BoneARItem GetItemByName(string p_name)
        {
            int index = _items.FindIndex(x => x.transform.name == p_name);
            if (index >= 0)
                return _items[index];

            return null;
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