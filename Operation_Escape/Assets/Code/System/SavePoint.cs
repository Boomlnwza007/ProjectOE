using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    public Animator animator;

    public void SetAc(bool on)
    {
        animator.SetBool("Activate_SavePoint", on);
    }

}
