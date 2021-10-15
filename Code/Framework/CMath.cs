// Primary Author : Viktor Dahlberg - vida6631

using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public static class CMath
    {
	    /// <summary>
	    ///     Remaps a value from a span to another, maintaining percent of span.
	    ///     I.e., 5 in the span (0,10) becomes 0.5 in the span (0,1).
	    /// </summary>
	    /// <param name="value">Value to remap.</param>
	    /// <param name="oldLow">Min value of old span.</param>
	    /// <param name="oldHigh">Max value of old span.</param>
	    /// <param name="newLow">Min value of new span.</param>
	    /// <param name="newHigh">Max value of new span.</param>
	    /// <returns>Value remapped from the old to the new span.</returns>
	    public static float Map(float value, float oldLow, float oldHigh, float newLow, float newHigh)
        {
            return newLow + (newHigh - newLow) * ((value - oldLow) / (oldHigh - oldLow));
        }

	    /// <summary>
	    ///     Returns the index of the point closest to the target.
	    /// </summary>
	    /// <param name="points">Points to choose from.</param>
	    /// <param name="target">Target point to compare with.</param>
	    /// <returns>The index of the closest point.</returns>
	    public static int ClosestPoint(IReadOnlyList<Vector3> points, Vector3 target)
        {
            return GetPointIndex(points, target, true);
        }

	    /// <summary>
	    ///     Returns the index of the point furthest from the target.
	    /// </summary>
	    /// <param name="points">Points to choose from.</param>
	    /// <param name="target">Target point to compare with.</param>
	    /// <returns>The index of the furthest point.</returns>
	    public static int FurthestPoint(IReadOnlyList<Vector3> points, Vector3 target)
        {
            return GetPointIndex(points, target, false);
        }

	    /// <summary>
	    ///     Returns the index of the point closest to or furthest from the target.
	    /// </summary>
	    /// <param name="points">Points to choose from.</param>
	    /// <param name="target">Target point to compare with.</param>
	    /// <param name="closest">Whether or not to choose the closest or furthest point.</param>
	    /// <returns>The index of the closest or furthest point.</returns>
	    private static int GetPointIndex(IReadOnlyList<Vector3> points, Vector3 target, bool closest)
        {
            var index = 0;
            var selected = (points[0] - target).sqrMagnitude;

            for (var i = 0; i < points.Count; i++)
            {
	            float current;
	            if (closest)
                {
                    if (!((current = (points[i] - target).sqrMagnitude) < selected))
                    {
	                    continue;
                    }
                }
                else
                {
                    if (!((current = (points[i] - target).sqrMagnitude) > selected))
                    {
	                    continue;
                    }
                }

                index = i;
                selected = current;
            }

            return index;
        }
    }
}