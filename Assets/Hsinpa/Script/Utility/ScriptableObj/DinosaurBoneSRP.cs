using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[CreateAssetMenu(fileName = "DinosaurBoneSRP", menuName = "ScriptableObjects/DinosaurBoneSRP", order = 2)]
public class DinosaurBoneSRP : ScriptableObject
{
    [SerializeField]
    private List<Hsinpa.View.BoneARTemplate> templates = new List<Hsinpa.View.BoneARTemplate>();

    public Hsinpa.View.BoneARTemplate GetRandomTemplate(string template_name = null) {

        int randomIndex = Random.Range(0, templates.Count);

        if (randomIndex >= 0) {

            if (template_name != null)
            {
                int selectedIndex = templates.FindIndex(x => x.name == template_name);
                if (selectedIndex >= 0)
                    return templates[selectedIndex];
            }

            return templates[randomIndex];
        }

        return null;
    }

}