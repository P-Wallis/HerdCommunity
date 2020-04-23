using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Predator : MonoBehaviour
{
	public Transform follow;
	public Flock flock;
    // Start by finding the camera to follow and copy its position
    void Start()
    {
        
    }

    // Update its position based on the movement of the flock
    void Update()
    {
        transform.position = follow.position + (Vector3.right * 3);
        //flock.boids
    }
}
