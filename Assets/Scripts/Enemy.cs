using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour {
    
    
    [SerializeField]
    private Transform exitPoint;
    [SerializeField]
    private Transform[] wayPoints;
    [SerializeField]
    private float navigationUpdate;
    [SerializeField]
    private int healthPoints;
    [SerializeField]
    private int rewardAmount;

    private Transform enemy;
    private Collider2D enemyCollider;
    private Animator enemyAnime;
    private float navigationTime = 0;
    private int target = 0;
    private bool isDead = false;


    public bool IsDead
    {
        get
        {
            return isDead;
        }
    }

    // Use this for initialization
    void Start () {
        enemy = GetComponent<Transform>();
        enemyCollider = GetComponent<Collider2D>();
        enemyAnime = GetComponent<Animator>();
        GameManager.Instace.RegisterEnemy(this);
	}
	
	// Update is called once per frame
	void Update () {
		if(wayPoints != null && !isDead)
        {
            navigationTime += Time.deltaTime;
            if (navigationTime > navigationUpdate)
            {
                if(target < wayPoints.Length)
                {
                    enemy.position = Vector2.MoveTowards(enemy.position, wayPoints[target].position, navigationTime);
                }else
                {
                    enemy.position = Vector2.MoveTowards(enemy.position, exitPoint.position, navigationTime);
                }
                navigationTime = 0; //reset timer
            }

        }
	}

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Checkpoint")
        {
            target += 1;
        }else if(collision.tag == "Finish")
        {
            GameManager.Instace.RoundEscaped += 1;
            GameManager.Instace.TotalEscaped += 1;
            GameManager.Instace.UnregisterEnemy(this);
            GameManager.Instace.IsWaveOver();

        }
        else if(collision.tag == "Projectiles")
        {
            Projectile newProj = collision.gameObject.GetComponent<Projectile>();
            if(newProj != null)
                EnemyHit(newProj.AttackDamage);

            Destroy(collision.gameObject);
        }
    }

    public void EnemyHit(int hitDmg)
    {
        if (healthPoints - hitDmg > 0) {
            healthPoints -= hitDmg;
            //call heart animation
            GameManager.Instace.AudioSource.PlayOneShot(SoundManager.Instace.Hit);
            enemyAnime.Play("Hurt");
        }
        else 
        {
            //call die animation
            enemyAnime.SetTrigger("Did_Die"); //Triggerul este in Animation - Tab Parameter -> Add -> Triiger :P ; Se face trigger, pt ca vine din Anyweare state (se cheama indiferent in ce animatie e in momentu ala). ; Did_die - se pune pe sageata de DIE!!!!

            //enemy die
            EnemyDie();
            
        }

    }


    public void EnemyDie()
    {
        isDead = true;
        enemyCollider.enabled = false;
        GameManager.Instace.TotalKills += 1;
        GameManager.Instace.AudioSource.PlayOneShot(SoundManager.Instace.Death);
        GameManager.Instace.AddMoney(rewardAmount);
        GameManager.Instace.IsWaveOver();
    }
}
