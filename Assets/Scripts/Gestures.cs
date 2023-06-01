using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gestures : MonoBehaviour
{
    [HideInInspector]
    public bool swipeLeft, swipeUp, swipeRight, swipeDown = false;

    Vector2 touchStartPos;

    float minSwipePixelDistance = 100f;
    bool touchStarted = false;

    void Start()
    {
        
    }

    
    void Update()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];

            switch(touch.phase)
            {
                case TouchPhase.Began:
                    touchStarted = true;
                    touchStartPos = touch.position;

                    swipeLeft
                    = swipeRight
                    = swipeUp
                    = swipeDown
                    = false;
                    break;

                case TouchPhase.Ended:
                    if(touchStarted)
                    {
                        Swipe(touch);
                        touchStarted = false;
                    }
                    break;

                case TouchPhase.Canceled:
                    touchStarted = false;                    
                    break;

            }

        }
    }

    void Swipe(Touch touch)
    {
        Vector2 touchLastPos = touch.position;
        float distance = Vector2.Distance(touchLastPos, touchStartPos);

        if(distance > minSwipePixelDistance)
        {
            float dy = touchLastPos.y - touchStartPos.y;
            float dx = touchLastPos.x - touchStartPos.x;

            float swipeAngle = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
            swipeAngle = (swipeAngle + 360) % 360;

            if(swipeAngle < 45 || swipeAngle > 315)
            {
                swipeRight = true;
                //Debug.Log("Right");
            }
            else if (swipeAngle > 45 && swipeAngle < 135)
            {
                swipeUp = true;
                //Debug.Log("Up");
            }

            else if (swipeAngle > 135 && swipeAngle < 225)
            {
                swipeLeft = true;
                //Debug.Log("Left");
            }

            else if (swipeAngle > 225 && swipeAngle < 315)
            {
                swipeDown = true;
                //Debug.Log("Down");
            }
        }
    }
}
