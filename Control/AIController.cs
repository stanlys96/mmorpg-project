using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using GameDevTV.Utils;

namespace RPG.Control
{
  public class AIController : MonoBehaviour
  {
    [SerializeField] float chaseDistance = 10f;
    [SerializeField] float suspicionTime = 5f;
    [SerializeField] float aggroCooldownTime = 5f;
    [SerializeField] PatrolPath patrolPath;
    [SerializeField] float dwellingTime = 5f;
    [Range(0, 1)]
    [SerializeField] float patrolSpeedFraction = 0.2f;
    [SerializeField] float shoutDistance = 10;

    float waypointTolerance = 1f;
    GameObject player;
    Fighter fighter;
    Mover mover;
    Health health;
    LazyValue<Vector3> guardPosition;
    float timeSinceLastSawPlayer = Mathf.Infinity;
    int currentWaypointIndex = 0;
    float timeSinceAtWaypoint = Mathf.Infinity;
    float timeSinceAggrevated = Mathf.Infinity;

    private void Awake()
    {
      player = GameObject.FindWithTag("Player");
      fighter = GetComponent<Fighter>();
      mover = GetComponent<Mover>();
      health = GetComponent<Health>();
      guardPosition = new LazyValue<Vector3>(GetGuardPosition);
    }

    private Vector3 GetGuardPosition()
    {
      return transform.position;
    }

    private void Start()
    {
      guardPosition.ForceInit();
    }
    // Update is called once per frame
    void Update()
    {
      if (health.IsDead()) return;
      if (IsAggrevated() && fighter.CanAttack(player))
      {
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

    public void Aggrevate()
    {
      timeSinceAggrevated = 0;
    }

    private void UpdateTimer()
    {
      timeSinceAtWaypoint += Time.deltaTime;
      timeSinceLastSawPlayer += Time.deltaTime;
      timeSinceAggrevated += Time.deltaTime;
    }

    private void AttackBehaviour()
    {
      timeSinceLastSawPlayer = 0;
      fighter.Attack(player);
      AggrevateNearbyEnemies();
    }

    private void AggrevateNearbyEnemies()
    {
      RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
      foreach (RaycastHit hit in hits)
      {
        AIController ai = hit.collider.GetComponent<AIController>();
        if (ai == null) continue;
        ai.Aggrevate();
      }
    }

    private void SuspicionBehaviour()
    {
      GetComponent<ActionScheduler>().CancelCurrentAction();
    }

    private void PatrolBehaviour()
    {
      if (timeSinceAtWaypoint < dwellingTime) return;
      Vector3 nextPosition = guardPosition.value;
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
        mover.StartMoveAction(nextPosition, patrolSpeedFraction);
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

    private bool IsAggrevated()
    {
      float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
      return distanceToPlayer < chaseDistance || timeSinceAggrevated < aggroCooldownTime;
    }

    // Called by Unity
    private void OnDrawGizmosSelected()
    {
      Gizmos.color = Color.blue;
      Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }
  }
}