using System.Collections.Generic;
using UnityEngine;

public class BoomerangProjectile : MonoBehaviour
{
    // ---- Tùy chỉnh va chạm / hiển thị ----
    [SerializeField] float hitRadius = 0.25f;     // bán kính quét trúng
    [SerializeField] float hitInterval = 0.15f;   // thời gian tối thiểu giữa 2 lần trúng cùng 1 mục tiêu

    Rigidbody2D rb;
    Vector2 origin, dir;
    float speed, maxDist, knock;
    int damage;
    LayerMask enemyMask;
    BoomerangSkill owner;
    bool returning;

    // ---- Hoạt ảnh ----
    [Header("Animation Frames")]
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite[] spinFrames;
    [SerializeField] float frameTime = 0.06f;

    int frameIndex;
    float frameTimer;

    // chống trừ máu 60 lần/giây
    readonly Dictionary<Transform, float> lastHitTime = new Dictionary<Transform, float>();

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(
        BoomerangSkill owner,
        Vector2 origin,
        Vector2 dir,
        float speed,
        float maxDist,
        int damage,
        float knockback,
        LayerMask enemyMask)
    {
        this.owner     = owner;
        this.origin    = origin;
        this.dir       = dir.normalized;
        this.speed     = speed;
        this.maxDist   = maxDist;
        this.damage    = damage;
        this.knock     = knockback;
        this.enemyMask = enemyMask;

        returning = false;
        lastHitTime.Clear();
    }

    void Update()
    {
        // 1) Tính vận tốc & đổi hướng khi đủ tầm
        Vector2 v;
        if (!returning)
        {
            if (Vector2.Distance(origin, rb.position) >= maxDist)
                returning = true;

            v = dir * speed;
        }
        else
        {
            var back = ((Vector2)owner.transform.position - rb.position).normalized;
            v = back * speed * 1.2f;

            if (Vector2.Distance(owner.transform.position, rb.position) < 0.4f)
            {
                Destroy(gameObject);
                return;
            }
        }

        // 2) Cập nhật rigidbody và xoay sprite theo hướng bay
        rb.linearVelocity = v;
        if (rb.linearVelocity.sqrMagnitude > 0.01f) transform.right = rb.linearVelocity;

        // 3) QUÉT VÒNG TRÒN: gây damage bằng EnemyHealth.DamageEnemy
        var hits = Physics2D.OverlapCircleAll(rb.position, hitRadius, enemyMask);
        foreach (var h in hits)
        {
            var target = h.transform;

            // chống spam trúng liên tục
            if (lastHitTime.TryGetValue(target, out var t) && Time.time - t < hitInterval)
                continue;
            lastHitTime[target] = Time.time;

            // ưu tiên EnemyHealth theo yêu cầu
            var eh = h.GetComponentInParent<EnemyHealth>();
            if (eh != null)
            {
                eh.DamageEnemy(damage);
                continue;
            }

            // dự phòng: nếu sau này bạn thêm IDamageable thì vẫn hoạt động
            var dmg = h.GetComponentInParent<IDamageable>();
            if (dmg != null)
                dmg.TakeDamage(damage, rb.position, knock);
        }

        // 4) Chạy hoạt ảnh boomerang (đổi frame)
        if (spinFrames != null && spinFrames.Length > 0 && spriteRenderer != null)
        {
            frameTimer += Time.deltaTime;
            if (frameTimer >= frameTime)
            {
                frameTimer -= frameTime;
                frameIndex = (frameIndex + 1) % spinFrames.Length;
                spriteRenderer.sprite = spinFrames[frameIndex];
            }
        }

    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere((rb ? (Vector3)rb.position : transform.position), hitRadius);
    }
#endif
}