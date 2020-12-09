using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralFlag
{
    public class Layer {
        public const int Plane = 1 << 20;
        public const int ARDetectable = 1 << 21;
    }

    public enum AnchorType { 
        Position,
        Text
    }
}
