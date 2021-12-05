using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using GameDevTV.Utils;

namespace RPG.Combat
{
  public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
  {
    [SerializeField] float timeBetweenAttacks = 1f;
    [SerializeField] Transform rightHandTransform = null;
    [SerializeField] Transform leftHandTransform = null;
    [SerializeField] Weapon defaultWeapon = null;
    [SerializeField] Shield defaultShield = null;

    Health target;
    float timeSinceLastAttack = Mathf.Infinity;
    LazyValue<Weapon> currentWeapon;
    Shield currentShield;

    [System.Serializable]
    struct FighterSaveData
    {
      public string weaponName;
      public string shieldName;
    }

    private void Awake()
    {
      currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
    }

    private Weapon SetupDefaultWeapon()
    {
      AttachWeapon(defaultWeapon);
      return defaultWeapon;
    }

    private void Start()
    {
      currentWeapon.ForceInit();
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
      float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
      if (currentWeapon.value.HasProjectile())
      {
        currentWeapon.value.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
      }
      else
      {
        target.TakeDamage(gameObject, damage);
      }
    }

    void Shoot()
    {
      Hit();
    }


    private bool GetIsInRange()
    {
      return Vector3.Distance(target.transform.position, transform.position) <= currentWeapon.value.GetAttackRange();
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

    public IEnumerable<float> GetAdditiveModifiers(Stat stat)
    {
      if (stat == Stat.Damage)
      {
        yield return currentWeapon.value.GetWeaponDamage();
      }
    }

    public IEnumerable<float> GetPercentageModifiers(Stat stat)
    {
      if (stat == Stat.Damage)
      {
        yield return currentWeapon.value.GetPercentageBonus();
      }
    }

    public void EquipWeapon(Weapon weapon)
    {
      currentWeapon.value = weapon;
      AttachWeapon(weapon);
    }

    private void AttachWeapon(Weapon weapon)
    {
      Animator animator = GetComponent<Animator>();
      weapon.SpawnWeapon(rightHandTransform, leftHandTransform, animator);
    }

    public Health GetTarget()
    {
      return target;
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
      if (currentWeapon.value == null || currentShield == null)
      {
        return data;
      }
      data.weaponName = currentWeapon.value.name;
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