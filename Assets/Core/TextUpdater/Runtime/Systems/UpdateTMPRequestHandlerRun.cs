using Core.Counter.Runtime.Requests;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.TextUpdater.Systems
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class UpdateTMPRequestHandlerRun : IEcsRunSystem
    {
        private readonly EcsWorldInject m_World;
        private readonly EcsFilterInject<Inc<UpdateTMPRequest>> m_Filter;

        public void Run(IEcsSystems systems)
        {
            if (m_Filter.Value.GetEntitiesCount() <= 0)
            {
                return;
            }

            foreach (var entity in m_Filter.Value)
            {
                ref var request = ref m_Filter.Pools.Inc1.Get(entity);
                request.TMP.text = request.Value;
                m_World.Value.DelEntity(entity);
            }
        }
    }
}