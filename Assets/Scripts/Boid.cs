using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public Vector2 position
    {
        get { return new Vector2(transform.position.x, transform.position.z); }
        set { transform.position = new Vector3(value.x, transform.position.y, value.y); }
    }
    [HideInInspector] public Vector2 velocity;
    [HideInInspector] public Vector2 acceleration;

    [HideInInspector] public float perceptionRadius;
    [HideInInspector] public float maxSpeed;
    [HideInInspector] public float alignmentFactor;
    [HideInInspector] public float cohesionFactor;
    [HideInInspector] public float separationFactor;

    [HideInInspector] public Vector2 bounds;

    protected bool flipByVelocity = true;
    protected Vector3 scale;

    private Transform cameraTransform;
    private Transform cameraParent { get { return cameraTransform != null ? cameraTransform.parent : null; } }

    protected virtual void Start()
    {
        scale = transform.localScale;
        cameraTransform = Camera.main.transform;
    }

    public virtual void SetParameters(float boidPerceptionRadius, float boidMaxSpeed, float boidAlignment, float boidCohesion, float boidSeparation, Vector2 boidBounds)
    {
        perceptionRadius = boidPerceptionRadius;
        maxSpeed = boidMaxSpeed;
        alignmentFactor = boidAlignment;
        cohesionFactor = boidCohesion;
        separationFactor = boidSeparation;
        bounds = boidBounds;
    }

    public virtual void CalculateAcceleration(List<Boid> flock)
    {
        List<Boid> localBoids = GetLocalBoids(flock);
        acceleration = Vector2.zero;
        acceleration += GetAlignment(localBoids) * alignmentFactor;
        acceleration += GetCohesion(localBoids) * cohesionFactor;
        acceleration += GetSeparation(localBoids) * separationFactor;
    }

    public virtual void DoMovement()
    {
        // Update position and velocity
        position += velocity * Time.deltaTime;
        velocity += acceleration * Time.deltaTime;

        // Limit the velocity to max speed
        if (velocity.magnitude > maxSpeed)
        {
            velocity = velocity.normalized * maxSpeed;
        }

        // Flip to face movement direction
        if (flipByVelocity)
        {
            float direction = Vector3.Dot(cameraTransform.right, new Vector3(velocity.x, 0, velocity.y).normalized);
            transform.localScale = new Vector3(scale.x * (direction < 0 ? 1 : -1), scale.y, scale.z);
        }


        //Keep Boids In Bounds
        if (cameraParent != null)
        {
            Vector3 camPos = cameraParent.InverseTransformPoint(transform.position);

            if (Mathf.Abs(camPos.x) > bounds.x)
                camPos.x = Mathf.Clamp(camPos.x * -1, -bounds.x, bounds.x);
            if (Mathf.Abs(camPos.z) > bounds.y)
                camPos.z = Mathf.Clamp(camPos.z * -1, -bounds.y, bounds.y);

            transform.position = cameraParent.TransformPoint(camPos);
        }
        else
        {
            if (Mathf.Abs(position.x) > bounds.x)
                position = new Vector2(Mathf.Clamp(position.x * -1, -bounds.x, bounds.x), position.y);
            if (Mathf.Abs(position.y) > bounds.y)
                position = new Vector2(position.x, Mathf.Clamp(position.y * -1, -bounds.y, bounds.y));
        }
        
    }

    private List<Boid> GetLocalBoids(List<Boid> flock)
    {
        List<Boid> localBoids = new List<Boid>();

        if (flock != null)
        {
            float distanceToBoid;
            for (int i = 0; i < flock.Count; i++)
            {
                distanceToBoid = Vector2.Distance(flock[i].position, position);
                if (distanceToBoid < perceptionRadius && distanceToBoid > 0)
                {
                    localBoids.Add(flock[i]);
                }
            }
        }

        return localBoids;
    }

    private Vector2 GetAlignment(List<Boid> boids)
    {
        Vector2 alignment = Vector2.zero;

        if (boids != null && boids.Count > 0)
        {
            for (int i = 0; i < boids.Count; i++)
            {
                alignment += boids[i].velocity.normalized;
            }
            alignment /= boids.Count;
        }
        return alignment;
    }

    private Vector2 GetCohesion(List<Boid> boids)
    {
        Vector2 cohesion = Vector2.zero;

        if (boids != null && boids.Count > 0)
        {
            for (int i = 0; i < boids.Count; i++)
            {
                cohesion += boids[i].position;
            }
            cohesion /= boids.Count;
            cohesion -= position;
        }
        return cohesion;
    }

    private Vector2 GetSeparation(List<Boid> boids)
    {
        Vector2 separation = Vector2.zero;

        if (boids != null && boids.Count > 0)
        {
            float distance;
            Vector2 direction;
            for (int i = 0; i < boids.Count; i++)
            {
                distance = Vector2.Distance(position, boids[i].position);
                direction = (position - boids[i].position).normalized;
                direction /= distance;
                separation += direction;
            }
        }
        return separation;
    }
}
