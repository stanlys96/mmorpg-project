using UnityEngine;
using RPG.Core;
using RPG.Attributes;

namespace RPG.Combat
{
  [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
  public class WeaponConfig : ScriptableObject
  {
    [SerializeField] Weapon weaponPrefab = null;
    [SerializeField] AnimatorOverrideController animatorOverride = null;
    [SerializeField] float attackRange = 1.75f;
    [SerializeField] float weaponDamage = 5f;
    [SerializeField] float percentageBonus = 0f;
    [SerializeField] bool isRightHanded = true;
    [SerializeField] Projectile projectile = null;
    const string weaponName = "Weapon";
    const string shieldName = "Shield";

    public Weapon SpawnWeapon(Transform rightHand, Transform leftHand, Animator animator)
    {
      DestroyOldWeapon(rightHand, leftHand);
      Weapon weapon = null;
      if (weaponPrefab != null)
      {
        weapon = Instantiate(weaponPrefab, GetTransform(rightHand, leftHand));
        if (weapon.gameObject.name != "Equipped Sword(Clone)")
        {
          DestroyOldShield(rightHand, leftHand);
        }
        weapon.gameObject.name = weaponName;
      }
      var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
      if (animatorOverride != null)
      {
        animator.runtimeAnimatorController = animatorOverride;
      }
      else if (overrideController != null)
      {
        animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
      }
      return weapon;
    }

    private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
    {
      Transform oldWeapon = rightHand.Find(weaponName);
      if (oldWeapon == null)
      {
        oldWeapon = leftHand.Find(weaponName);
      }
      if (oldWeapon == null) return;
      oldWeapon.name = "DESTROYED";
      Destroy(oldWeapon.gameObject);
    }

    private void DestroyOldShield(Transform rightHand, Transform leftHand)
    {
      Transform oldShield = rightHand.Find(shieldName);
      if (oldShield == null)
      {
        oldShield = leftHand.Find(shieldName);
      }
      if (oldShield == null) return;
      oldShield.name = "DESTROYED";
      Destroy(oldShield.gameObject);
    }

    public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
    {
      Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
      projectileInstance.SetTarget(target, instigator, calculatedDamage);
    }

    public bool HasProjectile()
    {
      return projectile != null;
    }

    private Transform GetTransform(Transform rightHand, Transform leftHand)
    {
      Transform handTransform;
      if (isRightHanded) handTransform = rightHand;
      else handTransform = leftHand;
      return handTransform;
    }

    public float GetAttackRange()
    {
      return attackRange;
    }

    public float GetPercentageBonus()
    {
      return percentageBonus;
    }

    public float GetWeaponDamage()
    {
      return weaponDamage;
    }
  }
}