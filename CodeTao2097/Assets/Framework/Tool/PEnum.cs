using System;

namespace QFramework
{
    // 位枚举格式 
    //[Flags]
    //public enum Enum
    //{
    //    A = 1 << 0,
    //    B = 1 << 1,
    //    C = 1 << 2,
    //}
    // (value & flags) != 0 判断当前枚举是否处于选中
    public static class PEnum
    {
        public static void Loop<T>(Action<T> callback) where T : Enum
        {
            if (callback == null) return;
            foreach (T item in Enum.GetValues(typeof(T)))
            {
                callback(item);
            }
        }
    }
}