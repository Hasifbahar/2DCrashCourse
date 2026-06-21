using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Animator animator;
    GameUI gameUI;

    // This lock prevents the dead slime from taking more damage
    private bool isDefeated = false;
    
    [Header("Tutorial Settings")]
    [TextArea(3, 10)] 
    public string myClueText = "Write this slime's specific clue here!";
    
    public float Health {
        set {
            // THE LOCK: If already defeated, instantly ignore any new sword hits!
            if (isDefeated) return; 

            health = value;
            
            if(health <= 0) {
                // LOCK THE ENEMY: Prevent future points from being given
                isDefeated = true; 
                
                Defeated();
                
                if (gameUI != null) {
                    // Call the correct AddScore method just once
                    gameUI.AddScore(1); 
                    // Pass THIS enemy's specific text to the tutorial
                    gameUI.ShowTutorial(myClueText);
                }
            }
            else {
                // This plays the hit animation if health is still > 0
                animator.SetTrigger("Hit"); 
            }
        }
        get {
            return health;
        }
    }

    public float health = 3; 

    public void Start() {
        animator = GetComponent<Animator>();
        gameUI = FindObjectOfType<GameUI>();
    }

    public void Defeated(){
        animator.SetTrigger("Defeated");
    }

    // (Make sure you call this from an Animation Event at the end of the death animation!)
    public void RemoveEnemy() { 
        Destroy(gameObject);
    }
}