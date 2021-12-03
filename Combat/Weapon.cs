using UnityEngine;
using RPG.Core;

namespace RPG.Combat
{
  [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
  public class Weapon : ScriptableObject
  {
    [SerializeField] GameObject weaponPrefab = null;
    [SerializeField] AnimatorOverrideController animatorOverride = null;
    [SerializeField] float attackRange = 1.75f;
    [SerializeField] float weaponDamage = 5f;
    [SerializeField] bool isRightHanded = true;
    [SerializeField] Projectile projectile = null;
    const string weaponName = "Weapon";
    const string shieldName = "Shield";

    public void SpawnWeapon(Transform rightHand, Transform leftHand, Animator animator)
    {
      DestroyOldWeapon(rightHand, leftHand);
      if (weaponPrefab != null)
      {
        GameObject weapon = Instantiate(weaponPrefab, GetTransform(rightHand, leftHand));
        if (weapon.gameObject.name != "Equipped Sword(Clone)")
        {
          DestroyOldShield(rightHand, leftHand);
        }
        weapon.name = weaponName;
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

    public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target)
    {
      Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
      projectileInstance.SetTarget(target, weaponDamage);
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

    public float GetWeaponDamage()
    {
      return weaponDamage;
    }
  }
}