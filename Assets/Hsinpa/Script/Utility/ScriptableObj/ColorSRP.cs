using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[CreateAssetMenu(fileName = "ColorSRP", menuName = "ScriptableObjects/ColorSRP", order = 1)]
public class ColorSRP : ScriptableObject
{
    [SerializeField]
    private List<ColorStruct> colors = new List<ColorStruct>();

    public ColorStruct GetColor(GeneralFlag.BoneType p_name) {
        int index = colors.FindIndex(x => x.name == p_name);
        if (index >= 0)
            return colors[index];

        return default(ColorStruct);
    }

    [System.Serializable]
    public struct ColorStruct {
        public GeneralFlag.BoneType name;
        public Color color;
    }
}