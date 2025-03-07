using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TriggerTimeline : MonoBehaviour
{
    public bool playAwake;
    [SerializeField] private PlayableDirector playable;
    [SerializeField] public SystemControl systemControl;
    public float speed = 0.5f;
    private Vector2 velocity = Vector2.zero;
    public Transform Actor;
    public Rigidbody2D ActorRb;
    public Vector3 StarPos;
    public Vector3 EndPos;
    bool Play;
    public KeyCode skipKey = KeyCode.B;

    private void Start()
    {
        if (playable != null)
        {
            playable.played += OnTimelinePlayed;
            playable.stopped += OnTimelineStopped;
        }

        if (playAwake)
        {
            if (!Play)
            {
                Play = true;
                systemControl.Cutscene(false);
                StartCoroutine(StartPosition());
            }
        }
    }

    private void Update()
    {
        if (playable.state == PlayState.Playing && Input.GetKeyDown(skipKey))
        {
            SkipCutscene();
        }
    }

    private void OnTimelinePlayed(PlayableDirector director)
    {
        Debug.Log("Play");
    }

    private void OnTimelineStopped(PlayableDirector director)
    {
        systemControl.Cutscene(true);
        PlayerControl.control.EnableUI(true);
        PlayerControl.control.ShowGun(true);
        if (playAwake)
        {
            Actor.position = EndPos;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!Play)
            {
                Play = true;
                systemControl.Cutscene(false);
                PlayerControl.control.ShowGun(false);
                StartCoroutine(StartPosition());
            }
        }        
    }

    public IEnumerator StartPosition()
    {
        PlayerControl.control.EnableUI(false);
        yield return new WaitForSeconds(0.5f);
        while (Vector3.Distance(Actor.position, StarPos) > 0.01f)
        {
            Vector2 targetDirection = ((Vector2)StarPos - (Vector2)Actor.position).normalized;
            ActorRb.velocity = targetDirection * speed;

            if (Vector3.Distance(Actor.position, StarPos) <= 0.5f)
            {
                Actor.position = StarPos;
                ActorRb.velocity = Vector2.zero;
                break;
            }

            yield return null;
        }

        playable.Play();
    }

    public void SkipCutscene()
    {
        playable.time = playable.duration;
        //playable.Stop();

        systemControl.Cutscene(true);
        Debug.Log("Cutscene skipped");
    }
}
