using Pathfinding;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private AIDestinationSetter destinationSetter;
    private void Start()
    {
        destinationSetter = GetComponent<AIDestinationSetter>();
        destinationSetter.target = GameObject.FindGameObjectWithTag("Player").transform;
    }
}
