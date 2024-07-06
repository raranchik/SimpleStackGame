using Core.InteractiveStack.Stack.Requests;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.InteractiveStack.Stack
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class PushRequestHandlerRun : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<StackPushRequest>> m_RequestFilter;

        public void Run(IEcsSystems systems)
        {
        }
    }
}