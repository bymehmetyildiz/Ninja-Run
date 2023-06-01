using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform playerTransform;
    public float fixedDistance;
    public float rotationFactor;
    public float height;
    public float heightFactor;

    float startRotationAngle; // This camera's rotation angle around the y-axis
    float endRotationAngle; // The player's rotation around the y-axis.
    float finalRotationAngle; // The final Smoothed out (interpolated) rotation angle of camera around the y-axis.

    float currentHeight;
    float wantedHeight;

    void Start()
    {
        
    }

    
    void LateUpdate()
    {
        currentHeight = this.transform.position.y;
        wantedHeight = playerTransform.position.y + height;
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightFactor * Time.deltaTime);

        startRotationAngle = this.transform.eulerAngles.y;
        endRotationAngle = playerTransform.eulerAngles.y;
        finalRotationAngle = Mathf.LerpAngle(startRotationAngle, endRotationAngle, Time.deltaTime * rotationFactor);

        Quaternion finalRotation = Quaternion.Euler(0, finalRotationAngle, 0);  // Convert angle value into actual rotation.
        this.transform.position = playerTransform.position;
        this.transform.position -= finalRotation * Vector3.forward * fixedDistance; // Same as: new Vector3(sin(finalRotationAngle), 0, cos(finalRotationAngle) * fixedDistance)

        this.transform.position = new Vector3(this.transform.position.x, currentHeight, this.transform.position.z);
        this.transform.LookAt(playerTransform);
    }
}
