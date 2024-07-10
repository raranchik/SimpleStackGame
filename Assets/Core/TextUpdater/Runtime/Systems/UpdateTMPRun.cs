using Core.TextUpdater.Requests;
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
    public class UpdateTMPRun : IEcsRunSystem
    {
        private readonly EcsWorldInject m_World;
        private readonly EcsPoolInject<UpdateTMPRequest> m_Request;
        private readonly EcsFilterInject<Inc<UpdateTMPRequest>> m_Requests;

        public void Run(IEcsSystems systems)
        {
            if (m_Requests.Value.GetEntitiesCount() <= 0)
            {
                return;
            }

            foreach (var requestEntity in m_Requests.Value)
            {
                ref var request = ref m_Request.Value.Get(requestEntity);
                if (request.TMP.gameObject.activeInHierarchy)
                {
                    request.TMP.text = request.Value;
                }

                m_World.Value.DelEntity(requestEntity);
            }
        }
    }
}