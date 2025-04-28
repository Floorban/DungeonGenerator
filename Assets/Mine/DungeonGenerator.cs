using System;
using System.Collections;
using System.Collections.Generic;
using deloneTriangulation;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Dungeon Settings")]
    public int roomRecursion = 15;
    public int borderSize = 20;
    public int roomMargin = 1;
    [Range(1, 10)][SerializeField] int minRoomWidth = 3;
    [Range(1, 10)][SerializeField] int maxRoomWidth = 7;
    [Range(1, 10)][SerializeField] int minRoomLength = 3;
    [Range(1, 10)][SerializeField] int maxRoomLength = 7;
    [Range(1, 10)] public int roomNumber = 3;

    [Header("Prefabs")]
    public GameObject floorPrefab;
    public Transform dungeonRoot;

    private int roomStartX;
    private int roomStartZ;
    private int roomWidth;
    private int roomLength;

    private HashSet<Vector3Int> occupiedTiles = new HashSet<Vector3Int>();
    private Vector3Int[][] rooms; // Array of room tiles arrays
    //private Vector3[] roomCenters;
    private HashSet<Vector3> roomCenters = new HashSet<Vector3>();
    public DebugDelone debugDelone;

    private void Awake()
    {
        rooms = new Vector3Int[roomNumber][]; 
        GenerateDungeon(roomNumber);
    }

    public void GenerateDungeon(int maxNum)
    {
        ClearDungeon();

        for (int i = 0; i < maxNum; i++)
        {
            if (!CreateRoom(0, i))
            {
                Debug.Log("Failed to create room after multiple attempts.");
            }
        }
        if (debugDelone != null)
        {
            List<Vector2> roomCenters2D = new List<Vector2>();
            foreach (Vector3 center in roomCenters)
            {
                roomCenters2D.Add(new Vector2(center.x, center.z)); // Convert to 2D
            }
            debugDelone.SetPoints(roomCenters2D);
        }
    }

    public void ClearDungeon()
    {
        if (dungeonRoot == null) return;

        foreach (Transform child in dungeonRoot)
        {
            Destroy(child.gameObject);
        }

        occupiedTiles.Clear();
        rooms = new Vector3Int[roomNumber][];  // Clear room tiles array
        //roomCenters = new Vector3[roomNumber];  // Clear room center positions array
        //roomCenters.Clear();
        roomCenters = new HashSet<Vector3>();
    }

    public bool CreateRoom(int currentAttempt, int roomIndex)
    {
        if (floorPrefab == null) return false;

        roomWidth = Random.Range(minRoomWidth, maxRoomWidth + 1);
        roomLength = Random.Range(minRoomLength, maxRoomLength + 1);

        int maxX = borderSize - roomWidth;
        int maxZ = borderSize - roomLength;

        roomStartX = Random.Range(0, maxX + 1);
        roomStartZ = Random.Range(0, maxZ + 1);

        // enable this to check the room spawn attempts
/*        GameObject roomParent = new GameObject($"Room_{roomStartX}_{roomStartZ}");
        roomParent.transform.SetParent(dungeonRoot);*/

        bool canPlaceRoom = true;

        for (int x = -roomMargin; x < roomWidth + roomMargin; x++)
        {
            for (int z = -roomMargin; z < roomLength + roomMargin; z++)
            {
                int spawnX = roomStartX + x;
                int spawnZ = roomStartZ + z;

                // Check if inside borders
                if (spawnX >= 0 && spawnX <= borderSize - 1 && spawnZ >= 0 && spawnZ <= borderSize - 1)
                {
                    // If any tile is already occupied, mark the room as not placeable
                    if (occupiedTiles.Contains(new Vector3Int(spawnX, 0, spawnZ)))
                    {
                        canPlaceRoom = false;
                        break;
                    }
                }
            }

            if (!canPlaceRoom) { break; }
        }

        if (canPlaceRoom)
        {
            GameObject roomParent = new GameObject($"Room_{roomStartX}_{roomStartZ}");
            roomParent.transform.SetParent(dungeonRoot);

            // An array to store the tiles for this room
            Vector3Int[] roomTiles = new Vector3Int[roomWidth * roomLength];

            for (int x = 0; x < roomWidth; x++)
            {
                for (int z = 0; z < roomLength; z++)
                {
                    int spawnX = roomStartX + x;
                    int spawnZ = roomStartZ + z;

                    Vector3 position = new Vector3(spawnX, 0, spawnZ);
                    GameObject tile = Instantiate(floorPrefab, position, Quaternion.identity);
                    tile.transform.SetParent(roomParent.transform);

                    Vector3Int tilePosition = new Vector3Int(spawnX, 0, spawnZ);
                    occupiedTiles.Add(tilePosition);
                    roomTiles[x * roomLength + z] = tilePosition;
                }
            }

            rooms[roomIndex] = roomTiles;
            float avg_x = roomStartX + (float)roomWidth / 2;
            float avg_z = roomStartZ + (float)roomLength / 2;
            //roomCenters[roomIndex] = new Vector3(avg_x, 0, avg_z);
            roomCenters.Add(new Vector3(avg_x, 0, avg_z));
            return true;
        }
        else
        {
            if (currentAttempt >= roomRecursion)
            {
                Debug.Log("Max room creation attempts reached. Skipping room placement.");
                return false; 
            }

            return CreateRoom(currentAttempt + 1, roomIndex); 
        }

        //2.0
        /*        for (int x = -roomMargin; x < roomWidth + roomMargin; x++)
                {
                    for (int z = -roomMargin; z < roomLength + roomMargin; z++)
                    {
                        int spawnX = roomStartX + x;
                        int spawnZ = roomStartZ + z;

                        // Check if inside borders
                        if (spawnX >= 0 && spawnX <= borderSize - 1 && spawnZ >= 0 && spawnZ <= borderSize - 1)
                        {
                            // Place the tiles if grids are not occupied
                            if (!occupiedTiles.Contains(new Vector2Int(spawnX, spawnZ)))
                            {
                                Vector3 position = new Vector3(spawnX, 0, spawnZ);
                                GameObject tile = Instantiate(floorPrefab, position, Quaternion.identity);
                                tile.transform.SetParent(roomParent.transform);

                                occupiedTiles.Add(new Vector2Int(spawnX, spawnZ));
                            }
                        }
                    }
                }*/

        //1.0
        /*        for (int x = 0; x < roomWidth; x++)
                {
                    for (int z = 0; z < roomLength; z++)
                    {
                        Vector3 position = new Vector3(roomStartX + x, 0, roomStartZ + z);
                        GameObject tile = Instantiate(floorPrefab, position, Quaternion.identity);
                        tile.transform.SetParent(roomParent.transform);
                    }
                }*/
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 offset = new Vector3(-0.5f, 0f, -0.5f); 

        for (int x = 0; x <= borderSize + 1; x++)
        {
            Gizmos.DrawWireCube(new Vector3(x, 0, 0) + offset, Vector3.one);
            Gizmos.DrawWireCube(new Vector3(x, 0, borderSize + 1) + offset, Vector3.one);
        }

        for (int z = 0; z <= borderSize + 1; z++)
        {
            Gizmos.DrawWireCube(new Vector3(0, 0, z) + offset, Vector3.one);
            Gizmos.DrawWireCube(new Vector3(borderSize + 1, 0, z) + offset, Vector3.one);
        }

        Gizmos.color = Color.green;
        if (roomCenters != null && roomCenters.Count > 0)
        {
            foreach (Vector3 center in roomCenters)
            {
                Gizmos.DrawSphere(center + Vector3.up, 0.5f);
            }
        }
    }

}
