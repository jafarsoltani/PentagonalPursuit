using System.Collections.Generic;
using UnityEngine;

public class PentagonalPursuitCurve : MonoBehaviour
{
    public int iterations = 30; // Number of pursuit steps
    public float stepFraction = 0.05f;  // Movement fraction per step
    public LineRenderer linePrefab; 

    private List<Vector2> points = new List<Vector2>(); 
    private List<LineRenderer> lines = new List<LineRenderer>();

    void Start()
    {
        float radius = 5f;  // Initial pentagon size
        
        for (int i = 0; i < 5; i++)
        {
            float angle = i * Mathf.PI * 2 / 5;
            points.Add(new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius));
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
                Vector2 target = points[(i + 1) % 5];
                Vector2 newPosition = Vector2.Lerp(start, target, stepFraction);
                newPoints.Add(newPosition);
            }
            
            DrawPentagon(newPoints);
            
            points = newPoints;
            yield return new WaitForSeconds(0.05f);
        }
    }
    
    void DrawPentagon(List<Vector2> vertices)
    {
        LineRenderer line = Instantiate(linePrefab, transform);
        line.positionCount = 6; // 5 sides + closing edge
        for (int i = 0; i < 5; i++)
        {
            line.SetPosition(i, new Vector3(vertices[i].x, vertices[i].y, 0));
        }
        line.SetPosition(5, new Vector3(vertices[0].x, vertices[0].y, 0)); // Close the shape
        SetGradient(line, Color.red, Color.yellow);
    }

    // Applies a color gradient to a LineRenderer
    void SetGradient(LineRenderer line, Color startColor, Color endColor)
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(startColor, 0f),
                new GradientColorKey(endColor, 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            }
        );

        line.colorGradient = gradient;
        line.material = new Material(Shader.Find("Sprites/Default")); 
    }
}
