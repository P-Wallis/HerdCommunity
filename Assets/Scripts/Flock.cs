using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public GameObject boidPrefab;
    [Range(1,100)] public int flockSize = 1;
    [Range(0f, 20f)] public float boundsX = 10;
    [Range(0f, 20f)] public float boundsY = 5;

    [Header("Boid Parameters")]
    [Range(0.01f, 10)] public float boidPerceptionRadius = 1;
    [Range(0.01f, 20)] public float boidMaxSpeed = 1;
    [Range(0f, 1f)] public float boidAlignment = 1;
    [Range(0f, 1f)] public float boidCohesion = 1;
    [Range(0f, 1f)] public float boidSeparation = 1;

#if UNITY_EDITOR
    private void OnValidate()
    {
        foreach (Boid b in boids)
            SetBoidBehaviorParameters(b);
    }
#endif


    private List<Boid> boids = new List<Boid>();

    private void Start()
    {
        for (int i = 0; i < flockSize; i++)
        {
            Vector3 randomPos = Random.insideUnitSphere * 3f;
            randomPos.y = 0;
            GameObject boidObject = Instantiate(boidPrefab, randomPos, Quaternion.identity, transform);
            boidObject.name = "boid "+ i;
            Boid boid = boidObject.AddComponent<Boid>();
            SetBoidBehaviorParameters(boid);
            boid.velocity = Random.insideUnitCircle * boidMaxSpeed;
            boids.Add(boid);
        }
    }

    private void Update()
    {
        // Calculate the next update
        for (int i = 0; i < boids.Count; i++)
            boids[i].CalculateAcceleration(boids);

        // Execute the update
        for (int i = 0; i < boids.Count; i++)
            boids[i].DoMovement();
    }

    private void SetBoidBehaviorParameters(Boid boid)
    {
        boid.perceptionRadius = boidPerceptionRadius;
        boid.maxSpeed = boidMaxSpeed;
        boid.alignmentFactor = boidAlignment;
        boid.cohesionFactor = boidCohesion;
        boid.separationFactor = boidSeparation;
        boid.bounds = new Vector2(boundsX, boundsY);
    }
}
