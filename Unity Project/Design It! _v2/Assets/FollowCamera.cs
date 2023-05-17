using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float smoothSpeed = 0.125f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Vector3 desiredPosition = target.position + offset;
        // //use the rotation of the target to rotate the transform
        // transform.rotation = target.rotation;
        

        // Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        // transform.position = smoothedPosition;

        // desired position = target position + offset (in the axis of rotation of the target)
        Vector3 desiredPosition = target.position + target.rotation * offset;
        //use the rotation of the target to rotate the transform
        transform.rotation = target.rotation;
        //smooth the position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        //update the position
        transform.position = smoothedPosition;

        
    }
}
