using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using RPG.Attributes;

namespace RPG.Combat
{
  public class EnemyHealthDisplay : MonoBehaviour
  {
    Fighter fighter;
    private void Awake()
    {
      fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
    }

    private void Update()
    {
      if (fighter.GetTarget() == null)
      {
        GetComponent<Text>().text = "N/A";
      }
      else
      {
        GetComponent<Text>().text = String.Format("{0:0}/{1:0}", fighter.GetTarget().GetHealthPoints(), fighter.GetTarget().GetMaxHealthPoints());
      }
    }
  }
}