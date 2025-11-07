using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CrawlerEnemy : MonoBehaviour
{
    [Header("Configuración de detección")]
    public Transform player;
    public float detectionRange = 10f;
    public float attackRange = 2f;

    [Header("Estadísticas del enemigo")]
    public float health = 100f;
    public float damage = 10f;
    public float attackCooldown = 2f;

    private Animator anim;
    private NavMeshAgent agent;
    private bool isDead = false;
    private float lastAttackTime = 0f;
    private PlayerHealth targetPlayerHealth;

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.baseOffset = 0f;

        if (player == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null) player = found.transform;
        }

        if (player != null)
        {
            targetPlayerHealth = player.GetComponent<PlayerHealth>();
            if (targetPlayerHealth == null)
                targetPlayerHealth = player.GetComponentInChildren<PlayerHealth>();
            if (targetPlayerHealth == null)
                targetPlayerHealth = player.GetComponentInParent<PlayerHealth>();
        }
    }

    void Update()
    {
        if (isDead) return;

        if (player == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null)
            {
                player = found.transform;
                targetPlayerHealth = player.GetComponent<PlayerHealth>();
            }
            else return;
        }

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= attackRange)
        {
            agent.isStopped = true;
            anim.SetBool("Run Forward", false);
            anim.SetTrigger("Gun Shoot Attack");

            if (Time.time > lastAttackTime + attackCooldown)
            {
                if (targetPlayerHealth != null)
                    targetPlayerHealth.TakeDamage(damage);

                lastAttackTime = Time.time;
            }
        }
        else if (distance <= detectionRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            anim.SetBool("Run Forward", true);
        }
        else
        {
            agent.isStopped = true;
            anim.SetBool("Run Forward", false);
        }
    }

    public void TakeDamage(float amount = 50f)
    {
        if (isDead) return;

        health -= amount;
        anim.SetTrigger("Take Damage");

        if (health <= 0f) Die();
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        anim.SetTrigger("Die");
        anim.SetBool("Run Forward", false);
        agent.isStopped = true;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Destroy(gameObject, 5f);
    }
}