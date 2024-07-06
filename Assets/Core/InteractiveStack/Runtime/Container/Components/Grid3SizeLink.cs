﻿using System;
using UnityEngine;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.InteractiveStack.Container.Components
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    [Serializable]
    public struct Grid3SizeLink
    {
        public Vector3 Value;
    }
}