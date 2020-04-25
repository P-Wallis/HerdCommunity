using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public Vector2 position // Mostly a 2D shorcut for transform.position, but using z as the y axis
    {
        get { return new Vector2(transform.position.x, transform.position.z); }
        set { transform.position = new Vector3(value.x, transform.position.y, value.y); }
    }
    [HideInInspector] public Vector2 velocity;
    [HideInInspector] public Vector2 acceleration;
    [HideInInspector] public Vector2 bounds;

    protected float perceptionRadius;
    protected float maxSpeed;
    protected float alignmentFactor;
    protected float cohesionFactor;
    protected float separationFactor;
    protected Flock flock;
    protected bool flipByVelocity = true;
    protected Vector3 scale;

    private Transform cameraTransform;
    private Transform cameraParent;
    private bool markedForDeath = false;
    private GameObject deathParticles;

    private void OnMouseDown()
    {
        Kill(); // Just for testing, kill the gazelle when it's clicked
    }

    public virtual void Init(Flock flock, Camera camera, GameObject deathParticles)
    {
        this.flock = flock;
        cameraTransform = camera!=null ? camera.transform : null;
        cameraParent = cameraTransform != null ? cameraTransform.parent : null;
        scale = transform.localScale;
        this.deathParticles = deathParticles;
    }

    public virtual void SetParameters(float perceptionRadius, float maxSpeed, Vector2 bounds, float alignment, float cohesion, float separation)
    {
        this.perceptionRadius = perceptionRadius;
        this.maxSpeed = maxSpeed;
        this.bounds = bounds;

        alignmentFactor = alignment;
        cohesionFactor = cohesion;
        separationFactor = separation;
    }

    public virtual void PerformDeathActions()
    {
        if (deathParticles != null)
        {
            Destroy(Instantiate(deathParticles, transform.position + (Vector3.up * 0.25f), transform.rotation, transform.parent), 1f);
        }
        Destroy(gameObject);
    }

    public virtual void CalculateAcceleration(List<Boid> flock, List<Vector2> avoidPoints = null)
    {
        List<Boid> localBoids = GetLocalBoids(flock);
        acceleration = Vector2.zero;
        acceleration += GetAlignment(localBoids) * alignmentFactor;
        acceleration += GetCohesion(localBoids) * cohesionFactor;
        acceleration += GetSeparation(localBoids) * separationFactor;
        acceleration += GetCollisionAvoidance(avoidPoints);
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

        // Rotate to face movement direction
        if (flipByVelocity)
        {
            Vector3 lookAtPos = new Vector3(transform.position.x + velocity.x, transform.position.y, transform.position.z + velocity.y);
            transform.LookAt(lookAtPos, Vector3.up);
        }

        // Optionally, Confine to Bounds
        if (flock.constrainBoidsToBounds)
            ConstrainToBounds();
    }

    public void Kill()
    {
        bool wasMarkedForDeath = markedForDeath;
        markedForDeath = true;

        if (!wasMarkedForDeath)
            flock.KillBoid(this);
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

    protected Vector2 GetCollisionAvoidance(List<Vector2> avoidPoints)
    {
        Vector2 collision = Vector2.zero;

        if (avoidPoints != null && avoidPoints.Count > 0)
        {
            float distance;
            Vector2 direction;
            for (int i = 0; i < avoidPoints.Count; i++)
            {
                distance = Vector2.Distance(position, avoidPoints[i]);
                if (distance < (perceptionRadius * 2))
                {
                    direction = (position - avoidPoints[i]).normalized;
                    direction *= perceptionRadius / distance;
                    collision += direction;
                }
            }
        }
        return collision;
    }

    private void ConstrainToBounds()
    {
        //Keep Boids In Bounds
        if (cameraParent != null)
        {
            // make bounds relative to the camera's position
            Vector3 camPos = cameraParent.InverseTransformPoint(transform.position);
            bool setPos = false;

            if (Mathf.Abs(camPos.x) > bounds.x)
            {
                setPos = true;
                camPos.x = Mathf.Clamp(camPos.x * -1, -bounds.x, bounds.x);
            }
            if (Mathf.Abs(camPos.z) > bounds.y)
            {
                setPos = true;
                camPos.z = Mathf.Clamp(camPos.z * -1, -bounds.y, bounds.y);
            }

            if (setPos)
            {
                Vector3 newPos = cameraParent.TransformPoint(camPos);
                newPos.y = 0;
                transform.position = newPos;
            }
        }
        else
        {
            // If no camera is present, use bounds based on global coordinates
            if (Mathf.Abs(position.x) > bounds.x)
                position = new Vector2(Mathf.Clamp(position.x * -1, -bounds.x, bounds.x), position.y);
            if (Mathf.Abs(position.y) > bounds.y)
                position = new Vector2(position.x, Mathf.Clamp(position.y * -1, -bounds.y, bounds.y));
        }
    }
}
