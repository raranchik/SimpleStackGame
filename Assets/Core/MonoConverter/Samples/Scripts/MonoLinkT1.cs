using Core.MonoConverter;
using Leopotam.EcsLite;

namespace MonoConverter.Samples
{
    public class MonoLinkT1 : MonoLink<T1>
    {
        public override void LinkTo(in EcsPackedEntityWithWorld packedEntityWithWorld)
        {
        }
    }
}