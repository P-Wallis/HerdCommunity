﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AvoidPoint = Boid.AvoidPoint;

public class Flock : MonoBehaviour
{
    // Reference Manager Fields
    [HideInInspector] public GameObject boidPrefab;
    [HideInInspector] public GameObject bloodParticlesPrefab;
    [HideInInspector] public Player player;
    [HideInInspector] public Camera mainCamera;
    [HideInInspector] public LevelGoal levelGoal;

    [Header("Flock Parameters")]
    [Range(1,100)] public int flockSize = 1;
    [Range(0f, 20f)] public float boundsX = 10;
    [Range(0f, 20f)] public float boundsY = 5;
    [Range(0f, 10f)] public float startRadius = 3;
    public bool constrainBoidsToBounds;

    [Header("Boid Parameters")]
    [Range(0.01f, 10)] public float boidPerceptionRadius = 1;
    [Range(0.01f, 20)] public float boidMaxSpeed = 1;
    [Range(0f, 1f)] public float boidSpeedVariation = 0.5f;
    [Range(0f, 10f)] public float boidRotateSpeed = 8;
    [Range(0f, 2f)] public float boidAlignment = 1;
    [Range(0f, 2f)] public float boidCohesion = 1;
    [Range(0f, 2f)] public float boidSeparation = 1;

    public List<Boid> boids = new List<Boid>();
    private List<Boid> deadBoids = new List<Boid>();
    protected List<AvoidPoint> avoidPoints = new List<AvoidPoint>();

#if UNITY_EDITOR
    private void OnValidate()
    {
        foreach (Boid b in boids)
            SetBoidBehaviorParameters(b);
    }

    private void OnDrawGizmos()
    {
        if (avoidPoints != null && avoidPoints.Count>0)
        {
            for (int i = 0; i < avoidPoints.Count; i++)
            {
                Gizmos.color = avoidPoints[i].Weight < 0.01f ? Color.gray : Color.Lerp(Color.blue, Color.red, avoidPoints[i].Weight);
                Gizmos.DrawSphere(avoidPoints[i].WorldPosition, 0.5f);
            }
        }

        Gizmos.color = Color.black;
        for (int i = 0; i < boids.Count; i++)
        {
            //Gizmos.DrawSphere(boids[i].transform.position, .25f);
        }
    }
#endif

    private void Start()
    {
        // Assign references via reflection
        ReferenceManager.GetReferences(this);

        for (int i = 0; i < flockSize; i++)
        {
            GameObject boidObject = Instantiate(boidPrefab, GetRandomXZPosition(startRadius), Quaternion.identity, transform);
            RandomizeColor(boidObject);
            boidObject.name = "boid "+ i;
            Boid boid = boidObject.AddComponent<Boid>();
            InitBoid(boid);
            boid.velocity = Random.insideUnitCircle * boidMaxSpeed;
            boids.Add(boid);
        }

        if (player != null)
        {
            InitBoid(player);
            boids.Add(player);
        }
    }

    private void Update()
    {
        // Handle Dead Boids
        for (int i = 0; i < deadBoids.Count; i++)
        {
            boids.Remove(deadBoids[i]);
            deadBoids[i].PerformDeathActions();
        }
        deadBoids.Clear();

        // Calculate the next update
        for (int i = 0; i < boids.Count; i++)
        {
            boids[i].CalculateAcceleration(boids, avoidPoints);
        }

        // Execute the update
        for (int i = 0; i < boids.Count; i++)
        {
            boids[i].DoMovement();
        }
    }

    public void KillBoid(Boid boid)
    {
        deadBoids.Add(boid);
        boid.Kill();
    }

    private void InitBoid(Boid boid)
    {
        boid.Init(this, mainCamera, bloodParticlesPrefab, levelGoal.transform);
        SetBoidBehaviorParameters(boid);
    }

    private void SetBoidBehaviorParameters(Boid boid)
    {
        boid.SetParameters(
            boidPerceptionRadius,
            boidMaxSpeed,
            boidSpeedVariation,
            boidRotateSpeed,
            new Vector2(boundsX, boundsY),
            boidAlignment,
            boidCohesion,
            boidSeparation
            );
    }

    private Vector3 GetRandomXZPosition(float radius)
    {
        Vector3 randomPos = Random.insideUnitSphere * radius;
        randomPos.y = 0;

        return randomPos;
    }

    private void RandomizeColor(GameObject boidObject)
    {
        Renderer[] renderers = boidObject.GetComponentsInChildren<Renderer>();
        Color randomColor = Random.ColorHSV(0, .2f, 0, .25f, 0f, 1f);
        foreach (Renderer r in renderers)
            r.material.color = randomColor;
    }

    public AvoidPoint AddAvoidPoint(Transform transform, float weight = 1)
    {
        AvoidPoint avoidPoint = new AvoidPoint(transform, weight);
        avoidPoints.Add(avoidPoint);
        return avoidPoint;
    }
}
