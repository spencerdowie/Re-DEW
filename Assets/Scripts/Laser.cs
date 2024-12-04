using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float laserSpeed = 2f;
    [SerializeField]
    private float laserMaxDistance = 10f;
    [SerializeField]
    private int maxSegments = 3;
    [SerializeField]
    private float laserLifetime = 5f;
    private Queue<Vector3> points;
    private Vector3 destination;
    private UnityAction returnAmmo;

    public void Setup(int player, UnityAction returnAmmoAction)
    {
        returnAmmo = returnAmmoAction;
        points = new Queue<Vector3>();
        float distanceLeft = laserMaxDistance;

        int numSegments = 0;
        Vector3 segmentOrigin = transform.position, segmentDir = transform.forward;

        while (distanceLeft > 0 && numSegments < maxSegments &&
            Physics.Raycast(segmentOrigin, segmentDir, out RaycastHit hit, distanceLeft))
        {
            numSegments++;
            distanceLeft -= hit.distance;
            Debug.DrawLine(segmentOrigin, hit.point, Color.red, 3);
            segmentOrigin = hit.point;
            segmentDir = Vector3.Reflect(segmentDir, hit.normal);
            points.Enqueue(hit.point);
        }
        StartCoroutine(MoveLaser());
        StartCoroutine(DespawnCountdown());
    }

    private IEnumerator MoveLaser()
    {
        bool hasDest = true;
        destination = points.Dequeue();
        while (hasDest)
        {
            float distance = laserSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, destination, distance);

            float remainingDistance = Vector3.Distance(transform.position, destination);
            if (remainingDistance <= distance)
            {
                hasDest = points.TryDequeue(out destination);
            }

            yield return null;
        }
    }

    private IEnumerator DespawnCountdown()
    {
        float despawnTimer = 0f;
        while (despawnTimer < laserLifetime)
        {
            despawnTimer += Time.deltaTime;
            yield return null;
        }
        returnAmmo.Invoke();
        yield return new WaitForFixedUpdate();
        Destroy(gameObject);
    }
}
