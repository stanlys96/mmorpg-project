using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;

namespace RPG.Movement
{
  public class Mover : MonoBehaviour, IAction
  {
    [SerializeField] Transform target;
    NavMeshAgent navMeshAgent;
    Health health;
    private void Start()
    {
      navMeshAgent = GetComponent<NavMeshAgent>();
      health = GetComponent<Health>();
    }
    // Update is called once per frame
    void Update()
    {
      navMeshAgent.enabled = !health.IsDead();
      UpdateAnimation();
    }

    public void StartMoveAction(Vector3 destination)
    {
      GetComponent<ActionScheduler>().StartAction(this);
      MoveTo(destination);
    }

    public void MoveTo(Vector3 destination)
    {
      navMeshAgent.isStopped = false;
      navMeshAgent.destination = destination;
    }

    public void Cancel()
    {
      navMeshAgent.isStopped = true;
    }

    // InverseTransformDirection = turn global velocity to local velocity
    private void UpdateAnimation()
    {
      Vector3 velocity = navMeshAgent.velocity;
      Vector3 localVelocity = transform.InverseTransformDirection(velocity);
      float speed = localVelocity.z;
      GetComponent<Animator>().SetFloat("forwardSpeed", speed);
    }
  }
}