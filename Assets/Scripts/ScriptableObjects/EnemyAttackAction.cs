using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YesserOthmene
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Enemy/EnemyAttackAction")]
    public class EnemyAttackAction : EnemyActions
    {
        public int attackDamage;
        public int attackScore = 3;
        public float recoveryTime = 2;

        public float maximumAttackAngle = 35;
        public float minimumAttackAngle = -35;

        public float minimumDistanceNeededToAttack = 0;
        public float maximumDistanceNeededToAttack = 3;
    }
}
