using Leopotam.EcsLite;
using UnityEngine;

namespace MonoConverter
{
    public abstract class MonoLinkBase : MonoBehaviour
    {
        public abstract void LinkTo(in EcsPackedEntityWithWorld packedEntityWithWorld);
    }
}