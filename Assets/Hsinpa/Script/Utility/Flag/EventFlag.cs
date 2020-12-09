using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventFlag 
{
    public class Event
    {
        public const string GameStart = "event@game_start";

        public const string OnAnchorClick = "event@anchor_click";
        public const string OnAnchorEditBack = "event@leave_anchor_edit";
    }
}