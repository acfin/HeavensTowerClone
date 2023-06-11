using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //Base stats, would like to change to baseSpeed, base jumpSpeed but don't want to cause errors.
    //All these stats should be unmodifiable. Would be best to make private const and use getters
    public float speed = 6;
    private float jumpSpeed = 600; //Don't intend to change this one with items
    //New stats
    private const float baseDamage = 1;
    private const float baseAttackDelay = 0.5f;
    private const float baseCritChance = 0.01f; //out of 1
    private const float baseMaxHealth = 100;
    //End base stats

    private AudioSource source;
    public AudioClip swordSound;
    public AudioClip jumpSound;

    //Stat modifiers, represents how much each item changes the respective stat.
    //This can be calculated many ways. Risk of Rain 2, for example, uses linear, hyperbolic, exponential, and more. Depends on the stat.
    //For now, we'll assume these apply linearly. Use the formula: baseStat + (statModifier * statStacks)
    //E.G. baseAttackSpeed + (baseAttackSpeedModifier * attackSpeedStack)
    private const float speedModifier = 0.25f;
    private const float damageModifier = 0.2f;
    private const float attackSpeedMod = 0.05f;
    private const float critChanceMod = 0.1f;
    private const float maxHealthMod = 0;
    private struct StatStacks
    {
        public StatStacks(int speedStacks, int damageStacks, int attackSpeedStacks, int critChanceStacks, int maxHealthStacks)
        {
            this.speedStacks = speedStacks;
            this.damageStacks = damageStacks;
            this.attackSpeedStacks = attackSpeedStacks;
            this.critChanceStacks = critChanceStacks;
            this.maxHealthStacks = maxHealthStacks;
        }
        public int speedStacks;
        public int damageStacks;
        public int attackSpeedStacks;
        public int critChanceStacks;
        public int maxHealthStacks;
    }
    private StatStacks statStacks;
    //End Stats. Num of stacks and current health (below) are the only variables that should change during runtime.
    //Num of stacks will be modified from the item manager script

    private GameObject axe;
    private Rigidbody playerMvmt;
    private Vector3 movement;
    private bool canMove = true;
    public GameObject projectile;
    private bool canAttack = true;
    float distToGround;
    private float movementX;
    private float movementZ;
    public float health = 100f; // !!!!!! Need to add a check for player health in update

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        axe = GameObject.FindWithTag("Melee");
    }

    public float GetMaxHealth()
    {
        return baseMaxHealth + (maxHealthMod * statStacks.maxHealthStacks);
    }

    public float GetDamage()
    {
        float critChance = baseCritChance + (critChanceMod + statStacks.critChanceStacks);
        bool isCrit = (Random.Range(0f, 1f) < critChance);
        float currentDamage;
        if (isCrit)
            currentDamage = (baseDamage + (damageModifier * statStacks.damageStacks)) * 2;
        else
            currentDamage = baseDamage + (damageModifier * statStacks.damageStacks);
        return currentDamage;
    }

    public void PlaySwing()
    {
        source.clip = swordSound;
        source.Play();
    }

    public void PlayJump()
    {
        source.clip = jumpSound;
        source.Play();
    }

    // Start is called before the first frame update
    void Start()
    {

        playerMvmt = GetComponent<Rigidbody>();
        distToGround = GetComponent<BoxCollider>().bounds.extents.y;
        statStacks = GetStacks();
    }

    // Update is called once per frame
    void Update()
    {
        statStacks = GetStacks();
        Movement();
        if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.W) && !Input.GetKeyDown(KeyCode.A) && !Input.GetKeyDown(KeyCode.S) && !Input.GetKeyDown(KeyCode.D))
            Actions();
        if (health <= 0)
            Die();

    }

    private StatStacks GetStacks()
    {
        return new StatStacks(
            PlayerPrefs.GetInt("SpeedStacks"),
            PlayerPrefs.GetInt("DamageStacks"),
            PlayerPrefs.GetInt("AttackSpeedStacks"),
            PlayerPrefs.GetInt("CritChanceStacks"),
            PlayerPrefs.GetInt("MaxHealthStacks"));
    }

    void Movement()
    {
        if (canMove)
        {
            float currentSpeed = speed + (speedModifier * statStacks.speedStacks);
            movement = transform.forward * Input.GetAxisRaw("Vertical") + transform.right * Input.GetAxisRaw("Horizontal");
            movementX = movement.normalized.x * currentSpeed;
            movementZ = movement.normalized.z * currentSpeed;
            playerMvmt.velocity = new Vector3(movementX, playerMvmt.velocity.y, movementZ);
        }
    }

    private bool isGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 1f);
    }

    void Actions()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            canMove = false;
            playerMvmt.velocity = movement * speed * 3;
            Invoke("Dash", 0.2f);
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (canAttack)
            {
                //transform.GetChild(0).gameObject.SetActive(true);
                canAttack = false;
                //Invoke("AttackDelay", 0.5f);
                AxeAttack();
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (canAttack)
            {
                Instantiate(projectile, transform.position, GameObject.FindWithTag("MainCamera").transform.rotation);
                canAttack = false;
                float attackDelay = baseAttackDelay - (attackSpeedMod * statStacks.attackSpeedStacks);
                Invoke("AttackDelay", attackDelay);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Pressed space");
            if (isGrounded())
            {
                Debug.Log("Is grounded");
                playerMvmt.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
                PlayJump();
            }
        }
    }

    void AxeAttack()
    {
        canAttack = false;
        float attackDelay = baseAttackDelay - (attackSpeedMod * statStacks.attackSpeedStacks);
        Animator anim = axe.GetComponent<Animator>();
        axe.GetComponent<BoxCollider>().enabled = true;
        anim.SetTrigger("Attack");
        Invoke("AttackDelay", attackDelay);
        PlaySwing();
    }

    void Dash()
    {
        canMove = true;
    }

    void AttackDelay()
    {
        canAttack = true;
        transform.GetChild(0).gameObject.SetActive(false);
        axe.GetComponent<BoxCollider>().enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("EnemyWeapon"))
        {
            health -= collision.gameObject.GetComponent<EnemyWeapon>().enemyWeapon.dmg;
        }
    }

    void Die()
    {
        Time.timeScale = 0;
    }
}
