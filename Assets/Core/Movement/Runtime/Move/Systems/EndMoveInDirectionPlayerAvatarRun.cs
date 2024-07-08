using Core.DevicesInput.Requests;
using Core.MonoConverter.Links;
using Core.Movement.Move.Request;
using Core.Player.Components;
using Core.Time;
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
    public class EndMoveInDirectionPlayerAvatarRun : IEcsRunSystem
    {
        private static readonly int IsMove = Animator.StringToHash("IsMove");

        private readonly EcsFilterInject<Inc<PlayerAvatarComponent, AnimatorLink>>
            m_MoveObjectRequestFilter;

        private readonly EcsWorldInject m_World;
        private readonly EcsFilterInject<Inc<IsEndMoveInDirectionRequest>> m_MoveInDirFilter;
        private readonly EcsCustomInject<TimeService> m_TimeService;
        private readonly EcsPoolInject<MoveRequest> m_MoveRequestPool;

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
            ref var animator = ref m_MoveObjectRequestFilter.Pools.Inc2.Get(entity);
            foreach (var requestEntity in m_MoveInDirFilter.Value)
            {
                animator.Value.SetBool(IsMove, false);
                m_World.Value.DelEntity(requestEntity);
            }
        }
    }
}