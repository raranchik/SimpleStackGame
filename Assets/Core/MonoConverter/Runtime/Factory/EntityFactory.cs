using System;
using Core.Base;
using Leopotam.EcsLite;
using Object = UnityEngine.Object;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.MonoConverter.Factory
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class EntityFactory : IFactory<EcsPackedEntityWithWorld>
    {
        private readonly EcsWorld m_World;
        private readonly MonoLinker m_Prefab;
        private readonly Action<int> m_OnCreate;

        public EntityFactory(EcsWorld world, MonoLinker prefab, Action<int> onCreate)
        {
            m_World = world;
            m_Prefab = prefab;
            m_OnCreate = onCreate;
        }

        public EcsPackedEntityWithWorld Create()
        {
            var instance = Object.Instantiate(m_Prefab);
            var entity = m_World.NewEntity();
            var packed = m_World.PackEntityWithWorld(entity);
            instance.LinkTo(packed);
            m_OnCreate?.Invoke(entity);
            return packed;
        }
    }
}