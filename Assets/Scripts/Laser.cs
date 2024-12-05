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
    [SerializeField]
    private Transform laserPoint;
    [SerializeField]
    private GameObject hitboxPrefab;
    private BoxCollider currentHitbox;
    private Queue<Vector3> points;
    private Vector3 destination;
    private UnityAction returnAmmo;
    public UnityAction<int> giveKill;
    private int playerNumber = -1;

    public void Setup(int player, Color playerColour, UnityAction returnAmmoAction, UnityAction<int> giveKillAction)
    {
        returnAmmo = returnAmmoAction;
        giveKill = giveKillAction;
        playerNumber = player;
        points = new Queue<Vector3>();
        float distanceLeft = laserMaxDistance;

        TrailRenderer trail = GetComponentInChildren<TrailRenderer>();
        trail.startColor = playerColour;
        trail.endColor = playerColour;

        int numSegments = 0;
        Vector3 segmentOrigin = laserPoint.position, segmentDir = laserPoint.forward;

        while (distanceLeft > 0 && numSegments < maxSegments &&
            Physics.Raycast(segmentOrigin, segmentDir, out RaycastHit hit, distanceLeft, LayerMask.GetMask("Default")))
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

    private BoxCollider SpawnHitbox()
    {
        BoxCollider hitbox = Instantiate(hitboxPrefab, transform).GetComponent<BoxCollider>();
        hitbox.transform.position = laserPoint.position;
        hitbox.gameObject.layer = 6 + playerNumber;
        hitbox.transform.LookAt(destination);
        hitbox.name = "Player " + playerNumber + " Laser";
        return hitbox;
    }

    private IEnumerator MoveLaser()
    {
        bool hasDest = true;
        destination = points.Dequeue();
        currentHitbox = SpawnHitbox();
        while (hasDest)
        {
            float distance = laserSpeed * Time.deltaTime;
            laserPoint.position = Vector3.MoveTowards(laserPoint.position, destination, distance);

            currentHitbox.size = currentHitbox.size + Vector3.forward * distance;
            currentHitbox.center = currentHitbox.center + Vector3.forward * (distance / 2f);

            float remainingDistance = Vector3.Distance(laserPoint.position, destination);
            if (remainingDistance <= distance)
            {
                hasDest = points.TryDequeue(out destination);
                currentHitbox = SpawnHitbox();
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
        returnAmmo?.Invoke();
        yield return new WaitForFixedUpdate();
        Destroy(gameObject);
    }
}
