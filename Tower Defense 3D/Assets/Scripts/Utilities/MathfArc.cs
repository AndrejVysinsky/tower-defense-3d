using UnityEngine;

public static class MathfArc
{
	public static float GetArcHeightAtPosition(Vector3 startingPosition, Vector3 currentPositionWithoutArc, Vector3 targetPosition, float maxArcHeight)
	{
		float currentDistance = Vector3.Distance(startingPosition, currentPositionWithoutArc);
		float targetDistance = Vector3.Distance(startingPosition, targetPosition);

		float arc = Mathf.Sin(currentDistance / targetDistance * Mathf.PI) * maxArcHeight;

		return arc;
	}
}