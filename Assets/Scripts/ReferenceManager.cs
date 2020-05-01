using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ReferenceManager : MonoBehaviour
{
    #region References
    [Header("Scene References")]
    public Player player;
    public Flock flock;
    public Camera mainCamera;
    public Transform cameraParent;
    public Transform planesParent;
    public Transform levelGoal;
    public GameManager gameManager;

    [Header("Prefab References")]
    public GameObject boidPrefab;
    public GameObject bloodParticlesPrefab;

    #endregion


    #region Reference Management
    // Singleton
    private static ReferenceManager instance;
    private void Awake()
    {
        SetUpReferences();

        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    // Static Method for Assigning References
    public static void GetReferences(object target, bool errorOnNullRef = true)
    {
        if (instance == null)
        {
            Debug.LogError("Reference Manager Can't Get References - instance is null");
            return;
        }

        if (target == null)
        {
            Debug.LogError("Reference Manager Can't Get References - no target object");
            return;
        }

        Type targetType = target.GetType();
        Reference reference;
        FieldInfo field;
        for (int i = 0; i < instance.references.Count; i++)
        {
            reference = instance.references[i];

            field = targetType.GetField(reference.name);
            if (field != null && field.FieldType == reference.type)
            {
                if (errorOnNullRef && reference.value.Equals(null))
                    Debug.LogError("Reference Manager Field Assignment Failed: reference for '" + reference.name + "' is null");

                field.SetValue(target, reference.value);
            }
        }
    }

    private List<Reference> references;

    private void SetUpReferences()
    {
        references = new List<Reference>();

        Type rmType = typeof(ReferenceManager);
        FieldInfo[] rmFields = rmType.GetFields();

        for (int i = 0; i < rmFields.Length; i++)
        {
            Reference reference = new Reference();
            reference.name = rmFields[i].Name;
            reference.type = rmFields[i].FieldType;
            reference.value = rmFields[i].GetValue(this);

            references.Add(reference);

            if (reference.value.Equals(null))
            {
                Debug.LogWarning("Reference Manager: reference for '" + reference.name + "' is null - this will probably cause errors");
            }
        }
    }

    private class Reference
    {
        public string name;
        public Type type;
        public object value;
    }
#endregion
}
