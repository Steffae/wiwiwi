using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 50f;
    protected float currentHealth;

    protected bool isDying = false;
    protected bool isHit = false;
    protected NavMeshAgent agent;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();
    }

    public virtual void TakeDamage(float damage)
    {
        if (isDying) return;

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} получил {damage} урона. Осталось: {currentHealth}");

        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
        else
        {
            StartCoroutine(HitReaction());
        }
    }

    protected virtual IEnumerator HitReaction()
    {
        isHit = true;

        if (agent != null) agent.isStopped = true;

        // Тычок назад
        Vector3 startPos = transform.position;
        Vector3 backDirection = -transform.forward * 1.5f;
        Vector3 targetPos = startPos + backDirection;

        float hitTime = 0.2f;
        float elapsed = 0;

        while (elapsed < hitTime)
        {
            float t = elapsed / hitTime;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Возвращаемся обратно
        elapsed = 0;
        while (elapsed < hitTime)
        {
            float t = elapsed / hitTime;
            transform.position = Vector3.Lerp(targetPos, startPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isHit = false;
        if (agent != null) agent.isStopped = false;
    }

    protected virtual IEnumerator Die()
    {
        if (isDying) yield break;
        isDying = true;

        Debug.Log($"{gameObject.name} умирает...");

        // Отключаем всё
        if (agent != null) agent.enabled = false;
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            if (script != this)
                script.enabled = false;
        }

        // Плавно увеличиваем scale и поднимаем вверх
        Vector3 originalScale = transform.localScale;
        Vector3 originalPosition = transform.position;
        float floatHeight = 1f;
        float floatTime = 0.5f;

        float elapsedTime = 0;
        while (elapsedTime < floatTime)
        {
            float t = elapsedTime / floatTime;
            transform.localScale = Vector3.Lerp(originalScale, originalScale * 1.5f, t);
            transform.position = originalPosition + Vector3.up * (floatHeight * t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Резко уменьшаем
        transform.localScale = Vector3.zero;

        // Уничтожаем
        Destroy(gameObject, 0.2f);
    }
}