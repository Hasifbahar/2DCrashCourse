using UnityEngine;

public class EnemyDoorUnlock : MonoBehaviour
{
    [Header("Door Setup")]
    public Animator doorAnimator;       // The animator that opens the door
    public string animatorParameterName = "DoorOpen"; // The boolean parameter from the tutorial
    public Collider2D solidCollider;    // ADDED: The physical wall blocking the door

    [Header("Enemies to Defeat")]
    // We will drag our specific room's slimes into this list in the Unity Inspector
    public GameObject[] roomEnemies;    

    private bool isDoorOpen = false;

    void Update()
    {
        // If the door is already open, we don't need to keep checking
        if (isDoorOpen) return;

        // Keep checking if the enemies are dead
        if (AreAllEnemiesDefeated())
        {
            OpenDoor();
        }
    }

    bool AreAllEnemiesDefeated()
    {
        // Go through every enemy in our list
        for (int i = 0; i < roomEnemies.Length; i++)
        {
            // If even ONE enemy is still alive (not destroyed / not null)
            if (roomEnemies[i] != null) 
            {
                return false; // The room is not clear yet!
            }
        }
        
        // If it finishes checking the list and they are all null (destroyed)
        return true; 
    }

    void OpenDoor()
    {
        isDoorOpen = true;
        
        // Play the opening animation just like the tutorial!
        if (doorAnimator != null)
        {
            doorAnimator.SetBool(animatorParameterName, true);
        }
        
        // ADDED: Turn off the invisible wall so the player can walk through!
        if (solidCollider != null)
        {
            solidCollider.enabled = false; 
        }
        
        Debug.Log("All enemies defeated! Door opened.");
        
        // OPTIONAL: If you want this door to teleport the player to the next level 
        // using your old DoorTransition/Interact script, you could enable that script here!
        // GetComponent<DoorTransition>().enabled = true; 
    }
}