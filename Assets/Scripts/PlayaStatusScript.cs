using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayaStatusScript : MonoBehaviour
{

    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float timeBeforeRegen = 5f;
    [SerializeField] private float healthValueIncrement = 1f;
    [SerializeField] private float healthTimeIncrement = .04f;
    private float currentHealth;
    private Coroutine regeneratingHealth;
    public float score = 0f;

    public static Action<float> OnTakeDamage;
    public static Action<float> OnDamage;
    public static Action<float> OnHeal;
    public static Action<float> OnUpdateScore;

    private void OnEnable()
    {
        OnTakeDamage += ApplyDamage;
        Enemy.OnKilledEnemy += UpdateScore;
        SpeedUpgradeScript.OnAddScore += UpdateScore;
    }

    private void OnDisable()
    {
        OnTakeDamage -= ApplyDamage;
        Enemy.OnKilledEnemy -= UpdateScore;
        SpeedUpgradeScript.OnAddScore -= UpdateScore;
    }

    void Awake()
    {
        currentHealth = maxHealth;
    }

    private void ApplyDamage(float dmg)
    {
        currentHealth -= dmg;
        OnDamage?.Invoke(currentHealth);

        if (currentHealth < 0)
            KillPlayer();
        else if(regeneratingHealth != null)
            StopCoroutine(regeneratingHealth);

        regeneratingHealth = StartCoroutine(RegenerateHealth());
    }

    private void UpdateScore(float scoreChange)
    {
        score += scoreChange;
        OnUpdateScore?.Invoke(score);
    }

    private void KillPlayer()
    {
        currentHealth = 0f;

        if (regeneratingHealth != null)
        {
            StopCoroutine(regeneratingHealth);

            print("ya died.");
        }
    }

    private IEnumerator RegenerateHealth()
    {
        yield return new WaitForSeconds(timeBeforeRegen);
        WaitForSeconds timeToWait = new WaitForSeconds(healthTimeIncrement);

        while (currentHealth < maxHealth)
        {
            currentHealth += healthValueIncrement;

            if (currentHealth > maxHealth)
                currentHealth = maxHealth;

            OnHeal?.Invoke(currentHealth);
            yield return timeToWait;
        }

        regeneratingHealth = null;
    }
    
    void Update()
    {
        
    }
}
