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
                gameUI.score += 1;
                gameUI.UpdateScore();
                gameUI.ShowTutorial();
            }
        }
        get {
            return health;
        }
    }

    public float health = 1;

    public void Start() {
        animator = GetComponent<Animator>();
        gameUI = FindObjectOfType<GameUI>();
    }

    public void Defeated(){
        animator.SetTrigger("Defeated");
    }

    public void RemoveEnemey() {
        Destroy(gameObject);
    }
}
