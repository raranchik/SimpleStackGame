using System;
using Leopotam.EcsLite;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.InteractiveStack.Stack.Requests
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    [Serializable]
    public struct StackPushRequest
    {
        public EcsPackedEntity Object;
        public EcsPackedEntity Destination;
    }
}