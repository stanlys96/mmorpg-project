using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Combat
{
  public class Projectile : MonoBehaviour
  {
    [SerializeField] float speed = 1f;
    [SerializeField] bool isHoming = true;
    [SerializeField] GameObject hitEffect = null;
    [SerializeField] float maxLifeTime = 10f;
    [SerializeField] GameObject[] destroyOnhit = null;
    [SerializeField] float lifeAfterImpact = 2f;
    Health target;
    float damage = 0f;

    private void Start()
    {
      if (target == null) return;
      transform.LookAt(GetAimLocation());
    }

    void Update()
    {
      if (target == null) return;
      if (isHoming && !target.IsDead())
      {
        transform.LookAt(GetAimLocation());
      }
      transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    public void SetTarget(Health target, float damage)
    {
      this.target = target;
      this.damage = damage;

      Destroy(gameObject, maxLifeTime);
    }

    private Vector3 GetAimLocation()
    {
      CapsuleCollider capsuleTarget = target.GetComponent<CapsuleCollider>();
      if (capsuleTarget == null) return target.transform.position;
      return target.transform.position + Vector3.up * capsuleTarget.height / 2;
    }

    private void OnTriggerEnter(Collider other)
    {
      if (other.GetComponent<Health>() != target) return;
      if (target.IsDead()) return;
      target.TakeDamage(damage);

      speed = 0;

      if (hitEffect != null)
      {
        Instantiate(hitEffect, GetAimLocation(), transform.rotation);
      }

      foreach (GameObject item in destroyOnhit)
      {
        Destroy(item);
      }

      Destroy(gameObject, lifeAfterImpact);
    }
  }
}