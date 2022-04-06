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

    private Vector3 directionToPlayer;
    private bool inSight = false;
    private bool canShoot = true;
    private float fireUntil = 0f;
    public Enemy enemySettings;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        equippedWeapon = Instantiate(enemySettings.gunPrefab, gunContainer);
        gun = equippedWeapon.GetComponent<GunScript>();
        gun.UsedByAI = true;
        equippedWeapon.GetComponent<Animator>().enabled = true;
        equippedWeapon.GetComponent<BoxCollider>().enabled = false;

        health = enemySettings.health;
    }

    // Update is called once per frame
    void Update()
    {
        if (!alive) return;
        UpdateStates();
        CheckForPlayer();
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
        directionToPlayer = player.position - transform.position;

        RaycastHit hit;

        if(Physics.Raycast(head.position, directionToPlayer.normalized, out hit)) {
            inSight = hit.transform.CompareTag("Player");
        }
    }

    private void GoToTarget() {
        if(agent.destination != navigationTarget)
            agent.SetDestination(navigationTarget);

        if(directionToPlayer.magnitude <= enemySettings.shootDistance && inSight) {
            agent.ResetPath();
            print("attack player state");
            state = States.AttackPlayer;
        }

        if (HasReached()) {
            state = States.AttackTarget;
            print("Attack target state");
        }
    }

    private void AttackPlayer() {
        if (agent.destination != player.transform.position)
           agent.SetDestination(player.transform.position);

        if(inSight) {
            LookAt(player);
            Shoot();
        }
    }

    private void AttackTarget() {
        LookAt(shootingTarget);
        Shoot();
    }

    private void Shoot() {
        if (!canShoot) return;

        if(fireUntil <= Time.time) {
            StartCoroutine(ShootingCooldown());
        }

        gun.Shoot(head);
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
        // play death animation

        // Drop weapon
        agent.ResetPath();

        RaycastHit hit;
        GetComponent<CapsuleCollider>().enabled = false;

        if(Physics.Raycast(equippedWeapon.transform.position, Vector3.down, out hit)) {
            equippedWeapon.transform.SetParent(null);
            Vector3 newPos = hit.point;
            newPos.y += 1.0f;
            equippedWeapon.transform.position = newPos;
            equippedWeapon.GetComponent<BoxCollider>().enabled = true;
            equippedWeapon.GetComponent<Animator>().enabled = false ;
        }

        Destroy(this.gameObject, 1);
    }

    // Utility functions

    private void LookAt(Transform target) {
        Vector3 lookDirection = target.position - transform.position;

        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed);
    }

    private bool HasReached() {
        return agent.hasPath && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
    }

}
