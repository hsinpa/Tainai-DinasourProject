using Hsinpa.View;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace Hsinpa.View
{
    public class DialogueModal : Modal
    {

        [SerializeField]
        private Text titleText;

        [SerializeField]
        private Text contentText;

        [SerializeField]
        private Transform btnHolder;

        [SerializeField]
        private DialogueButtonObj btnPrefab;

        public void SetDialogue(string title, string content, string[] allowBtns, System.Action<int> btnEvent) {
            ResetContent();

            titleText.text = title;
            contentText.text = content;

            RegisterButtons(allowBtns, btnEvent);
        }

        private void RegisterButtons(string[] allowBtns, System.Action<int> btnEvent) {
            int btnlength = allowBtns.Length;

            for (int i = 0; i < btnlength; i++) {
                var DialogueButtonObj = UtilityMethod.CreateObjectToParent<DialogueButtonObj>(btnHolder, btnPrefab.gameObject);
                int index = i;

                DialogueButtonObj.gameObject.SetActive(true);

                DialogueButtonObj.SetBtnEvent(allowBtns[i], () => {
                    if (btnEvent != null)
                        btnEvent(index);
                });
            }
        }

        private void ResetContent() {
            UtilityMethod.ClearChildObject(btnHolder);
        }


    }
}