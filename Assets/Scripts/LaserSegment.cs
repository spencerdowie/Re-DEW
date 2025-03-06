using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSegment : MonoBehaviour
{
    private BoxCollider hitbox;
    private LineRenderer lineRenderer;
    private float length = -1f, speed = 0f;
    private Vector3 startPosition, destination;
    private float distanceTravelled = 0f;


    private void Awake()
    {
        hitbox = GetComponent<BoxCollider>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void Setup(Vector3 startPosition, int hitLayer, Vector3 destination, int playerIndex, float speed)
    {
        this.startPosition = startPosition;
        this.destination = destination;
        this.speed = speed;
        transform.position = startPosition;
        gameObject.layer = hitLayer;
        transform.LookAt(destination);
        name = "Player " + playerIndex + " Laser Hitbox";
    }

    public void UpdateSegment(float distance)
    {
        transform.position += Vector3.forward * distance;
        if (distanceTravelled < length)
        {
            distanceTravelled += distance;
            hitbox.size += Vector3.forward * distance;
            hitbox.center += Vector3.forward * (distance / 2f);
            lineRenderer.SetPosition(1, transform.position - startPosition);
        }
    }

    public IEnumerator ShrinkSegment()
    {
        Vector3 tailPosition = lineRenderer.GetPosition(1);
        float distance = -1f;
        do
        {
            distance = speed * Time.deltaTime;
            if (distanceTravelled < length)
            {
                distanceTravelled += distance;
            }
            else
            {
                tailPosition += Vector3.forward * distance;
                lineRenderer.SetPosition(1, tailPosition);
            }
            yield return null;
        }
        while (Vector3.Distance(tailPosition, destination) > distance);
    }
}
