using UnityEngine;
using System.Collections.Generic;

public class ObjectGenerator : MonoBehaviour
{
    public float treeDensity = .65f;

    public void GenerateTrees(int size,float[,] heightMap,GameObject prefabToSpawn, MeshSettings meshSettings, Vector2 position, HeightMapSettings settings, Vector2 sampleCenter) 
    {
        Transform parentTrransform = transform.parent;
        Vector2 topLeft = new Vector2(-1, 1) * meshSettings.meshWorldSize / 2f;

        float[,] noiseMap = Noise.GenerateNoiseMap(size, size, settings.noiseSettings, sampleCenter);

        for (int y = 0; y < size; y++) {
            for(int x = 0; x < size; x++) {
                float v2 = Random.Range(0, 0.75f);
                if ((noiseMap[x, y] > treeDensity) &&(noiseMap[x, y] < v2)) 
                {
                    Vector2 percent = new Vector2(x - 1, y - 1) / (meshSettings.numVertsPerLine - 3);
                    Vector2 vertexPosition2D = topLeft + new Vector2(percent.x, -percent.y) * meshSettings.meshWorldSize;

                     GameObject tree = Instantiate(prefabToSpawn, transform);

                    tree.transform.position = new Vector3( position.x+ vertexPosition2D.x, heightMap[x, y], position.y+ vertexPosition2D.y);
                    tree.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
                    tree.transform.localScale = Vector3.one * Random.Range(.8f, 1.2f);
                }
            }
        }
    }
}
