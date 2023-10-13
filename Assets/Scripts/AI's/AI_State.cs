using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class AI_State
{
    public abstract void Init(Base_Enemy context);

    /* 
     * Takes in all information about the Enemy to be used so it can make informed decisions.
     */
    public abstract void Action(Base_Enemy context);

    public virtual void OnDrawGizmos(Base_Enemy context)
    {

    }
}
