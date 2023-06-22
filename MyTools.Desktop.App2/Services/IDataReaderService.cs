namespace MyTools.Desktop.App2.Services;

public interface IFileReaderService<T>
{
    T Get();

    void Set(T obj);
}