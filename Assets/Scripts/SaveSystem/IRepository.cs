public interface IRepository<T>
{
    T GetData();
    void SaveData(T data);
    void Reset();
}