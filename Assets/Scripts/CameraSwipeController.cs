using UnityEngine;
using UnityEngine.EventSystems;

public class CameraSwipeController : MonoBehaviour
{
    public Camera mainCamera;
    public float swipeSpeed = 0.1f;
    public Vector2 xLimits = new Vector2(-10f, 10f);
    public Vector2 yLimits = new Vector2(-5f, 5f); // Adjust as needed
    public float elasticFactor = 0.2f;
    public float bounceBackSpeed = 5f;

    private Vector2 lastTouchPosition;
    private bool isDragging = false;
    private bool needsBounceBack = false;
    private bool isHorizontalSwipe = false;
    private bool swipeDirectionDetermined = false;

    void Update()
    {
        bool inputActive = false;

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;

            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPosition = touch.position;
                isDragging = true;
                swipeDirectionDetermined = false;
                inputActive = true;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Vector2 delta = touch.position - lastTouchPosition;

                if (!swipeDirectionDetermined && delta.magnitude > 5f)
                {
                    isHorizontalSwipe = Mathf.Abs(delta.x) > Mathf.Abs(delta.y);
                    swipeDirectionDetermined = true;
                }

                MoveCamera(delta, true);
                lastTouchPosition = touch.position;
                inputActive = true;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
                needsBounceBack = true;
            }
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            lastTouchPosition = Input.mousePosition;
            isDragging = true;
            swipeDirectionDetermined = false;
            inputActive = true;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 delta = (Vector2)Input.mousePosition - lastTouchPosition;

            if (!swipeDirectionDetermined && delta.magnitude > 5f)
            {
                isHorizontalSwipe = Mathf.Abs(delta.x) > Mathf.Abs(delta.y);
                swipeDirectionDetermined = true;
            }

            MoveCamera(delta, true);
            lastTouchPosition = Input.mousePosition;
            inputActive = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            needsBounceBack = true;
        }
#endif

        if (!inputActive && needsBounceBack)
        {
            BounceBack();
        }
    }

    void MoveCamera(Vector2 delta, bool allowElastic)
    {
        Vector3 move = Vector3.zero;

        if (swipeDirectionDetermined)
        {
            if (isHorizontalSwipe)
            {
                move = new Vector3(delta.x, 0f, 0f) * swipeSpeed * Time.deltaTime;
            }
            else
            {
                move = new Vector3(0f, -delta.y, 0f) * swipeSpeed * Time.deltaTime; // invert Y
            }
        }

        Vector3 newPos = mainCamera.transform.position + move;

        if (!allowElastic)
        {
            newPos.x = Mathf.Clamp(newPos.x, xLimits.x, xLimits.y);
            newPos.y = Mathf.Clamp(newPos.y, yLimits.x, yLimits.y);
        }
        else
        {
            newPos.x = Mathf.Clamp(newPos.x, xLimits.x - elasticFactor, xLimits.y + elasticFactor);
            newPos.y = Mathf.Clamp(newPos.y, yLimits.x - elasticFactor, yLimits.y + elasticFactor);
        }

        mainCamera.transform.position = newPos;
    }
    void BounceBack()
    {
        Vector3 pos = mainCamera.transform.position;
        float clampedX = Mathf.Clamp(pos.x, xLimits.x, xLimits.y);
        float clampedY = Mathf.Clamp(pos.y, yLimits.x, yLimits.y);

        Vector3 target = new Vector3(clampedX, clampedY, pos.z);
        mainCamera.transform.position = Vector3.Lerp(pos, target, Time.deltaTime * bounceBackSpeed);

        if (Vector3.Distance(mainCamera.transform.position, target) < 0.01f)
        {
            mainCamera.transform.position = target;
            needsBounceBack = false;
        }
    }
}
