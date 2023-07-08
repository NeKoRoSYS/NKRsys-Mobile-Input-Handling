using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NeKoRoSYS.InputHandling.Mobile.Legacy
{
    public class ControlPad : MonoBehaviour
    {
        [SerializeField] private int touchLimit = 10;
        [HideInInspector] public Vector2 delta = Vector2.zero;
        public UnityEvent<Vector2> OnTouchDrag;
        private Vector2 currentPos, lastPos;
        private HashSet<int> availableTouchIds = new HashSet<int>();

        private bool IsTouchingPad(Touch touch) => RectTransformUtility.RectangleContainsScreenPoint((RectTransform)transform, touch.position);
        private void Update()
        {
            if (Input.touchCount == 0) return;
            if (availableTouchIds.Count <= touchLimit) GetTouch();
            delta = Vector2.zero;
            if (availableTouchIds.Count == 0) return;
            foreach (var touch in Input.touches)
                { if (availableTouchIds.Contains(touch.fingerId)) ApplyTouch(touch); }
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

        private void ApplyTouch(Touch touch)
        {
            if (touch.phase == TouchPhase.Moved)
            {
                currentPos += touch.deltaPosition;
                delta += currentPos - lastPos;
                lastPos = currentPos;
            } else if (touch.phase == TouchPhase.Stationary) delta = Vector2.zero;
        }
    }
}
