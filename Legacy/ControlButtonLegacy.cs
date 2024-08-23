using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using DG.Tweening;

namespace NeKoRoSYS.InputHandling.Mobile.Legacy
{
    public class ControlButtonLegacy : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Visuals")]
        [SerializeField] private Sprite pressedIcon;
        [SerializeField] private Color releasedColor = Color.white, pressedColor = Color.grey;
        [SerializeField] private bool animateSprite = true, animateColor = true;
        [SerializeField] private Image icon;
        private Sprite releasedIcon;
        [Space]
        [Header("Inputs")]
        private Vector2 startPos;
        public bool touched = false;
        public int touchId, touchAmount;
        private readonly float maxTapInterval = 15f;
        [Space]
        [Header("Events")]
        public Action<bool> OnButtonAction;
        public Action OnDoubleTap;
        private GraphicRaycaster raycaster;

        internal void ResetTapAmount() => touchAmount = 0;
        private void OnDisable() => ForceStopTouch();
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
            DOTween.Kill(this);
            if (gameObject.activeInHierarchy) PlayVisuals(pressed ? pressedIcon : releasedIcon, pressed ? pressedColor : releasedColor);
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

        private void PlayVisuals(Sprite targetIcon, Color targetColor)
        {
            if (targetIcon != null && animateSprite) icon.sprite = targetIcon;
            if (animateColor) DOTween.To(() => icon.color, x => icon.color = x, targetColor, 1.5f).SetId("ButtonVisuals");
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

        public void ForceStopTouch()
        {
            touched = false;
            touchId = 0;
            icon.sprite = releasedIcon;
            icon.color = releasedColor;
            DOTween.Kill("ButtonVisuals");
        }
    }
}