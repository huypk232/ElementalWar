using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<Dummy>(out Dummy dummy)) {
            dummy.TakeDamamge(1);
        }

        if (other.gameObject.TryGetComponent<CombatController>(out CombatController controller)){
            controller.TakeDamage();
        }
    }
}
