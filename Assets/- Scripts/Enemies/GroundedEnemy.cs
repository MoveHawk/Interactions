using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GroundedEnemy : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;
    public float gravity = -20f;

    [Header("Combat")]
    public float attackRange = 1.5f;
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;
    private float attackTimer = 0f;

    [Header("Health")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Healthbar")]
    public GameObject healthBarPrefab;
    private Slider hpSlider;
    private Canvas hpCanvas;
    private float hpTimer;
    public float hpVisibleTime = 3f;

    [Header("Death")]
    public float pushForce = 4f;
    public float spinSpeed = 240f;

    private CharacterController controller;
    private Transform player;
    private float verticalVelocity;
    private bool dead = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        currentHealth = maxHealth;

        if (healthBarPrefab)
        {
            var hb = Instantiate(healthBarPrefab, transform.position, Quaternion.identity);
            hpCanvas = hb.GetComponentInChildren<Canvas>();
            hpSlider = hpCanvas.GetComponentInChildren<Slider>();
            hpCanvas.enabled = false;
        }
    }

    void Update()
    {
        if (dead) return;

        attackTimer -= Time.deltaTime;

        ApplyGravity();
        UpdateHealthbar();

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= attackRange)
            AttackPlayer();
        else
            MoveTowardsPlayer();
    }

    void MoveTowardsPlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;

        controller.Move(dir * speed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(dir);
    }

    void AttackPlayer()
    {
        if (attackTimer > 0) return;

        attackTimer = attackCooldown;

        if (player.TryGetComponent<Player>(out var p))
            p.TakeDamage(attackDamage);
    }

    void ApplyGravity()
    {
        if (controller.isGrounded)
            verticalVelocity = -2f;
        else
            verticalVelocity += gravity * Time.deltaTime;

        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    // ---------------------------------------------------------
    //  DAMAGE
    // ---------------------------------------------------------
    public void TakeDamage(float dmg)
    {
        if (dead) return;

        currentHealth -= dmg;

        if (hpCanvas)
        {
            hpCanvas.enabled = true;
            hpSlider.value = currentHealth / maxHealth;
            hpTimer = hpVisibleTime;
        }

        if (currentHealth <= 0)
            StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        dead = true;
        controller.enabled = false;
        if (hpCanvas) hpCanvas.enabled = false;

        Vector3 push = (-transform.forward + Vector3.up).normalized;

        float t = 1f;
        while (t > 0)
        {
            transform.position += push * pushForce * Time.deltaTime;
            transform.Rotate(Vector3.forward * spinSpeed * Time.deltaTime);
            t -= Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    // ---------------------------------------------------------
    //  HEALTHBAR UI
    // ---------------------------------------------------------
    void UpdateHealthbar()
    {
        if (!hpCanvas) return;

        float height = controller.bounds.extents.y * 2;
        hpCanvas.transform.position = transform.position + Vector3.up * (height + 0.2f);

        Vector3 camFwd = Camera.main.transform.forward;
        camFwd.y = 0;
        hpCanvas.transform.rotation = Quaternion.LookRotation(camFwd);

        if (hpTimer > 0)
        {
            hpTimer -= Time.deltaTime;
            if (hpTimer <= 0)
                hpCanvas.enabled = false;
        }
    }
}
