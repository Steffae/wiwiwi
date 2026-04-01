public interface IRepository<T>
{
    T GetData();
    void Restore(T data);
}