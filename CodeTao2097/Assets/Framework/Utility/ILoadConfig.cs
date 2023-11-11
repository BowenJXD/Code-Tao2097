namespace QFramework
{
    public interface IJsonConfig : IUtility
    {
        bool Load<T>(string fileName, out T data) where T : class;
        bool Load<T>(out T data) where T : class;
    }
    public interface ICsvConfig : IUtility
    {
        bool Load<T>(string fileName, out T data) where T : class;
    }
    public class LoadConfig : IJsonConfig, ICsvConfig
    {
        public bool Load<T>(string fileName, out T data) where T : class
        {
            data = SerializeHelper.LoadJson<T>(CommonData.ConfigPath + fileName + ".json");
            return data != null;
        }
        bool IJsonConfig.Load<T>(out T data) => Load<T>(typeof(T).Name, out data);
        bool ICsvConfig.Load<T>(string fileName, out T data)
        {
            data = default;
            return true;
        }
    }
}