using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class TerrainGenerator : MonoBehaviour
{
    const float viewerMoveThreasholdForChunkUpdate = 15f;
    const float sqrViewerMoveThreasholdForChunkUpdate = viewerMoveThreasholdForChunkUpdate * viewerMoveThreasholdForChunkUpdate;

    public int ColliderLODIndex;
    public LODInfo[] detailLevels;

    [Space]

    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureData textureSettings;

    [Space]

    public Transform viewer;
    public Material mapMaterial;

    Vector2 viewerPosition;
    Vector2 viewerPositionOld;

    float meshWorldSize;
    int chunksVisibleInViewDistance;

    [Header("Max Map Chunks")]
    [SerializeField]Vector2 maxChunkCoords;
    [SerializeField] int mapChunks;

    [Header("Prefabs to spawn")]
    [SerializeField] GameObject tree;

    Dictionary<Vector2,TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2,TerrainChunk>();
    List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();

    Dictionary<Vector2, GameObject> treeChunkDictionary = new Dictionary<Vector2, GameObject>();


    private void Start()
    {
        heightMapSettings.noiseSettings.seed = Random.Range(-10000, 10000);
        textureSettings.ApplyToMaterial(mapMaterial);
        textureSettings.UpdateMeshHeights(mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

        float maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDistanceThreshold;
        meshWorldSize = meshSettings.meshWorldSize;
        chunksVisibleInViewDistance =Mathf.RoundToInt(maxViewDistance / meshWorldSize);

        UpdateVisibleChunks();
        mapChunks = (2 * (Mathf.RoundToInt(maxChunkCoords.x))+1) * (2 * (Mathf.RoundToInt(maxChunkCoords.y))+1);
    }

    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

        if (viewerPosition != viewerPositionOld)
        {
            foreach (TerrainChunk chunk in visibleTerrainChunks)
            {
                chunk.UpdateCollisionMesh();
            }
        }

        if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThreasholdForChunkUpdate)
        {
            viewerPositionOld= viewerPosition;
            UpdateVisibleChunks();
        }
    }

    void UpdateVisibleChunks()
    {
        HashSet<Vector2> alreadyUpdatedChunkCoord = new HashSet<Vector2>();
        for (int i = visibleTerrainChunks.Count-1; i >= 0; i--)
        {
            alreadyUpdatedChunkCoord.Add(visibleTerrainChunks[i].coord);
            visibleTerrainChunks[i].UpdateTerrainChunk();
        }

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshWorldSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshWorldSize);

        for (int yOffset = -chunksVisibleInViewDistance; yOffset <= chunksVisibleInViewDistance; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDistance; xOffset <= chunksVisibleInViewDistance; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
                bool canSpawnChunk = viewedChunkCoord.x >= -maxChunkCoords.x && viewedChunkCoord.x <= maxChunkCoords.x && viewedChunkCoord.y >= -maxChunkCoords.y && viewedChunkCoord.y <= maxChunkCoords.y;
                if (canSpawnChunk)
                {
                    if (!alreadyUpdatedChunkCoord.Contains(viewedChunkCoord))
                    {
                        if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                        {
                            terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                        }
                        else
                        {
                            TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, detailLevels, ColliderLODIndex, transform.GetChild(0), viewer, mapMaterial, tree);
                            terrainChunkDictionary.Add(viewedChunkCoord, newChunk);
                            newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged;
                            newChunk.Load();
                        }
                    }
                }
            }
        }
    }

    void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible)
    {
        if (isVisible)
        {
            visibleTerrainChunks.Add(chunk);
        }
        else
        {
            visibleTerrainChunks.Remove(chunk);
        }
    }
}
[System.Serializable]
public struct LODInfo
{
    [Range(0f, MeshSettings.numSupportedLODs - 1)]
    public int lod;
    public float visibleDistanceThreshold;

    public float sqeVisibleDstThreshold
    {
        get
        {
            return visibleDistanceThreshold * visibleDistanceThreshold;
        }
    }
}
