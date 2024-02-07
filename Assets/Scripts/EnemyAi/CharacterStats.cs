using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YesserOthmene
{
    public class CharacterStats : MonoBehaviour
    {
        public int maxHealth;
        public int health;

        private void Update()
        {
            if (health <= 0)
            {
                Debug.Log("Dead");
            }
        }

    }
}
