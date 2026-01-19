using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [HideInInspector] public int max;
     public int current;

    EnemyAI enemyAI;
    BossAI bossAI;
    private void Awake()
    {
        current = max;
        enemyAI = GetComponent<EnemyAI>();
        bossAI = GetComponent<BossAI>();

    }

    // Gọi khi PlayerSlash trúng đòn (hoặc các nguồn gây damage gọi DamageEnemy)
    public void DamageEnemy(int amount)
    {
        // Nếu có EnemyAI — ủy quyền cho AI xử lý (anim + death + sync)
        if (enemyAI != null)
        {
            enemyAI.TakeDamage(amount);
            // cập nhật current để UI đọc
            current = Mathf.Max(0, enemyAI.CurrentHealth);
            return;
        }

        if (bossAI != null)
        {
            bossAI.TakeDamage(amount);
            // cập nhật current để UI đọc
            current = Mathf.Max(0, bossAI.CurrentHealth);
            return;
        }

        // fallback: nếu không có AI, xử lý ở đây
        current -= amount;
        if (current <= 0)
        {
            current = 0;
            Destroy(gameObject);
        }
    }
}
