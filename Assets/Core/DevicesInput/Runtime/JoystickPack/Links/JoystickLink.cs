using System;
using Joystick_Pack.Scripts.Base;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.DevicesInput.JoystickPack.Links
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    [Serializable]
    public struct JoystickLink
    {
        public Joystick Value;
    }
}