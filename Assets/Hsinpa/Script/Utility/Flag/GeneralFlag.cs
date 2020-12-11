using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralFlag
{
    public class Layer {
        public const int Plane = 1 << 20;
        public const int ARDetectable = 1 << 21;
    }

    public class MatPropertyName {
        public const string Color = "_BaseColor";
    }

    public enum AnchorType { 
        Position,
        Text
    }

    public enum BoneType { 
        Idle,
        Selected,
        Locked,
        TemplateIdle,
        TemplateHint
    }
}
