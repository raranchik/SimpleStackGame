using Core.MonoConverter.Requests;
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
    public abstract class TriggerStayPhysicsMonoLink<TMarker> : PhysicsMonoLink<TMarker, TriggerStayRequest>
        where TMarker : struct
    {
        private void OnTriggerStay(Collider other)
        {
            if (!m_Value.Unpack(m_World, out var source))
            {
                return;
            }

            var entity = m_World.NewEntity();
            m_MarkerPool.Add(entity);
            ref var trigger = ref m_PhysicsRequestPool.Add(entity);
            trigger.Sender = m_Value;
            trigger.Value = other;
        }
    }
}