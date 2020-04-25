using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Boid
{
    [Range(0f, 10f)] public float playerMaxSpeed;
    [Range(0f, 1f)] public float accelerateFactor;
    [Range(0f, 1f)] public float brakeFactor;
    [Range(0f, 1f)] public float steerFactor;
    [Range(0f,10f)] public float drag;

    private float boidMaxSpeed;

    public override void SetParameters(float perceptionRadius, float maxSpeed, Vector2 bounds, float alignment, float cohesion, float separation)
    {
        boidMaxSpeed = maxSpeed;
        base.SetParameters(perceptionRadius, playerMaxSpeed, bounds, alignment, cohesion, separation);
    }

    public override void CalculateAcceleration(List<Boid> flock, List<Vector2> avoidPoints = null)
    {
        float accelerate = Input.GetAxisRaw("Vertical");
        float steer = Input.GetAxisRaw("Horizontal");
        bool braking = accelerate < 0;

        if (Input.anyKey)
        {
            maxSpeed = playerMaxSpeed;

            Vector2 direction = (velocity.magnitude > 0.001) ? velocity.normalized : Vector2.up;
            Vector2 normal = new Vector2(direction.y, -direction.x);

            acceleration = direction * accelerate * (braking ? brakeFactor * velocity.magnitude : maxSpeed * accelerateFactor);
            acceleration += normal * steer * steerFactor * velocity.magnitude;
            acceleration /= Time.deltaTime;

            //Flip based on steering
            //flipByVelocity = false;
            //Vector3 lookAtPos = new Vector3(transform.position.x + velocity.x, transform.position.y, transform.position.z + velocity.y);
            //transform.LookAt(lookAtPos, Vector3.up);
        }
        else
        {
            maxSpeed = boidMaxSpeed;

            flipByVelocity = true;
            base.CalculateAcceleration(flock, avoidPoints);
        }
    }

    public override void DoMovement()
    {
        // Do usual movement calcs for boids
        base.DoMovement();

        // Also add extra drag
        velocity = Vector2.Lerp(velocity, Vector2.zero, drag * Time.deltaTime);
    }
}
