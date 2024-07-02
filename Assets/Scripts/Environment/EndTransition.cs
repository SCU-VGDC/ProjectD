
using UnityEngine;


public class EndTransition : MonoBehaviour
{
    GameObject player;
    public string nextScene;
    private float startTransitionDistance = 10.0f;


    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.inst.player;
    }

    // Update is called once per frame
    void Update()
    {
        float playerDist = Vector2.Distance(player.transform.position, transform.position);

        if (playerDist < startTransitionDistance)
        {
            PlayerMov_FSM.FrameInput moveRightFrameInput = new PlayerMov_FSM.FrameInput
            {
                RightButton = true
            };

            // transition fade
            StartCoroutine(GameSceneManager.inst.StartFadeScreen(true, nextScene));

            // override the player movement to continuously move right 
            EventManager.singleton.AddEvent(new overrideMovement(gameObject, moveRightFrameInput, true));
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(gameObject.transform.position, startTransitionDistance);
    }
}
