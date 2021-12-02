using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat
{
  public class Fighter : MonoBehaviour, IAction
  {
    Transform target;
    [SerializeField] float attackRange = 1.75f;
    [SerializeField] float timeBetweenAttacks = 1f;
    float timeSinceLastAttack = Mathf.Infinity;

    private void Update()
    {
      timeSinceLastAttack += Time.deltaTime;
      if (target == null) return;
      if (!GetIsInRange())
      {
        GetComponent<Mover>().MoveTo(target.position);
      }
      else
      {
        GetComponent<Mover>().Cancel();
        AttackBehaviour();
      }
    }

    private void AttackBehaviour()
    {
      if (timeSinceLastAttack > timeBetweenAttacks)
      {
        GetComponent<Animator>().SetTrigger("attack");
        timeSinceLastAttack = 0;
      }
    }


    private bool GetIsInRange()
    {
      return Vector3.Distance(target.position, transform.position) <= attackRange;
    }

    public void Attack(CombatTarget combatTarget)
    {
      GetComponent<ActionScheduler>().StartAction(this);
      target = combatTarget.transform;
    }

    public void Cancel()
    {
      target = null;
    }

    void Hit()
    {

    }
  }
}