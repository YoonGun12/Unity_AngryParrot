using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
   public delegate void EnemyDeactivated();

   public event EnemyDeactivated OnEnemyDeactivated;

   private void OnTriggerEnter(Collider other)
   {
      if (other.gameObject.layer == LayerMask.NameToLayer("Boundary"))
      {
         if (OnEnemyDeactivated != null)
         {
            OnEnemyDeactivated.Invoke();
         }
         gameObject.SetActive(false);
      }
   }
}
