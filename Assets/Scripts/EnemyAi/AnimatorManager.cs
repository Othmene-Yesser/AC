using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YesserOthmene
{
    public class AnimatorManager : MonoBehaviour
    {
        public Animator animator;
        public void PlayTargetAnimation(string targetAnimation, bool isInteracting)
        {
            animator.applyRootMotion= isInteracting;
            animator.SetBool("IsInteracting", isInteracting);
            animator.CrossFade(targetAnimation, 0.2f);
        }
    }
}
