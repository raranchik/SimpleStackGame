using Leopotam.EcsLite;
using UnityEngine;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.MonoConverter
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public abstract class MonoLink<T> : MonoLinkBase where T : struct
    {
        [SerializeField] protected T m_Value;

        public override void LinkTo(in EcsPackedEntityWithWorld packedEntityWithWorld)
        {
            if (!packedEntityWithWorld.Unpack(out var world, out var entity))
            {
                return;
            }

            ref var component = ref world.GetPool<T>()
                .Add(entity);
            component = m_Value;
        }
    }
}