using Core.DevicesInput.Requests;
using Core.MonoConverter.Links;
using Core.Movement.Move.Link;
using Core.Movement.Move.Request;
using Core.TimeManagement.Time;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.Movement.Move.Systems
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class MoveInDirectionRun : IEcsRunSystem
    {
        private readonly EcsWorldInject m_World;
        private readonly EcsPoolInject<MoveInDirectionRequest> m_MoveInDirRequestPool;
        private readonly EcsFilterInject<Inc<MoveInDirectionRequest>> m_MoveInDirRequestFilter;
        private readonly EcsFilterInject<Inc<TransformLink, MoveMaxSpeedLink>> m_MoveLinkFilter;
        private readonly EcsPoolInject<TransformLink> m_TransformLinkPool;
        private readonly EcsPoolInject<MoveMaxSpeedLink> m_MoveLinkPool;
        private readonly EcsCustomInject<TimeListener> m_TimeService;
        private readonly EcsPoolInject<MoveRequest> m_MoveRequestPool;
        private readonly EcsFilterInject<Inc<MoveRequest>> m_MoveRequestFilter;

        public void Run(IEcsSystems systems)
        {
            var world = m_World.Value;
            var moveRequestFilter = m_MoveRequestFilter.Value;
            if (moveRequestFilter.GetEntitiesCount() > 0)
            {
                foreach (var moveRequestEntity in moveRequestFilter)
                {
                    world.DelEntity(moveRequestEntity);
                }
            }

            var moveInDirRequestFilter = m_MoveInDirRequestFilter.Value;
            if (moveInDirRequestFilter.GetEntitiesCount() <= 0)
            {
                return;
            }

            var moveLinkFilter = m_MoveLinkFilter.Value;
            if (moveLinkFilter.GetEntitiesCount() <= 0)
            {
                return;
            }

            var transformLinkPool = m_TransformLinkPool.Value;
            var moveInDirRequestPool = m_MoveInDirRequestPool.Value;
            var timeService = m_TimeService.Value;
            var moveLinkPool = m_MoveLinkPool.Value;
            var moveRequestPool = m_MoveRequestPool.Value;
            foreach (var moveLinkEntity in moveLinkFilter)
            {
                ref var transformLink = ref transformLinkPool.Get(moveLinkEntity);
                var transform = transformLink.Value;
                ref var movementLink = ref moveLinkPool.Get(moveLinkEntity);
                foreach (var moveInDirRequestEntity in moveInDirRequestFilter)
                {
                    ref var moveInDirRequest = ref moveInDirRequestPool.Get(moveInDirRequestEntity);
                    ref var direction = ref moveInDirRequest.Direction;
                    var speed = direction * movementLink.Value * timeService.DeltaTime;
                    transform.Translate(speed, Space.World);

                    world.DelEntity(moveInDirRequestEntity);
                }

                var moveRequestEntity = world.NewEntity();
                ref var moveRequest = ref moveRequestPool.Add(moveRequestEntity);
                moveRequest.Value = world.PackEntity(moveLinkEntity);
            }
        }
    }
}