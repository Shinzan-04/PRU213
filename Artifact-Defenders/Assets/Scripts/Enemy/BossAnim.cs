using System.Collections;
using UnityEngine;

public class BossAnim : MonoBehaviour
{
    [Header("Animation Sprites")]
    [SerializeField] private Sprite[] idleSprites;
    [SerializeField] private Sprite[] walkSprites;
    [SerializeField] private Sprite[] attackSprites;
    [SerializeField] private Sprite[] hurtSprites;
    [SerializeField] private Sprite[] deathSprites;
    [SerializeField] private Sprite[] castSprites; // Thêm sprites cho kỹ năng

    [Header("Animation Settings")]
    [SerializeField] private float frameTime = 0.15f;

    private SpriteRenderer spriteRenderer;
    private BossAI bossAI;
    private int frameIndex = 0;
    private float timer;
    private bool isDead = false;

    private Sprite[] lastAnim = null;
    private bool isPlayingOnce = false;
    private Coroutine playOnceCoroutine = null;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        bossAI = GetComponent<BossAI>();
    }

    void Update()
    {
        if (isDead) return;

        Sprite[] currentAnim = GetCurrentAnim();
        if (currentAnim == null || currentAnim.Length == 0) return;

        if (currentAnim != lastAnim)
        {
            lastAnim = currentAnim;
            frameIndex = 0;
            timer = Time.time + frameTime;
            spriteRenderer.sprite = currentAnim[frameIndex];
        }

        if (!isPlayingOnce)
        {
            if (Time.time >= timer)
            {
                frameIndex = (frameIndex + 1) % currentAnim.Length;
                spriteRenderer.sprite = currentAnim[frameIndex];
                timer = Time.time + frameTime;
            }
        }

        spriteRenderer.flipX = bossAI.left;
    }

    private Sprite[] GetCurrentAnim()
    {
        if (bossAI.isDead)
        {
            if (playOnceCoroutine == null)
                playOnceCoroutine = StartCoroutine(PlayOnce(deathSprites, true));
            return deathSprites;
        }

        if (bossAI.isHurt)
        {
            if (playOnceCoroutine == null)
                playOnceCoroutine = StartCoroutine(PlayOnce(hurtSprites, false));
            return hurtSprites;
        }

        if (bossAI.isCasting)
            return castSprites; // Animation cast chạy bình thường

        if (bossAI.isAttacking)
            return attackSprites;

        if (bossAI.isMoving)
            return walkSprites;

        return idleSprites;
    }

    private IEnumerator PlayOnce(Sprite[] anim, bool dieAfter)
    {
        if (anim == null || anim.Length == 0)
        {
            if (dieAfter) isDead = true;
            else bossAI.isHurt = false;
            playOnceCoroutine = null;
            yield break;
        }

        isPlayingOnce = true;

        for (int i = 0; i < anim.Length; i++)
        {
            spriteRenderer.sprite = anim[i];
            yield return new WaitForSeconds(frameTime);
        }

        isPlayingOnce = false;
        playOnceCoroutine = null;

        if (dieAfter)
        {
            isDead = true;
        }
        else
        {
            bossAI.isHurt = false;
        }

        if (dieAfter)
        {
            isDead = true;
            spriteRenderer.sprite = anim[anim.Length - 1];
        }

        frameIndex = 0;
        timer = Time.time + frameTime;
    }
}