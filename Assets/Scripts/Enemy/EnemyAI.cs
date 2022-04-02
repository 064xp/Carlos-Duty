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
    [SerializeField]
    private Transform navigationnTarget;
    [SerializeField]
    private Transform shootingTarget;
    [SerializeField]
    private Transform player;
    [SerializeField]
    private Transform gunContainer;
    private GunScript gun;

    private Vector3 directionToPlayer;
    private bool inSight = false;

    public float shootDistance = 15.0f;
    public GameObject gunPrefab;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GameObject gunObject = Instantiate(gunPrefab, gunContainer);
        gun = gunObject.GetComponent<GunScript>();
        gun.UsedByAI = true;
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

        if(Physics.Raycast(transform.position, directionToPlayer.normalized, out hit)) {
            inSight = hit.transform.CompareTag("Player");
        }
    }

    private void GoToTarget() {
        if(agent.destination != navigationnTarget.position)
            agent.SetDestination(navigationnTarget.position);

        if(directionToPlayer.magnitude <= shootDistance && inSight) {
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
            gun.Shoot();
        }
    }

    private void AttackTarget() {
        LookAt(shootingTarget);
        gun.Shoot();
    }

    public override void Die() {
        // play death animation
        agent.ResetPath();
        gun.UsedByAI = false;
        Destroy(this.gameObject, 5);
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
