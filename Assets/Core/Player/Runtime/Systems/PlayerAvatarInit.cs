using Core.MonoConverter;
using Core.Player.Components;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
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
        private readonly MonoLinkBase m_PlayerAvatarLinker;
        private readonly EcsWorldInject m_World = default;

        public PlayerAvatarInit(MonoLinkBase playerAvatarLinker)
        {
            m_PlayerAvatarLinker = playerAvatarLinker;
        }

        public void Init(IEcsSystems systems)
        {
            var world = m_World.Value;
            var playerAvatarEntity = world.NewEntity();
            var playerAvatarPackedEntity = world.PackEntityWithWorld(playerAvatarEntity);
            m_PlayerAvatarLinker.LinkTo(playerAvatarPackedEntity);
            var playerAvatarPool = world.GetPool<PlayerAvatarComponent>();
            playerAvatarPool.Add(playerAvatarEntity);
        }
    }
}