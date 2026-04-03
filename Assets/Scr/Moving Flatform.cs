using UnityEngine;

public class MovingFlatform : MonoBehaviour
{
    public Transform[] points;
    public float movingSpeed;
    private int pointIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pointIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, points[pointIndex].position) < 0.1f)
        {
            pointIndex += 1;
        }
        if(pointIndex == points.Length)
        {
            pointIndex = 0;
        }
        transform.position = Vector2.MoveTowards(transform.position, points[pointIndex].position, movingSpeed * Time.deltaTime);
        
    }
}
