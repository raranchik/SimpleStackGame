namespace Core.Base.Runtime
{
    public interface IOutFactory<T>
    {
        bool Create(out T result);
    }
}