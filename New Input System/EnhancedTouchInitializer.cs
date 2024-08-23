using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

namespace NeKoRoSYS.InputHandling
{
    public static class EnhancedTouchInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize() => EnhancedTouchSupport.Enable();
    }
}