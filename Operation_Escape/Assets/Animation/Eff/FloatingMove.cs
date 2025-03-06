using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class FloatingMove : MonoBehaviour
{
    public Transform target; 
    public float moveSpeed = 2f; 
    public float floatSpeed = 2f; 
    public float floatAmount = 0.5f;
    public float stopDistance = 0.1f;
    public Animator animator;
    public ControlScene control;
    public GameObject Laser;
    private bool go;
    private Vector3 startPos;
    private bool reachedTarget = false;
    private bool first;
    public AudioSource audioShip;
    public AudioClip clip;
    public AudioManager audioManager;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (target == null || reachedTarget || !go) return;

        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmount;

        if (Vector3.Distance(transform.position, target.position) <= stopDistance)
        {
            reachedTarget = true;
            OnReachedTarget();
            return;
        }

        Vector3 moveDirection = (target.position - transform.position).normalized;
        Vector3 newPosition = transform.position + moveDirection * moveSpeed * Time.deltaTime;
        transform.position = new Vector3(newPosition.x, newY, newPosition.z);
    }

    void OnReachedTarget()
    {
        if (!first)
        {
            first = true;
            audioShip.PlayOneShot(clip);
            Laser.SetActive(true);
            control.ChangeScene(1);
        }
    }

    public void Go()
    {
        animator.SetTrigger("Play");
        audioManager.CrossfadeBGM().Forget();
        go = true;
    }
}
