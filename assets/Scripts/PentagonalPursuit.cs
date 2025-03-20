using System.Collections.Generic;
using UnityEngine;

public class PentagonalPursuit2D : MonoBehaviour
{
    public int iterations = 500; // Number of pursuit steps
    public float speed = 0.05f;  // Speed of movement
    public LineRenderer linePrefab;  // Assign in Inspector
    public GameObject pointPrefab;   // Assign a small circle sprite in Inspector

    private List<Vector2> points = new List<Vector2>();
    private List<GameObject> markers = new List<GameObject>();
    private List<LineRenderer> lines = new List<LineRenderer>();

    void Start()
    {
        // Define pentagon points
        float radius = 5f;
        for (int i = 0; i < 5; i++)
        {
            float angle = i * Mathf.PI * 2 / 5;  // Angle of each vertex
            Vector2 pos = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
            points.Add(pos);

            // Create a visual sprite marker
            GameObject marker = Instantiate(pointPrefab, pos, Quaternion.identity);
            markers.Add(marker);

            // Create LineRenderer for each point
            LineRenderer line = Instantiate(linePrefab, transform);
            line.positionCount = 1;
            line.SetPosition(0, pos);
            lines.Add(line);
        }

        StartCoroutine(DrawPursuitCurve());
    }

    System.Collections.IEnumerator DrawPursuitCurve()
    {
        for (int step = 0; step < iterations; step++)
        {
            List<Vector2> newPoints = new List<Vector2>();

            for (int i = 0; i < 5; i++)
            {
                Vector2 start = points[i];
                Vector2 target = points[(i + 1) % 5]; // Chase the next point

                // Move towards target using Lerp
                Vector2 newPosition = Vector2.Lerp(start, target, speed);
                newPoints.Add(newPosition);

                // Update sprite marker positions
                markers[i].transform.position = new Vector3(newPosition.x, newPosition.y, 0);

                // Update line renderer
                lines[i].positionCount++;
                lines[i].SetPosition(lines[i].positionCount - 1, new Vector3(newPosition.x, newPosition.y, 0));
            }

            points = newPoints;
            yield return new WaitForSeconds(0.02f); // Pause for smooth animation
        }
    }
}
