using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventFlag 
{
    public class Event
    {
        public const string GameStart = "event@game_start";
        public const string GameRestart = "event@game_restart";

        public const string OnARMode_PlaneAR = "event@planeAR_activate";
        public const string OnARMode_SpatialAR = "event@spatialAR_activate";
        public const string OnARMode_NoAR= "event@noAR_activate";
    }
}