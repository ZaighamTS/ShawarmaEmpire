using UnityEngine;
using System.Collections;

public class NPCMovement : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 2f;
    public float waitTime = 2f;
    public float rotationSpeed = 5f;

    private int currentIndex = 0;
    public Animator animator;

    private void Start()
    {
      //  animator = GetComponent<Animator>();
        StartCoroutine(MoveInLoop());
    }

    IEnumerator MoveInLoop()
    {
        while (true)
        {
            Transform target = waypoints[currentIndex];

          
            yield return StartCoroutine(RotateTowards(target.position));

            animator.SetBool("isWalking", true);

            // Move toward the target
            while (Vector3.Distance(transform.position, target.position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
                yield return null;
            }

            animator.SetBool("isWalking", false);

            // Wait at the waypoint
            yield return new WaitForSeconds(waitTime);

            // Go to next point in loop
            currentIndex = (currentIndex + 1) % waypoints.Length;
        }
    }

    IEnumerator RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                yield return null;
            }

            transform.rotation = targetRotation;
        }
    }
}
