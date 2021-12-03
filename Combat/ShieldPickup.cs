using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
  public class ShieldPickup : MonoBehaviour
  {
    [SerializeField] Shield shield = null;

    private void OnTriggerEnter(Collider other)
    {
      if (other.gameObject.tag == "Player")
      {
        other.GetComponent<Fighter>().EquipShield(shield);
        Destroy(gameObject);
      }
    }
  }
}