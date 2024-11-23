using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyPathingTst : MonoBehaviour
{
    [Header("Waypoint Settings")]
    public GameObject WaypointPrefab;
    public Transform StartPoint;         // Starting position for the waypoints
    public Transform EndPoint;           // Central point towards which waypoints move
    public int WaypointCount = 10;       // Total number of waypoints to create
    public float PathWidth = 10f;        // Maximum width for snaking effect

    public Vector3 EndDirection;

    [Header("Map Settings")]
    public LayerMask GroundLayer;        // Layer that represents the ground
    public float GroundCheckDistance = 100f;  // Max distance for raycasting to find ground level

    private List<GameObject> Waypoints = new List<GameObject>();  // Store created waypoints

    private void Start()
    {
        
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.G))
        {
            GenerateWaypoints();
        }
    }

    private void GenerateWaypoints()
    {
        Vector3 startPos = StartPoint.position;
        Vector3 TowerPosition = EndPoint.position;

        // Calculate the total distance and direction towards the endpoint
        Vector3 directionToEnd = (TowerPosition - startPos).normalized;
        float totalDistance = Vector3.Distance(startPos, TowerPosition);

        EndDirection = directionToEnd;
        EndDirection.Normalize();
        GetOneAxisDirection(EndDirection);

        // Divide the distance into equal segments based on the number of waypoints
        float segmentLength = totalDistance / WaypointCount;

        // Track the current position for placing the next waypoint
        Vector3 currentPos = startPos;

        Waypoints.Add(Instantiate(WaypointPrefab, startPos, Quaternion.identity));
        
        for (int i = 0; i < WaypointCount; i++)
        {
            // Calculate next point on the path
            Vector3 nextPos = currentPos + directionToEnd * segmentLength;

            // Add a snaking effect by moving the waypoint left or right alternately
            float offset = (i % 2 == 0) ? PathWidth : -PathWidth;
            Vector3 offsetPosition = Vector3.Cross(Vector3.up, directionToEnd) * offset;
            nextPos += offsetPosition;

            // Ensure the waypoint is placed on the ground level by raycasting
            nextPos = AdjustToGround(nextPos);

            // Instantiate the waypoint at the calculated position
            GameObject newWaypoint = Instantiate(WaypointPrefab, nextPos, Quaternion.identity);
            Waypoints.Add(newWaypoint);

            // Set the current position to the new waypoint position for the next iteration
            currentPos = nextPos;
        }



        // Optionally: Add a final waypoint at the exact endpoint
        Vector3 finalWaypointPos = AdjustToGround(TowerPosition);
        GameObject finalWaypoint = Instantiate(WaypointPrefab, finalWaypointPos, Quaternion.identity);
        Waypoints.Add(finalWaypoint);
    }

    private Vector3 GetOneAxisDirection(Vector3 CurrentDirection)
    {
        Vector3 InputDirection = CurrentDirection.normalized;
        Vector3 AxisDirection=Vector3.zero;

        //AxisDirection

        return Vector3.one;
    }

    private Vector3 AdjustToGround(Vector3 targetPos)
    {
        RaycastHit hit;
        // Raycast downward from the target position to find the ground
        if (Physics.Raycast(targetPos + Vector3.up * GroundCheckDistance, Vector3.down, out hit, GroundCheckDistance * 2, GroundLayer))
        {
            Debug.Log("did it hit");
            return hit.point;  // Return the ground level point
        }

        return targetPos;  // Return original position if no ground hit
    }

    // Optional: Draw the waypoints and path in the editor
    private void OnDrawGizmos()
    {
        if (Waypoints.Count > 0)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < Waypoints.Count - 1; i++)
            {
                Gizmos.DrawLine(Waypoints[i].transform.position, Waypoints[i + 1].transform.position);
                Gizmos.DrawLine(Waypoints[i].transform.position, Vector3.down*GroundCheckDistance*2);
            }
        }
    }

}
