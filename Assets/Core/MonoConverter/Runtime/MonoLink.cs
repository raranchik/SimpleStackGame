using Leopotam.EcsLite;
using UnityEngine;

namespace Core.MonoConverter
{
    public abstract class MonoLink<T> : MonoLinkBase where T : struct
    {
        [SerializeField] protected T m_EcsComponent;

        public override void LinkTo(in EcsPackedEntityWithWorld packedEntityWithWorld)
        {
            if (!packedEntityWithWorld.Unpack(out var world, out var entity))
            {
                return;
            }

            ref var component = ref world.GetPool<T>()
                .Add(entity);
            component = m_EcsComponent;
        }
    }
}