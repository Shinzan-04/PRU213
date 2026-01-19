using System.Collections;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] int maxHealth = 10;
    [SerializeField] int attackDamage = 1;
    [SerializeField] float attackCooldown = 1.5f;

    [Header("Skill Settings")]
    [SerializeField] int attacksBeforeSkill = 3; // Số đòn tấn công thường trước khi cast
    [SerializeField] float skillCastTime = 2f;
    [SerializeField] int skillDamage = 3;
    [SerializeField] GameObject spellEffectPrefab;
    [SerializeField] private Vector3 spellOffset = new Vector3(0f, 3f, 0f); 

    [HideInInspector] public bool isMoving;
    [HideInInspector] public bool left;
    [HideInInspector] public bool isAttacking;
    [HideInInspector] public bool isHurt;
    [HideInInspector] public bool isDead;
    [HideInInspector] public bool isCasting;

    // trạng thái
    int currentHealth;
    float attackTimer;
    int attackCount; // Đếm số đòn tấn công thường
    bool attacking;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public GameObject[] UpgradeItems;
    // mục tiêu
    Artifact artifact;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Start()
    {
        artifact = GameObject.FindGameObjectWithTag("Artifact").GetComponent<Artifact>();
        attacking = false;
        attackCount = 0;

        // Đồng bộ EnemyHealth UI
        EnemyHealth eh = GetComponent<EnemyHealth>();
        if (eh != null)
        {
            eh.max = maxHealth;
            eh.current = currentHealth;
        }
    }

    void Update()
    {
        if (isDead) return;

        if (isHurt || isCasting)
        {
            // vẫn giữ hướng đúng
            if (artifact != null)
                left = artifact.transform.position.x < transform.position.x;
            return;
        }
        HandleAttacker();
    }

    void HandleAttacker()
    {
        if (artifact == null) return;

        float distance = Vector2.Distance(transform.position, artifact.transform.position);

        // Kiểm tra đủ số đòn tấn công để dùng skill
        if (attackCount >= attacksBeforeSkill && !isAttacking && !isCasting)
        {
            StartCoroutine(CastSkillRoutine());
            return;
        }

        if (distance > 1.5f)
        {
            MoveTowards(artifact.transform.position);
        }
        else
        {
            isMoving = false;

            if (!isAttacking && Time.time > attackTimer)
            {
                StartCoroutine(AttackRoutine());
                attackTimer = Time.time + attackCooldown;
            }
        }

        left = artifact.transform.position.x < transform.position.x;
    }

    IEnumerator CastSkillRoutine()
    {
        isCasting = true;
        isMoving = false;

        // Thời gian cast kỹ năng
        yield return new WaitForSeconds(skillCastTime);

        // Thực hiện kỹ năng - tạo hiệu ứng spell
        CastSpell();

        // Reset đếm tấn công
        attackCount = 0;
        isCasting = false;
    }

    void CastSpell()
    {
        // Tạo hiệu ứng spell
        if (spellEffectPrefab != null)
        {
            Vector3 spawnPos = artifact.transform.position + spellOffset;
            GameObject spellEffect = Instantiate(spellEffectPrefab, spawnPos, Quaternion.identity);

            // Lấy component damage từ hiệu ứng spell
            SpellEffect effect = spellEffect.GetComponent<SpellEffect>();
            if (effect != null)
            {
                effect.damage = skillDamage;
                effect.target = artifact.gameObject;
            }

            // Tự hủy sau 2 giây
            Destroy(spellEffect, 2f);
        }
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        yield return null;
        yield return new WaitForSeconds(0.15f * 5);
        Attack();
        yield return new WaitForSeconds(0.15f * 4);
        isAttacking = false;

        // Tăng đếm tấn công
        attackCount++;
    }

    void MoveTowards(Vector3 targetPos)
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        isMoving = true;
        isAttacking = false;
    }

    void Attack()
    {
        artifact.Damage(attackDamage);
    }

    // === NHẬN SÁT THƯƠNG ===
    public void TakeDamage(int dmg)
    {
        if (isDead || isCasting) return;

        currentHealth = Mathf.Max(0, currentHealth - dmg);

        EnemyHealth eh = GetComponent<EnemyHealth>();
        if (eh != null) eh.current = currentHealth;

        if (currentHealth > 0)
            StartCoroutine(HurtRoutine());
        else
            StartCoroutine(DieRoutine());
    }

    IEnumerator HurtRoutine()
    {
        isHurt = true;
        isMoving = false;
        isAttacking = false;
        isCasting = false; // Hủy cast nếu đang bị đánh
        yield return new WaitForSeconds(0.4f);
        isHurt = false;
    }

    IEnumerator DieRoutine()
    {
        isDead = true;
        isMoving = false;
        isAttacking = false;
        isHurt = false;
        isCasting = false;

        EnemyHealth eh = GetComponent<EnemyHealth>();
        if (eh != null) eh.current = 0;

        yield return new WaitForSeconds(1.0f);

        if (UpgradeItems != null && UpgradeItems.Length > 0)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.3f;
            int rand = Random.Range(0, UpgradeItems.Length); // chọn ngẫu nhiên 1 loại đá
            GameObject item = Instantiate(UpgradeItems[rand], spawnPos, Quaternion.identity);

            Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Văng nhẹ ra hướng ngẫu nhiên
                Vector2 dir = Random.insideUnitCircle.normalized;
                rb.AddForce(dir * 2f, ForceMode2D.Impulse);
            }
        }

        Destroy(gameObject);
    }
}