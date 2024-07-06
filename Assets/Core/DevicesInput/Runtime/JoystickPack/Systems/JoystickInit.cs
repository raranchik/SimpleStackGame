using Core.MonoConverter;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.DevicesInput.JoystickPack.Systems
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class JoystickInit : IEcsInitSystem
    {
        private readonly MonoLinkBase m_JoystickLinker;
        private readonly EcsWorldInject m_World = default;

        public JoystickInit(MonoLinkBase joystickLinker)
        {
            m_JoystickLinker = joystickLinker;
        }

        public void Init(IEcsSystems systems)
        {
            var world = m_World.Value;
            var entity = world.NewEntity();
            var packed = world.PackEntityWithWorld(entity);
            m_JoystickLinker.LinkTo(packed);
        }
    }
}