﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using STouch = UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.InputSystem.Utilities;

namespace LightHouse.Edit {
    public class InputEditMode : MonoBehaviour
    {
        private GameObject _targetObject;

        public enum Mode { Idle, Translate, Rotation }
        public Mode _mode;

        InputEditTranslate _inputEditTranslate;
        InputEditRotate _inputEditRotate;

        public void SetUp(Camera p_camera)
        {
            _mode = Mode.Idle;
            _inputEditTranslate = new InputEditTranslate();
            _inputEditRotate = new InputEditRotate();
        }

        public void SetTargetAnchor(GameObject p_targetAnchor) {
            _mode = Mode.Rotation;
            this._targetObject = p_targetAnchor;

            _inputEditTranslate.SetUp(p_targetAnchor);
            _inputEditRotate.SetUp(p_targetAnchor);
        }

        public void OnUpdate(ReadOnlyArray<STouch.Touch> touches, int touchCount)
        {
            if (_mode == Mode.Translate)
                _inputEditTranslate.OnUpdate();

            if (_mode == Mode.Rotation)
                _inputEditRotate.OnUpdate(touches, touchCount);
        }


    }
}