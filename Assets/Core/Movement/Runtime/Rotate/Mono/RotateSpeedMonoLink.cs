using Core.MonoConverter;
using Core.Movement.Rotate.Links;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.Movement.Rotate.Mono
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public class RotateSpeedMonoLink : MonoLink<RotateSpeedLink>
    {
    }
}