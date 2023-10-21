using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lust : MonoBehaviour
{
    [SerializeField]
    bool tuckeredOut;
    public int stamina;
    public int maxStamina;
    [Tooltip("Stamina refill time in seconds.")]
    public float refillTime;

    public float cooldown;

    BossAction[] actions;

    [SerializeField]
    LustTeleportAction teleport;
    [SerializeField]
    LustVentAction vent;
    [SerializeField]
    LustSkeletonHeadAction skeleton;

    // Start is called before the first frame update
    void Start()
    {
        actions = new BossAction[]
        {
            teleport
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (tuckeredOut) return;

        if (stamina <= 0)
        {
            tuckeredOut = true;
            StartCoroutine(RefillStamina());
        }

        if (cooldown <= 0)
        {
            if (stamina >= vent.staminaCost && Random.Range(0f, 1f) > 0.5f)
            {
                DoMove(vent);
            }
            else
            {
                DoMove(teleport);
            }
        }
        else
        {
            cooldown -= Time.deltaTime;
        }
    }

    void DoMove(BossAction action)
    {
        if (stamina >= action.staminaCost)
        {
            //TODO: This could probably be cleaner? Maybe move to Activate() or its own member function of BossAction.
            stamina -= action.staminaCost;
            cooldown = action.cooldown;
            action.Activate(this);
        }
    }

    public IEnumerator RefillStamina()
    {
        yield return new WaitForSeconds(refillTime);

        stamina = maxStamina;
        tuckeredOut = false;
    }

    private void OnDrawGizmos()
    {
        if (GameManager.inst != null)
        {
            Gizmos.DrawWireSphere(teleport.FindFurthestAnchor(GameManager.inst.player.transform).position, 1f);
        }
    }
}

[System.Serializable]
public class LustTeleportAction : BossAction
{
    Transform lastAnchor;
    [SerializeField]
    Transform[] teleportAnchors;

    public Transform FindFurthestAnchor(Transform avoidant)
    {
        Transform furthest = teleportAnchors[0];
        float furthestDist = 0;

        foreach (Transform trans in teleportAnchors)
        {
            if (lastAnchor != null && trans.position == lastAnchor.position) {
                continue;
            }

            if (furthestDist < Vector2.Distance(avoidant.position, trans.position))
            {
                furthest = trans;
                furthestDist = Vector2.Distance(avoidant.position, trans.position);
            }
        }

        return furthest;
    }

    public override void Activate(Lust boss)
    {
        //TODO: Probably would want a more nuanced way of choosing where to TP. i.e. Could be cool to have Lust avoid her own gas clouds?
        lastAnchor = FindFurthestAnchor(GameManager.inst.player.transform);

        boss.transform.position = lastAnchor.position;
    }
}


[System.Serializable]
public class LustVentAction : BossAction
{
    Lust doer;

    [SerializeField]
    LayerMask collisionLayers;

    [SerializeField]
    float delay = 1.0f;
    [SerializeField]
    float gasDespawnDelay = 10f;
    [SerializeField]
    float gasDespawnTime = 0.25f;
    [SerializeField]
    Vector2 gasOffset = new Vector2(0, 5f);

    [SerializeField]
    GameObject hellPortal;
    [SerializeField]
    GameObject gasCloud;

    public override void Activate(Lust boss)
    {
        doer = boss;

        doer.StartCoroutine(SpawnGas());
    }

    //TODO: This function kinda blows lmao, but I can't call StartCoroutine unless the base class is a MonoBehaviour so we're kinda stuck.
    public IEnumerator SpawnGas()
    {
        RaycastHit2D[] hits = new RaycastHit2D[1];
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(collisionLayers);

        //Get floor beneath Player
        Physics2D.Raycast(GameManager.inst.player.transform.position, Vector2.down, filter, hits);

        //If the player is over a bottomless pit, attack fails.
        if (hits.Length == 0)
        {
            Fail(doer);
            yield return false;
        }
        
        //Spawn portal and wait
        GameObject portal = Object.Instantiate(hellPortal, hits[0].point, Quaternion.identity);
        yield return new WaitForSeconds(delay);

        //Spawn gas, kill portal, and wait
        GameObject gas = Object.Instantiate(gasCloud, hits[0].point + gasOffset, Quaternion.identity);
        Object.Destroy(portal);
        yield return new WaitForSeconds(gasDespawnDelay);

        //Kill gas cloud
        doer.StartCoroutine(KillGas(gas.GetComponent<SpriteRenderer>(), gasDespawnTime));

        yield return true;
        
    }

    public IEnumerator KillGas(SpriteRenderer renderer, float killTime)
    {
        //Despawn gas.
        Color start = Color.white;
        Color end = new Color(1, 1, 1, 0);

        renderer.color = start;
        float elapsed = 0;
        while (renderer.color.a > 0)
        {
            elapsed += Time.deltaTime;
            Color curr = Color.Lerp(start, end, elapsed / killTime);
            renderer.color = curr;

            yield return new WaitForEndOfFrame();
        }
        Object.Destroy(renderer.gameObject);
    }
}

[System.Serializable]
public class LustSkeletonHeadAction : BossAction
{
    [SerializeField]
    GameObject skeletonHead;

    public override void Activate(Lust boss)
    {
        Vector3 targetPosition = GameManager.inst.player.transform.position;
        GameObject head = Object.Instantiate(skeletonHead, boss.transform.position, Quaternion.identity);
    }
}


[System.Serializable]
public abstract class BossAction
{
    public int staminaCost;
    public float cooldown;

    public abstract void Activate(Lust boss);
    public virtual void Fail(Lust boss)
    {
        boss.stamina += staminaCost;
        boss.cooldown = 0;
    }
}