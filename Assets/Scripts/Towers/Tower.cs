using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {

    [SerializeField]
    private float timeBetweenAtacks;
    [SerializeField]
    private float attackRadius;
    [SerializeField]
    private Projectile projectile = null;

    private Enemy targetEnemy = null;
    private float attackCounter;
    private bool isAttacking = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        attackCounter -= Time.deltaTime;
        if(targetEnemy == null || targetEnemy.IsDead)
        {
            Enemy nearestEnemy = GetNearestEnemyInRange();
            if (nearestEnemy != null && Vector2.Distance(this.transform.localPosition, nearestEnemy.transform.localPosition) <= attackRadius )
            {
                targetEnemy = nearestEnemy;
            }
        }
        else
        {
            if (attackCounter <= 0)
            {
                isAttacking = true;
                //reset attack
                attackCounter = timeBetweenAtacks;
            }
            else
                isAttacking = false;


            if (Vector2.Distance(this.transform.localPosition, targetEnemy.transform.localPosition) > attackRadius)
            {
                targetEnemy = null;
            }

        }

       
	}

    void FixedUpdate()
    {
        if (isAttacking)
        {
            AttackEnemy();
        }
    }


    private void AttackEnemy()
    {
        isAttacking = false;
        Projectile newProjectile = Instantiate(projectile);
        newProjectile.transform.localPosition = this.transform.localPosition;
        //Sounds
        if(newProjectile.ProjectileType == proType.arrow)
        {
            GameManager.Instace.AudioSource.PlayOneShot(SoundManager.Instace.Arrow);
        }else if(newProjectile.ProjectileType == proType.rock)
        {
            GameManager.Instace.AudioSource.PlayOneShot(SoundManager.Instace.Rock);
        }
        else if (newProjectile.ProjectileType == proType.fireball)
        {
            GameManager.Instace.AudioSource.PlayOneShot(SoundManager.Instace.FireBall);
        }


            if (targetEnemy == null)
        {
            Destroy(newProjectile);
        }else
        {
            //move projectile to enemy
            StartCoroutine(MoveProjectile(newProjectile));
             
        }
    }

    IEnumerator MoveProjectile(Projectile _projectile)
    {
        while(getTargetDistance(targetEnemy) > 0.10f && _projectile != null  && targetEnemy != null)
        {
            //De ce localPosition VS position  | position e fata world, localPosition e fata de parinte
            var projectDirection = targetEnemy.transform.position - this.transform.localPosition;
            var angleDirection = Mathf.Atan2(projectDirection.y, projectDirection.x) * Mathf.Rad2Deg;
            _projectile.transform.rotation = Quaternion.AngleAxis(angleDirection, Vector3.forward);
            _projectile.transform.localPosition = Vector2.MoveTowards(_projectile.transform.localPosition, targetEnemy.transform.position, 5f * Time.deltaTime);

            yield return null;
        }
        

        if (_projectile != null ||  targetEnemy != null)
        {
            Destroy(_projectile);
        }
    }

    private float getTargetDistance(Enemy enemy)
    {
        if(enemy == null)
        {
            enemy = GetNearestEnemyInRange();
            if(enemy == null)
            {
                return 0f;
            }
        }
        return Mathf.Abs(Vector2.Distance(this.transform.localPosition, enemy.transform.localPosition));
    }

    private List<Enemy> GetEnemiesInRange()
    {
        List<Enemy> enemiesInRange = new List<Enemy>();
        foreach(Enemy e in GameManager.Instace.EnemyList)
        {
            //Comparam distanta dintre turn (this) si enemy (e)
            if(Vector2.Distance(this.transform.localPosition, e.transform.localPosition) <= attackRadius)
            {
                enemiesInRange.Add(e);
            }
        }

        return enemiesInRange;
    }

    private Enemy GetNearestEnemyInRange()
    {
        Enemy nearestEnemy = null;
        float smallestDistance = float.PositiveInfinity;
        foreach(Enemy e in GetEnemiesInRange())
        {
            float enemyDistance = Vector2.Distance(this.transform.localPosition, e.transform.localPosition);
            if (enemyDistance < smallestDistance)
            {
                smallestDistance = enemyDistance;
                nearestEnemy = e;
            }
        }
        return nearestEnemy;
    }
}
