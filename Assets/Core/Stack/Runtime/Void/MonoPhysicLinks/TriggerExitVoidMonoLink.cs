﻿using Core.MonoConverter;
using Core.Stack.Void.Tags;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.Stack.Void.MonoPhysicLinks
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class TriggerExitVoidMonoLink : TriggerExitPhysicsMonoLink<TriggerExitVoidTag>
    {
    }
}