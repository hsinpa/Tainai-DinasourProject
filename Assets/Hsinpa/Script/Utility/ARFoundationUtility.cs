using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Hsinpa {
    public class ARFoundationUtility
    {

		public static IEnumerator AysncCheckARReady(System.Action<bool> hasARAvailable)
		{
			if ((ARSession.state == ARSessionState.None) ||
				(ARSession.state == ARSessionState.CheckingAvailability))
			{
				yield return ARSession.CheckAvailability();
			}

			hasARAvailable(ARSession.state != ARSessionState.Unsupported);

		}
	}
}