using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using Hsinpa.GameInput;

public class AnimalSwitchDetection : MonoBehaviour
{
    [SerializeField]
    private AnimalSwitchHolder _spwanAnimalHolder;

    [SerializeField]
    private RaycastInputHandler _raycastInputHandler;

    [SerializeField]
    private RectTransform canvasHolder;

    [SerializeField]
    private Button leftStyleBtn;

    [SerializeField]
    private Button rightStyleBtn;

    [SerializeField]
    private Text currentStyleText;

    private AnimalARItem currentSelectObject;

    // Start is called before the first frame update
    void Start()
    {
        _raycastInputHandler.OnInputEvent += OnInputEvent;

        leftStyleBtn.onClick.AddListener(() => { OnUIStyleClick(-1); });
        rightStyleBtn.onClick.AddListener(() => { OnUIStyleClick(1); });

        currentSelectObject = _spwanAnimalHolder.SelectByIndex(index: 0);

        SetSelectObject(currentSelectObject);
        //SpawnObjectOnPos(Vector3.zero);
    }

    private void OnInputEvent(RaycastInputHandler.InputStruct inputStruct) {
        Debug.Log("inputStruct.inputType " + inputStruct.inputType);
        Debug.Log("raycastPosition " + inputStruct.raycastPosition);

        if (inputStruct.inputType == RaycastInputHandler.InputType.SingleTap)
            SpawnObjectOnPos(inputStruct.raycastPosition);

        if (inputStruct.inputType == RaycastInputHandler.InputType.DoubleTap)
            canvasHolder.gameObject.SetActive(!canvasHolder.gameObject.activeSelf);
    }

    private void SpawnObjectOnPos(Vector3 position, bool includeOffset = true) {
        if (currentSelectObject == null) return;

        _spwanAnimalHolder.HideTheRestObjectExceptObject(currentSelectObject);


        currentSelectObject.transform.position = position;

        currentSelectObject.transform.rotation = _raycastInputHandler.GetFrontQuaternion(position, currentSelectObject.rotationOffset);

        if (includeOffset)
        {
            var forwardOffset = currentSelectObject.transform.forward * currentSelectObject.positionOffset.z;
            var upOffset = currentSelectObject.transform.up * currentSelectObject.positionOffset.y;

            currentSelectObject.transform.position = position + new Vector3(forwardOffset.x, upOffset.y, forwardOffset.z);
        }

        currentSelectObject.gameObject.SetActive(true);
    }

    private void OnUIStyleClick(int direction) {
        AnimalARItem selectObject = (direction == -1) ? _spwanAnimalHolder.SwitchLeft() : _spwanAnimalHolder.SwitchRight();
        SetSelectObject(selectObject);
    }

    private void SetSelectObject(AnimalARItem p_object) {
        if (p_object != null) {
            currentStyleText.text = p_object.name;

            Vector3 previousPos = currentSelectObject.transform.position;
            var previousObj = currentSelectObject;
            currentSelectObject = p_object;

            if (previousObj.name != p_object.name) {
                SpawnObjectOnPos(previousPos, false);
            }
        }
    }
}