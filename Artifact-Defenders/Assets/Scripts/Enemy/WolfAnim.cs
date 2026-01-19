using System.Collections;
using UnityEngine;

public class WolfAnim : MonoBehaviour
{
    [Header("Animation Sprites")]
    [SerializeField] private Sprite[] idleSprites;
    [SerializeField] private Sprite[] walkSprites;
    [SerializeField] private Sprite[] attackSprites;
    [SerializeField] private Sprite[] hurtSprites;
    [SerializeField] private Sprite[] deathSprites;

    [Header("Animation Settings")]
    [SerializeField] private float frameTime = 0.15f;

    private SpriteRenderer spriteRenderer;
    private EnemyAI enemyAI;
    private int frameIndex = 0;
    private float timer;
    private bool isDead = false;

    private Sprite[] lastAnim = null;
    private bool isPlayingOnce = false;
    private Coroutine playOnceCoroutine = null;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyAI = GetComponent<EnemyAI>();
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

        spriteRenderer.flipX = enemyAI.left;
    }

    private Sprite[] GetCurrentAnim()
    {
        if (enemyAI.isDead)
        {
            if (playOnceCoroutine == null)
                playOnceCoroutine = StartCoroutine(PlayOnce(deathSprites, true));
            return deathSprites;
        }

        if (enemyAI.isHurt)
        {
            if (playOnceCoroutine == null)
                playOnceCoroutine = StartCoroutine(PlayOnce(hurtSprites, false));
            return hurtSprites;
        }

        if (enemyAI.isAttacking)
            return attackSprites;

        if (enemyAI.isMoving)
            return walkSprites;

        return idleSprites;
    }

    private IEnumerator PlayOnce(Sprite[] anim, bool dieAfter)
    {
        if (anim == null || anim.Length == 0)
        {
            if (dieAfter) isDead = true;
            else enemyAI.isHurt = false;
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
            enemyAI.isHurt = false;
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
