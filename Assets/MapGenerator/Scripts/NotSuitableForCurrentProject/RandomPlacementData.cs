using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPlacementData : ScriptableObject
{
    public GameObject prefabToSpawn;

    [Header("Ray Cast Settings")]
    public int density;

    [Space]

    public float minHeight;
    public float maxHeight;
    public Vector2 xRange;
    public Vector2 zRange;

    [Header("Prefab Variation Settings")]
    
    [Range(0, 1)] public float rotaionTowardsNormal;
    public Vector2 rotationRange;
    public Vector3 minScale;
    public Vector3 maxScale;
}
