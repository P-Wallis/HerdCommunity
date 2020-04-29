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
    // Constants
    const string attackAnimBool = "IsAttacking";

    // Reference Manager Fields
    [HideInInspector] public Transform cameraParent;
    [HideInInspector] public Flock flock;

    // Inspector Fields
    public float stalkingTime = 60;
    public float attackingTime = 1;
    public float maxEatingTime = 2;
    public float chaseSpeed = 5;
    public float retreatSpeed = 2;
    [Range(0,5)]public float boidFear = 1;
    public GameObject[] visuals;

    // Private Variables
    private EnemyState currentState = EnemyState.stalking;
    private float timer = 0;
    private float speed = 1f;
    private Boid target;
    private float startPointX;
    private float startPointY;
    private Animator animator;
    private Boid.AvoidPoint boidAvoid;

    // Properies
    public Vector2 position // Mostly a 2D shorcut for transform.position, but using z as the y axis
    {
        get { return new Vector2(transform.position.x, transform.position.z); }
        set { transform.position = new Vector3(value.x, transform.position.y, value.y); }
    }

    void Start()
    {
        ReferenceManager.GetReferences(this);
        animator = GetComponentInChildren<Animator>();
        ShowVisuals(false);
        boidAvoid = flock.AddAvoidPoint(transform, 0);
    }

    void Update()
    {
        switch(currentState) {
            case EnemyState.stalking: {
                if (UpdateTimer(stalkingTime))
                {
                    speed = chaseSpeed;
                    RandomizeStartPoint();
                    position = GetStartPoint();
                    ShowVisuals(true);
                    boidAvoid.Weight = boidFear;

                    currentState = EnemyState.targeting;
                }
                break;
            }
            case EnemyState.targeting:{
                if (target == null) {
                    target = GetClosestEnemy(flock.boids);
                }
                if (MoveTowardPosition(target.position, 0.2f))
                {
                    currentState = EnemyState.attacking;
                }
              break;
            }
            case EnemyState.attacking: {
                if (target != null)
                {
                    animator.SetBool(attackAnimBool, true);
                    target.Kill();
                    target = null;
                }
                if (UpdateTimer(attackingTime))
                {
                    speed = retreatSpeed;
                    animator.SetBool(attackAnimBool, false);
                    boidAvoid.Weight = .1f;

                    currentState = EnemyState.eatting;
                }
                break;
            }
            case EnemyState.eatting: {
                if (MoveTowardPosition(GetStartPoint()) || UpdateTimer(maxEatingTime))
                {
                    ShowVisuals(false);
                    boidAvoid.Weight = 0; // We're invisible, so boids shouldn't flee
                    timer = 0; // in case we got to our destination early, reset the timer

                    currentState = EnemyState.stalking;
                }
                break;
            }
        }

    }

    Boid GetClosestEnemy (List<Boid> enemies)
    {
        Boid bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        foreach(Boid potentialTarget in enemies)
        {
            Vector2 directionToTarget = potentialTarget.position - position;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if(dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }

    bool UpdateTimer(float endTime)
    {
        timer += Time.deltaTime;

        if (timer >= endTime)
        {
            timer = 0; // Reset when done
            return true;
        }
        return false;
    }

    bool MoveTowardPosition(Vector2 target, float detectRadius = 0)
    {
        float distance = Vector2.Distance(position, target);
        float maxMovement = speed * Time.deltaTime;

        if (distance <= (maxMovement+detectRadius))
        {
            position = target;
            return true;
        }

        Vector2 direction = (target - position).normalized;
        position += direction * maxMovement;
        transform.rotation = Quaternion.Euler(0, Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg, 0);
        return false;
    }

    void RandomizeStartPoint()
    {
        startPointX = (Random.value > 0.5f ? 1 : -1) * 5;
        startPointY = Random.Range(-2f, 2f);
    }

    Vector2 GetStartPoint()
    {
        Vector3 worldPos = cameraParent.position + (cameraParent.right * startPointX) + (cameraParent.forward * startPointY);
        return new Vector2(worldPos.x, worldPos.z);
    }

    void ShowVisuals(bool show)
    {
        foreach (GameObject go in visuals)
            go.SetActive(show);
    }
}
