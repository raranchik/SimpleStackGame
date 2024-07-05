using Core.MonoConverter;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Core.Follower
{
    public class FollowerInit : IEcsInitSystem
    {
        private readonly MonoLinkBase m_Linker;
        private readonly EcsWorldInject m_WorldInject = default;

        public FollowerInit(MonoLinkBase linker)
        {
            m_Linker = linker;
        }

        public void Init(IEcsSystems systems)
        {
            var world = m_WorldInject.Value;
            var entity = world.NewEntity();
            var packed = world.PackEntityWithWorld(entity);
            m_Linker.LinkTo(packed);
        }
    }
}