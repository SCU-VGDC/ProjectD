using System.Collections;
using UnityEngine;

public class ActorShooting : MonoBehaviour
{
    Color rayColor = Color.red;
    float fadeTime = 0.5f;
    public GameObject trailSpawn;
    public  Transform bulletspawn; //this puts the spawn position 
    public GameObject bulletprefab; //this creates a gameobject;
    public LayerMask hittableLayers;
    public bool raycastToggle = false;

    private void Awake()
    {
    }

    public void Shoot(Transform target = null)
    {
        GameObject proj = Instantiate(bulletprefab, bulletspawn.position, bulletspawn.rotation);
        if(target)
        {
            proj.transform.right = target.position - transform.position;
        }
    }

    public void ShootRaycast(int penetrations)
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(hittableLayers);
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPos.z = bulletspawn.position.z;

        RaycastHit2D[] shotHits = new RaycastHit2D[10];

        //RaycastHit2D hit = Physics2D.Raycast(bulletspawn.position, targetPos - bulletspawn.position, 100f, hittableLayers, -50f, 50f);
        Physics2D.Raycast(bulletspawn.position, targetPos - bulletspawn.position, filter, shotHits);

        //If penetrations = 0, hit only the first enemy in the bullet's path.
        for (int i = 0; i < penetrations + 1 && i < shotHits.Length; i++)
        {
            //shotHits is initialized to a size, so we have to make sure that a hit actually exists
            if (shotHits[i].collider == null) break;

            EventManager.singleton.AddEvent(new applyDamagemsg(gameObject, shotHits[i].transform.GetComponent<ActorHealth>(), 10));
        }

        GameObject line = Instantiate(trailSpawn);
        LineRenderer renderer = line.GetComponent<LineRenderer>();

        Ray ray = new Ray(bulletspawn.position, (targetPos - bulletspawn.position));
        renderer.SetPositions(new Vector3[2] { bulletspawn.position, ray.GetPoint(100) });
        StartCoroutine(KillTrail(renderer));
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
