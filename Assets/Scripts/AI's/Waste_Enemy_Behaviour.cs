using Player_Movement_Namespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waste_Enemy_Behaviour : MonoBehaviour
{

    public GameObject AttackSprite; //delete after animation temp

    [Header("Patrol Points")]
    public Transform[] PointOfMovement;
    public bool Patrol;
    
    [Header("AI Unit Stats")]
    public float zombiespeed;
    public int AttackDamage;
    public float AttackRange;

    private bool attackIsOn;
    private Transform leftmost; //leftmost point of movement
    private Transform rightmost; //rightmost point of movement
    private int currentPoint; //current patroling point ID
    private bool angryAtPlayer; //aggro state
    private bool m_angryAtPlayer; //variable watecher for aggro
    private Player_Health pl; //player
    private Transform pathTarget; //current pathfidning tarhet

    void Start()
    {
        pl = GameObject.Find("Player").GetComponent<Player_Health>(); //finding a player
        ChangeTarget(); //initialize target

        leftmost = PointOfMovement[0]; //initializes
        rightmost = PointOfMovement[0]; //initializes
        for(int i = 0; i < PointOfMovement.Length; i++) //sets the leftmost and rightmost
        {
            if (PointOfMovement[i].position.x < leftmost.position.x)
            {
                leftmost = PointOfMovement[i];
            }
            if (PointOfMovement[i].position.x > rightmost.position.x)
            {
                rightmost= PointOfMovement[i];
            }
        }

    }

    void FixedUpdate()
    {
        //this code will be deleted.
        //it is used to draw a gizmo over the enemy
        if (pl.is_vul == true && attackIsOn == true)
        {
            AttackSprite.SetActive(false);
            attackIsOn = false;
        }

        if(pathTarget)
        {
            float dist = transform.position.x - pathTarget.position.x; //distance to target
            dist = Mathf.Abs(dist);

            if (angryAtPlayer == true)
            {
                if (((pl.transform.position.y - transform.position.y) > -0.1f) &&
                     dist < AttackRange) //ifplayer is near
                {
                    MeleeAttack();
                }
            }

            if(dist < 0.5f)
            {
                ChangeTarget(); //if arrived to target
                return;
            }
        }


        if (pl.transform.position.x < rightmost.position.x && pl.transform.position.x > leftmost.position.x)
        {
            angryAtPlayer = true; //player in the agrozone
        }
        else
        {
            angryAtPlayer = false; //player left it
        }

        if(m_angryAtPlayer != angryAtPlayer) //check that agro was changed. Find new target
        {
            m_angryAtPlayer = angryAtPlayer;
            ChangeTarget();
            return;
        }    



        float xmov = (zombiespeed / 100);

        if (angryAtPlayer == false && Patrol == false) //stop if not patrol
        {
            xmov = 0;
        }

        if (transform.position.x > pathTarget.position.x)
        {
            xmov = xmov * -1;
        }
        transform.position = new Vector2(transform.position.x + xmov, transform.position.y); //getting position
    }

    void ChangeTarget()
    {
        if (angryAtPlayer == true)
        {
            pathTarget = pl.transform; //player as a target
        }

        if(angryAtPlayer == false) 
        {
            pathTarget = PointOfMovement[currentPoint]; 
            currentPoint = currentPoint + 1; //going to the next point
            if(currentPoint >= PointOfMovement.Length)
            {
                currentPoint = 0;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (leftmost)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(leftmost.position, rightmost.position); //drawing the player agro area

            //aggrozone old
            //Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + AgroArea, transform.position.y));
            //Gizmos.DrawLine(transform.position, new Vector3(transform.position.x - AgroArea, transform.position.y));
        }
    }

    void MeleeAttack()
    {
        //pl.GetComponent<Player_Health>().GetDamage(AttackDamage); //calling attack damage
        //play animation here
        attackIsOn = true; //delete after aniamtion implemented
        AttackSprite.SetActive(true); //delete after animation implemented
    }
}
