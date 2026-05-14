using UnityEngine;

/*
 * Spawns ball prefabs at regular intervals and launches them upward.
 * Used to test physics collisions with spatial anchors in MR scenes.
 * - Nikita Harris [ May 2026 ]
 */
public class BallSpawner : MonoBehaviour
{
    // The ball prefab to spawn
    public GameObject ballPrefab;

    // Time in seconds between each ball spawn
    public float spawnRate = 0.2f;

    // Upward force applied to each spawned ball
    public float speed = 5f;

    // Reference to the most recently spawned ball
    private GameObject spawnedBall;

    void Start()
    {
        // InvokeRepeating calls the SpawnBall method repeatedly:
        // - First parameter: name of the method to call
        // - Second parameter: delay before first call (0f = start immediately)
        // - Third parameter: repeat interval (spawnRate seconds between spawns)
        InvokeRepeating("SpawnBall", 0f, spawnRate);
    }

    // Spawns a single ball at this GameObject's position and launches it upward.
    // Called repeatedly by InvokeRepeating.
    private void SpawnBall()
    {
        // Instantiate creates a new instance of the ballPrefab in the scene
        // Parameters:
        // - ballPrefab: the prefab to spawn
        // - transform.position: where to spawn (at this spawner's position)
        // - Quaternion.Euler: rotation of the spawned ball
        //   - Random.Range(-10, 10) adds slight random tilt to X and Z rotation
        //   - This makes balls shoot in slightly different directions for some randomness
        spawnedBall = Instantiate(
            ballPrefab,
            transform.position,
            Quaternion.Euler(Random.Range(-10, 10), 0f, Random.Range(-10, 10))
        );

        // Get the Rigidbody component from the spawned ball
        // AddForce() applies physics force to launch the ball
        // Parameters:
        // - spawnedBall.transform.up: direction (up from ball's perspective)
        // - * speed: multiply direction by speed value for force magnitude
        // - ForceMode.Impulse: apply force instantly (like a cannon shot)
        spawnedBall.GetComponent<Rigidbody>().AddForce(
            spawnedBall.transform.up * speed,
            ForceMode.Impulse
        );
    }
}