using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ZebraInfo : MonoBehaviour
{
    string infoText = "Remaining Zebras:\n{0}";

    public TextMeshProUGUI text;

    [HideInInspector] public Flock flock;
    int flockCount = 0;

    public void Start()
    {
        ReferenceManager.GetReferences(this);
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (flock.boids.Count != flockCount)
        {
            flockCount = flock.boids.Count;
            text.text = string.Format(infoText, flockCount);
        }
    }

}
