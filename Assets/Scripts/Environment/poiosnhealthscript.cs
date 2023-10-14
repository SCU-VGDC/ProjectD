using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class poiosnhealthscript : MonoBehaviour
{
    public float totaltime=0f;
    public float damagetime=0.5f;//how long damage intervals will last.
    private int  damage=5;//how much damage 
    // Start is called before the first frame update

    public void OnTriggerStay2D(Collider2D collision)
        {
            totaltime+=Time.deltaTime;
            Debug.Log("Hit");
            if(collision.tag=="Player")
            {   
                Debug.Log("hit player");
                if(totaltime>damagetime)///checks if the ammount of time is greatear than the total time to not constantly apply damage.
                {
                    GameManager.inst.player.GetComponent<ActorHealth>().ApplyDamage(damage);//applies damage
                    Debug.Log("damage");
                    totaltime=0;//resets so it doesn't instantl tick and kill

                }
            }
        }
}
   
