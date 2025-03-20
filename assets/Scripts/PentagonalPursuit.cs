using System.Collections.Generic;
using UnityEngine;

public class DualPentagonalPursuit2D : MonoBehaviour
{
    public int iterations = 500; // Number of pursuit steps
    public float speed = 0.05f;  // Speed of movement
    public LineRenderer linePrefab;  // Assign in Inspector
    public GameObject pointPrefab;   // Assign a small circle sprite in Inspector

    private List<Vector2> innerPoints = new List<Vector2>(); // Moving points
    private List<Vector2> outerPoints = new List<Vector2>(); // Fixed outer pentagon
    private List<GameObject> innerMarkers = new List<GameObject>();
    private List<GameObject> outerMarkers = new List<GameObject>();
    private List<LineRenderer> pursuitLines = new List<LineRenderer>();
    private List<LineRenderer> connectionLines = new List<LineRenderer>();
    private LineRenderer outerPentagonLine; // Line connecting outer points

    void Start()
    {
        float innerRadius = 3f;  // Radius of inner pentagon
        float outerRadius = 6f;  // Radius of outer pentagon

        outerPentagonLine = Instantiate(linePrefab, transform);
        outerPentagonLine.positionCount = 6; // 5 points + closing back to start
        SetGradient(outerPentagonLine, Color.red, Color.yellow); // Colour Gradient

        for (int i = 0; i < 5; i++)
        {
            float angle = i * Mathf.PI * 2 / 5;  // Angle of each vertex

            // Define Outer Pentagon (Static)
            Vector2 outerPos = new Vector2(Mathf.Cos(angle) * outerRadius, Mathf.Sin(angle) * outerRadius);
            outerPoints.Add(outerPos);

            GameObject outerMarker = Instantiate(pointPrefab, outerPos, Quaternion.identity);
            outerMarkers.Add(outerMarker);

            // Define Inner Pentagon (Initial positions)
            Vector2 innerPos = new Vector2(Mathf.Cos(angle) * innerRadius, Mathf.Sin(angle) * innerRadius);
            innerPoints.Add(innerPos);

            GameObject innerMarker = Instantiate(pointPrefab, innerPos, Quaternion.identity);
            innerMarkers.Add(innerMarker);

            // Create LineRenderers for pursuit paths
            LineRenderer pursuitLine = Instantiate(linePrefab, transform);
            pursuitLine.positionCount = 1;
            pursuitLine.SetPosition(0, innerPos);
            SetGradient(pursuitLine, Color.blue, Color.cyan);
            pursuitLines.Add(pursuitLine);

            // Create LineRenderers for outer-inner connections
            LineRenderer connectionLine = Instantiate(linePrefab, transform);
            connectionLine.positionCount = 2;
            connectionLine.SetPosition(0, outerPos);
            connectionLine.SetPosition(1, innerPos);
            SetGradient(connectionLine, Color.green, Color.magenta);
            connectionLines.Add(connectionLine);
        }

        // Close the outer pentagon by looping back to the first point
        for (int i = 0; i < 5; i++)
        {
            outerPentagonLine.SetPosition(i, new Vector3(outerPoints[i].x, outerPoints[i].y, 0));
        }
        outerPentagonLine.SetPosition(5, new Vector3(outerPoints[0].x, outerPoints[0].y, 0)); // Close shape

        StartCoroutine(DrawPursuitCurve());
    }

    System.Collections.IEnumerator DrawPursuitCurve()
    {
        for (int step = 0; step < iterations; step++)
        {
            List<Vector2> newInnerPoints = new List<Vector2>();

            for (int i = 0; i < 5; i++)
            {
                Vector2 start = innerPoints[i];
                Vector2 target = innerPoints[(i + 1) % 5]; // Chase next point
                Vector2 newPosition = Vector2.Lerp(start, target, speed);
                newInnerPoints.Add(newPosition);

                // Update inner markers
                innerMarkers[i].transform.position = new Vector3(newPosition.x, newPosition.y, 0);

                // Update pursuit path
                pursuitLines[i].positionCount++;
                pursuitLines[i].SetPosition(pursuitLines[i].positionCount - 1, new Vector3(newPosition.x, newPosition.y, 0));

                // Update outer-inner connection lines
                connectionLines[i].SetPosition(1, new Vector3(newPosition.x, newPosition.y, 0));
            }

            innerPoints = newInnerPoints;
            yield return new WaitForSeconds(0.02f);
        }
    }

    // Function to Apply a Colour Gradient to LineRenderers
    void SetGradient(LineRenderer line, Color startColor, Color endColor)
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(startColor, 0f), new GradientColorKey(endColor, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
        );

        line.colorGradient = gradient;
    }
}
