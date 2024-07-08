namespace Core.Base
{
    public interface IOutFactory<T>
    {
        bool Create(out T result);
    }
}