using UnityEngine;

/*
 * Tests custom hand pose detection by changing an object's material
 * when the gesture is performed.
 * - Nikita Harris (May 2026)
 */
public class ThumbsUp : MonoBehaviour
{
    // The object whose material will change
    public GameObject obj;

    // Random materials to choose from when gesture/hand pose activates
    public Material[] materials;

    // Stores the original material to restore when gesture ends
    private Material oldMaterial;

    void Start()
    {
        // Get the Renderer component from the target object
        // Store its current material so we can restore it later
        oldMaterial = obj.GetComponent<Renderer>().material;
    }

    // Called when the thumbs down gesture is detected (Activated event)
    // Changes the object's material to a random one from the pool
    public void ThumbsUpActivated()
    {
        // Get a random index from the materials array
        // Random.Range(0, materials.Length) gives 0 to Length-1
        int randomIndex = Random.Range(0, materials.Length);

        // Apply the randomly selected material to the object
        obj.GetComponent<Renderer>().material = materials[randomIndex];
    }

    // Called when the thumbs down gesture ends (Deactivated event)
    // Restores the object's original material
    public void ThumbsUpDeactivated()
    {
        // Restore the original material when gesture is released
        obj.GetComponent<Renderer>().material = oldMaterial;
    }
}