using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralFlag
{
    public class Layer {
        public const int Plane = 1 << 20;
        public const int ARDetectable = 1 << 21;
        public const int IgnoreRaycast = 1 << 2;

        public const int PlaneInt = 20;
        public const int ARDetectableInt = 21;
        public const int IgnoreRaycastInt = 2;
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
        TemplateHint,
        TemplateLocked
    }
}
