using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public Player player;
    //public Camera camera;
    public float speed;
    public Transform planesParent;

    private Renderer[] planes;

    private void Start()
    {
        planes = planesParent.GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, player.transform.position, 1);// speed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(15, Mathf.Atan2(player.velocity.x, player.velocity.y) * Mathf.Rad2Deg, 0);

        planesParent.position = new Vector3(transform.position.x, -0.2f, transform.position.z);
        Vector2 offset = new Vector2(transform.position.x, transform.position.z) /-10;
        foreach (Renderer r in planes)
            r.material.SetTextureOffset("_MainTex", offset);
    }
}
