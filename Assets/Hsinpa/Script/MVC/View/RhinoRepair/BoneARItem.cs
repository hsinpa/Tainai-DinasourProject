using Hsinpa.GameInput;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsinpa.View {
    [RequireComponent(typeof(InputTouchable))]
    public class BoneARItem : MonoBehaviour
    {
        private MeshRenderer _meshRenderer;
        private MaterialPropertyBlock m_PropertyBlock;
        private InputTouchable _inputTouchable;

        public GeneralFlag.BoneType boneType = GeneralFlag.BoneType.Idle;

        public InputTouchable inputTouchable
        {
            get
            {
                if (_inputTouchable == null)
                    _inputTouchable = GetComponent<InputTouchable>();
                return _inputTouchable;
            }
        }

        public void SetColor(Color p_color) {
            if (m_PropertyBlock == null)
                m_PropertyBlock = new MaterialPropertyBlock();

            if (_meshRenderer == null)
                _meshRenderer = GetComponent<MeshRenderer>();

            m_PropertyBlock.SetColor(GeneralFlag.MatPropertyName.Color, p_color);
            _meshRenderer.SetPropertyBlock(m_PropertyBlock);
        }
    }
}
