namespace Core.Base
{
    public interface IPool<T>
    {
        T Pop();
        void Push(in T value);
    }
}