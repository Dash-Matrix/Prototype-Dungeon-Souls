using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Combat")]
    public int maxHealth = 100;
    public int weaponDamage = 20;

    [Header("Shield Settings")]
    public float maxShieldDuration = 5f;
    public float shieldCooldown = 2f;

    [Header("References")]
    public Slider healthSlider;

    public TriggerForwarder triggerForwarder;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator animator;
    private int currentHealth;
    private bool isDead;
    private bool shielded;
    private bool facingRight = true;

    private Coroutine shieldRoutine;
    private bool isCooldown = false;

    private float shieldTimer = 0f;
    private float cooldownTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    void Update()
    {
        if (isDead) return;

        GetInput();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        if (isDead) return;

        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void GetInput()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        if (Input.GetMouseButtonDown(0) && !shielded)
        {
            animator.SetTrigger("Attack");
            triggerForwarder.DealDamage();
            VFXManager.instance.SwordSlash(triggerForwarder.transform.position);
            SFXManager.instance.SwordSlash();
        }

        // Shield logic using coroutines
        if (Input.GetMouseButtonDown(1) && !shielded && !isCooldown)
        {
            shieldRoutine = StartCoroutine(StartShield());
        }

        if (Input.GetMouseButtonUp(1))
        {
            shielded = false;
            animator.SetBool("Shield", false);

            if (shieldRoutine != null)
                StopCoroutine(shieldRoutine);
        }
    }

    IEnumerator StartShield()
    {
        shielded = true;
        animator.SetBool("Shield", true);

        float elapsed = 0f;
        while (elapsed < maxShieldDuration)
        {
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        StopShield();
    }

    void StopShield()
    {
        if (shielded)
        {
            shielded = false;
            animator.SetBool("Shield", false);

            if (shieldRoutine != null)
                StopCoroutine(shieldRoutine);

            StartCoroutine(ShieldCooldown());
        }
    }

    IEnumerator ShieldCooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(shieldCooldown);
        isCooldown = false;
    }

    void UpdateAnimations()
    {
        bool isRunning = movement.magnitude > 0.1f;
        animator.SetBool("Run", isRunning);

        if (movement.x > 0 && !facingRight)
        {
            facingRight = !facingRight;
            gameObject.transform.localScale = new Vector2(1, 1);
        }
        else if (movement.x < 0 && facingRight)
        {
            facingRight = !facingRight;
            gameObject.transform.localScale = new Vector2(-1, 1);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || shielded) return;


        VFXManager.instance.PlayerDamage(gameObject.transform.position);
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        animator.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");

        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        animator.SetTrigger("Die");
        GameManager.instance.PlayerDied();
        enabled = false;
    }
}
