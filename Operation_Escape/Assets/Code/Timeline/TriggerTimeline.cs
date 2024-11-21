using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TriggerTimeline : MonoBehaviour
{
    [SerializeField] private PlayableDirector playable;
    [SerializeField] public SystemControl systemControl;
    public float speed = 0.5f;
    private Vector2 velocity = Vector2.zero;
    public Transform Actor;
    public Rigidbody2D ActorRb;
    public Vector3 StarPos;
    bool Play;
    public KeyCode skipKey = KeyCode.B;

    private void Start()
    {
        if (playable != null)
        {
            playable.played += OnTimelinePlayed;
            playable.stopped += OnTimelineStopped;
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

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Play)
        {
            Play = true;
            systemControl.Cutscene(false);
            StartCoroutine(StartPosition());            
        }
    }

    public IEnumerator StartPosition()
    {
        PlayerControl.control.EnableUI(false);
        yield return new WaitForSeconds(0.5f);
        while (Vector3.Distance(Actor.position, StarPos) > 0.01f)
        {
            Vector2 targetDirection = ((Vector2)StarPos - (Vector2)Actor.position).normalized;
            ActorRb.velocity = Vector2.SmoothDamp(ActorRb.velocity, targetDirection * speed, ref velocity, 0.1f);

            if (Vector3.Distance(Actor.position, StarPos) <= 0.1f)
            {
                // Ensure the actor reaches the target position exactly
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
