using UnityEngine;
using System.Collections;

public class SpellEffect : MonoBehaviour
{
    [Header("Spell Settings")]
    public int damage = 3;
    public GameObject target;
    [SerializeField] private float effectDuration = 1.5f;
    [SerializeField] private float startDelay = 0.3f;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem impactParticles;
    [SerializeField] private Light spellLight;
    [SerializeField] private AudioClip castSound;
    [SerializeField] private AudioClip impactSound;

    private Animator animator;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private bool hasDamaged = false;

    void Start()
    {
        // Get components
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Setup audio source
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Play cast sound
        if (castSound != null)
            audioSource.PlayOneShot(castSound);

        // Start spell routine
        StartCoroutine(SpellRoutine());
    }

    IEnumerator SpellRoutine()
    {
        // Delay trước khi gây damage (cho animation bắt đầu)
        yield return new WaitForSeconds(startDelay);

        // Gây damage
        if (!hasDamaged && target != null)
        {
            ApplyDamage();
        }

        // Chờ hết animation rồi hủy
        yield return new WaitForSeconds(effectDuration - startDelay);

        // Fade out hoặc effect biến mất
        if (spriteRenderer != null)
            StartCoroutine(FadeOut());
        else
            Destroy(gameObject);
    }

    void ApplyDamage()
    {
        if (hasDamaged) return;

        hasDamaged = true;

        // Gây damage cho artifact
        Artifact artifact = target.GetComponent<Artifact>();
        if (artifact != null)
        {
            artifact.Damage(damage);
            Debug.Log($"Spell dealt {damage} damage to artifact!");
        }

        // Play impact effects
        if (impactSound != null)
            audioSource.PlayOneShot(impactSound);

        if (impactParticles != null)
            impactParticles.Play();

        if (spellLight != null)
            StartCoroutine(FadeLight());
    }

    IEnumerator FadeOut()
    {
        float fadeTime = 0.5f;
        float elapsed = 0f;
        Color originalColor = spriteRenderer.color;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }

    IEnumerator FadeLight()
    {
        float fadeTime = 0.3f;
        float elapsed = 0f;
        float originalIntensity = spellLight.intensity;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            spellLight.intensity = Mathf.Lerp(originalIntensity, 0f, elapsed / fadeTime);
            yield return null;
        }

        spellLight.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Dự phòng: gây damage nếu có va chạm
        if (!hasDamaged && other.CompareTag("Artifact"))
        {
            target = other.gameObject;
            ApplyDamage();
        }
    }
}