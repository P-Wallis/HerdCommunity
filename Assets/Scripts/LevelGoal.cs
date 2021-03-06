﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoal : MonoBehaviour
{
    // Reference Manager Field
    [HideInInspector] public Player player;
    [HideInInspector] public GameManager gameManager;
    [HideInInspector] public GameObject oasisPrefab;

    // Inspector Fields
    [Range(0f, 20f)] public float goalTimeMins = 10;
	[Range(0f, 20f)] public float goalDevation = 10;
	[Range(0f, 20f)] public float dectionRadius = 10;
    [Range(0f, 20f)] public float oasisDectionRadius = 10;
    [Range(0, 20)] public int numberOfPoints = 10;

    // Private Variables
	private List<Vector3> goals = new List<Vector3>();
	private int currentGoalIndex = -1;
    private GameObject oasis;

    // Properies
    public Vector3 oasisPosition { get; private set; }

    void Start()
    {
    	ReferenceManager.GetReferences(this);

        // Get Zebra Path
    	float goalDistance = goalTimeMins*60*player.playerMaxSpeed;
    	Vector2 goalDirection = Random.insideUnitCircle.normalized;
        Vector3 levelGoal = new Vector3(goalDirection.x, 0, goalDirection.y)*goalDistance;
        Vector3 normalDirection = new Vector3(goalDirection.y, 0, -goalDirection.x);
        int oddOrEven = Mathf.RoundToInt(Random.value);
        for (int i = 0; i < numberOfPoints; i++) {
        	float fraction = i/(float) numberOfPoints;
        	Vector3 currectPoint = levelGoal*fraction;
        	Vector3 offset = normalDirection*goalDevation*Random.value;
        	if (i%2 == oddOrEven) currectPoint -= offset;
        	else currectPoint += offset;
        	goals.Add(currectPoint);
        }
        goals.Add(levelGoal);
        ChangePosition();

        // Instantiate Oasis
        oasisPosition = levelGoal;
        oasis = Instantiate(oasisPrefab, oasisPosition, Quaternion.identity);
    }

    void ChangePosition()
    {
    	currentGoalIndex++;
    	transform.position = goals[currentGoalIndex];
    }

    void Update()
    {
        float playerDistance = Vector3.Distance(player.transform.position, goals[currentGoalIndex]);
        if (currentGoalIndex == goals.Count - 1)
        {
            if(playerDistance < oasisDectionRadius)
                gameManager.WinGame();
        }
        else if(playerDistance < dectionRadius)
            ChangePosition();
    }
}
