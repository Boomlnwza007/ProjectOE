using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1AniControl : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private string currentAnimaton;
    public float timeplay;
    public GameObject BossBody;
    public AttackCollider[] hitZone;
    public GameObject[] hitZoneBar;
    public Transform[] bone;
    public Vector3[] bonePos = new Vector3[13];
    public Quaternion[] boneRota = new Quaternion[13];
    public bool endAnim;


    private void Awake()
    {
        for (int i = 0; i < bone.Length; i++)
        {
            bonePos[i] = bone[i].localPosition;
            boneRota[i] = bone[i].localRotation;
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
        }


        for (int i = 0; i < bonePos.Length; i++)
        {
            bone[i].localPosition = bonePos[i];
            bone[i].localRotation = boneRota[i];
        }
    }

    public void ChangeAnimationState(string newAnimation)
    {
        // if (currentAnimaton == newAnimation.ToString()) return;

        endAnim = false;
        animator.Play(newAnimation, 0);
        currentAnimaton = newAnimation;
    }
    public void EndAnimation()
    {
        endAnim = true;
    }
}
