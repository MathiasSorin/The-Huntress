using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

private void DrawTrajectory() {
    // Create a list of trajectory points
    var curvePoints = new List<Vector3>();
    curvePoints.Add(throwPos.position);
    var currentPosition = throwPos.position;
    var currentVelocity = camT.rotation * new Vector3(0, 0, throwStrength);
    RaycastHit hit;
    Ray ray = new Ray(currentPosition, currentVelocity.normalized);
    // Loop until hit something or distance is too great

    while (!Physics.Raycast(ray, out hit, 0.25f) && Vector3.Distance(throwPos.position, currentPosition) < drawRange) {
        var t = 0.25f / currentVelocity.magnitude;
        currentVelocity = currentVelocity + t * Physics.gravity;
        currentPosition = currentPosition + t * currentVelocity;
        curvePoints.Add(currentPosition);
        ray = new Ray(currentPosition, currentVelocity.normalized);

    }
    if (hit.transform) {
        curvePoints.Add(hit.point);
    }
    line.positionCount = curvePoints.Count;
    line.SetPositions(curvePoints.ToArray());
}

private void ClearTrajectory() {
    line.positionCount = 0;
}