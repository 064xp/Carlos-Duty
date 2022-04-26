using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : Damagable
{
    private enum States {
        GoToTarget,
        AttackPlayer,
        BreakBarricade,
        AttackTarget,
        EnterBuilding,
        Die
    };

    private NavMeshAgent agent;
    [SerializeField]
    private States state = States.GoToTarget;
    public Vector3 navigationTarget;
    public Transform shootingTarget;
    public Transform player;
    [SerializeField]
    private Transform gunContainer;
    private GunScript gun;
    private GameObject equippedWeapon;
    [SerializeField]
    private Transform head;
    private float followUntil = 0f;
    [SerializeField]
    private Animator animator;

    private Vector3 directionToPlayer;
    [SerializeField]
    private bool inSight = false;
    private bool canShoot = true;
    private float fireUntil = 0f;
    public Enemy enemySettings;
    public GameManager gameManager;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GameObject enemyWeapon = Instantiate(enemySettings.gunPrefab, gunContainer);
        equippedWeapon = enemyWeapon.transform.GetChild(0).gameObject;
        // Enemy weapon variants are contained within an empty container
        gun = equippedWeapon.GetComponent<GunScript>();
        gun.UsedByAI = true;
        equippedWeapon.GetComponent<BoxCollider>().enabled = false;
        agent.speed = enemySettings.movementSpeed; 

        health = enemySettings.health;

        if(gameManager == null) 
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!alive) return;
        CheckForPlayer();
        UpdateStates();
    }

    private void UpdateStates() {
        switch (state) {
            case States.GoToTarget:
                GoToTarget();
                break;
            case States.AttackPlayer:
                AttackPlayer();
                break;
            case States.BreakBarricade:
                break;
            case States.AttackTarget:
                AttackTarget();
                break;
            case States.EnterBuilding:
                break;
            case States.Die:
                Die();
                break;
        }
    }

    void CheckForPlayer() {
        directionToPlayer = player.position - head.position;

        inSight = IsPlayerInView();
        if (inSight) {
            followUntil = Time.time + enemySettings.followMemoryTime;
        }
    }

    bool IsPlayerInView() {
        float directionAngle = Vector3.Angle(transform.forward, directionToPlayer.normalized);

        if (directionToPlayer.magnitude > enemySettings.followDistance) return false;
        if (directionAngle >= 180f / 2) return false;

        RaycastHit hit;

        if(Physics.Raycast(head.position, directionToPlayer.normalized, out hit)) {
            if (hit.transform.CompareTag("Player"))
                return true;
        }

        return false;
    }

    private void GoToTarget() {
        animator.SetBool("IsWalking", true);
        if(agent.destination != navigationTarget)
            agent.SetDestination(navigationTarget);

        if(directionToPlayer.magnitude <= enemySettings.shootDistance && inSight) {
            agent.ResetPath();
            state = States.AttackPlayer;
        }

        if (HasReached()) {
            animator.SetBool("IsWalking", false);
            state = States.AttackTarget;
        }
    }

    private void AttackPlayer() {
        if (Time.time >= followUntil) state = States.GoToTarget;

        if (agent.destination != player.transform.position)
           agent.SetDestination(player.transform.position);

        if (inSight && directionToPlayer.magnitude <= enemySettings.shootDistance) {
            LookAt(player);
            Shoot(player.position);
        } 
    }

    private void AttackTarget() {
        LookAt(shootingTarget);
        Shoot(shootingTarget.position);
    }

    private void Shoot(Vector3 target) {
        if (!canShoot) return;

        if(fireUntil <= Time.time) {
            StartCoroutine(ShootingCooldown());
        }

        float inaccuracy = Random.Range(
            enemySettings.firingPattern.inaccuracy.minValue,
            enemySettings.firingPattern.inaccuracy.maxValue
        );

        animator.SetTrigger("Shoot");
        gun.Shoot(head.position, target - head.position, inaccuracy);
    }

    IEnumerator ShootingCooldown() {
        canShoot = false;
        float cooldownTime = Random.Range(
            enemySettings.firingPattern.cooldownTime.minValue,
            enemySettings.firingPattern.cooldownTime.maxValue
        );

        yield return new WaitForSeconds(cooldownTime);

        canShoot = true;
        float fireTime = Random.Range(
            enemySettings.firingPattern.fireTime.minValue,
            enemySettings.firingPattern.fireTime.maxValue
        );

        fireUntil = Time.time + fireTime;
    }

    public override void Die() {
        animator.SetBool("IsDead", true);
        agent.ResetPath();

        RaycastHit hit;
        GetComponent<CapsuleCollider>().enabled = false;

        if(Physics.Raycast(equippedWeapon.transform.position, Vector3.down, out hit)) {
            equippedWeapon.transform.SetParent(null);
            BoxCollider weaponCollider = equippedWeapon.GetComponent<BoxCollider>();
            Vector3 newPos = hit.point;

            weaponCollider.enabled = true;
            newPos.y += weaponCollider.bounds.size.y;
            equippedWeapon.transform.position = newPos;
            equippedWeapon.transform.localScale = new Vector3(1, 1, 1);
            equippedWeapon.transform.rotation = Quaternion.Euler(0, 0, 0);
            equippedWeapon.GetComponent<GunScript>().animator.enabled = false ;

            GameObject drop = WeightedRandomChoice.RandomChoice<GameObject>(enemySettings.enemyDrops);
            if(drop != null) {
                GameObject instDrop = Instantiate(drop);

                Vector3 dropPos = hit.point;
                dropPos.x += 1;
                dropPos.y += instDrop.GetComponent<BoxCollider>().bounds.size.y;
                instDrop.transform.position = dropPos;
            }
        }


        gameManager.NotifyEnemyKilled();
        Destroy(this.gameObject, 3f);
    }

    // Utility functions
    private void LookAt(Transform target) {
        Vector3 lookDirection = target.position - head.position;
        lookDirection.y = 0f;

        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed);
    }

    private bool HasReached() {
        return agent.hasPath && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
    }

}
