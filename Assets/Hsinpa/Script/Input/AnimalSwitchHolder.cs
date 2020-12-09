using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSwitchHolder : MonoBehaviour
{
    [SerializeField]
    private List<AnimalARItem> gameObjectList;
    private int _index;

    private bool isListAvailable => (gameObjectList != null && gameObjectList.Count > 0);

    public AnimalARItem SelectByIndex(int index) {
        if (!isListAvailable) return null;

        _index = index;

        return gameObjectList[index];
    }

    public AnimalARItem SwitchLeft() {
        if (!isListAvailable) return null;

        _index -= 1;

        if (_index < 0) {
            _index = gameObjectList.Count - 1;
            return gameObjectList[_index];
        }
        return gameObjectList[_index];
    }

    public AnimalARItem SwitchRight() {
        if (!isListAvailable) return null;
        _index += 1;

        if (_index >= gameObjectList.Count)
        {
            _index = 0;
            return gameObjectList[0];
        }

        return gameObjectList[_index];
    }

    public void HideTheRestObjectExceptObject(AnimalARItem p_selectObject) {
        if (isListAvailable && p_selectObject != null) {
            gameObjectList.ForEach(x => x.gameObject.SetActive(false));
        }
    }

}
