using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyController : MonoBehaviour
{
    public EnemyData enemy_data;
    public delegate void EnemyKilled();
    public static event EnemyKilled OnEnemyKilled;

    public NavMeshAgent agent;
    public LayerMask whatIsGround, whatIsPlayer;
    [SerializeField] private Transform player;
    [SerializeField] private Transform projectile;
    [SerializeField] private Transform gunpoint;

    Animator anim;

    private bool isGrounded = false;
    
    //Patrol
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attack
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    // Enemy speed, aggro distance, and attack distance
    private float agrRange, atkRange;
    public bool playerInAggroRange, playerInAtkRange;

    // Enemy Stats
    public int hp, dmg, def;
    private static string type;
    private float speed;

    public int getHP() { return hp; }

    private void Start()
    {
        speed = enemy_data.speed;
        hp = enemy_data.health;
        dmg = enemy_data.damage;
        def = enemy_data.defense;
        agent.speed = enemy_data.speed;
        agent.isStopped = false;
        agrRange = enemy_data.aggroRange;
        atkRange = enemy_data.atkRange;
        type = enemy_data.enemyType;
        if (type == "Fast" || type == "Basic" || type == "Tanky")
        {
            anim = GetComponent<Animator>();
            isGrounded = true;
        }
    }

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        hp = enemy_data.health;
        dmg = enemy_data.damage;
        def = enemy_data.defense;
        agrRange = enemy_data.aggroRange;
        atkRange= enemy_data.atkRange;
    }

    private void Update()
    {
        if (hp <= 0)
        {
            Die();
        }
      //Checks for aggro and attack range
      playerInAggroRange = Physics.CheckSphere(transform.position, agrRange, whatIsPlayer);
      playerInAtkRange = Physics.CheckSphere(transform.position, atkRange, whatIsPlayer);

        if (!playerInAggroRange && !playerInAtkRange) Patrol();
        if (playerInAggroRange && !playerInAtkRange) ChasePlayer();
        if (playerInAggroRange && playerInAtkRange) Attack();

        
    }

    private void Patrol()
    {
        if (!walkPointSet) SearchWalkPoint();
        
        if (walkPointSet) {
            if (isGrounded)
                anim.SetBool("walk", true);
            agent.SetDestination(walkPoint); 
        }
            

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        anim.SetBool("walk", false);
        anim.SetBool("run", true);
        agent.SetDestination(player.position);
    }

    private void Attack()
    {
        agent.SetDestination(player.position);
  
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            if (type == "Flying") FlyAttack();
            else if (type == "Fast") FastAttack();
            else if (type == "Tanky") TankAttack();
            else RegularEnemyATK();

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Melee"))
        {
            hp -= 10;
        }
        if (other.gameObject.CompareTag("Ranged"))
        {
            hp -= 5;
            Destroy(other.gameObject);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked= false;
    }

    public void Die()
    {

        if (OnEnemyKilled != null)
        {
            OnEnemyKilled();
        }
        Destroy(gameObject);
    }

    private void FlyAttack()
    {
        ///Attack code here
        Transform projectileTransform = Instantiate(projectile, gunpoint.position, Quaternion.identity);
        
        Vector3 shootDir = (player.position - gunpoint.position).normalized;
        projectileTransform.GetComponent<E_Bullet>().Setup(shootDir);
        ///End of attack code
    }

    private void FastAttack()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Run", false);
        anim.SetBool("Attack1", true);
        Stop();
        Invoke("Move", 0.8333f);
    }

    private void TankAttack()
    {
        anim.SetBool("walk", false);
        anim.SetBool("run", false);
        anim.SetBool("attack1", true);
        Stop();
        Invoke("Move", 1.083f);
    }

    private void RegularEnemyATK()
    {
        anim.SetBool("Attack1h1", true);
        Stop();
        Invoke("Move", 0.7405f);
    }

    void Stop()
    {
        agent.isStopped = true;
        agent.speed = 0;
    }

    void Move()
    {
        agent.isStopped = false;
        switch (type)
        {
            case "Fast":
                anim.SetBool("Attack2", false);
                break;
            case "Tanky":
                anim.SetBool("attack1", false);
                break;
            case "Basic":
                anim.SetBool("Attack1h1", false);
                break;
        }
        agent.speed = speed;
    }

}