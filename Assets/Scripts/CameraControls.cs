﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    // Reference Manager Fields
    [HideInInspector] public Player player;
    [HideInInspector] public Transform planesParent;

    [Range(0,10)] public float speed;
    private Renderer[] planes;

    private void Start()
    {
        ReferenceManager.GetReferences(this);
        planes = planesParent.GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
        float t = speed * Time.deltaTime;

        // Position (based on player position)
        transform.position = Vector3.Lerp(transform.position, player.transform.position, t);

        // Rotation (based on player facing direction)
        Quaternion currentRotation = transform.rotation;
        Quaternion targetCameraRotation = Quaternion.Euler(15, player.facingAngle, 0);

        transform.rotation = Quaternion.Lerp(currentRotation, targetCameraRotation, t);

        // Update Ground Plane Textures
        planesParent.position = new Vector3(transform.position.x, -0.05f, transform.position.z);
        Vector2 offset = new Vector2(transform.position.x, transform.position.z) / -3;
        foreach (Renderer r in planes)
            r.material.SetTextureOffset("_MainTex", offset);
    }
}
