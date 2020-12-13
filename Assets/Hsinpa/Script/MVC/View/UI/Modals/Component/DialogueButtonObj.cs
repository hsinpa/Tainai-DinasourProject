using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hsinpa.View
{
    public class DialogueButtonObj : MonoBehaviour
    {
        [SerializeField]
        private Button button;

        [SerializeField]
        private Text _text;


        public void SetBtnEvent(string p_text, System.Action action) {
            _text.text = p_text;
            button.onClick.RemoveAllListeners();

            button.onClick.AddListener(() => action());
        }
    }
}