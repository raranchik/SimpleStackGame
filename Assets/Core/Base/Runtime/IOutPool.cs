namespace Core.Base.Runtime
{
    public interface IOutPool<T>
    {
        bool Pop(out T result);
        void Push(in T value);
    }
}