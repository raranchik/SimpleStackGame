using Core.InteractiveStack.SelfRequests;
using Core.Stack.Components;
using Core.Stack.Links;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.InteractiveStack.Systems
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class GenerateStackItemSelfRequestHandlerRun : IEcsRunSystem
    {
        private readonly EcsFilterInject<
            Inc<GenerateStackItemSelfRequest, StackComponent, StackMaxCountLink>
        > m_RequestFilter;

        public void Run(IEcsSystems systems)
        {
            if (m_RequestFilter.Value.GetEntitiesCount() <= 0)
            {
                return;
            }

            foreach (var requestEntity in m_RequestFilter.Value)
            {
                m_RequestFilter.Pools.Inc1.Del(requestEntity);
            }
        }
    }
}