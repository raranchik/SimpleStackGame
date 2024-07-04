using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MonoConverter;

namespace Core.Player
{
    public class PlayerInit : IEcsInitSystem
    {
        private readonly MonoLinkBase m_PlayerLinker;
        private readonly EcsWorldInject m_WorldInject = default;

        public PlayerInit(MonoLinkBase playerLinker)
        {
            m_PlayerLinker = playerLinker;
        }

        public void Init(IEcsSystems systems)
        {
            var world = m_WorldInject.Value;
            var entity = world.NewEntity();
            var packed = world.PackEntityWithWorld(entity);
            m_PlayerLinker.LinkTo(packed);
        }
    }
}