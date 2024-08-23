using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using System;

namespace NeKoRoSYS.InputHandling.Mobile
{
    public enum StickMode { Fixed, Free, Floating }
    public enum StickAxis { X, Y, Both }
    public class ControlStick : OnScreenControl, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Header("Visuals")]
        [SerializeField] private RectTransform inputArea = null;
        [SerializeField] public RectTransform stickBounds = null;
        [SerializeField] public RectTransform stickHandle = null;
        [SerializeField] private float fullInputThreshold = 0.75f;
        [SerializeField] private float followThreshold = 1;
        [SerializeField] private float stickRange = 1f;
        [SerializeField] private float moveThreshold = 0.17f;
        [Space]
        [Header("Inputs")]
        [InputControl(layout = "Vector2")]
        [SerializeField] private string m_ControlPath;
        protected override string controlPathInternal { get => m_ControlPath; set => m_ControlPath = value; }
        [SerializeField] private ControlExtension controlExtension;
        [SerializeField] public StickMode stickMode;
        public StickMode StickMode
        {
            get => stickMode;
            set
            {
                stickMode = value;

                if (stickMode == StickMode.Fixed)
                {
                    stickBounds.anchoredPosition = fixedPosition;
                    stickHandle.anchoredPosition = Vector2.zero;
                }
                stickBounds.gameObject.SetActive(stickMode == StickMode.Fixed);
                inputArea.GetComponent<Image>().raycastTarget = stickMode != StickMode.Fixed;
            }
        }
        [SerializeField] private StickAxis stickAxis = StickAxis.Both;
        public StickAxis StickAxis { get => stickAxis; set => stickAxis = value; }
        [SerializeField] private bool fullInput;
        public bool FullInput
        {
            get => fullInput;
            set
            {
                fullInput = value;
                OnStickFullInput?.Invoke(fullInput);
                controlExtension?.ProcessButton(fullInput == true ? 1f : 0f);
            }
        }
        private Vector2 rawInput = Vector2.zero;
        public Vector2 RawInput
        {
            get => rawInput;
            set
            {
                rawInput = value;
                input = value;
                input = StickAxis == StickAxis.X ? new(input.x, 0f) : StickAxis == StickAxis.Y ? new(0f, input.y) : input;
            }
        }
        public Vector2 input = Vector2.zero;
        public bool touched = false;
        public int touchId;

        [Header("Events")]
        public Action<Vector2> OnStickDrag;
        public Action<bool> OnStickFullInput;

        private Vector2 center = new(0.5f, 0.5f);
        private Vector2 fixedPosition;
        private Canvas canvas;
        private Camera cam;

        private void Start() => FormatControlStick();

        protected override void OnDisable()
        {
            base.OnDisable();
            ForceStopTouch();
        }
        
        private void FormatControlStick()
        {
            canvas = GetComponentInParent<Canvas>();
            stickBounds.pivot = center;
            fixedPosition = stickBounds.anchoredPosition;
            stickHandle.anchorMin = center;
            stickHandle.anchorMax = center;
            stickHandle.pivot = center;
            stickHandle.anchoredPosition = Vector2.zero;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (touched) return;
            touched = true;
            touchId = eventData.pointerId;
            if (eventData.pointerId != touchId) return;
            if (StickMode != StickMode.Fixed)
            {
                bool inRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(inputArea, eventData.position, cam, out Vector2 localPoint);
                stickBounds.anchoredPosition = inRect ? localPoint - (stickBounds.anchorMax * inputArea.sizeDelta) + inputArea.pivot * inputArea.sizeDelta : Vector2.zero;
                stickBounds.gameObject.SetActive(true);
            }
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!touched || eventData.pointerId != touchId) return;
            cam = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;
            Vector2 radius = stickBounds.sizeDelta / 2;
            RawInput = (eventData.position - RectTransformUtility.WorldToScreenPoint(cam, stickBounds.position)) / (radius * canvas.scaleFactor);
            ProcessInput(input.magnitude, input.normalized, radius);
        }

        private void ProcessInput(float magnitude, Vector2 normalized, Vector2 radius)
        {
            if (StickMode == StickMode.Floating && magnitude > followThreshold)
            {
                Vector2 difference = normalized * (magnitude - followThreshold) * radius;
                stickBounds.anchoredPosition += difference;
            }
            FullInput = magnitude > fullInputThreshold;
            input = magnitude > moveThreshold ? (magnitude < fullInputThreshold ? normalized * 0.45f : normalized * 0.90f) : Vector2.zero;
            stickHandle.anchoredPosition = input * stickRange * radius;
            OnStickDrag?.Invoke(input);
            SendValueToControl(input);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId != touchId) return;
            touched = false;
            if (StickMode != StickMode.Fixed) stickBounds.gameObject.SetActive(false);
            FullInput = false;
            input = Vector2.zero;
            stickHandle.anchoredPosition = Vector2.zero;
            OnStickDrag?.Invoke(Vector2.zero);
            SendValueToControl(Vector2.zero);
        }

        public void SetJoystick(int stickIndex) => StickMode = (StickMode)stickIndex;

        public void ForceStopTouch()
        {
            touched = false;
            touchId = 0;
            FullInput = false;
            stickHandle.transform.position = Vector2.zero;
            input = Vector2.zero;
            stickBounds.gameObject.SetActive(!(stickMode != StickMode.Fixed));
        }
    }
}