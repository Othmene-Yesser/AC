using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YesserOthmene;

public class EnemyAnimatorManagerNoNavMesh : AnimatorManager
{
    EnemyMovementManager enemyMovementManager;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemyMovementManager = GetComponent<EnemyMovementManager>();
    }

    //Every time the enemy animator plays an animation with rootmotion it reserts the model back on the gameObject
    private void OnAnimatorMove()
    {
        float delta = Time.deltaTime;
        enemyMovementManager.enemyRB.drag = 0;
        Vector3 deltaPosition = animator.deltaPosition;
        deltaPosition.y = 0;
        Vector3 velocity = deltaPosition / delta;
        velocity = (enemyMovementManager.currentTarget != null) ?
            enemyMovementManager.useVelocityChange ?
            (enemyMovementManager.distanceFromTarget < 1.3f && enemyMovementManager.distanceFromTarget >= 1) ? velocity * enemyMovementManager.distanceFromTarget
            : (enemyMovementManager.distanceFromTarget > 1.3f)? velocity * 1.3f
            : velocity
            : velocity
            : velocity;
        enemyMovementManager.enemyRB.velocity = velocity;
    }
}
