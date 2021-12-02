using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;

namespace RPG.Control
{
  public class AIController : MonoBehaviour
  {
    [SerializeField] float chaseDistance = 10f;
    [SerializeField] float suspicionTime = 5f;
    [SerializeField] PatrolPath patrolPath;
    [SerializeField] float dwellingTime = 5f;
    float waypointTolerance = 1f;

    GameObject player;
    Fighter fighter;
    Mover mover;
    Health health;
    Vector3 guardPosition;
    float timeSinceLastSawPlayer = Mathf.Infinity;
    int currentWaypointIndex = 0;
    float timeSinceAtWaypoint = Mathf.Infinity;

    private void Start()
    {
      player = GameObject.FindWithTag("Player");
      fighter = GetComponent<Fighter>();
      mover = GetComponent<Mover>();
      health = GetComponent<Health>();
      guardPosition = transform.position;
    }
    // Update is called once per frame
    void Update()
    {
      if (health.IsDead()) return;
      if (InAttackRangeOfPlayer() && fighter.CanAttack(player))
      {
        timeSinceLastSawPlayer = 0;
        AttackBehaviour();
      }
      else if (timeSinceLastSawPlayer < suspicionTime)
      {
        // Suspicion state
        SuspicionBehaviour();
      }
      else
      {
        PatrolBehaviour();
      }
      UpdateTimer();
    }

    private void UpdateTimer()
    {
      timeSinceAtWaypoint += Time.deltaTime;
      timeSinceLastSawPlayer += Time.deltaTime;
    }

    private void AttackBehaviour()
    {
      fighter.Attack(player);
    }

    private void SuspicionBehaviour()
    {
      GetComponent<ActionScheduler>().CancelCurrentAction();
    }

    private void PatrolBehaviour()
    {
      if (timeSinceAtWaypoint < dwellingTime) return;
      Vector3 nextPosition = guardPosition;
      if (patrolPath != null)
      {
        if (AtWaypoint())
        {
          timeSinceAtWaypoint = 0f;
          CycleWaypoint();
        }
        nextPosition = GetCurrentWaypoint(currentWaypointIndex);
      }
      if (timeSinceAtWaypoint > dwellingTime)
      {
        mover.StartMoveAction(nextPosition);
      }
    }

    private Vector3 GetCurrentWaypoint(int index)
    {
      return patrolPath.GetWaypoint(index);
    }

    private void CycleWaypoint()
    {
      currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
    }

    private bool AtWaypoint()
    {
      float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint(currentWaypointIndex));
      return distanceToWaypoint < waypointTolerance;
    }

    private bool InAttackRangeOfPlayer()
    {
      float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
      return distanceToPlayer < chaseDistance;
    }

    // Called by Unity
    private void OnDrawGizmosSelected()
    {
      Gizmos.color = Color.blue;
      Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }
  }
}