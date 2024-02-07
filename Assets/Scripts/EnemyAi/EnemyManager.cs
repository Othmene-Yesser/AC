using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YesserOthmene
{
    public class EnemyManager : MonoBehaviour
    {
        EnemyLocomotionManager enemyLocomotionManager;
        public bool isPreformingAction;

        [Header("A.I Settings")]
        public float detectionRadius = 20;
        //The higher and lower, respectively these Angles are the greater detection Field Of View (Basically like a Huamn Eye)
        public float maximumDetectionAngle = 50;
        public float minimumDetectionAngle = -50;
        
        private void Awake()
        {
            enemyLocomotionManager= GetComponent<EnemyLocomotionManager>();
        }

        private void FixedUpdate()
        {
            HandleCurerentAction();
        }

        private void HandleCurerentAction()
        {
            if (enemyLocomotionManager.currentTarget == null)
            {
                //Debug.Log("nullPlayer");
                enemyLocomotionManager.HandleDetection();
            }
            else
            {
                enemyLocomotionManager.HandleMoveToTarget();
            }
        }
    }
}
