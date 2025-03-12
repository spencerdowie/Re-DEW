using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Laser : MonoBehaviour
{
    private const float LaserHeight = 0.2f;
    [SerializeField]
    private Transform laserPoint;
    [SerializeField]
    private GameObject hitboxPrefab;
    private BoxCollider currentHitbox;
    private Vector3 destination;
    private UnityAction returnAmmo;
    private TrailRenderer trail;
    public int PlayerIndex { get; private set; } = -1;
    private int hitLayer = -1;

    [field: SerializeField]
    public float LaserSpeed { get; private set; } = 6f;
    [field: SerializeField]
    public float LaserMaxDistance { get; private set; } = 20f;
    [field: SerializeField]
    public int MaxSegments { get; private set; } = 3;
    [field: SerializeField]
    public float LaserLifetime { get; private set; } = 5f;
    [field: SerializeField]
    public float LaserLength { get; private set; } = -1f;
    public bool shortLaser = false;
    private Color playerColour;

    public void Setup(int playerIndex, int playerLayer, Color playerColour, UnityAction returnAmmoAction)
    {
        returnAmmo = returnAmmoAction;
        PlayerIndex = playerIndex;
        hitLayer = playerLayer;
        //Debug.Log("Laser Spawned by Player " + playerIndex);

        name = "Player " + playerIndex + " Laser";


        trail = GetComponentInChildren<TrailRenderer>();
        //trail.startColor = playerColour;
        //trail.endColor = playerColour;
        trail.material.color = playerColour;
        trail.time = LaserLength / LaserSpeed;
        this.playerColour = playerColour;

        transform.SetParent(null);

        Vector3 position = transform.position;
        position.y = LaserHeight;
        transform.position = position;

        if (shortLaser)
        {
            StartCoroutine(MoveLaserShort(CreatePoints()));
        }
        else
        {
            StartCoroutine(MoveLaser(CreatePoints()));
            StartCoroutine(DespawnCountdown());
        }
    }

    private Queue<Vector3> CreatePoints()
    {
        Queue<Vector3> points = new Queue<Vector3>();
        float distanceLeft = LaserMaxDistance;
        int numSegments = 0;
        Vector3 segmentOrigin = laserPoint.position, segmentDir = laserPoint.forward;

        segmentOrigin.y = LaserHeight;
        segmentDir.y = 0;

        //Debug.Log("Laser Origin: " + segmentOrigin.ToString());

        while (distanceLeft > 0 && numSegments < MaxSegments)
        {
            bool missed = !Physics.Raycast(segmentOrigin, segmentDir, out RaycastHit hit, 100, LayerMask.GetMask("Default"));
            if (missed)
            {
                hit.point = segmentOrigin + (segmentDir * distanceLeft);
                hit.distance = LaserMaxDistance;
            }

            Vector3 point = hit.point;
            point.y = LaserHeight;

            Vector3 normal = hit.normal;
            normal.y = 0;

            numSegments++;
            distanceLeft -= hit.distance;
            Debug.DrawLine(segmentOrigin, point, Color.red, 3);
            segmentOrigin = point;
            segmentDir = Vector3.Reflect(segmentDir, normal);
            points.Enqueue(point);
        }
        return points;
    }

    private BoxCollider SpawnHitbox()
    {
        BoxCollider hitbox = Instantiate(hitboxPrefab, transform).GetComponent<BoxCollider>();
        hitbox.transform.position = laserPoint.position;
        hitbox.gameObject.layer = hitLayer;
        hitbox.transform.LookAt(destination);
        hitbox.name = "Player " + PlayerIndex + " Laser Hitbox";
        return hitbox;
    }

    private IEnumerator MoveLaser(Queue<Vector3> points)
    {
        bool hasDest = true;
        destination = points.Dequeue();
        currentHitbox = SpawnHitbox();
        while (hasDest)
        {
            yield return new WaitForFixedUpdate();

            if (PauseMenu.Instance.IsPaused)
                continue;

            float distance = LaserSpeed * Time.deltaTime;
            Vector3 newPos = Vector3.MoveTowards(laserPoint.position, destination, distance);
            newPos.y = 0.2f;
            laserPoint.position = newPos;

            currentHitbox.size = currentHitbox.size + Vector3.forward * distance;
            currentHitbox.center = currentHitbox.center + Vector3.forward * (distance / 2f);

            float remainingDistance = Vector3.Distance(laserPoint.position, destination);
            if (remainingDistance <= distance)
            {
                hasDest = points.TryDequeue(out destination);
                currentHitbox = SpawnHitbox();
            }
        }
    }

    private LaserSegment SpawnSegment(Vector3 destination)
    {
        LaserSegment segment = Instantiate(hitboxPrefab, transform).GetComponent<LaserSegment>();
        segment.Setup(hitLayer, laserPoint.position, destination, PlayerIndex, LaserSpeed, LaserLength, playerColour);
        return segment;
    }

    private IEnumerator MoveLaserShort(Queue<Vector3> points)
    {
        bool hasDest = true;
        Vector3 destination = points.Dequeue();
        LaserSegment currentSegment = SpawnSegment(destination);
        while (hasDest)
        {
            yield return new WaitForFixedUpdate();

            if (PauseMenu.Instance.IsPaused)
                continue;

            float distance = LaserSpeed * Time.deltaTime;
            Vector3 newPos = Vector3.MoveTowards(laserPoint.position, destination, distance);
            newPos.y = 0.2f;
            laserPoint.position = newPos;

            currentSegment.UpdateSegment(distance);

            float remainingDistance = Vector3.Distance(laserPoint.position, destination);
            if (remainingDistance <= distance)
            {
                StartCoroutine(currentSegment.ShrinkSegment());
                hasDest = points.TryDequeue(out destination);
                currentSegment = SpawnSegment(destination);
            }
        }
        returnAmmo?.Invoke();
        yield return new WaitForFixedUpdate();
        Destroy(gameObject);
    }

    private IEnumerator DespawnCountdown()
    {
        float despawnTimer = 0f;
        while (despawnTimer < LaserLifetime)
        {
            if (!PauseMenu.Instance.IsPaused)
                despawnTimer += Time.deltaTime;
            yield return null;
        }
        returnAmmo?.Invoke();
        yield return new WaitForFixedUpdate();
        Destroy(gameObject);
    }
}
