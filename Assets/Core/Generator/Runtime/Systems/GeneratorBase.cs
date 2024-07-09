using Leopotam.EcsLite;
#if ENABLE_IL2CPP
    using Unity.IL2CPP.CompilerServices;
#endif

namespace Core.Generator.Systems
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public abstract class GeneratorBase<TObjectComponent> : IEcsRunSystem where TObjectComponent : struct
    {
        public abstract void Run(IEcsSystems systems);
        public abstract ref TObjectComponent CreateObjectComponent(in int entity);
    }
}