using System.Linq;
using Leopotam.EcsLite;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.MonoConverter
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class MonoLinker : MonoLinkBase
    {
        public override void LinkTo(in EcsPackedEntityWithWorld packedEntityWithWorld)
        {
            var links = GetComponentsInChildren<MonoLinkBase>(true);
            foreach (var link in links.Where(x => x is not MonoLinker))
            {
                link.LinkTo(packedEntityWithWorld);
            }

            foreach (var link in links.Where(x => x is not PhysicsMonoLinkBase))
            {
                Destroy(link);
            }

            Destroy(this);
        }
    }
}