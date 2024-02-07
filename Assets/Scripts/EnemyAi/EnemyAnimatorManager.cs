using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YesserOthmene
{
    public class EnemyAnimatorManager : AnimatorManager
    {
        EnemyLocomotionManager enemyLocomotionManager;

        private void Awake()
        {
            animator= GetComponent<Animator>();
            enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
        }
        //Every time the enemy animator plays an animation with rootmotion it reserts the model back on the gameObject
        private void OnAnimatorMove()
        {
            float delta = Time.deltaTime;
            enemyLocomotionManager.enemyRB.drag = 0;
            Vector3 deltaPosition = animator.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta;
            enemyLocomotionManager.enemyRB.velocity = velocity;
        }
    }
}
