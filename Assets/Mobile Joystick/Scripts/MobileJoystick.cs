using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileJoystick : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private RectTransform joystickZone;
    [SerializeField] private RectTransform joystickOutline;
    [SerializeField] private RectTransform joystickKnob;

    [Header(" Settings ")]
    [SerializeField] private float moveFactor;
    private Vector3 clickedPosition;
    private Vector3 move;
    private bool canControl;

    // Start is called before the first frame update
    void Start() => HideJoystick();

    private void OnDisable() => HideJoystick();

    // Update is called once per frame
    void Update()
    {
        if (canControl)
            ControlJoystick();
        else
            move = Vector3.zero;
    }

    public void ClickedOnJoystickZoneCallback()
    {
        // I think in here I need to detect which touch is controlling the joystick
        bool touchFound = false;

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            bool isInside = RectTransformUtility.RectangleContainsScreenPoint(joystickZone, touch.position);

            if (!isInside)
                continue;

            touchFound = true;

            clickedPosition = touch.position;

            ShowJoystick();
            break;
        }

        if(!touchFound)
        {
            canControl = false;
            HideJoystick();
        }
    }

    private void ShowJoystick()
    {
        joystickOutline.gameObject.SetActive(true);
        canControl = true;
    }

    private void HideJoystick()
    {
        canControl = false;
        move = Vector3.zero;

        joystickOutline.gameObject.SetActive(false);
    }

    private void ControlJoystick()
    {
        Vector3 currentPosition = GetCurrentTouchPosition();
        Vector3 direction = currentPosition - clickedPosition;

        float canvasScale = GetComponentInParent<Canvas>().GetComponent<RectTransform>().localScale.x;

        float moveMagnitude = direction.magnitude * moveFactor * canvasScale;

        float absoluteWidth = joystickOutline.rect.width / 2;
        float realWidth = absoluteWidth * canvasScale;

        moveMagnitude = Mathf.Min(moveMagnitude, realWidth);

        move = direction.normalized;
        Vector3 knobMove = move * moveMagnitude;

        // Added this to get a value between 0 & 1
        move = knobMove / realWidth;

        Vector3 targetPosition = joystickOutline.position + knobMove;

        joystickKnob.position = targetPosition;

        if (Input.GetMouseButtonUp(0) || Input.touchCount <= 0)
            HideJoystick();
    }

    Vector3 lastTouchPos;
    private Vector3 GetCurrentTouchPosition()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            bool isInside = RectTransformUtility.RectangleContainsScreenPoint(joystickZone, touch.position);

            if (!isInside)
                continue;

            lastTouchPos = touch.position;
            return touch.position;
        }

        return lastTouchPos;
    }

    public Vector3 GetMoveVector() => move;
}
