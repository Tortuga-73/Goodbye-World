using System;
using Unity.VisualScripting;
//using UnityEditorInternal;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{

    Rigidbody rb;
    public float health = 50f;
    public float damage = 10f;
    public float range = 2f;
    public float attackInterval = 2;
    public float distToPlayer;
    public float timer = 2f;
    public bool canAttack;
    public bool inRange;
    private float deathScore = 100f;

    public static Action<float> OnKilledEnemy;
    public static Action<bool> OnAddKill;

    public void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
        OnKilledEnemy?.Invoke(deathScore);
        OnAddKill?.Invoke(true);
    }

    private void Update()
    {
        checkCanAttack();
        if(timer > attackInterval)
        {
            attack();
            timer = 0f;
        }
    }
    private void attack()
    {
        Debug.Log("Where my hug at?");
        PlayaStatusScript.OnTakeDamage(10);
    }
    private void FixedUpdate()
    {
        attackTiming();
    }

    private bool checkCanAttack()
    {
        distToPlayer = Vector3.Distance(PlayerManager.instance.player.transform.position, this.transform.position);
        
        if (distToPlayer < range)
        {
            canAttack = true;
        }
        else
        {
            canAttack = false;
        }
        return canAttack;
    }
    private void attackTiming()
    {
        if(canAttack)
        {
            timer += 0.02f;
        }
    }
     void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            inRange = true;
        }
    }
     void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            inRange = false;
        }
    }

}
