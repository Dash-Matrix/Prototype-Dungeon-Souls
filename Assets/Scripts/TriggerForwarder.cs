using UnityEngine;

public class TriggerForwarder : MonoBehaviour
{
    public PlayerController parent;

    public void DealDamage()
    {
        float attackRadius = 0.2f;
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRadius, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent<EnemyAI>(out var enemy))
            {
                Debug.Log("Dealt damage to " + enemy.name);
                enemy.TakeDamage(parent.weaponDamage);

                VFXManager.instance.SwordHit(gameObject.transform.position);
            }
        }
    }
}
