using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoal : MonoBehaviour
{

	[Range(0f, 20f)] public float goalTimeMins = 10;
	public Player player;
    // Start is called before the first frame update
    void Start()
    {
    	ReferenceManager.GetReferences(this);
    	float goalDistance = (goalTimeMins*60)*player.playerMaxSpeed;
    	Vector2 goalDirection = Random.insideUnitCircle.normalized;
        transform.position = new Vector3(goalDirection.x, 0, goalDirection.y)*goalDistance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
