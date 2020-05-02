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
        public bool randomizeScale;
        [Range(0, 1)] public float scaleRandomFactor;
        public bool addToHerdAvoidList;
        [Range(0,1)]public float avoidanceWeight;
    }

    private class ScatterPositionResetter : MonoBehaviour
    {
        public Vector2 bounds;
        public Vector2 oasisBounds;
        public Vector2 padding;
        public Transform cameraParent;
        public LevelGoal levelGoal;

        private void Start()
        {
            ReferenceManager.GetReferences(this);
        }

        private void Update()
        {
            KeepInBounds();
            HideInsideOasis();
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

        void HideInsideOasis()
        {
            if (levelGoal != null)
            {
                Vector3 oasisPos = transform.position - levelGoal.oasisPosition;

                bool show = (Mathf.Abs(oasisPos.x) > oasisBounds.x || Mathf.Abs(oasisPos.y) > oasisBounds.y);
                if (show != gameObject.activeSelf)
                    gameObject.SetActive(show);

            }
        }
    }

    public Flock flock;
    public Transform cameraParent;
    [Range(0f, 20f)] public float scatterAreaX = 10;
    [Range(0f, 20f)] public float scatterAreaY = 5;

    [Range(0f, 20f)] public float oasisSizeX = 10;
    [Range(0f, 20f)] public float oasisSizeY = 5;
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
                spr.oasisBounds = new Vector2(oasisSizeX, oasisSizeY);
                spr.padding = Vector2.one;
                spr.cameraParent = cameraParent;

                if (data.addToHerdAvoidList)
                    flock.AddAvoidPoint(instance.transform, data.avoidanceWeight);

                if(data.randomizeScale)
                    instance.transform.localScale = GetScatterSize(data.scaleRandomFactor, instance.transform.localScale);
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

    Vector3 GetScatterSize(float proportion, Vector3 originalScale)
    {
        return originalScale * Random.Range(1 - proportion, 1 + proportion);
    }
}