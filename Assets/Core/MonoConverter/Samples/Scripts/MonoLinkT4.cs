using Core.MonoConverter;
using Leopotam.EcsLite;

namespace MonoConverter.Samples
{
    public class MonoLinkT4 : MonoLink<T4>
    {
        public override void LinkTo(in EcsPackedEntityWithWorld packedEntityWithWorld)
        {
        }
    }
}