using UnityEngine;

public class EagleCircularSwoop : MonoBehaviour
{
    public Transform centerPoint;       // The point around which the eagle circles
    public float radius = 10f;          // Radius of the circular path
    public float angularSpeed = 1f;     // How fast the eagle circles (radians/sec)
    public float swoopAmplitude = 2f;   // How high/low the swoop goes
    public float swoopFrequency = 1f;   // How fast the eagle swoops up and down
    public bool faceDirection = true;   // Should eagle rotate to face movement

    private float angle = 0f;
    private float baseHeight;

    void Start()
    {
        if (centerPoint != null)
            baseHeight = centerPoint.position.y + 10f; // Base flying height above center
        else
            baseHeight = transform.position.y;
    }

    void Update()
    {
        // Update angle over time
        angle += angularSpeed * Time.deltaTime;

        // Circular XZ position
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        // Swooping vertical motion (Y)
        float y = baseHeight + Mathf.Sin(Time.time * swoopFrequency) * swoopAmplitude;

        // Final position
        Vector3 newPosition = new Vector3(x, y, z) + centerPoint.position;

        // Move eagle to new position
        Vector3 previousPosition = transform.position;
        transform.position = newPosition;

        // Optional: Face movement direction
        if (faceDirection)
        {
            Vector3 direction = (newPosition - previousPosition).normalized;
            if (direction != Vector3.zero)
                transform.forward = direction;
        }
    }
}
