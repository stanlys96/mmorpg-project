using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour
{
  [SerializeField] Transform target;
  // Update is called once per frame
  void Update()
  {
    if (Input.GetMouseButton(1))
    {
      MoveToCursor();
    }
    UpdateAnimation();
  }

  private void MoveToCursor()
  {
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;
    bool hasHit = Physics.Raycast(ray, out hit);
    if (hasHit)
    {
      GetComponent<NavMeshAgent>().destination = hit.point;
    }
  }

  // InverseTransformDirection = turn global velocity to local velocity
  private void UpdateAnimation()
  {
    Vector3 velocity = GetComponent<NavMeshAgent>().velocity;
    Vector3 localVelocity = transform.InverseTransformDirection(velocity);
    float speed = localVelocity.z;
    GetComponent<Animator>().SetFloat("forwardSpeed", speed);
  }
}
