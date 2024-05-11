using System.Collections;
using UnityEngine;

public class ActorShooting : MonoBehaviour
{
    Color rayColor = Color.white;
    float fadeTime = 0.5f;
    public GameObject trailSpawn;
    public  Transform bulletspawn; //this puts the spawn position 
    public GameObject bulletprefab; //this creates a gameobject;
    public LayerMask hittableLayers;
    public LayerMask reflectiveLayers;
    private LayerMask allLayers;
    public bool raycastToggle = false;

    private void Awake()
    {
        allLayers = hittableLayers + reflectiveLayers;
        
    }

    public void Shoot(Transform target = null)
    {
        GameObject proj = Instantiate(bulletprefab, bulletspawn.position, bulletspawn.rotation);
        if(target)
        {
            proj.transform.right = target.position - transform.position;
        }
    }

    public void ShootRaycast(int numOfMaxPenetrations, int numOfMaxRicochets)
    {
        // 0 index babyyy
        if (numOfMaxRicochets > 0) {
            numOfMaxRicochets--;
        }

        Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPos.z = bulletspawn.position.z;

        // set initial raycast values from player
        Vector2 rayCastOrigin = bulletspawn.position;
        Vector2 rayCastDir = (targetPos - bulletspawn.position).normalized;

        // check for ricochets and hittable objects
        RaycastHit2D hit;
        float moveOriginBy = 0.0f; // used so the raycast doesnt collider when instantiated again
        int numOfRicochets = 0;
        int numOfPenetrations = 0;
        while (numOfRicochets <= numOfMaxRicochets && numOfPenetrations <= numOfMaxPenetrations) {
            // make raycast to see what is hit from shot
            hit = Physics2D.Raycast(rayCastOrigin + (rayCastDir * moveOriginBy), rayCastDir, 100f, allLayers);

            float lastHitDistance = hit.collider == null ? 30.0f : hit.distance;

            // draw a line
            GameObject line = Instantiate(trailSpawn);
            LineRenderer renderer = line.GetComponent<LineRenderer>();

            Ray ray = new Ray(rayCastOrigin, rayCastDir);
            renderer.SetPositions(new Vector3[2] { rayCastOrigin, ray.GetPoint(lastHitDistance) });
            StartCoroutine(KillTrail(renderer));
            
            // make sure hit actually exists
            if (hit.collider == null) break;

            // if layer hit is in reflectiveLayers
            if (reflectiveLayers == (reflectiveLayers | (1 << hit.collider.gameObject.layer)))
            {
                numOfRicochets++;

                // setup reflected direction for next raycast iteration
                rayCastDir = Vector2.Reflect((hit.point - rayCastOrigin).normalized, hit.normal);
                moveOriginBy = 0.1f;
            } else {
                // hit an object in hittableLayers
                EventManager.singleton.AddEvent(new applyDamagemsg(gameObject, hit.transform.GetComponent<ActorHealth>(), 10));

                numOfPenetrations++;
                moveOriginBy = 5.0f; // TODO: will need a better fix eventually as some enemies may be larger
            }

            // setup raycast for next iteration
            rayCastOrigin = hit.point;
        }
    }

    public void SetBullet(GameObject newBullet)
    {
        bulletprefab = newBullet;
    }

    IEnumerator KillTrail(LineRenderer renderer)
    {
        Color start = new Color(rayColor.r, rayColor.g, rayColor.b, 1);
        Color end = new Color(rayColor.r, rayColor.g, rayColor.b, 0);

        renderer.startColor = start;
        renderer.endColor = start;
        float elapsed = 0;
        while(renderer.endColor.a > 0)
        {
            elapsed += Time.deltaTime;
            Color curr = Color.Lerp(start, end, elapsed / fadeTime);
            renderer.startColor = curr;
            renderer.endColor = curr;

            yield return new WaitForEndOfFrame();
        }
        Destroy(renderer.gameObject);

        yield return true;
    }
}
