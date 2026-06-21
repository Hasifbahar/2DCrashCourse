using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // We added this to listen to the keyboard!

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI Elements")]
    public Image healthFill; 

    private Animator animator;
    private PlayerController playerController;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    // --- TEMPORARY DEBUG / CHEAT CODE ---
    void Update()
    {
        // If the H key is pressed on the keyboard...
        if (Keyboard.current != null && Keyboard.current.hKey.wasPressedThisFrame)
        {
            currentHealth = maxHealth; // Fill health to max
            UpdateHealthUI(); // Update the red bar

            // If the player was dead, unlock their movement and snap them back to Idle!
            if (playerController != null) 
            {
                playerController.UnlockMovement();
            }
            if (animator != null)
            {
                animator.Play("Player Idle"); // Snaps out of the death animation
            }

            Debug.Log("CHEAT ACTIVATED: Health Restored to 100!");
        }
    }
    // ------------------------------------

    public void TakeDamage(float damageAmount)
    {
        if (currentHealth <= 0) return; 

        currentHealth -= damageAmount;
        
        if (currentHealth <= 0) 
        {
            currentHealth = 0;
            Die(); 
        }

        UpdateHealthUI();
    }

    private void Die()
    {
        if (animator != null) 
        {
            animator.SetTrigger("Death");
        }

        if (playerController != null) 
        {
            playerController.LockMovement();
        }
        
        Debug.Log("Java Hero has been defeated!");
    }

    public void Heal(float healAmount)
    {
        if (currentHealth <= 0) return; 

        currentHealth += healAmount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthFill != null)
        {
            healthFill.fillAmount = currentHealth / maxHealth;
        }
    }
}