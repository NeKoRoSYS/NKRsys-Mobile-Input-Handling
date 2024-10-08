using UnityEngine;
using System;
using System.Collections.Generic;

namespace NeKoRoSYS.InputHandling.Mobile.Legacy
{
    public class ControlPadLegacy : MonoBehaviour
    {
        [Header("Inputs")]
        [HideInInspector] public Vector2 delta = Vector2.zero;
        [SerializeField] private int touchLimit = 10;
        public HashSet<int> availableTouchIds = new();
        private Vector2 currentPos, lastPos;
        [Space]
        [Header("Events")]
        public Action<Vector2> OnTouchDrag;

        private bool IsTouchingPad(Touch touch) => RectTransformUtility.RectangleContainsScreenPoint((RectTransform)transform, touch.screenPosition);
        private void OnDisable() => ForceStopTouch();
        private void Update()
        {
            if (Input.touchCount == 0) return;
            if (availableTouchIds.Count <= touchLimit) GetTouch();
            delta = Vector2.zero;
            if (availableTouchIds.Count == 0) return;
            foreach (var touch in Input.touches)
                { if (availableTouchIds.Contains(touch.fingerId)) ProcessInput(touch); }
            OnTouchDrag?.Invoke(delta);
        }

        private void GetTouch()
        {
            foreach (var touch in Input.touches)
            {
                var touching = touch.phase == TouchPhase.Began;
                var notTouching = touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled;
                if (IsTouchingPad(touch) && touching) availableTouchIds.Add(touch.fingerId);
                else if (notTouching) availableTouchIds.Remove(touch.fingerId);
            }
        }

        private void ProcessInput(Touch touch)
        {
            if (touch.phase == TouchPhase.Moved)
            {
                currentPos += touch.deltaPosition;
                delta += currentPos - lastPos;
                lastPos = currentPos;
            } else if (touch.phase == TouchPhase.Stationary) delta = Vector2.zero;
        }

        public void ForceStopTouch()
        {
            delta = Vector2.zero;
            currentPos = Vector2.zero;
            lastPos = Vector2.zero;
            availableTouchIds.Clear();
        }
    }
}
