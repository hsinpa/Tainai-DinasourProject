using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using STouch = UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class InputEditRotate
{ 
    private GameObject targetObject;
    Vector3 lastStandPoint;

    public InputEditRotate()
    {

    }

    public void SetUp(GameObject targetObject)
    {
        this.targetObject = targetObject;
    }

    #region Device Input Handler
    public void OnUpdate(ReadOnlyArray<STouch.Touch> touches, int touchCount)
    {
        Debug.Log(touchCount);
        if (touchCount == 2) { 
            
        }
    }

    private int ProcessRotation()
    {
        Vector3 currentStandPoint = Input.mousePosition;
        float direction = (currentStandPoint - lastStandPoint).x;
        direction = Mathf.Clamp(direction, -3, 3);

        Vector3 rotation = new Vector3(0, direction, 0);

        targetObject.transform.Rotate(rotation, Space.Self);

        return (direction > 0) ? 1 : -1;
    }
    #endregion
}
