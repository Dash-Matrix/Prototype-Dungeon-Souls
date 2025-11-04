using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    public float roamSpeed = 1f;
    public float moveSpeed = 2f;
    public float followRange = 5f;
    public float attackRange = 3f;
    public float retreatRange = 2f;
    public float roamChangeTime = 3f;

    [Header("Combat")]
    public int health = 50;
    public int damage = 10;
    public float attackCooldown = 2f;
    public GameObject arrowPrefab;
    public Transform firePoint;

    [Header("References")]
    public Slider healthSlider;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 roamDirection;
    private float roamTimer;
    private float attackTimer;
    private bool isDead = false;
    bool attackTriggered = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        SetNewRoamDirection();

        if (healthSlider != null)
        {
            healthSlider.maxValue = health;
            healthSlider.value = health;
        }
    }

    void Update()
    {
        if (isDead) return;

        roamTimer -= Time.deltaTime;
        if (roamTimer <= 0) SetNewRoamDirection();
        if (attackTimer > 0) attackTimer -= Time.deltaTime;

        HandleBehavior();
        UpdateAnimations();
    }

    void HandleBehavior()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= retreatRange)
            Retreat();
        else if (distance <= attackRange)
            Attack();
        else if (distance <= followRange)
            Follow();
        else
            Roam();
    }

    void Roam()
    {
        rb.linearVelocity = roamDirection * roamSpeed;
        animator.SetTrigger("CancelAttack");
    }

    void Follow()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
        animator.SetTrigger("CancelAttack");
    }

    void Retreat()
    {
        Vector2 direction = (transform.position - player.position).normalized;
        rb.linearVelocity = direction * moveSpeed;

        animator.SetTrigger("CancelAttack");
    }

    void Attack()
    {
        rb.linearVelocity = Vector2.zero;

        if (attackTimer > 0) return;

        animator.SetTrigger("AttackPrepare");
        Invoke(nameof(FireArrow), 1f); // Delay for animation wind-up
        attackTimer = attackCooldown;
    }

    void FireArrow()
    {
        if (isDead) return;

        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        Vector2 direction = (player.position - firePoint.position).normalized;

        Arrow projectile = arrow.GetComponent<Arrow>();
        if (projectile != null)
        {
            projectile.SetDirection(direction);
            projectile.damage = damage;
        }
    }

    void SetNewRoamDirection()
    {
        roamDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        roamTimer = roamChangeTime;
    }

    void UpdateAnimations()
    {
        animator.SetBool("IsMoving", rb.linearVelocity.magnitude > 0.1f);
        if (rb.linearVelocity.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(-rb.linearVelocity.x), 1, 1);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        VFXManager.instance.EnemyDamage(gameObject.transform.position);
        health -= damage;
        healthSlider.value = health;
        animator.SetTrigger("Hit");

        if (health <= 0) Die();
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        enabled = false;
        VFXManager.instance.EnemyDie(gameObject.transform.position);
        SFXManager.instance.Damage();

        enabled = false;

        EnemyManager.instance.HandleEnemyDeath();
        GameManager.instance.EnemyKilled();
        healthSlider.gameObject.SetActive(false);
        // Destroy(gameObject, 2f);

    }
}
