using System.Collections.Generic;
using UnityEngine;

public class TrajectoryRenderer : MonoBehaviour
{
    // Reference to the line renderer
    private LineRenderer line;

    // Initial trajectory position
    [SerializeField]
    private Vector3 startPosition;

    // Initial trajectory velocity
    [SerializeField]
    private Vector3 startVelocity;

    // Step distance for the trajectory
    [SerializeField]
    private float trajectoryVertDist = 0.25f;

    // Max length of the trajectory
    [SerializeField]
    private float maxCurveLength = 5;

    /// <summary>
    /// Method called on initialization.
    /// </summary>
    private void Awake() {
        // Get line renderer reference
        line = GetComponent<LineRenderer>();
    }

    /// <summary>
    /// Sets ballistic values for trajectory.
    /// </summary>
    /// <param name="startPosition">Start position.</param>
    /// <param name="startVelocity">Start velocity.</param>
    void SetBallisticValues(Vector3 startPosition, Vector3 startVelocity) {
        this.startPosition = startPosition;
        this.startVelocity = startVelocity;
    }

    /// <summary>
    /// Draws the trajectory with line renderer.
    /// </summary>
    public void DrawTrajectory(Vector3 pos, Vector3 vel) {
        SetBallisticValues(pos, vel);

        // Create a list of trajectory points
        var curvePoints = new List<Vector3>();
        curvePoints.Add(startPosition);

        // Initial values for trajectory
        var currentPosition = startPosition;
        var currentVelocity = startVelocity;

        // Init physics variables
        RaycastHit hit;
        Ray ray = new Ray(currentPosition, currentVelocity.normalized);

        // Loop until hit something or distance is too great
        while (!Physics.Raycast(ray, out hit, trajectoryVertDist) && Vector3.Distance(startPosition, currentPosition) < maxCurveLength) {
            // Time to travel distance of trajectoryVertDist
            var t = trajectoryVertDist / currentVelocity.magnitude;

            // Update position and velocity
            currentVelocity = currentVelocity + t * Physics.gravity;
            currentPosition = currentPosition + t * currentVelocity;

            // Add point to the trajectory
            curvePoints.Add(currentPosition);

            // Create new ray
            ray = new Ray(currentPosition, currentVelocity.normalized);
        }

        // If something was hit, add last point there
        if (hit.transform) {
            curvePoints.Add(hit.point);
        }

        // Display line with all points
        line.positionCount = curvePoints.Count;
        line.SetPositions(curvePoints.ToArray());
    }

    /// <summary>
    /// Clears the trajectory.
    /// </summary>
    public void ClearTrajectory() {
        // Hide line
        line.positionCount = 0;
    }
}
