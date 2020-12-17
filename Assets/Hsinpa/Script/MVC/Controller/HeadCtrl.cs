using Hsinpa.View;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hsinpa.Other;

namespace Hsinpa.Ctrl
{
    public class HeadCtrl : ObserverPattern.Observer
    {
        [SerializeField]
        private RhinoBoneRepairCtrl _rhinoBoneRepairCtrl;

        public override void OnNotify(string p_event, params object[] p_objects)
        {
            switch (p_event)
            {
                case EventFlag.Event.GameStart:
                {
                    ShowSettingModal();
                } break;

            }
        }

        private void ShowSettingModal() {
            DialogueModal dialogueModal = Modals.instance.OpenModal<DialogueModal>();

            string[] eventArray = new string[] { EventFlag.Event.OnARMode_PlaneAR, EventFlag.Event.OnARMode_NoAR };

            dialogueModal.SetDialogue(StringSet.Dialogue.StartModeTitle, StringSet.Dialogue.StartModeContent,
                new string[] { StringSet.Dialogue.StartModePlaneAR, StringSet.Dialogue.StartModeNoAR },
             (int index) => {
                 Modals.instance.Close();
                 Hsinpa.DinosaurApp.Instance.Notify(eventArray[index]);
             });
        }

    }
}