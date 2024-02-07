using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace YesserOthmene
{
    public class Bake : MonoBehaviour
    {
        NavMeshSurface navMeshSurface;
        private void Awake()
        {
            navMeshSurface = GetComponent<NavMeshSurface>();
            navMeshSurface.BuildNavMesh();
        }
    }
}