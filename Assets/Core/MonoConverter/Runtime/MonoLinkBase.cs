using Leopotam.EcsLite;
using UnityEngine;

namespace Core.MonoConverter
{
    public abstract class MonoLinkBase : MonoBehaviour
    {
        public abstract void LinkTo(in EcsPackedEntityWithWorld packedEntityWithWorld);
    }
}