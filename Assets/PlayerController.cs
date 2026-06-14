using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    public SwordAttack swordAttack;
    public bool interact;

    // --- NEW FOOTSTEP VARIABLES ---
    private bool playingFootsteps = false;
    public float footstepSpeed = 0.5f;

    Vector2 movementInput;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    Animator animator;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    bool canMove = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void FixedUpdate(){

    if(canMove) {
        // If movement input is not 0, try to movement
            if(movementInput != Vector2.zero){
                bool success = TryMove(movementInput);

                if(!success && movementInput.x > 0) {
                    success = TryMove(new Vector2(movementInput.x, 0));
                }

                 if(!success  && movementInput.y > 0) {
                        success = TryMove(new Vector2(0, movementInput.y));
                 }
            
                // YOUR ORIGINAL ANIMATION LOGIC (RESTORED)
                animator.SetBool("isMoving", success);

                // --- NEW FOOTSTEP LOGIC ---
                if (success && !playingFootsteps)
                {
                    StartFootsteps();
                }
                else if (!success && playingFootsteps)
                {
                    StopFootsteps();
                }

            } else {
                // YOUR ORIGINAL ANIMATION LOGIC (RESTORED)
                animator.SetBool("isMoving", false);

                // --- NEW FOOTSTEP LOGIC ---
                if(playingFootsteps)
                {
                    StopFootsteps();
                }
            }

            // Set direction of sprite to movement direction (RESTORED)
            if(movementInput.x < 0) {
                spriteRenderer.flipX = true;
            } else if (movementInput.x > 0) {
                spriteRenderer.flipX = false;
            }
        }
        
    }

    private bool TryMove(Vector2 direction) {
        if(direction != Vector2.zero) {
         // Check for potential collisions
            int count = rb.Cast(
                direction, // X and Y values between -1 and 1 that represent the direction from the body to look for collisions
                movementFilter, // The settings that determine where a collision can occur on such as layers to collide with
                castCollisions, // List of collisons to store the found collisions into after the Cast is finished
                moveSpeed * Time.fixedDeltaTime + collisionOffset);

            if(count == 0){
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
                return true;
            }else {
                return false;
            }
        }else {
            // Can't move if there's no direction to move in
            return false;
        }
    }

    void OnMove(InputValue movementValue) {
        movementInput = movementValue.Get<Vector2>();
    }

    void OnFire() {
        animator.SetTrigger("swordAttack");
    }
    void OnInteract()
    {
        interact = true;
    }
    public void SwordAttack() {
        LockMovement();

        if(spriteRenderer.flipX == true){
            swordAttack.AttackLeft();
        }else {
            swordAttack.AttackRight();
        }
      
    }

    public void EndSwordAttack() {
        UnlockMovement();
        swordAttack.StopAttack();
    }


    public void LockMovement() {
        canMove = false;
    }

    public void UnlockMovement() {
        canMove = true;
    }

    // --- NEW FOOTSTEP METHODS CAREFULLY ADDED AT THE BOTTOM ---
    void StartFootsteps()
    {
        playingFootsteps = true;
        InvokeRepeating(nameof(PlayFootstep), 0f, footstepSpeed);
    }

    void StopFootsteps()
    {
        playingFootsteps = false;
        CancelInvoke(nameof(PlayFootstep));
    }

    void PlayFootstep()
    {
        // Make sure your SoundEffectManager has a group perfectly named "Footstep"
        SoundEffectManager.Play("Footstep"); 
    }
}