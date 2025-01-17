﻿using Leopotam.EcsLite;
using UnityEngine;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.MonoConverter
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public abstract class MonoLinkBase : MonoBehaviour
    {
        public abstract void LinkTo(in EcsPackedEntityWithWorld packedEntityWithWorld);
    }
}