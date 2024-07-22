using System.Collections;
using UnityEngine;

public class ActorShooting : MonoBehaviour
{
    Color rayColor = Color.white;
    float fadeTime = 0.5f;
    public GameObject trailSpawn;
    public Transform bulletspawn; //this puts the spawn position 
    public GameObject bulletprefab; //this creates a gameobject;
    public GameObject grenadePrefab;
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

    private Vector2 getDirectionAiming()
    {
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPos.z = bulletspawn.position.z;

        return (targetPos - bulletspawn.position).normalized;
    }

    public void ShootGrenade(float force = 10)
    {
        GameObject proj = Instantiate(grenadePrefab, bulletspawn.position, bulletspawn.rotation);
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();

        rb.AddForce(getDirectionAiming() * force, ForceMode2D.Impulse);
    }

    public void ShootRaycastSingleBullet(int damage, int numOfMaxPenetrations, int numOfMaxRicochets)
    {
        // 0 index babyyy
        if (numOfMaxRicochets > 0) {
            numOfMaxRicochets--;
        }

        Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPos.z = bulletspawn.position.z;

        // set initial raycast values from player
        Vector2 rayCastOrigin = bulletspawn.position;
        Vector2 rayCastDir = getDirectionAiming();

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
                EventManager.singleton.AddEvent(new applyDamagemsg(gameObject, hit.transform.GetComponent<ActorHealth>(), damage));

                numOfPenetrations++;
                moveOriginBy = Mathf.Sqrt((hit.collider.bounds.size.x * hit.collider.bounds.size.x) + (hit.collider.bounds.size.y * hit.collider.bounds.size.y));
            }

            // setup raycast for next iteration
            rayCastOrigin = hit.point;
        }
    }

    public void ShootRaycastSpreadBullets(int damage, float range, float degrees, int numOfRays)
    {
        // set initial raycast values from player
        Vector2 rayCastOrigin = bulletspawn.position;
        Vector2 rayCastDirMiddle = getDirectionAiming();

        // just a vector used to figure out angles relative to, prob no touchy
        Vector3 referenceAngle = Vector3.forward;

        // calculate the angle endpoints
        float middleAngle = Vector2.Angle(rayCastDirMiddle, referenceAngle);
        float startAngle = middleAngle + (degrees / 2);
        float endAngle = middleAngle - (degrees / 2);        

        for (int i = 0; i < numOfRays; i++) {
            // find angle to shoot at
            float angle = Mathf.Lerp(startAngle, endAngle, (float)i / numOfRays);
            
            // convert angle to Vector2
            Vector2 rayCastDir = Quaternion.AngleAxis(angle, referenceAngle) * rayCastDirMiddle;

            // save hit
            RaycastHit2D hit = Physics2D.Raycast(rayCastOrigin, rayCastDir, range, allLayers);
            float lineDist = hit.collider == null ? range : hit.distance;

            // make sure you hit something that takes damage, skill issue
            if (hit.collider != null && hittableLayers == (hittableLayers | (1 << hit.collider.gameObject.layer))) {
                // apply damage
                EventManager.singleton.AddEvent(new applyDamagemsg(gameObject, hit.transform.GetComponent<ActorHealth>(), damage));
            }

            // draw line
            GameObject line = Instantiate(trailSpawn);
            LineRenderer renderer = line.GetComponent<LineRenderer>();

            Ray ray = new Ray(rayCastOrigin, rayCastDir);
            renderer.SetPositions(new Vector3[2] { rayCastOrigin, ray.GetPoint(lineDist) });
            StartCoroutine(KillTrail(renderer));
        }
    }

    public void SetBullet(GameObject newBullet)
    {
        bulletprefab = newBullet;
    }

    IEnumerator KillTrail(LineRenderer renderer)
    {
        renderer.startWidth = renderer.endWidth = 0.1f;

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
