using Core.InteractiveStack.Container.Components;
using Core.MonoConverter;

#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.InteractiveStack.Container.Mono
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class GridCellScaleMonoLink : MonoLink<GridCellScaleLink>
    {
    }
}