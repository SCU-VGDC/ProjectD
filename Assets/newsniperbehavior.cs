using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class newsniperbehavior: Base_Enemy
{
    public Rigidbody2D rb;
    public Move_Forward_State move_state;

    private void Start()
    {
        base.Init();

        current_state = move_state;
        current_state.Init(this);
        contactDamageAmount=5;
        speed=10;
        destroyOnContact=true;
    }

    private void Update()
    {
        current_state.Action(this);
    }

    /*void OnTriggerEnter2D(Collider2D collision)    {
       Debug.Log("Hit");
        if (collision.gameObject.tag == "Enemy") {
            destroyOnContact=false;
        }
        else
        {
            destroyOnContact=true;
        }
    }*/

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        current_state.OnDrawGizmos(this);
    }
}

