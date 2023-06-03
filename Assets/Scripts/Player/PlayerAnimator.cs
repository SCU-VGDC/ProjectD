using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Animator anim;
    public GameObject DashFx;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void SetAnim(string animValue, bool Value=false)
    {
        switch (animValue)
        {
            case "Moving":
                anim.SetBool(animValue, Value);
                break;
            case "Jump":
                anim.SetTrigger(animValue);
                break;
            case "Ground":
                anim.SetBool(animValue, Value);
                break;
            case "Wall":
                anim.SetBool(animValue, Value);
                break;
            case "Death":
                anim.SetBool(animValue, Value);
                break;
            case "Dash":
                anim.SetTrigger(animValue);
                StartCoroutine(FXspawnandDestroy(DashFx, 1.0f));
                break;
        }
    }

    IEnumerator FXspawnandDestroy(GameObject FXpassed, float time)
    {
        GameObject FX = Instantiate(FXpassed, transform.position, transform.rotation) as GameObject;
        FX.transform.parent = transform;
        yield return new WaitForSeconds(time);
        Destroy(FX);
    }

}
