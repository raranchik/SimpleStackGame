using Core.Base;
using Leopotam.EcsLite;
using UnityEngine;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.MonoConverter.Factory
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class EntityOutFactory : IOutFactory<EcsPackedEntityWithWorld>
    {
        private readonly EcsWorld m_World;
        private readonly MonoLinker m_Prefab;

        public EntityOutFactory(EcsWorld world, MonoLinker prefab)
        {
            m_World = world;
            m_Prefab = prefab;
        }

        public bool Create(out EcsPackedEntityWithWorld result)
        {
            var instance = Object.Instantiate(m_Prefab);
            var entity = m_World.NewEntity();
            result = m_World.PackEntityWithWorld(entity);
            instance.LinkTo(result);
            return true;
        }
    }
}