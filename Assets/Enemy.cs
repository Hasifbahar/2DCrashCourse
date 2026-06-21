using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Animator animator;
    GameUI gameUI;
    SpriteRenderer spriteRenderer;

    private bool isDefeated = false;
    
    [Header("Tutorial Settings")]
    [TextArea(3, 10)] 
    public string myClueText = "Write this slime's specific clue here!";
    
    [Header("Attack Settings")]
    public float damageToPlayer = 20f; 
    public float attackCooldown = 1.5f; 
    private bool canAttack = true;

    [Header("AI & Movement")]
    public float patrolSpeed = 1f;
    public float chaseSpeed = 1.5f;
    public float detectionRadius = 3f; 
    public float patrolRadius = 2f; 
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;

    // --- NEW: FLEXIBLE AUDIO SETTINGS ---
    [Header("Audio Settings")]
    [Tooltip("Leave this blank for short sounds. Drop an AudioSource here for LONG sounds!")]
    public AudioSource localAttackAudio; 
    [Tooltip("The name of the short sound in the global SoundManager (e.g. SlimeAttack)")]
    public string globalAttackSoundName;
    // ------------------------------------

    private Transform playerTransform;
    private Vector2 startingPosition;
    private Vector2 targetPatrolPoint;
    private bool isWaiting = false;

    public float Health {
        set {
            if (isDefeated) return; 

            health = value;
            
            if(health <= 0) {
                isDefeated = true; 
                
                // --- NEW: KILL THE SOUND INSTANTLY ON DEATH ---
                if (localAttackAudio != null) 
                {
                    localAttackAudio.Stop();
                }
                
                Defeated();
                
                if (gameUI != null) {
                    gameUI.AddScore(1); 
                    gameUI.ShowTutorial(myClueText);
                }
            }
            else {
                animator.SetTrigger("Hit"); 
            }
        }
        get { return health; }
    }

    public float health = 3; 

    public void Start() {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameUI = FindObjectOfType<GameUI>();

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        startingPosition = transform.position;
        GetNewPatrolPoint();
    }

    private void Update()
    {
        if (isDefeated) return; 

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRadius)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    void ChasePlayer()
    {
        isWaiting = false; 
        
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, chaseSpeed * Time.deltaTime);
        
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        UpdateFacingDirection(direction);
        animator.SetBool("isMoving", true);
    }

    void Patrol()
    {
        if (isWaiting) return; 

        transform.position = Vector2.MoveTowards(transform.position, targetPatrolPoint, patrolSpeed * Time.deltaTime);
        Vector2 direction = (targetPatrolPoint - (Vector2)transform.position).normalized;
        
        UpdateFacingDirection(direction);
        animator.SetBool("isMoving", true);

        if (Vector2.Distance(transform.position, targetPatrolPoint) < 0.1f)
        {
            StartCoroutine(WaitRoutine());
        }
    }

    IEnumerator WaitRoutine()
    {
        isWaiting = true;
        animator.SetBool("isMoving", false); 

        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
        GetNewPatrolPoint();
        isWaiting = false;
    }

    void GetNewPatrolPoint()
    {
        float randomX = Random.Range(-patrolRadius, patrolRadius);
        float randomY = Random.Range(-patrolRadius, patrolRadius);
        targetPatrolPoint = startingPosition + new Vector2(randomX, randomY);
    }

    void UpdateFacingDirection(Vector2 direction)
    {
        if (direction.x > 0) spriteRenderer.flipX = true; 
        else if (direction.x < 0) spriteRenderer.flipX = false;
    }

    public void Defeated(){
        animator.SetTrigger("Defeated");
    }

    public void RemoveEnemy() { 
        Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isDefeated && canAttack)
        {
            animator.SetTrigger("Attack"); 
            
            // --- NEW: SMART AUDIO TRIGGER ---
            if (localAttackAudio != null)
            {
                // Play the personal speaker if it isn't already playing!
                if (!localAttackAudio.isPlaying) localAttackAudio.Play();
            }
            else if (!string.IsNullOrEmpty(globalAttackSoundName))
            {
                // Fallback to the global manager for short sounds
                SoundEffectManager.Play(globalAttackSoundName);
            }

            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageToPlayer);
            }
            StartCoroutine(AttackCooldownRoutine());
        }
    }

    private IEnumerator AttackCooldownRoutine()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}