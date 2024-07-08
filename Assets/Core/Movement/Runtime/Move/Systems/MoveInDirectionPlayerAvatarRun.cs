using Core.DevicesInput.Requests;
using Core.MonoConverter.Links;
using Core.Movement.Move.Link;
using Core.Movement.Move.Request;
using Core.Player.Components;
using Core.Time;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.Movement.Move.Systems
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class MoveInDirectionPlayerAvatarRun : IEcsRunSystem
    {
        private readonly EcsWorldInject m_World;
        private readonly EcsFilterInject<Inc<MoveInDirectionRequest>> m_MoveInDirFilter;
        private readonly EcsCustomInject<TimeService> m_TimeService;
        private readonly EcsFilterInject<Inc<MoveRequest>> m_MoveRequestFilter;

        private readonly EcsFilterInject<Inc<RigidbodyLink, MoveSpeedLink, PlayerAvatarComponent>>
            m_MoveObjectRequestFilter;

        public void Run(IEcsSystems systems)
        {
            if (m_MoveInDirFilter.Value.GetEntitiesCount() <= 0)
            {
                return;
            }

            if (m_MoveObjectRequestFilter.Value.GetEntitiesCount() <= 0)
            {
                return;
            }

            foreach (var entity in m_MoveObjectRequestFilter.Value)
            {
                MoveEntity(entity);
            }
        }

        private void MoveEntity(in int entity)
        {
            ref var rigidbody = ref m_MoveObjectRequestFilter.Pools.Inc1.Get(entity);
            ref var moveSpeed = ref m_MoveObjectRequestFilter.Pools.Inc2.Get(entity);
            foreach (var requestEntity in m_MoveInDirFilter.Value)
            {
                ref var direction = ref m_MoveInDirFilter.Pools.Inc1.Get(requestEntity);
                var position = rigidbody.Value.position +
                               direction.Value * moveSpeed.Value * m_TimeService.Value.DeltaTime;
                rigidbody.Value.MovePosition(position);
                CreateMoveRequest(entity);
                m_World.Value.DelEntity(requestEntity);
            }
        }

        private void CreateMoveRequest(in int target)
        {
            var moveRequestEntity = m_World.Value.NewEntity();
            ref var moveRequest = ref m_MoveRequestFilter.Pools.Inc1.Add(moveRequestEntity);
            moveRequest.Value = m_World.Value.PackEntity(target);
        }
    }
}