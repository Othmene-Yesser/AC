using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YesserOthmene {
    public class Damage : MonoBehaviour
    {
        [SerializeField] EnemyMovementManager enemy;
        [SerializeField] Damageable damageable;

        private void Start()
        {
            damageable = new Damageable();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (enemy.currentTarget == null)
                    return;
                Debug.Log("Hit " + other.name);
                enemy.currentTarget.health = damageable.TakeDamage(enemy.currentAttack.attackDamage, enemy.currentTarget.health);
            }
        }

    }
}
