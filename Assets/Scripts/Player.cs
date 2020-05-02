using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Boid
{
    [HideInInspector] public GameManager gameManager;

    [Range(0f, 10f)] public float accelerateFactor;
    [Range(0f, 180f)] public float steerRate;
    [Range(0f, 10f)] public float playerMaxSpeed;
    [Range(0f, 10f)] public float drag;

    [HideInInspector] public float facingAngle = 0;

    bool allowInput = true;

    public override void Init(Flock flock, Camera camera, GameObject deathParticles, Transform levelGoal)
    {
        base.Init(flock, camera, deathParticles, levelGoal);
        ReferenceManager.GetReferences(this);
    }

    public override void SetParameters(float perceptionRadius, float maxSpeed, float speedVariation, float rotateSpeed, Vector2 bounds,
        float alignment, float cohesion, float separation)
    {
        base.SetParameters(perceptionRadius, maxSpeed, speedVariation, rotateSpeed, bounds, alignment, cohesion, separation);

        SetSpeedAbsolute(playerMaxSpeed);
    }

    public override void CalculateAcceleration(List<Boid> flock, List<AvoidPoint> avoidPoints = null)
    {
        // Get Input
        float accelerate = allowInput ? Input.GetAxisRaw("Vertical") : 0;
        float steer = allowInput ? Input.GetAxisRaw("Horizontal") : 0;

        // Do steering
        facingAngle = ConstrainToAngleRange(facingAngle + (steer * steerRate * Time.deltaTime));

        // Update velocity based on steering
        Vector2 direction = GetDirectionFromAngle(facingAngle);
        float speed = velocity.magnitude;
        if (speed > .5f)
            velocity = speed * direction; // only update velocity if it's not tiny (to prevent the zebra sliding around oddly)

        // Set acceleration based on facing direction and accelerate button
        acceleration = direction * Mathf.Clamp01(accelerate) * playerMaxSpeed * accelerateFactor;

        // Slow down when acclelerate not pressed
        velocity *= (accelerate <= 0) ? 1 - (drag * Time.deltaTime) : 1;

        // Play animation based on speed
        animator.speed = Mathf.Clamp01(speed);
        animator.SetFloat(runAnimationFloat, (speed-1) / (playerMaxSpeed - 1));
    }

    public override void Kill()
    {
        velocity = Vector2.zero;
        allowInput = false;
        gameManager.EndGame();

        transform.GetChild(0).gameObject.SetActive(false); // turn off zebra model

        if (deathParticles != null)
        {
            Destroy(Instantiate(deathParticles, transform.position + (Vector3.up * 0.25f), transform.rotation, transform.parent), 1f);
        }
    }

    float ConstrainToAngleRange(float input)
    {
        if (input < -180)
            return input + 360;
        if (input > 180)
            return input - 360;
        return input;
    }

    Vector2 GetDirectionFromAngle(float angle)
    {
        float radAngle = -angle * Mathf.Deg2Rad;
        return new Vector2(-Mathf.Sin(radAngle), Mathf.Cos(radAngle));
    }
}
