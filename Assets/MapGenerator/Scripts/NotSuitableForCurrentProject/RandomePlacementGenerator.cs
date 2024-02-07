using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace YesserOthmene
{
    public class RandomePlacementGenerator : MonoBehaviour
    {
        public void Generate(RandomPlacementData data)
        {
            Clear();
            for (int i = 0; i < data.density; i++)
            {
                Debug.Log("the count i ="+i);

                float sampleX = Random.Range(data.xRange.x, data.xRange.y);
                float sampleY = Random.Range(data.zRange.x, data.zRange.y);

                Vector3 rayStart = new Vector3(sampleX, data.maxHeight, sampleY);
                Debug.Log(rayStart);
                if (!Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, Mathf.Infinity))
                    continue;
                Debug.Log("1st continue"+hit.collider.name);
                if (hit.point.y < data.minHeight)
                    continue;
                Debug.Log("2st continue");

                GameObject instantiatePrefab = (GameObject)PrefabUtility.InstantiatePrefab(data.prefabToSpawn, transform);
                instantiatePrefab.transform.position = hit.point;
                instantiatePrefab.transform.Rotate(Vector3.up, Random.Range(data.rotationRange.x, data.rotationRange.y), Space.Self);
                instantiatePrefab.transform.rotation= Quaternion.Lerp(transform.rotation,transform.rotation* Quaternion.FromToRotation(instantiatePrefab.transform.up,hit.normal),data.rotaionTowardsNormal);
                instantiatePrefab.transform.localScale = new Vector3(
                    Random.Range(data.minScale.x, data.maxScale.x),
                    Random.Range(data.minScale.y, data.maxScale.y),
                    Random.Range(data.minScale.z, data.maxScale.z)
                    );
            }
        }

        private void Clear()
        {
            Debug.Log("Cleared");
        }

    }
}
