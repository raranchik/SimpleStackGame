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
            var links = GetComponents<MonoLinkBase>().Where(x => x is not MonoLinker);
            foreach (var link in links)
            {
                link.LinkTo(packedEntityWithWorld);
                Destroy(link);
            }

            Destroy(this);
        }
    }
}