using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject groundPrefab;       // The prefab for the ground tile
    public GameObject[] objectPrefabs;    // Array of prefabs for the 3D objects to spawn
    public float spawnInterval = 10f;     // The interval between spawning ground tiles
    public float despawnDistance = 50f;   // The distance at which ground tiles should be despawned
    public float X = 10f;
    private Transform playerTransform;
    public float spawnPositionZ;          // The z-position to spawn the next ground tile
    public float spawnPositionY;          // The y-position to spawn the next ground tile
    private float lastSpawnPositionZ;     // The x-position where the last ground tile was spawned


    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        spawnPositionZ = 0f;
        spawnPositionY = 0f;
        lastSpawnPositionZ = 0f;

        // Spawn initial ground tiles
        for (int i = 0; i < 5; i++)
        {
            SpawnGround();
        }
    }

    private void Update()
    {
        // Check if the player has moved far enough to spawn a new ground tile
        if (playerTransform.position.z + X + despawnDistance > lastSpawnPositionZ)
        {
            SpawnGround();
        }
    }

    private void SpawnGround()
    {
        // Instantiate a new ground tile at the spawn position
        GameObject newGround = Instantiate(groundPrefab, new Vector3(0f, spawnPositionY, spawnPositionZ), Quaternion.identity);

        // Update the spawn and last spawn positions
        spawnPositionZ += newGround.transform.localScale.z;
        lastSpawnPositionZ = spawnPositionZ;
        spawnPositionY -= newGround.transform.localScale.y;

        // Check if it's time to spawn the objects
        if (spawnPositionZ >= 25f)
        {
            SpawnObjects();
        }
    }

    private void SpawnObjects()
    {
        // Calculate the X positions for each object prefab
        float xPos1 = -3f;
        float xPos2 = -1f;
        float xPos3 = 1f;
        float xPos4 = 3f;

        // Instantiate the object prefabs at their respective positions
        Instantiate(objectPrefabs[0], new Vector3(xPos1, spawnPositionY+2, spawnPositionZ - 25f), Quaternion.identity);
        Instantiate(objectPrefabs[1], new Vector3(xPos2, spawnPositionY+2, spawnPositionZ - 25f), Quaternion.identity);
        Instantiate(objectPrefabs[2], new Vector3(xPos3, spawnPositionY+2, spawnPositionZ - 25f), Quaternion.identity);
        Instantiate(objectPrefabs[3], new Vector3(xPos4, spawnPositionY+2, spawnPositionZ - 25f), Quaternion.identity);
    }
}
