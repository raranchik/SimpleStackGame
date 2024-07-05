using Core.DevicesInput;
using Core.MonoConverter.Links;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using TimeListener;
using UnityEngine;

namespace Core.Movement.Move
{
    public class MoveInDirectionRun : IEcsRunSystem
    {
        private readonly EcsWorldInject m_WorldInject;
        private readonly EcsPoolInject<MoveInDirectionRequest> m_MoveInDirRequestPoolInject;
        private readonly EcsFilterInject<Inc<MoveInDirectionRequest>> m_MoveInDirRequestFilterInject;
        private readonly EcsFilterInject<Inc<TransformLink, MoveLink>> m_MoveLinkFilterInject;
        private readonly EcsPoolInject<TransformLink> m_TransformLinkPoolInject;
        private readonly EcsPoolInject<MoveLink> m_MoveLinkPoolInject;
        private readonly EcsCustomInject<TimeService> m_TimeServiceInject;
        private readonly EcsPoolInject<MoveRequest> m_MoveRequestPoolInject;
        private readonly EcsFilterInject<Inc<MoveRequest>> m_MoveRequestFilterInject;

        public void Run(IEcsSystems systems)
        {
            var world = m_WorldInject.Value;
            var moveRequestFilter = m_MoveRequestFilterInject.Value;
            if (moveRequestFilter.GetEntitiesCount() > 0)
            {
                foreach (var moveRequestEntity in moveRequestFilter)
                {
                    world.DelEntity(moveRequestEntity);
                }
            }

            var moveInDirRequestFilter = m_MoveInDirRequestFilterInject.Value;
            if (moveInDirRequestFilter.GetEntitiesCount() <= 0)
            {
                return;
            }

            var moveLinkFilter = m_MoveLinkFilterInject.Value;
            if (moveLinkFilter.GetEntitiesCount() <= 0)
            {
                return;
            }

            var transformLinkPool = m_TransformLinkPoolInject.Value;
            var moveInDirRequestPool = m_MoveInDirRequestPoolInject.Value;
            var timeService = m_TimeServiceInject.Value;
            var moveLinkPool = m_MoveLinkPoolInject.Value;
            var moveRequestPool = m_MoveRequestPoolInject.Value;
            foreach (var moveLinkEntity in moveLinkFilter)
            {
                ref var transformLink = ref transformLinkPool.Get(moveLinkEntity);
                var transform = transformLink.Transform;
                ref var movementLink = ref moveLinkPool.Get(moveLinkEntity);
                foreach (var moveInDirRequestEntity in moveInDirRequestFilter)
                {
                    ref var moveInDirRequest = ref moveInDirRequestPool.Get(moveInDirRequestEntity);
                    ref var direction = ref moveInDirRequest.Direction;
                    var speed = direction * movementLink.m_Speed * timeService.DeltaTime;
                    transform.Translate(speed, Space.World);

                    world.DelEntity(moveInDirRequestEntity);
                }

                var moveRequestEntity = world.NewEntity();
                ref var moveRequest = ref moveRequestPool.Add(moveRequestEntity);
                moveRequest.PackedEntity = world.PackEntity(moveLinkEntity);
            }
        }
    }
}