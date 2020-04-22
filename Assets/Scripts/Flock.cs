using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public GameObject boidPrefab;
    [Range(1,100)] public int flockSize = 1;
    [Range(0f, 20f)] public float boundsX = 10;
    [Range(0f, 20f)] public float boundsY = 5;

    public Player player;
    public Camera mainCamera;

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
            GameObject boidObject = Instantiate(boidPrefab, GetRandomXZPosition(3), Quaternion.identity, transform);
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
        // Calculate the next update
        for (int i = 0; i < boids.Count; i++)
            boids[i].CalculateAcceleration(boids);

        // Execute the update
        for (int i = 0; i < boids.Count; i++)
            boids[i].DoMovement();
    }

    private void InitBoid(Boid boid)
    {
        boid.Init(this, mainCamera);
        SetBoidBehaviorParameters(boid);
    }

    private void SetBoidBehaviorParameters(Boid boid)
    {
        boid.SetParameters(boidPerceptionRadius, boidMaxSpeed, new Vector2(boundsX, boundsY),
            boidAlignment, boidCohesion, boidSeparation);
    }

    private Vector3 GetRandomXZPosition(float radius)
    {
        Vector3 randomPos = Random.insideUnitSphere * radius;
        randomPos.y = 0;

        return randomPos;
    }

    private void RandomizeColor(GameObject boidObject)
    {
        SpriteRenderer[] renderers = boidObject.GetComponentsInChildren<SpriteRenderer>();
        Color randomColor = Random.ColorHSV(0, .2f, 0, .25f, 0.5f, 1f);
        foreach (SpriteRenderer r in renderers)
            r.color = randomColor;
    }
}
