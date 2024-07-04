using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MonoConverter;

namespace Core.JoystickInput
{
    public class JoystickInit : IEcsInitSystem
    {
        private readonly MonoLinkBase m_JoystickLinker;
        private readonly EcsWorldInject m_WorldInject = default;

        public JoystickInit(MonoLinkBase joystickLinker)
        {
            m_JoystickLinker = joystickLinker;
        }

        public void Init(IEcsSystems systems)
        {
            var world = m_WorldInject.Value;
            var entity = world.NewEntity();
            var packed = world.PackEntityWithWorld(entity);
            m_JoystickLinker.LinkTo(packed);
        }
    }
}