using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayaStatusScript : MonoBehaviour
{
    
    public float maxHealth = 100f;
    public float currentHealth;
    public float healStartTimer;
    public float healInterval;
    
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0)
        {
            Debug.Log("you died POJIOIJEWOJ:::::::::;;;:::;::::;::::;::::;;");
            currentHealth = maxHealth;
        }

        if (healStartTimer > 5f && healInterval >= 1f)
        {
            currentHealth += 10f;
            healInterval = 0f;
        }
    }

    private void FixedUpdate()
    {
        if (currentHealth != maxHealth)
        {
            healStartTimer += 0.02f;
        }
        else
        {
            healStartTimer = 0f;
        }
        
        if (healStartTimer > 5f)
        {
            healInterval += .02f;
        }
        else
        {
            healInterval = 0f;
        }
    }
}
