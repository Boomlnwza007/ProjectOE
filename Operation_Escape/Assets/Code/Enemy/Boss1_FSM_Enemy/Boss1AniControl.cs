using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1AniControl : MonoBehaviour
{
    public static Boss1AniControl boss1AniControl;
    [SerializeField] private Animator animator;
    private string currentAnimaton;
    public float timeplay;
    public GameObject BossBody;
    public AttackCollider[] hitZone;
    public GameObject[] hitZoneBar;
    public Transform[] bone;
    public Vector3[] bonePos;

    private void Awake()
    {
        boss1AniControl = this;
        for (int i = 0; i < bone.Length; i++)
        {
            bonePos[i] = bone[i].localPosition;
        }
    }

    public void ResetAnim()
    {
        BossBody.SetActive(true);
        foreach (var item in hitZone)
        {
            item.Re();
        }

        foreach (var item in hitZoneBar)
        {
            item.SetActive(false);
            item.transform.position = Vector3.zero;
        }


        for (int i = 0; i < bonePos.Length; i++)
        {
            bone[i].localPosition = bonePos[i];
        }
    }

    public void ChangeAnimationState(string newAnimation)
    {
        // if (currentAnimaton == newAnimation.ToString()) return;

        animator.Play(newAnimation, 0);
        currentAnimaton = newAnimation;
    }

}
