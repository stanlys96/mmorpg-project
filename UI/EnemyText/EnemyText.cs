using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Attributes;

public class EnemyText : MonoBehaviour
{
  [SerializeField] GameObject gameObject = null;
  [SerializeField] Text enemyText = null;
  // Start is called before the first frame update
  void Start()
  {
    if (gameObject != null && enemyText != null)
    {
      enemyText.text = gameObject.tag;
    }
  }

  private void Update()
  {
    if (Mathf.Approximately(gameObject.GetComponent<Health>().GetFraction(), 0))
    {
      enemyText.enabled = false;
    }
  }
}
