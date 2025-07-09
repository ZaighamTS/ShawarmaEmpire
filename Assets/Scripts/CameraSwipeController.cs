using UnityEngine;
using UnityEngine.EventSystems;

public class CameraSwipeController : MonoBehaviour
{
    [Header("Camera Settings")]
    public GameObject mainCamera;
    public float swipeSpeed = 0.2f;
    public float bounceBackSpeed = 5f;

    [Header("Movement Limits")]
    public Vector2 xLimits = new Vector2(-10f, 10f);
    public Vector2 zLimits = new Vector2(-10f, 10f);
    public float elasticMargin = 1.5f;

    private Vector2 lastInputPosition;
    private bool isDragging = false;
    private bool needsBounceBack = false;

    void Update()
    {
        HandleInput();
        if (!isDragging && needsBounceBack)
            BounceBack();
    }

    private void HandleInput()
    {
        Vector2 inputPosition = Vector2.zero;
        bool isTouch = false;
        bool inputBegan = false;
        bool inputMoved = false;
        bool inputEnded = false;

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUI()) return;
            inputPosition = Input.mousePosition;
            inputBegan = true;
        }
        else if (Input.GetMouseButton(0))
        {
            if (IsPointerOverUI()) return;
            inputPosition = Input.mousePosition;
            inputMoved = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            inputEnded = true;
        }
#else
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (IsPointerOverUI(touch.fingerId)) return;

            inputPosition = touch.position;
            isTouch = true;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    inputBegan = true;
                    break;
                case TouchPhase.Moved:
                    inputMoved = true;
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    inputEnded = true;
                    break;
            }
        }
#endif

        if (inputBegan)
        {
            lastInputPosition = inputPosition;
            isDragging = true;
            needsBounceBack = false;
        }
        else if (inputMoved && isDragging)
        {
            Vector2 delta = inputPosition - lastInputPosition;
            MoveCamera(delta);
            lastInputPosition = inputPosition;
        }
        else if (inputEnded)
        {
            isDragging = false;
            needsBounceBack = true;
        }
    }

    private void MoveCamera(Vector2 delta)
    {
        // Get camera's right and forward directions (ignore vertical component)
        Vector3 camRight = mainCamera.transform.right;
        Vector3 camForward = Vector3.Cross(camRight, Vector3.up); // ensures planar forward

        // Calculate movement relative to camera's orientation
        Vector3 move = (-delta.x * camRight + -delta.y * camForward) * swipeSpeed * Time.deltaTime;

        Vector3 newPos = mainCamera.transform.position + move;

        // Clamp with elastic margin
        newPos.x = Mathf.Clamp(newPos.x, xLimits.x - elasticMargin, xLimits.y + elasticMargin);
        newPos.z = Mathf.Clamp(newPos.z, zLimits.x - elasticMargin, zLimits.y + elasticMargin);

        mainCamera.transform.position = newPos;
    }

    private void BounceBack()
    {
        Vector3 pos = mainCamera.transform.position;
        float clampedX = Mathf.Clamp(pos.x, xLimits.x, xLimits.y);
        float clampedZ = Mathf.Clamp(pos.z, zLimits.x, zLimits.y);
        Vector3 target = new Vector3(clampedX, pos.y, clampedZ);

        mainCamera.transform.position = Vector3.Lerp(pos, target, Time.deltaTime * bounceBackSpeed);

        if (Vector3.Distance(pos, target) < 0.01f)
        {
            mainCamera.transform.position = target;
            needsBounceBack = false;
        }
    }

    private bool IsPointerOverUI(int fingerId = -1)
    {
        if (EventSystem.current == null) return false;
#if UNITY_EDITOR || UNITY_STANDALONE
        return EventSystem.current.IsPointerOverGameObject();
#else
        return EventSystem.current.IsPointerOverGameObject(fingerId);
#endif
    }
}
