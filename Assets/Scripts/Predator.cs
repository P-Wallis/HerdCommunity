using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState {
    stalking,
    targeting,
    attacking,
    eatting
}

public class Predator : MonoBehaviour
{
    // Reference Manager Fields
    [HideInInspector] public Transform cameraParent;
    [HideInInspector] public Flock flock;
    private float timer = 0;
    public float stalkingTime = 60;
    Boid newTarget;
    EnemyState currentState = EnemyState.stalking;
    // Start by finding the camera to follow and copy its position
    void Start()
    {
        ReferenceManager.GetReferences(this);
    }

    // Update its position based on the movement of the flock
    void Update()
    {
        switch(currentState) {
            case EnemyState.stalking: {
                transform.position = cameraParent.position + (Vector3.right * 5); 
                if(timer >= stalkingTime) {
                    timer = 0;
                    currentState = EnemyState.targeting;
                }
                else timer += Time.deltaTime;
                break;
            }
            case EnemyState.targeting: {
                if (newTarget == null) {
                    newTarget = GetClosestEnemy(flock.boids);
                    Debug.Log(newTarget.gameObject.name);
                    currentState = EnemyState.attacking; 
                }
                break;
            }
            case EnemyState.attacking: {
                newTarget.Kill(); 
                currentState = EnemyState.eatting;
                break;
            }
            case EnemyState.eatting: currentState = EnemyState.stalking; break;
        
    }

    }

    Boid GetClosestEnemy (List<Boid> enemies)
    {
        Boid bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector2 currentPosition = new Vector2 (transform.position.x, transform.position.z);
        foreach(Boid potentialTarget in enemies)
        {
            Vector2 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if(dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }

}
