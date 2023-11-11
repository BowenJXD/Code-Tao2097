namespace QFramework
{
    public interface IStorage : IUtility
    {
        void Save<T>(string file, T data) where T : class;
        bool Load<T>(string file, out T data) where T : class;
    }
    // 方案不支持字典
    public class JsonStorage : IStorage
    {
        bool IStorage.Load<T>(string file, out T data)
        {
            data = SerializeHelper.LoadJson<T>(CommonData.StoragePath + file + ".json");
            return data != null;
        }
        void IStorage.Save<T>(string file, T data)
        {
            SerializeHelper.SaveJson(CommonData.StoragePath + file + ".json", data);
        }
    }
    public class BinaryStorage : IStorage
    {
        /// <summary>
        /// 读取二进制存档文件
        /// </summary>
        bool IStorage.Load<T>(string file, out T data)
        {
            data = SerializeHelper.LoadBinary<T>(CommonData.StoragePath + file + ".sav");
            return data != null;
        }
        /// <summary>
        /// 保存为二进制  
        /// </summary>
        /// <param name="path">需要创建存档的路径</param>
        void IStorage.Save<T>(string file, T data)
        {
            SerializeHelper.SaveBinary(CommonData.StoragePath + file + ".sav", data);
        }
    }
}