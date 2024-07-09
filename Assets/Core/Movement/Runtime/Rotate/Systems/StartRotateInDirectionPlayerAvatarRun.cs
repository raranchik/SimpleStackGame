using Core.DevicesInput.Requests;
using Core.MonoConverter.Links;
using Core.Movement.Rotate.Links;
using Core.Player.Tags;
using Core.Time;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.Movement.Rotate.Systems
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class StartRotateInDirectionPlayerAvatarRun : IEcsRunSystem
    {
        private readonly EcsWorldInject m_World;
        private readonly EcsFilterInject<Inc<IsStartRotateInDirection>> m_RotateInDirFilter;
        private readonly EcsCustomInject<TimeService> m_TimeService;

        private readonly EcsFilterInject<Inc<RigidbodyLink, RotateSpeedLink, PlayerAvatarTag>>
            m_RotateObjectFilter;

        public void Run(IEcsSystems systems)
        {
            if (m_RotateInDirFilter.Value.GetEntitiesCount() <= 0)
            {
                return;
            }

            if (m_RotateObjectFilter.Value.GetEntitiesCount() <= 0)
            {
                return;
            }

            foreach (var entity in m_RotateObjectFilter.Value)
            {
                RotateEntity(entity);
            }
        }

        private void RotateEntity(in int entity)
        {
            ref var rigidbody = ref m_RotateObjectFilter.Pools.Inc1.Get(entity);
            foreach (var requestEntity in m_RotateInDirFilter.Value)
            {
                ref var direction = ref m_RotateInDirFilter.Pools.Inc1.Get(requestEntity);
                var toRotation = Quaternion.LookRotation(direction.Value, Vector3.up);
                rigidbody.Value.MoveRotation(toRotation);
                m_World.Value.DelEntity(requestEntity);
            }
        }
    }
}