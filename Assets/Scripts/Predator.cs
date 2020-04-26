using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Predator : MonoBehaviour
{
    // Reference Manager Fields
    [HideInInspector] public Transform cameraParent;
    [HideInInspector] public Flock flock;

    private bool target;

    // Start by finding the camera to follow and copy its position
    void Start()
    {
        ReferenceManager.GetReferences(this);
        target = false;
    }

    // Update its position based on the movement of the flock
    void Update()
    {
        transform.position = cameraParent.position + (Vector3.right * 6);
       /* if (target = false) {
           transform. GetClosestEnemy(flock.boids);
        }*/
    }

    /*Transform GetClosestEnemy (Transform[] enemies)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach(Transform potentialTarget in enemies)
        {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if(dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }
     
        return bestTarget;
    }*/

}
