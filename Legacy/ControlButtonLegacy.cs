using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace NeKoRoSYS.InputHandling.Mobile.Legacy
{
    public class ControlButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Visuals")]
        [SerializeField] private Sprite pressedIcon;
        [SerializeField] private Color releasedColor = Color.white, pressedColor = Color.grey;
        [SerializeField] private bool animateSprite = true, animateColor = true;
        private Image icon;
        private Sprite releasedIcon;

        [Header("Input")]
        public bool isPressed = false;
        private Vector2 startPos;
        private bool touched = false;
        private int touchId, touchAmount;
        private readonly float maxTapInterval = 15f;

        [Header("Events")]
        public UnityEvent<bool> OnButtonAction;
        public UnityEvent OnDoubleTap;
        private Coroutine visualCoroutine;
        
        private GraphicRaycaster raycaster;

        internal void ResetTapAmount() => touchAmount = 0;
        public void OnPointerDown(PointerEventData eventData)
        {
            if (touched) return;
            touched = true;
            touchId = eventData.pointerId;
            if (eventData.pointerId != touchId) return;
            startPos = eventData.position;
            CheckOverlap(eventData, ExecuteEvents.pointerDownHandler);
            ProcessInput(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId != touchId) return;
            touched = false;
            CheckOverlap(eventData, ExecuteEvents.pointerUpHandler);
            ProcessInput(false);
        }

        private void Start()
        {
            raycaster = FindObjectOfType<GraphicRaycaster>();
            icon = GetComponent<Image>();
            releasedIcon = icon.sprite;
        }

        public void ProcessInput(bool pressed)
        {
            if (pressed)
            {
                CheckDoubleTap();
                CancelInvoke(nameof(ResetTapAmount));
                Invoke(nameof(ResetTapAmount), maxTapInterval * Time.deltaTime);
            }
            OnButtonAction?.Invoke(pressed);
            if (visualCoroutine != null) StopCoroutine(visualCoroutine);
            if (gameObject.activeInHierarchy) visualCoroutine = StartCoroutine(PlayVisuals(pressed ? pressedIcon : releasedIcon, pressed ? pressedColor : releasedColor));
        }

        private void CheckDoubleTap()
        {
            touchAmount++;
            if (touchAmount >= 2)
            {
                ResetTapAmount();
                OnDoubleTap?.Invoke();
            }
        }

        private IEnumerator PlayVisuals(Sprite targetIcon, Color targetColor)
        {
            if (targetIcon != null && animateSprite) icon.sprite = targetIcon;
            if (animateColor)
            {
                while (icon.color != targetColor)
                {
                    icon.color = Color.Lerp(icon.color, targetColor, Time.deltaTime * 15f);
                    yield return null;
                }
                icon.color = targetColor;
            }
        }

        public void CheckOverlap<T>(PointerEventData eventData, ExecuteEvents.EventFunction<T> eventFunction) where T : IEventSystemHandler
        {
            List<RaycastResult> results = new();
            eventData.position = startPos;
			raycaster.Raycast(eventData, results);
			foreach (RaycastResult result in results)
			{
				if (result.gameObject == gameObject) continue;
                if (!result.gameObject.TryGetComponent(out ControlButton button)) return;
                if (Equals(eventFunction, ExecuteEvents.pointerDownHandler) && button.touched) break;
                else if (Equals(eventFunction, ExecuteEvents.pointerUpHandler) && !button.touched) break;
                ExecuteEvents.Execute(result.gameObject, eventData, eventFunction);
			}
        }
    }
}