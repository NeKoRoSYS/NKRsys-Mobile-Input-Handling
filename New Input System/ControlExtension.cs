using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

namespace NeKoRoSYS.InputHandling.Mobile
{
    public class ControlExtension : OnScreenControl
    {
        [Header("Inputs")]
        [InputControl(layout = "Button")]
        [SerializeField] private string buttonPath = "<Keyboard>/leftShift";
        [InputControl(layout = "Vector2")]
        [SerializeField] private string vectorPath = "<Joystick>/stick";
        protected override string controlPathInternal { get => useVector ? vectorPath : buttonPath; set { if (useVector) vectorPath = value; else buttonPath = value; } }

        [Header("Config")]
        public bool useVector;
        
        public void ProcessButton(float value) => SendValueToControl(value);
        public void ProcessVector(Vector2 value) => SendValueToControl(value);
    }
}