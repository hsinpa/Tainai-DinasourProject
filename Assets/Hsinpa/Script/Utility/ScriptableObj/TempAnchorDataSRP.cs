using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnchorDataSRP", menuName = "ScriptableObjects/AnchorDataSRP", order = 3)]

public class TempAnchorDataSRP : ScriptableObject
{
    public List<AnchorSet> anchorSet = new List<AnchorSet>();

    public List<AnchorStruct> FindAnchorsByMissionID(string mission_id) {
        int i = anchorSet.FindIndex(x => x._id == mission_id);

        if (i >= 0 && anchorSet[i].anchors != null)
            return anchorSet[i].anchors;

        return new List<AnchorStruct>();
    }

    [System.Serializable]
    public struct AnchorSet {
        public string _id;
        public List<AnchorStruct> anchors;
    }

    [System.Serializable]
    public struct AnchorStruct {
        public string _id;
        public AnchorType anchorType;       
    }

    public enum AnchorType { 
        Main, Support
    }
}
