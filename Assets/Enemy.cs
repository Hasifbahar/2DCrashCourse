using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Animator animator;
    GameUI gameUI;
    
    public float Health {
        set {
            health = value;
            if(health <= 0) {
                Defeated();
                
                // Safety check: Prevent errors if GameUI isn't in the scene yet
                if (gameUI != null) {
                    gameUI.score += 1;
                    gameUI.UpdateScore();
                    gameUI.ShowTutorial();
                }
            }
            else {
                // ADDED: This plays the hit animation if health is still > 0
                animator.SetTrigger("Hit"); 
            }
        }
        get {
            return health;
        }
    }

    public float health = 3; // Increased starting health so it survives a hit!

    public void Start() {
        animator = GetComponent<Animator>();
        gameUI = FindObjectOfType<GameUI>();
    }

    public void Defeated(){
        animator.SetTrigger("Defeated");
    }

    // (Make sure you call this from an Animation Event at the end of the death animation!)
    public void RemoveEnemy() { // (Fixed spelling from RemoveEnemey)
        Destroy(gameObject);
    }
}