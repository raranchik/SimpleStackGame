using Leopotam.EcsLite;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.InteractiveStack.Base.Systems
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class GenerateStackItemRequestHandlerRun : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
        }
    }
}