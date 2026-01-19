using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAttackSkill : MonoBehaviour
{
    [Header("Damage shape")]
    [Min(0.1f)] public float radius = 1.3f;

    [Header("Damage")]
    public int damage = 2;
    public float knockback = 3f;               // nếu địch có IDamageable
    [Min(1f)] public float ticksPerSecond = 8f;
    [Min(0.05f)] public float duration = 0.45f;
    [Min(0f)] public float cooldown = 0.8f;

    [Header("Mana Cost")]                 //Mana
    public int manaCost = 25;

    [Header("Targets")]
    public LayerMask enemyMask;                // để 0 sẽ tự lấy “Enemy”
    public Transform weaponPivot;              // optional: xoay vũ khí

    [Header("Spin visuals (no Animator)")]
    public SpriteRenderer bodyRenderer;        // SpriteRenderer của SpinOverlay hoặc thân
    public Sprite[] spinFrames;                // các khung ảnh spin
    [Min(0.01f)] public float frameTime = 0.06f;

    [Header("Hit throttling")]
    [Min(0f)] public float hitInterval = 0.15f;

    // runtime
    float lastUse = -999f;
    bool running;
    readonly Dictionary<Transform, float> lastHit = new();

    PlayerMana playerMana;

    int fIdx;
    float fTimer;
    Sprite originalSprite;
    bool originalEnabled;

    // nếu có script khác đang đổi sprite (Idle/Run/Attack) thì tắt tạm
    Component otherSpriteAnimator;

    static readonly Collider2D[] hitsBuf = new Collider2D[32];

    void Awake()
    {
        if (enemyMask.value == 0)
        {
            int enemyLayer = LayerMask.NameToLayer("Enemy");
            if (enemyLayer >= 0) enemyMask = 1 << enemyLayer;
        }

        // bắt thử component điều khiển sprite cũ (đúng tên thì càng tốt)
        otherSpriteAnimator = GetComponent("PlayerSpriteAnim");

        playerMana = GetComponent<PlayerMana>();
    }

    public void TryUse()
    {
        if (running) return;
        if (Time.time < lastUse + cooldown) return;

        if (playerMana == null)
        {
            return;
        }
        if (!playerMana.TryUseMana(manaCost)) return;

        lastUse = Time.time;
        fIdx = 0;
        fTimer = 0f;

        // LƯU trạng thái renderer rồi BẬT nó lên (overlay mặc định đang tắt)
        if (bodyRenderer)
        {
            originalSprite = bodyRenderer.sprite;
            originalEnabled = bodyRenderer.enabled;
            bodyRenderer.enabled = true;                  // quan trọng cho overlay
            if (spinFrames != null && spinFrames.Length > 0)
                bodyRenderer.sprite = spinFrames[0];
        }

        StartCoroutine(SpinRoutine());
    }

    IEnumerator SpinRoutine()
    {
        running = true;
        lastHit.Clear();

        if (otherSpriteAnimator) ((Behaviour)otherSpriteAnimator).enabled = false;

        float t = 0f;
        float step = 1f / Mathf.Max(1f, ticksPerSecond);
        float nextTick = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;

            if (weaponPivot) weaponPivot.Rotate(0, 0, -720f * Time.deltaTime);

            // chạy khung hình spin
            if (bodyRenderer && spinFrames != null && spinFrames.Length > 0)
            {
                fTimer += Time.deltaTime;
                if (fTimer >= frameTime)
                {
                    fTimer -= frameTime;
                    fIdx = (fIdx + 1) % spinFrames.Length;
                    bodyRenderer.sprite = spinFrames[fIdx];
                }
            }

            // gây sát thương theo tick
            if (t >= nextTick)
            {
                nextTick += step;

                ContactFilter2D filter = new ContactFilter2D();
                filter.useLayerMask = true;
                filter.layerMask = enemyMask;
                filter.useTriggers = true;

                int n = Physics2D.OverlapCircle(
                    transform.position,
                    radius,
                    filter,
                    hitsBuf
                );

                for (int i = 0; i < n; i++)
                {
                    var col = hitsBuf[i];
                    if (!col) continue;

                    var target = col.transform;
                    if (lastHit.TryGetValue(target, out var prev) && Time.time - prev < hitInterval) continue;
                    lastHit[target] = Time.time;

                    // ưu tiên code EnemyHealth hiện có của bạn
                    var eh = col.GetComponentInParent<EnemyHealth>();
                    if (eh != null) { eh.DamageEnemy(damage); continue; }

                    // fallback nếu sau này bạn dùng interface
                    var dmg = col.GetComponentInParent<IDamageable>();
                    if (dmg != null) dmg.TakeDamage(damage, transform.position, knockback);
                }
            }

            yield return null;
        }

        // khôi phục renderer về đúng trạng thái ban đầu
        if (weaponPivot) weaponPivot.localRotation = Quaternion.identity;

        if (bodyRenderer)
        {
            bodyRenderer.sprite = originalSprite;
            bodyRenderer.enabled = originalEnabled;       // overlay sẽ tắt lại, body thì giữ nguyên
        }

        if (otherSpriteAnimator) ((Behaviour)otherSpriteAnimator).enabled = true;

        running = false;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}