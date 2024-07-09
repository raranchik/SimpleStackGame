using System.Collections.Generic;
using Core.Container.Components;
using Core.Container.Links;
using Core.EcsMapper;
using Core.MonoConverter;
using Core.MonoConverter.Links;
using Core.Player.Tags;
using Core.Stack.Components;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.Player.Systems
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class PlayerAvatarInit : IEcsInitSystem
    {
        private readonly EcsCustomInject<LeoEcsLiteEntityMapper> m_EntityMapper;
        private readonly MonoLinkBase m_AvatarLinker;
        private readonly EcsWorldInject m_World;
        private readonly EcsPoolInject<GridEmptyPositionsComponent> m_EmptyPositionsPool;
        private readonly EcsPoolInject<Grid3SizeLink> m_Grid3SizePool;
        private readonly EcsPoolInject<StackComponent> m_StackPool;
        private readonly EcsPoolInject<GameObjectLink> m_GameObjectPool;

        public PlayerAvatarInit(MonoLinkBase avatarLinker)
        {
            m_AvatarLinker = avatarLinker;
        }

        public void Init(IEcsSystems systems)
        {
            var entity = m_World.Value.NewEntity();
            m_AvatarLinker.LinkTo(m_World.Value.PackEntityWithWorld(entity));
            InitializePlayerTag(entity);
            InitializeStack(entity);
            InitializeContainer(entity);
            InitializeMapper(entity);
        }

        private void InitializePlayerTag(in int entity)
        {
            var playerTag = m_World.Value.GetPool<PlayerAvatarTag>();
            playerTag.Add(entity);
        }

        private void InitializeContainer(in int entity)
        {
            ref var emptyPositions = ref m_EmptyPositionsPool.Value.Add(entity);
            emptyPositions.Value = new Queue<Vector3Int>();
            ref var grid3Size = ref m_Grid3SizePool.Value.Get(entity);
            for (var x = 0; x < grid3Size.Value.x; x++)
            {
                for (var y = 0; y < grid3Size.Value.y; y++)
                {
                    for (var z = 0; z < grid3Size.Value.z; z++)
                    {
                        emptyPositions.Value.Enqueue(new Vector3Int(x, y, z));
                    }
                }
            }
        }

        private void InitializeStack(in int entity)
        {
            ref var stack = ref m_StackPool.Value.Add(entity);
            stack.Value = new Stack<EcsPackedEntity>();
        }

        private void InitializeMapper(in int entity)
        {
            ref var gameObject = ref m_GameObjectPool.Value.Get(entity);
            m_EntityMapper.Value.Register(gameObject.Value, m_World.Value.PackEntity(entity));
        }
    }
}