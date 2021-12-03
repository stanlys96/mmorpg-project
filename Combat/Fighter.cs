using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat
{
  public class Fighter : MonoBehaviour, IAction
  {
    Health target;
    [SerializeField] float attackRange = 1.75f;
    [SerializeField] float timeBetweenAttacks = 1f;
    [SerializeField] float weaponDamage = 5f;
    [SerializeField] GameObject weaponPrefab = null;
    [SerializeField] GameObject shieldPrefab = null;
    [SerializeField] Transform rightHandTransform = null;
    [SerializeField] Transform leftHandTransform = null;
    [SerializeField] AnimatorOverrideController weaponOverride = null;

    float timeSinceLastAttack = Mathf.Infinity;

    private void Start()
    {
      SpawnWeapon();
      SpawnShield();
    }

    private void Update()
    {
      timeSinceLastAttack += Time.deltaTime;
      if (target == null) return;
      if (target.IsDead()) return;
      if (!GetIsInRange())
      {
        GetComponent<Mover>().MoveTo(target.transform.position, 1f);
      }
      else
      {
        GetComponent<Mover>().Cancel();
        AttackBehaviour();
      }
    }

    private void AttackBehaviour()
    {
      transform.LookAt(target.transform);
      if (timeSinceLastAttack > timeBetweenAttacks)
      {
        // This will trigger the Hit() event.
        TriggerAttack();
        timeSinceLastAttack = 0;
      }
    }

    private void TriggerAttack()
    {
      GetComponent<Animator>().ResetTrigger("stopAttack");
      GetComponent<Animator>().SetTrigger("attack");
    }

    // Animation event
    void Hit()
    {
      if (target == null) return;
      target.TakeDamage(weaponDamage);
    }


    private bool GetIsInRange()
    {
      return Vector3.Distance(target.transform.position, transform.position) <= attackRange;
    }

    public void Attack(GameObject gameObject)
    {
      GetComponent<ActionScheduler>().StartAction(this);
      target = gameObject.GetComponent<Health>();
    }

    public bool CanAttack(GameObject gameObject)
    {
      if (gameObject == null) return false;
      Health targetToTest = gameObject.GetComponent<Health>();
      if (targetToTest != null && !targetToTest.IsDead())
      {
        return true;
      }
      return false;
    }

    public void Cancel()
    {
      StopAttack();
      target = null;
    }

    private void StopAttack()
    {
      GetComponent<Animator>().ResetTrigger("attack");
      GetComponent<Animator>().SetTrigger("stopAttack");
    }

    private void SpawnWeapon()
    {
      Instantiate(weaponPrefab, rightHandTransform);
      Animator animator = GetComponent<Animator>();
      animator.runtimeAnimatorController = weaponOverride;
    }

    private void SpawnShield()
    {
      if (gameObject.tag != "Player") return;
      Instantiate(shieldPrefab, leftHandTransform);
    }
  }
}