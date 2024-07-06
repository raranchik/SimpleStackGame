using Core.MonoConverter;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.Player
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class PlayerInit : IEcsInitSystem
    {
        private readonly MonoLinkBase m_PlayerLinker;
        private readonly EcsWorldInject m_World = default;

        public PlayerInit(MonoLinkBase playerLinker)
        {
            m_PlayerLinker = playerLinker;
        }

        public void Init(IEcsSystems systems)
        {
            var world = m_World.Value;
            var entity = world.NewEntity();
            var packed = world.PackEntityWithWorld(entity);
            m_PlayerLinker.LinkTo(packed);
        }
    }
}