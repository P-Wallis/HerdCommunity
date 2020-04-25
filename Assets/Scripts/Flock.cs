using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    [Header("Object References")]
    public GameObject boidPrefab;
    public GameObject bloodParticlesPrefab;
    public Player player;
    public Camera mainCamera;
    public Transform[] avoidTransforms;

    [Header("Flock Parameters")]
    [Range(1,100)] public int flockSize = 1;
    [Range(0f, 20f)] public float boundsX = 10;
    [Range(0f, 20f)] public float boundsY = 5;
    [Range(0f, 10f)] public float startRadius = 3;
    public bool constrainBoidsToBounds;

    [Header("Boid Parameters")]
    [Range(0.01f, 10)] public float boidPerceptionRadius = 1;
    [Range(0.01f, 20)] public float boidMaxSpeed = 1;
    [Range(0f, 1f)] public float boidAlignment = 1;
    [Range(0f, 1f)] public float boidCohesion = 1;
    [Range(0f, 1f)] public float boidSeparation = 1;

    public List<Boid> boids = new List<Boid>();
    private List<Boid> deadBoids = new List<Boid>();
    public List<Vector2> avoidPoints;

#if UNITY_EDITOR
    private void OnValidate()
    {
        foreach (Boid b in boids)
            SetBoidBehaviorParameters(b);
    }

    private void OnDrawGizmos()
    {
        if (avoidPoints != null)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < avoidPoints.Count; i++)
            {
                Gizmos.DrawSphere(new Vector3(avoidPoints[i].x, 0, avoidPoints[i].y), .5f);
            }
        }

        Gizmos.color = Color.black;
        for (int i = 0; i < boids.Count; i++)
        {
            Gizmos.DrawSphere(boids[i].transform.position, .25f);
        }
    }
#endif

    private void Start()
    {
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

        avoidPoints = GetAvoidPoints();
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
        boid.Init(this, mainCamera, bloodParticlesPrefab);
        SetBoidBehaviorParameters(boid);
    }

    private void SetBoidBehaviorParameters(Boid boid)
    {
        boid.SetParameters(boidPerceptionRadius, boidMaxSpeed, new Vector2(boundsX, boundsY), boidAlignment, boidCohesion, boidSeparation);
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

    private List<Vector2> GetAvoidPoints()
    {
        if(avoidTransforms.Length<1)
            return null;

        List<Vector2> points = new List<Vector2>();

        for (int i = 0; i < avoidTransforms.Length; i++)
        {
            if (avoidTransforms[i] == null)
                continue;

            points.Add(new Vector2(avoidTransforms[i].position.x, avoidTransforms[i].position.z));
        }

        /*
        // Testing Collision Circle 
        const float r = 20f;
        int num = Mathf.CeilToInt(Mathf.PI * 0.5f * r * boidPerceptionRadius);
        for (int i = 0; i < num; i++)
        {
            float theta = (2f * Mathf.PI * i) / num;
            points.Add(new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)) * r);
        }
        */
        return points;
    }
}
