using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public Vector2 position {
        get { return transform.position; }
        set { transform.position = new Vector3(value.x, value.y, transform.position.z); }
    }
    [HideInInspector] public Vector2 velocity;
    [HideInInspector] public Vector2 accleration;

    [HideInInspector] public float perceptionRadius;
    [HideInInspector] public float maxSpeed;
    [HideInInspector] public float alignmentFactor;
    [HideInInspector] public float cohesionFactor;
    [HideInInspector] public float separationFactor;


    [HideInInspector] public Vector2 bounds;

    public void CalculateAcceleration(List<Boid> flock)
    {
        List<Boid> localBoids = GetLocalBoids(flock);
        accleration = Vector2.zero;
        accleration += GetAlignment(localBoids) * alignmentFactor;
        accleration += GetCohesion(localBoids) * cohesionFactor;
        accleration += GetSeparation(localBoids) * separationFactor;
    }

    public void DoMovement()
    {
        // Update position and velocity
        position += velocity * Time.deltaTime;
        velocity += accleration * Time.deltaTime;

        // Limit the velocity to max speed
        if (velocity.magnitude > maxSpeed)
            velocity = velocity.normalized * maxSpeed;

        //Keep Boids In Bounds
        if (Mathf.Abs(position.x) > bounds.x)
            position = new Vector2(Mathf.Clamp(position.x * -1, -bounds.x, bounds.x), position.y);
        if (Mathf.Abs(position.y) > bounds.y)
            position = new Vector2(position.x, Mathf.Clamp(position.y * -1, -bounds.y, bounds.y));
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
