using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Boid
{
    [Range(0f, 10f)] public float playerMaxSpeed;
    [Range(0f, 1f)] public float accelerateFactor;
    [Range(0f, 1f)] public float brakeFactor;
    [Range(0f, 1f)] public float steerFactor;
    [Range(0f,10f)] public float drag;

    public override void SetParameters(float perceptionRadius, float maxSpeed, float speedVariation, Vector2 bounds, float alignment, float cohesion, float separation)
    {
        base.SetParameters(perceptionRadius, maxSpeed, speedVariation, bounds, alignment, cohesion, separation);

        SetSpeedInRange(0.4f);
    }

    public override void CalculateAcceleration(List<Boid> flock, List<AvoidPoint> avoidPoints = null)
    {
        float accelerate = Input.GetAxisRaw("Vertical");
        float steer = Input.GetAxisRaw("Horizontal");
        bool braking = accelerate < 0;

        if (Input.anyKey)
        {
            SetSpeedAbsolute(playerMaxSpeed);

            Vector2 direction = (velocity.magnitude > 0.001) ? velocity.normalized : Vector2.up;
            Vector2 normal = new Vector2(direction.y, -direction.x);

            acceleration = direction * accelerate * (braking ? brakeFactor * velocity.magnitude : maxSpeed * accelerateFactor);
            acceleration += normal * steer * steerFactor * velocity.magnitude;
            acceleration /= Time.deltaTime;
        }
        else
        {
            SetSpeedInRange(0.4f);

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

    public override void Kill() {
        int sceneId = SceneManager.GetActiveScene().buildIndex;
        Debug.Log("Player Died!!!!");
        SceneManager.LoadScene(sceneId);

    }
}
