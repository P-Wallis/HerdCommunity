using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    Transform cameraTransform;

    private void Start()
    {
        // This is doing a "Find" behind the scenes (which is slow), so this should be changed
        cameraTransform = Camera.main.transform;
    }

    void  LateUpdate()
    {
        Vector3 lookPos = new Vector3(cameraTransform.position.x, 0, cameraTransform.position.z);
        transform.LookAt(lookPos, Vector3.up);
    }
}
