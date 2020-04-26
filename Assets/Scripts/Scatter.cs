using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scatter : MonoBehaviour
{
    [System.Serializable]
    public struct ScatterObjectData
    {
        [HideInInspector] public string name;
        public GameObject prefab;
        [Range(0, 100)] public int count;
        public bool randomizeRotation;
    }

    private class ScatterPositionResetter : MonoBehaviour
    {
        public Vector2 bounds;
        public Vector2 padding;
        public Transform cameraParent;

        private void Update()
        {
            KeepInBounds();
        }

        void KeepInBounds()
        {
            if (cameraParent != null)
            {
                Vector3 camPos = cameraParent.InverseTransformPoint(transform.position);
                bool setPos = false;

                if (Mathf.Abs(camPos.x) > (bounds.x + padding.x))
                {
                    setPos = true;
                    camPos.x = Mathf.Clamp(camPos.x * -1, -bounds.x, bounds.x);
                }
                if (Mathf.Abs(camPos.z) > (bounds.y + padding.y))
                {
                    setPos = true;
                    camPos.z = Mathf.Clamp(camPos.z * -1, -bounds.y, bounds.y);
                }

                if (setPos)
                {
                    Vector3 newPos = cameraParent.TransformPoint(camPos);
                    newPos.y = 0;
                    transform.position = newPos;
                }
            }
        }
    }

    public Transform cameraParent;
    [Range(0f, 20f)] public float scatterAreaX = 10;
    [Range(0f, 20f)] public float scatterAreaY = 5;
    public List<ScatterObjectData> scatterObjects = new List<ScatterObjectData>();

#if UNITY_EDITOR
    private void OnValidate()
    {
        ScatterObjectData data;
        for (int i = 0; i < scatterObjects.Count; i++)
        {
            data = scatterObjects[i];
            data.name = data.prefab != null ? data.prefab.name : "[Unassigned]";
            scatterObjects[i] = data;
        }
    }
#endif

    void Start()
    {
        // Scatter
        ScatterObjectData data;
        for (int i = 0; i < scatterObjects.Count; i++)
        {
            data = scatterObjects[i];
            if (data.prefab == null || data.count<1)
                continue;

            for (int j = 0; j < data.count; j++)
            {
                GameObject instance = Instantiate(
                    data.prefab,
                    GetScatterPosition(),
                    GetScatterRotation(data.randomizeRotation),
                    transform);

                ScatterPositionResetter spr = instance.AddComponent<ScatterPositionResetter>();
                spr.bounds = new Vector2(scatterAreaX, scatterAreaY);
                spr.padding = Vector2.one;
                spr.cameraParent = cameraParent;
            }
        }
    }

    Vector3 GetScatterPosition()
    {
        return new Vector3(Random.Range(-scatterAreaX, scatterAreaX), 0, Random.Range(-scatterAreaY, scatterAreaY));
    }

    Quaternion GetScatterRotation(bool randomize)
    {
        if (randomize)
            return Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        return Quaternion.identity;
    }
}
