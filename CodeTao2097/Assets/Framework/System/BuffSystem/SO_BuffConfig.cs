using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    //  表示buff所在的逻辑
    public enum E_BuffGroup
    {
        Health, Attack
    }
    public enum E_Operator
    {
        Add, Sub, Mul, Div
    }
    // buff配置信息
    [Serializable]
    public class BuffConfig
    {
        // BUFF的名字
        [field: SerializeField] public string Name { get; private set; }
        // BUFF所在的群组
        [field: SerializeField] public E_BuffGroup Group { get; private set; }
        // BUFF的运算类型 例如是乘法运算 还是加法运算
        [field: SerializeField] public E_Operator Operator { get; private set; }
        // 叠加类型
        [field: SerializeField] public E_BuffStackType stackType { get; private set; }
        // BUFF的基础值
        [field: SerializeField] public float Value { get; private set; }
        // 持续时间
        [field: SerializeField] public float Duration { get; private set; }
        // 间隔时间
        [field: SerializeField] public float IntervalTime { get; private set; }
        // Buff的图标
        [PnShowSprite] public Sprite Sprite;
        // 当持续时间大于 0 其实就可以说明是计时BUFF 如果等于 0 就是永久BUFF
        public bool IsTimeBuff => Duration > 0;
        // 间隔时间为0就是被动buff
        public bool IsPassive => IntervalTime == 0;
    }
    [CreateAssetMenu(fileName = "New SO_BuffConfig", menuName = "Data/SO/BuffConfig")]
    public class SO_BuffConfig : ScriptableObject
    {
        [SerializeField] private BuffConfig[] mConfigs;
        // 对外提供一个将配置列表转换成字典的函数
        public Dictionary<string, BuffConfig> ToDic()
        {
            var dic = new Dictionary<string, BuffConfig>();
            foreach (var config in mConfigs)
            {
                if (dic.ContainsKey(config.Name))
                {
                    throw new Exception($"配置表中存在重复信息{config.Name}");
                }
                dic.Add(config.Name, config);
            }
            return dic;
        }
    }
}