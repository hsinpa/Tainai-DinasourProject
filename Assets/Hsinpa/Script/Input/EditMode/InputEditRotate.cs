using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using STouch = UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class InputEditRotate
{ 
    private GameObject targetObject;
    private DetectTouchMovement _detectTouchMovement;
    Vector3 lastStandPoint;


    public InputEditRotate()
    {
        _detectTouchMovement = new DetectTouchMovement();
    }

    public void SetUp(GameObject targetObject)
    {
        this.targetObject = targetObject;
    }

    #region Device Input Handler
    public void OnUpdate(ReadOnlyArray<STouch.Touch> touches, int touchCount)
    {
        _detectTouchMovement.Calculate(touches, touchCount);

        if (Mathf.Abs(_detectTouchMovement.turnAngleDelta) > 0)
        { // rotate
            Quaternion desiredRotation = targetObject.transform.rotation;
            Vector3 rotationDeg = Vector3.zero;
            rotationDeg.y = - _detectTouchMovement.turnAngleDelta;
            desiredRotation *= Quaternion.Euler(rotationDeg);
            targetObject.transform.rotation = desiredRotation;
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
