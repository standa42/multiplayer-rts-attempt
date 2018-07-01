using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common;
using Assets.Scripts.Controls;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInput : MonoBehaviour
{
    public event SlideDelegate SlideEvent;
    public event TapDelegate TapEvent;
    public event DragDelegate DragEvent;
    public event DragEndDelegate DragEndEvent;

    public delegate void SlideDelegate(Vector2 vector);
    public delegate void TapDelegate(Vector2 position);
    public delegate void DragDelegate(Vector2 begin, Vector2 current);
    public delegate void DragEndDelegate(Vector2 begin, Vector2 end);

    /* variables belonging to the last touch
     * used for detection of gestures regarding
     * multiple begin phases
     */
    private TouchType lastTouchType;
    private DateTime lastTouchBeganTime;
    private Vector2 lastTouchBeganPosition;
    private DateTime lastTouchEndTime;

    /* variables holding state of current touch
     * constraint: touch starts with TouchType.Tap and than it may change
     */
    private TouchType touchType;
    private DateTime touchBeganTime;
    private Vector2 touchBeganPosition;
    private Vector2 touchPreviousPosition;

    /* Tap - finger touches display in one place for limited time
     * Slide - finger moves over the display
     * Drag - Tap + Slide following in short interval
     */
    private enum TouchType
    {
        Tap,
        Drag,
        Slide
    }
    
    void Start () {
		// initialization of lastTouch variables
        // for the sake of avoiding potential errors
        lastTouchType = TouchType.Tap;
        lastTouchBeganTime = DateTime.Now.AddSeconds(-100);
        lastTouchEndTime = DateTime.Now.AddSeconds(-99);
        lastTouchBeganPosition = new Vector2(0,0);

        Log.LogMessage("DistValTap: " + TouchConfig.AcceptedDistanceForTapInPixels);
        Log.LogMessage("DistDrag: " + TouchConfig.AcceptedDifferenceForDoubleTapInPixels);
    }
	
	// Update is called once per frame
	void Update ()
	{
        TouchProcessing();
    }

    /// <summary>
    /// Chcecks if there is touch and isn't over UI, than passes it to gesture detector
    /// </summary>
    private void TouchProcessing()
    {
        if (Input.touches.Length <= 0) return;

        var firstTouch = Input.touches[0];

        if (!EventSystem.current.IsPointerOverGameObject(firstTouch.fingerId))
        {
            GestureDetection(firstTouch);
        }
    }

    /// <summary>
    /// Calls appropriate function acording to touch phase
    /// </summary>
    /// <param name="touch"> Concrete touch to be processed </param>
    private void GestureDetection(Touch touch)
    {
        switch (touch.phase)
        {
            case TouchPhase.Began:
                TouchBegan(touch);
                break;
                
            case TouchPhase.Moved:
                TouchMovedOrPressed(touch);
                break;

            case TouchPhase.Stationary:
                TouchMovedOrPressed(touch);
                break;

            case TouchPhase.Ended:
                TouchEnded(touch);
                break;

            case TouchPhase.Canceled:
                TouchEnded(touch);
                break;
        }
    }

    private void TouchBegan(Touch touch)
    {
        // Initialization of touch variables
        touchBeganTime = DateTime.Now;
        touchBeganPosition = touch.position;
        touchPreviousPosition = touch.position;
        touchType = TouchType.Tap;

        // Checks if it is the beggining of drag gesture
        if (
            lastTouchType == TouchType.Tap &&
            (touchBeganTime - lastTouchBeganTime).TotalSeconds < TouchConfig.TimeBetweenTaps &&
            Vector2.Distance( touch.position, lastTouchBeganPosition ) < TouchConfig.AcceptedDifferenceForDoubleTapInPixels
            )
        {
            touchType = TouchType.Drag;
        }
    }

    private void TouchMovedOrPressed(Touch touch)
    {
        if (touchType == TouchType.Tap)
        {
            // Checks if it switches to the slide gesture
            touchType = (
                (Vector2.Distance(touch.position, touchBeganPosition) > TouchConfig.AcceptedDistanceForTapInPixels) &&
                (touchType == TouchType.Tap))
                ? TouchType.Slide : TouchType.Tap;
        }

        if (touchType == TouchType.Slide)
        { 
            Slide(touch.position - touchPreviousPosition);
        }

        if (touchType == TouchType.Drag)
        {
            Drag(touchBeganPosition, touch.position);
        }

        touchPreviousPosition = touch.position;
    }

    private void TouchEnded(Touch touch)
    {
        if (touchType == TouchType.Tap && ((DateTime.Now - touchBeganTime).TotalSeconds < TouchConfig.TimeForValidTap))
        {
            Tap(touchBeganPosition);
        }

        if (touchType == TouchType.Drag)
        {
            DragEnd(touchBeganPosition, touch.position);
        }

        // Assign touch variables to lastTouch variables for next update
        lastTouchType = touchType;
        lastTouchBeganTime = touchBeganTime;
        lastTouchBeganPosition = touchBeganPosition;
        lastTouchEndTime = DateTime.Now;
    }

    /// <summary>
    /// Tap event
    /// </summary>
    /// <param name="position"> Position where tap took place on display</param>
    private void Tap(Vector2 position)
    {
        Log.LogMessage("Tap: " + position);
    }

    /// <summary>
    /// Drag event
    /// </summary>
    /// <param name="begin"> Position of origin of drag</param>
    /// <param name="current"> Current position of drag</param>
    private void Drag(Vector2 begin, Vector2 current)
    {
        Log.LogMessage("Drag: " + begin + " " + current);
    }

    /// <summary>
    /// Drag end event
    /// </summary>
    /// <param name="begin"> Origin of drag event on display</param>
    /// <param name="end"> End position of drag event on display</param>
    private void DragEnd(Vector2 begin, Vector2 end)
    {
        Log.LogMessage("Drag end: " + begin + " " + end);
    }

    /// <summary>
    /// Slide event
    /// </summary>
    /// <param name="differenceVector"> Vector describing difference between cursor on current and previous frame </param>
    private void Slide(Vector2 differenceVector)
    {
        Log.LogMessage("Slide: " + differenceVector);
        SlideEvent(differenceVector);
    }

}


