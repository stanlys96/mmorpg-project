using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using RPG.Movement;
using RPG.Combat;
using RPG.Core;
using RPG.Attributes;

namespace RPG.Control
{
  public class PlayerController : MonoBehaviour
  {
    Health health;

    enum CursorType
    {
      None,
      Movement,
      Combat
    }

    [System.Serializable]
    struct CursorMapping
    {
      public CursorType type;
      public Texture2D texture;
      public Vector2 hotspot;
    }

    [SerializeField] CursorMapping[] cursorMappings = null;

    private void Awake()
    {
      health = GetComponent<Health>();
    }
    // Update is called once per frame
    void Update()
    {
      if (InteractWithUI()) return;
      if (health.IsDead())
      {
        SetCursor(CursorType.None);
        return;
      }
      if (InteractWithCombat()) return;
      if (InteractWithMovement()) return;
      SetCursor(CursorType.None);
    }

    private bool InteractWithUI()
    {
      if (EventSystem.current.IsPointerOverGameObject())
      {
        SetCursor(CursorType.None);
        return true;
      }
      return false;
    }

    private bool InteractWithCombat()
    {
      RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
      foreach (RaycastHit hit in hits)
      {
        CombatTarget target = hit.transform.GetComponent<CombatTarget>();
        if (target == null) continue;
        if (!GetComponent<Fighter>().CanAttack(target.gameObject))
        {
          continue;
        }

        if (Input.GetMouseButton(1))
        {
          GetComponent<Fighter>().Attack(target.gameObject);
        }
        SetCursor(CursorType.Combat);
        return true;
      }
      return false;
    }

    public bool InteractWithMovement()
    {
      RaycastHit hit;
      bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
      if (hasHit)
      {
        if (Input.GetMouseButton(1))
        {
          GetComponent<Mover>().StartMoveAction(hit.point, 1f);
        }
        SetCursor(CursorType.Movement);
        return true;
      }
      return false;
    }

    private void SetCursor(CursorType type)
    {
      CursorMapping mapping = GetCursorMapping(type);
      Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
    }

    private CursorMapping GetCursorMapping(CursorType type)
    {
      foreach (CursorMapping mapping in cursorMappings)
      {
        if (mapping.type == type)
        {
          return mapping;
        }
      }
      return cursorMappings[0];
    }

    private Ray GetMouseRay()
    {
      return Camera.main.ScreenPointToRay(Input.mousePosition);
    }
  }
}