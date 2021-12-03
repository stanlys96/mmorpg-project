using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;

namespace RPG.Combat
{
  public class Fighter : MonoBehaviour, IAction, ISaveable
  {
    [SerializeField] float timeBetweenAttacks = 1f;
    [SerializeField] Transform rightHandTransform = null;
    [SerializeField] Transform leftHandTransform = null;
    [SerializeField] Weapon defaultWeapon = null;
    [SerializeField] Shield defaultShield = null;

    Health target;
    float timeSinceLastAttack = Mathf.Infinity;
    Weapon currentWeapon;
    Shield currentShield;

    [System.Serializable]
    struct FighterSaveData
    {
      public string weaponName;
      public string shieldName;
    }

    private void Start()
    {
      if (currentWeapon == null)
      {
        EquipWeapon(defaultWeapon);
      }
      EquipShield(defaultShield);
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
      if (currentWeapon.HasProjectile())
      {
        currentWeapon.LaunchProjectile(rightHandTransform, leftHandTransform, target);
      }
      else
      {
        target.TakeDamage(currentWeapon.GetWeaponDamage());
      }
    }

    void Shoot()
    {
      Hit();
    }


    private bool GetIsInRange()
    {
      return Vector3.Distance(target.transform.position, transform.position) <= currentWeapon.GetAttackRange();
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

    public void EquipWeapon(Weapon weapon)
    {
      currentWeapon = weapon;
      Animator animator = GetComponent<Animator>();
      weapon.SpawnWeapon(rightHandTransform, leftHandTransform, animator);
    }

    public void EquipShield(Shield shield)
    {
      if (gameObject.tag != "Player") return;
      currentShield = shield;
      Animator animator = GetComponent<Animator>();
      shield.SpawnShield(leftHandTransform, animator);
    }

    public object CaptureState()
    {
      FighterSaveData data = new FighterSaveData();
      if (currentWeapon == null || currentShield == null)
      {
        return data;
      }
      data.weaponName = currentWeapon.name;
      // data.shieldName = currentShield.name;
      return data;
    }

    public void RestoreState(object state)
    {
      FighterSaveData data = (FighterSaveData)state;
      if (data.weaponName == null) return;
      string weaponName = data.weaponName;
      // string shieldName = data.shieldName;
      Weapon weapon = Resources.Load<Weapon>(weaponName);
      // Shield shield = Resources.Load<Shield>(shieldName);
      EquipWeapon(weapon);
      // EquipShield(shield);
    }
  }
}