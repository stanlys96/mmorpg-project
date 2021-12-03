using UnityEngine;

namespace RPG.Combat
{
  [CreateAssetMenu(fileName = "Shield", menuName = "Shield/Make New Shield", order = 0)]
  public class Shield : ScriptableObject
  {
    [SerializeField] GameObject shieldPrefab = null;
    [SerializeField] AnimatorOverrideController animatorOverride = null;
    const string shieldName = "Shield";

    public void SpawnShield(Transform handTransform, Animator animator)
    {
      if (handTransform.Find("Weapon") != null) return;
      if (shieldPrefab != null)
      {
        GameObject shield = Instantiate(shieldPrefab, handTransform);
        shield.name = shieldName;
      }
      if (animatorOverride != null)
      {
        animator.runtimeAnimatorController = animatorOverride;
      }
    }
  }
}