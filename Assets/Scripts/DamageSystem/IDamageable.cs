using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YesserOthmene
{
    public interface IDamageable
    {
        int TakeDamage(int dmg, int health);
    }

    public class Damageable : IDamageable
    {
        public int TakeDamage(int dmg, int health)
        {
            return health - dmg;
        }


    }
}
