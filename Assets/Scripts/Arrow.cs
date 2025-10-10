using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 8f;
    public int damage = 10;
    public int maxBounces = 1;

    private Rigidbody2D rb;
    private Vector2 direction;
    private int currentBounces = 0;
    private bool hasHit = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (!hasHit)
        {
            rb.linearVelocity = direction * speed;
        }
    }

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;

        // Rotate arrow to face direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasHit) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            // Damage player
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            DestroyArrow();
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            HandleWallBounce(collision);
        }
    }

    void HandleWallBounce(Collision2D collision)
    {
        currentBounces++;

        if (currentBounces > maxBounces)
        {
            DestroyArrow();
            VFXManager.instance.ArrowHit(gameObject.transform.position);
        }
        else
        {
            // Calculate bounce direction
            Vector2 reflectDirection = Vector2.Reflect(direction, collision.contacts[0].normal);
            SetDirection(reflectDirection);
        }
    }

    void DestroyArrow()
    {
        hasHit = true;
        // Play destruction effect here if needed
        Destroy(gameObject);
    }
}