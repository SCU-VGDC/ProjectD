using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float radius = 3.0f;
    public float explodeTime = 3.0f;
    public int damage = 2;
    public LayerMask layersToDamage;
    private float startTime;

    void Start()
    {
        startTime = Time.deltaTime;
    }

    void Update()
    {
        explodeTime -= Time.deltaTime;

        if (explodeTime <= 0.0f)
        {
            Explode();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Helpers.MatchesLayerMask(collision.gameObject, layersToDamage))
        {
            Explode();
        }
    }

    private void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, layersToDamage);

        foreach (Collider2D hit in hits)
        {
            ActorHealth hitActorHealth = hit.GetComponent<ActorHealth>();

            if (hitActorHealth)
            {
                EventManager.singleton.AddEvent(new applyDamagemsg(gameObject, hitActorHealth, damage));
            }
        }

        Destroy(gameObject);
    }
}
