using UnityEngine;
using Meta.XR.MRUtilityKit;  // MRUK API - provides access to room data and spatial anchors
using System.Collections.Generic;  // Needed for List<T>
using System.Linq;  // Needed for .All() LINQ query

/* 
 * Automatically places objects on specific surfaces (e.g., tables) in an MR room.
 * Adapts to any room configuration by querying MRUK spatial anchor data.
 * - Nikita Harris [ May 2026 ]
 */
public class AutoObjectPlacement : MonoBehaviour
{
    // The prefab to spawn on surfaces (e.g., Ball)
    public GameObject prefab;

    // Reference to the current room data from MRUK
    private MRUKRoom room;

    // Stores all spawn positions to enforce minimum distance between objects
    private List<Vector3> positions = new List<Vector3>();

    // Spawns objects randomly on table surfaces in the current room.
    // Called by MRUK's OnRoomCreated event.
    // - Parameter: Minimum distance between spawned objects (default 0.5 meters)
    public void PopulateRoomWithObjects(float minDist = 0.5f)
    {
        // Generate random number of objects to spawn (between 10 and 15)
        // This adapts to room size - smaller rooms might fail to place all 10
        int rng = Random.Range(10, 15);

        // Get the current room from MRUK singleton instance
        // MRUK.Instance provides global access to the room management system
        // GetCurrentRoom() returns the room the player is currently in
        room = MRUK.Instance.GetCurrentRoom();

        // Create a filter to only select TABLE anchors
        // LabelFilter tells MRUK which type of spatial anchors we want
        // SceneLabels.TABLE is a predefined label for table surfaces
        // Other options: COUCH, WALL_FACE, FLOOR, CEILING, WINDOW_FRAME, DOOR_FRAME, etc.
        LabelFilter tableFilter = new LabelFilter(MRUKAnchor.SceneLabels.WALL_FACE);

        // Attempt to spawn 'rng' number of objects (5-10 balls)
        for (int index = 0; index < rng; index++)
        {
            // Try to find a random valid position on a table surface
            // GenerateRandomPositionOnSurface parameters:
            // - MRUK.SurfaceType.FACING_UP: Only upward-facing surfaces (table tops, not sides)
            // - 0.2f: Minimum distance from edge (similar to a 20% padding from edges)
            // - tableFilter: Only consider TABLE anchors (ignore walls, floor, etc.)
            // - out Vector3 position: Outputs the found spawn position
            // - out Vector3 normal: Outputs the surface normal (direction surface is facing)
            room.GenerateRandomPositionOnSurface(
                MRUK.SurfaceType.VERTICAL,
                0.2f,
                tableFilter,
                out Vector3 position,
                out Vector3 normal
            );

            // Check if a valid position was found
            // If no valid position exists, the method returns Vector3.zero for both outputs
            // This happens when: no tables exist, all tables are full, or no valid space remains
            if (position == Vector3.zero && normal == Vector3.zero)
            {
                break;  // Stop trying - no more valid positions available
            }

            // Check if this position is far enough from all previously spawned objects
            // positions.All() is a LINQ method that checks if ALL items meet a condition
            // For each existing position (pos), calculate distance to new position
            // isClear is true only if ALL existing positions are >= minDist away
            // This prevents objects from overlapping or spawning too close together
            // This is a lambda experssion
            bool isClear = positions.All(pos => Vector3.Distance(pos, position) >= minDist);

            // Only spawn if the position passes the distance check
            if (isClear)
            {
                // Add this position to our tracking list for future distance checks
                positions.Add(position);

                // Spawn the prefab at the found position
                // Instantiate creates a new GameObject from the prefab template
                // Parameters:
                // - prefab: what to spawn
                // - position: where to spawn (on the table surface)
                // - Quaternion.Euler(normal): rotation aligned to surface normal
                //   Note: This might not be ideal for all objects - balls don't need rotation
                //   For other objects, you might want: Quaternion.LookRotation(normal)
                Instantiate(prefab, position, Quaternion.Euler(normal));
            }
            // If not clear (too close to another object), skip this spawn attempt
            // The loop continues to try finding another position
        }
    }

    // Removes all objects with a specific tag from the scene.
    // Called by MRUK's OnRoomRemoved event before switching rooms.
    // - Parameter: The tag to search for (e.g., "Ball")
    public void RemoveAllObjects(string tag)
    {
        // Find all GameObjects in the scene with the specified tag
        // GameObject.FindGameObjectsWithTag searches the entire scene hierarchy
        // Returns an array of all matching objects
        // NOTE: This is relatively slow for large scenes
        GameObject[] balls = GameObject.FindGameObjectsWithTag(tag);

        // Iterate through each found object and destroy it
        foreach (GameObject ball in balls)
        {
            // Destroy removes the GameObject from the scene
            // The object is actually destroyed at the end of the current frame
            Destroy(ball);
        }

        // Clear the positions list for the next room
        // Without this, the next room would think positions from the old room still exist
        positions.Clear();
    }
}