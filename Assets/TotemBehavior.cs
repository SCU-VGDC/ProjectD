using UnityEngine;
using System.Collections.Generic;

public class TotemScript : MonoBehaviour
{
    public GameObject Connector; // Reference to the Totem/Connector prefab
    public GameObject TotemInvincible; // Reference to the Totem/TotemInvincible prefab
    public List<Base_Enemy> TotemList;

    private void Start()
    {
        foreach (Base_Enemy enemy in TotemList)
        {
            DrawLineToEnemy(enemy);
            InstantiateCircleAroundEnemy(enemy);
        }
    }

    private void DrawLineToEnemy(Base_Enemy enemy)
    {
        // Instantiate a new line renderer object
        GameObject newConnector = Instantiate(Connector, transform.position, Quaternion.identity, transform);
        
        // Get the line renderer component and set its positions
        LineRenderer line = newConnector.GetComponent<LineRenderer>();
        if (line != null)
        {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, enemy.transform.position);
        }
    }

    private void InstantiateCircleAroundEnemy(Base_Enemy enemy)
    {
        Instantiate(TotemInvincible, enemy.transform.position, Quaternion.identity, enemy.transform);
    }
}
