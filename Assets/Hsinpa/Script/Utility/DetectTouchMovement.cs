using UnityEngine;
using System.Collections;
using STouch = UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.Utilities;

public class DetectTouchMovement
{
	const float pinchTurnRatio = Mathf.PI / 2;
	const float minTurnAngle = 0;

	const float pinchRatio = 1;
	const float minPinchDistance = 0;

	const float panRatio = 1;
	const float minPanDistance = 0;

	/// <summary>
	///   The delta of the angle between two touch points
	/// </summary>
	public float turnAngleDelta;
	/// <summary>
	///   The angle between two touch points
	/// </summary>
	public float turnAngle;

	/// <summary>
	///   The delta of the distance between two touch points that were distancing from each other
	/// </summary>
	public float pinchDistanceDelta;
	/// <summary>
	///   The distance between two touch points that were distancing from each other
	/// </summary>
	public float pinchDistance;

	/// <summary>
	///   Calculates Pinch and Turn - This should be used inside LateUpdate
	/// </summary>
	public void Calculate(ReadOnlyArray<STouch.Touch> touches, int touchCount)
	{
		pinchDistance = pinchDistanceDelta = 0;
		turnAngle = turnAngleDelta = 0;

		// if two fingers are touching the screen at the same time ...
		if (touchCount == 2)
		{
			var touch1 = touches[0];
			var touch2 = touches[1];


			// ... if at least one of them moved ...
			if (touch1.phase == UnityEngine.InputSystem.TouchPhase.Moved || touch2.phase == UnityEngine.InputSystem.TouchPhase.Moved)
			{
				// ... check the delta distance between them ...
				pinchDistance = Vector2.Distance(touch1.screenPosition, touch2.screenPosition);
				float prevDistance = Vector2.Distance(touch1.screenPosition - touch1.delta,
													  touch2.screenPosition - touch2.delta);
				pinchDistanceDelta = pinchDistance - prevDistance;

				// ... if it's greater than a minimum threshold, it's a pinch!
				if (Mathf.Abs(pinchDistanceDelta) > minPinchDistance)
				{
					pinchDistanceDelta *= pinchRatio;
				}
				else
				{
					pinchDistance = pinchDistanceDelta = 0;
				}

				// ... or check the delta angle between them ...
				turnAngle = Angle(touch1.screenPosition, touch2.screenPosition);
				float prevTurn = Angle(touch1.screenPosition - touch1.delta,
									   touch2.screenPosition - touch2.delta);
				turnAngleDelta = Mathf.DeltaAngle(prevTurn, turnAngle);

				// ... if it's greater than a minimum threshold, it's a turn!
				if (Mathf.Abs(turnAngleDelta) > minTurnAngle)
				{
					turnAngleDelta *= pinchTurnRatio;
				}
				else
				{
					turnAngle = turnAngleDelta = 0;
				}
			}
		}
	}

	private float Angle(Vector2 pos1, Vector2 pos2)
	{
		Vector2 from = pos2 - pos1;
		Vector2 to = new Vector2(1, 0);

		float result = Vector2.Angle(from, to);
		Vector3 cross = Vector3.Cross(from, to);

		if (cross.z > 0)
		{
			result = 360f - result;
		}

		return result;
	}
}