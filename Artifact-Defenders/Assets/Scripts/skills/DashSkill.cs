using System.Collections;
using UnityEngine;

public class DashSkill : MonoBehaviour
{
    [Header("Tuning")]
    public float dashSpeed = 18f;
    public float dashDur = 0.15f;
    public float cooldown = 1.0f;

    [Header("I-Frame (tùy chọn)")]
    public bool grantIFrame = true;
    public string invincibleLayer = "PlayerIFrame";

    [Header("Mana Cost")] //     Mana
    public int manaCost = 10;

    Rigidbody2D rb;
    MonoBehaviour move; // PlayerMovement hoặc script di chuyển khác
    float lastUse = -999f;
    bool dashing;

    // ✅ Hướng nhận từ UI (Joystick hoặc Button)
    Vector2 uiDir = Vector2.zero;

    PlayerMana playerMana;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // tìm component di chuyển để tắt trong lúc dash
        move = GetComponent<MonoBehaviour>(); // placeholder nếu bạn đổi tên
        // nếu có script tên PlayerMovement thì dùng đúng nó
        var pm = GetComponent("PlayerMovement") as MonoBehaviour;
        if (pm != null) move = pm;

        playerMana = GetComponent<PlayerMana>();
    }

    // ✅ PC dùng
    public void TryUse()
    {
        uiDir = Vector2.zero;
        TryDash();
    }

    // ✅ Mobile dùng: truyền hướng từ UI (joystick/virtual dpad)
    public void TryUse(Vector2 directionFromUI)
    {
        uiDir = directionFromUI.normalized;
        TryDash();
    }

    void TryDash()
    {
        if (dashing) return;
        if (Time.time < lastUse + cooldown) return;
        if (playerMana == null)
        {
            return;
        }
        if (!playerMana.TryUseMana(manaCost)) return;
        StartCoroutine(DashRoutine());
    }

    IEnumerator DashRoutine()
    {
        lastUse = Time.time;
        dashing = true;

        Vector2 dir = GetDashDir();
        if (dir.sqrMagnitude < 0.0001f)
            dir = Vector2.right;

        int originalLayer = gameObject.layer;

        if (move != null) move.enabled = false;
        if (grantIFrame) gameObject.layer = LayerMask.NameToLayer(invincibleLayer);

        float t = 0f;
        float origDrag = rb.linearDamping;
        rb.linearDamping = 0f;

        while (t < dashDur)
        {
            t += Time.deltaTime;
            rb.linearVelocity = dir * dashSpeed;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        rb.linearDamping = origDrag;
        if (grantIFrame) gameObject.layer = originalLayer;
        if (move != null) move.enabled = true;
        dashing = false;
    }

    Vector2 GetDashDir()
    {
        // ✅ Ưu tiên UI (mobile)
        if (uiDir != Vector2.zero)
            return uiDir;

        // ✅ nếu đang di chuyển → dash theo velocity
        if (rb != null && rb.linearVelocity.sqrMagnitude > 0.01f)
            return rb.linearVelocity.normalized;

        // ✅ đứng yên → dash theo player facing direction
        return transform.right;
    }
}