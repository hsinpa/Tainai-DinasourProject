using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using STouch = UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class InputEditRotateDesktop
{ 
    private GameObject targetObject;

    public void SetUp(GameObject targetObject)
    {
        this.targetObject = targetObject;
    }

    #region Device Input Handler
    public void OnUpdate()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            ProcessRotation(1);
        }
        else if (Input.GetKey(KeyCode.LeftArrow)) {
            ProcessRotation(-1);
        }
    }

    private int ProcessRotation(float direction)
    {
        direction = Mathf.Clamp(direction, -3, 3);

        Vector3 rotation = new Vector3(0, direction, 0);

        targetObject.transform.Rotate(rotation, Space.Self);

        return (direction > 0) ? 1 : -1;
    }
    #endregion
}
