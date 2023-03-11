using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Trail_Behavior : MonoBehaviour
{
    private Vector3 star_pos;
    private Vector3 my_target_pos;
    private float progress;

    [SerializeField] private float speed = 40f;

    // Start is called before the first frame update
    void Start()
    {
        star_pos = transform.position.WithAxis(Axis.Z, value: -1);
    }

    // Update is called once per frame
    void Update()
    {
        progress += Time.deltaTime * speed;
        transform.position = Vector3.Lerp(a: star_pos, b: my_target_pos, progress);

        //make prefab destroy itself after 2 seconds
        Destroy(gameObject, 2f);
    }

    public void SetTargetPosition(Vector3 target_pos)
    {
        my_target_pos = target_pos.WithAxis(Axis.Z, value: -1);
    }
}
