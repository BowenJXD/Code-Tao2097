using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    // Õâ¸ö¿Î³Ì ÐèÒª´ó¼ÒÊìÏ¤Î¯ÍÐ lambda±í´ïÊ½ ÀïÊÏÌæ»» µÈÖªÊ¶
    public class Buff
    {
        public string name { get; }
        private int level = 1;
        public int Level => level;
        public void AddHierarchy() => level++;
        public void SubHierarchy()
        {
            if (level == 1) return;
            level--;
        }
        public float duration;
        public Buff(string name)
        {
            this.name = name;
        }
        public bool IsContinue()
        {
            return duration > 0;
        }
        public void Update()
        {
            duration -= Time.deltaTime;
        }
    }
    // buff ³ÖÓÐÕß ½Ó¿Ú
    public interface IBuffOwner : ICanSendEvent
    {
        float GetSourceValue(string buffName);
        void SetValue(string buffName, float result);
        float DataProcess(string buffName, float source);
        void SendAddBuffEvent(Buff buff, BuffConfig config);
        void SendRemoveBuffEvent(string buffName);
        void SendUpdateBuffEvent();
    }
    public enum E_BuffStackType
    {
        OnlyStackLevel,
        OnlyStackTime,
        StackTimeAndLevel,
        NotStack
    }
    // buffÏµÍ³½Ó¿Ú 
    public interface IBuffSystem : ISystem
    {
        // Ìí¼ÓBUFFÊ± ÐèÒªÖªµÀ ¸øË­Ìí¼Ó Ìí¼ÓÄÄ¸öBuff
        void AddBuff(string buffName, IBuffOwner owner);
        // ÒÆ³ýBUFFÊ± ÐèÒªÖªµÀ ¸øË­ÒÆ³ý ÒÆ³ýÄÄ¸öBUFF
        void RemoveBuff(string buffName, IBuffOwner owner);
        // ÅÐ¶ÏÔÚÄ³¸ö¶ÔÏóÖÐ ÊÇ·ñ°üº¬Ä³¸öbuff
        bool ContainsBuff(string buffName, IBuffOwner owner);
        // Ò»°ãÓÃÓÚ¼ÆËãµÄBuff ÎÒÍ¨³£½ÐËû±»¶¯BUFF
        // Ìá¹©Ò»¸öÓÃÓÚ¼ÆËãÄ³¸ö¶ÔÏóÉíÉÏËùÓÐÍ¬×é±ðBuffµÄº¯Êý 
        double CalcPassive(IBuffOwner owner, E_BuffGroup group, float source);
        // Ìá¹©Ò»¸öÓÃÓÚ¼ÆËãÄ³¸ö¶ÔÏóÉíÉÏÄ³¸öBuffµÄ¼ÆËãº¯Êý
        float CalcPassive(IBuffOwner owner, string buffName, float source);
        // Ìá¹©¸øÍâ²¿»ñÈ¡BUFFÅäÖÃÐÅÏ¢µÄ·½·¨
        bool TryGetConfig(string buffName, out BuffConfig config);
    }
    public class BuffSystem : AbstractSystem, IBuffSystem
    {
        private class UpdateInfo
        {
            // ÐèÒª±£´æÒ»¸ö³ÖÓÐÕß
            private IBuffOwner Owner;
            // ¶ÔÍâÌá¹©Ò»¸ö°üº¬·½·¨ ÓÃÓÚÍâ²¿½øÐÐÅÐ¶Ï
            public bool Contains(IBuffOwner owner) => Owner == owner;
            // ÓÃÓÚ´æ´¢¶ÔÓ¦µÄbuffµÄ¼ÆÊ±Æ÷
            private Dictionary<Buff, DelayTask> mTasks = new Dictionary<Buff, DelayTask>();
            // ÒòÎªÄÚ²¿¿ÉÄÜÐèÒª¶ÔÍâ²éÑ¯ ËùÒÔÕâÀïÓÃÁËÒ»¸öFuncÎ¯ÍÐÀ´½«Âß¼­½øÐÐÕûºÏÉè¼Æ
            // Func<string, BuffConfig> mCallBack;
            public UpdateInfo(IBuffOwner owner)
            {
                Owner = owner;
            }
            public void Add(Buff buff, BuffConfig config)
            {
                if (mTasks.ContainsKey(buff)) return;
                // ÐèÒª½øÐÐ½×¶ÎÐÔ¸üÐÂ ¿ÉÒÔÊ¹ÓÃ¼ÆÊ±Æ÷È¥¼ÆÊ±
                var task = new DelayTask();
                task.Start(config.IntervalTime, true, () => Owner.ExcuteBuff(config, buff));
                mTasks.Add(buff, task);
            }
            public void Remove(Buff buff)
            {
                mTasks.Remove(buff);
            }
            public void Update()
            {
                foreach (var task in mTasks.Values)
                {
                    // ¸üÐÂ¼ÆÊ±Æ÷
                    task.Update();
                }
            }
        }

        private Dictionary<IBuffOwner, List<Buff>> mBuffs;
        private Dictionary<string, BuffConfig> mConfigs;
        private List<UpdateInfo> mUpdateInfos;
        protected override void OnInit()
        {
            mBuffs = new Dictionary<IBuffOwner, List<Buff>>();
            mConfigs = Resources.Load<SO_BuffConfig>("SO_Data/SO_BuffConfig").ToDic();
            mUpdateInfos = new List<UpdateInfo>();

            PublicMono.Instance.OnUpdate += OnUpdate;
        }
        // ÓÃÓÚ¸üÐÂÒ»Ð©·Ç¼ÆÊ±µÄ·Ç±»¶¯buff
        private void OnUpdate()
        {
            for (int i = 0; i < mUpdateInfos.Count; i++)
            {
                mUpdateInfos[i].Update();
            }
        }

        public bool TryGetConfig(string buffName, out BuffConfig config)
        {
            if (mConfigs.TryGetValue(buffName, out config)) return true;
            throw new Exception($"ÔÚ»ñÈ¡buffÅäÖÃÊ±,²éÑ¯´íÎó,BuffÃû×Ö£º{buffName}");
        }
        // ´´½¨BUFFº¯Êý
        private void CreateBuff(string buffName, IBuffOwner owner, List<Buff> buffs)
        {
            // Ê×ÏÈ»ñÈ¡BUFFÅäÖÃ Èç¹û²éÑ¯³É¹¦ ËµÃ÷³ÌÐòÅÜÍ¨ÁË
            if (TryGetConfig(buffName, out var config))
            {
                // ´´½¨ÁËÒ»¸öbuffÊý¾Ý
                var buff = new Buff(buffName);
                // ÐèÒª½øÐÐ·ÖÖ§ ÊÇ·ñÊÇ¼ÆÊ±BUFF ÐèÒªÔÚbuffµÄÅäÖÃÖÐ»ñÈ¡Ò»¸öboolÖµ
                if (config.IsTimeBuff)
                {
                    //  ÐèÒª½«ÅäÖÃÀïÃæµÄ³ÖÐøÊ±¼ä½øÐÐ»º´æ ÎªÊ²Ã´ÄØ ÒòÎªÎÒÃÇºóÐøÐèÒªÊµÏÖË¢ÐÂbuffÊ±¼äµÄ¹¦ÄÜ 
                    buff.duration = config.Duration;
                    // ¼ÆÊ±»ù±¾¶¼ÊÇÒ»´ÎµÄ µ½Ê±¼ä¾ÍÏú»Ù ËùÒÔÕâÀïÊ¹ÓÃÐ­³ÌÊÇ·Ç³£ºÏÊÊµÄ
                    PublicMono.Instance.StartCoroutine(StartTime(buff, owner, config));
                }
                // ÓÐÒ»ÖÖ·Ç¼ÆÊ± µ«ÊÇ»á¼ä¶ÏÐÔÖ´ÐÐµÄbuff ÕâÖÖÎÒ½ÐËûÓÀ¾ÃÐÔÖ÷¶¯buff ÔõÃ´ÅÐ¶Ï ²»ÊÇ¼ÆÊ±ÓÖ²»ÊÇ±»¶¯ 
                else if (!config.IsPassive)
                {

                    // ²éÕÒ¸üÐÂÁÐ±íÖÖ ÊÇ·ñÓÐ¸Ã³ÖÓÐÕß¶ÔÏóÔÚ¸üÐÂ
                    var upInfo = mUpdateInfos.Find(info => info.Contains(owner));
                    // Èç¹ûÃ»ÓÐ ¾ÍÖ±½ÓÌí¼Ó
                    if (upInfo == null)
                    {
                        upInfo = new UpdateInfo(owner);
                        mUpdateInfos.Add(upInfo);
                    }
                    upInfo.Add(buff, config);
                }
                // ÕâÀïÐèÒª¸øÒ»¸öÍâ²¿½Ó¿Ú·½·¨ ÓÃÓÚ·¢ËÍµ±Ç°¶ÔÏó¶ÔÓ¦µÄÊÂ¼þ·½·¨
                // ÕâÀïÎªÊ²Ã´²»Ö±½Ó·¢ËÍÊÂ¼þÄØ ÒòÎªÊÂ¼þÍ¨³£ÊÇÈ«¾ÖµÄ ¸üÆ«ÏòÒ»¶Ô¶à
                // Èç¹ûÏëÒª½øÐÐ¶à½ÇÉ«·¢ËÍ²»Í¬µÄÊÂ¼þ ¾Í¿ÉÒÔÈÃbuff³ÖÓÐÕß×Ô¼ºÊµÏÖ¶ÔÓ¦µÄÊÂ¼þ
                owner.SendAddBuffEvent(buff, config);
                // ½«ÐÂ´´½¨µÄBUFFÌí¼Óµ½buffÁÐ±íÖÐ
                buffs.Add(buff);
            }
        }
        private IEnumerator StartTime(Buff buff, IBuffOwner owner, BuffConfig config)
        {
            // Èç¹ûÊÇÓÐÊ±¼äÏÞÖÆµÄ±»¶¯buffÊ±
            if (config.IsPassive)
            {
                while (buff.IsContinue())
                {
                    buff.Update();
                    yield return null;
                }
            }
            // Èç¹û²»ÊÇ
            else
            {
                // »º´æµ±Ç°¼ä¸ôÊ±¼ä ÕâÀïÎªÊ²Ã´²»ÊÇ0ÄØ ÒòÎªÎÒÃÇµÚÒ»´Î½øÈë¼ÆÊ±µÄÊ±ºò ÎÒ¾ÍÏ£Íû¼ÆÊ±Æ÷´¥·¢Ò»´Î
                float timer = config.IntervalTime;

                while (buff.IsContinue())
                {
                    buff.Update();
                    // ´¦Àíµ±Ç°buffµÄÖ÷¶¯Ð§¹û ÒòÎª²»ÊÇ±»¶¯ ÒâÎ¶×Å»áÓÐÖ´ÐÐ¼ä¸ô
                    timer += Time.deltaTime;
                    if (timer >= config.IntervalTime)
                    {
                        // Ö´ÐÐbuffº¯Êý
                        owner.ExcuteBuff(config, buff.Level);
                        timer = 0;
                    }
                    yield return null;
                }
            }
            // Ö´ÐÐ½áÊø ÒÆ³ýbuff
            RemoveBuff(buff.name, owner);
        }
        void IBuffSystem.AddBuff(string buffName, IBuffOwner owner)
        {
            // ²éÑ¯µ±Ç°³ÖÓÐÕßÊÇ·ñÔÚBuffÏµÍ³ÖÐ¸üÐÂ Èç¹ûÃ»ÔÚ
            if (!mBuffs.TryGetValue(owner, out var list))
            {
                // ½«µ±Ç°³ÖÓÐÕßÌí¼Óµ½ÏµÍ³ÖÐ
                list = new List<Buff>();
                mBuffs.Add(owner, list);
            }
            // ÅÐ¶ÏÁÐ±íÖÐÊÇ·ñÓÐbuff Èç¹ûÃ»ÓÐ¾ÍÌø¹ý²éÑ¯Ö±½Ó´´½¨
            if (list.Count == 0)
            {
                CreateBuff(buffName, owner, list);
                return;
            }
            // µ±ÁÐ±íÖÐÓÐÒ»Ð©buffÊ± ²éÑ¯ÊÇ·ñ´æÔÚµ±Ç°Ãû×ÖµÄbuff
            // Èç¹û´æÔÚ ¾Í³¢ÊÔµþ¼Ó ²»´æÔÚ¾ÍÖ±½Ó´´½¨
            var buff = list.Find(buff => buff.name == buffName);
            // Èç¹ûbuffÎª¿Õ ±íÊ¾Ã»ÓÐÕÒµ½ Ö±½Ó´´½¨
            if (buff == null)
            {
                CreateBuff(buffName, owner, list);
                return;
            }
            // Èç¹ûÕÒµ½ÁË¶ÔÓ¦µÄBUFF ÐèÒªÏÈÕÒµ½¶ÔÓ¦µÄÅäÖÃ±í
            if (TryGetConfig(buffName, out var config))
            {
                switch (config.stackType)
                {
                    case E_BuffStackType.OnlyStackLevel:
                        // Èç¹ûµþ¼ÓÀàÐÍÊÇ½öµþ¼ÓµÈ¼¶Ê¯ ÌáÉýbuffµÈ¼¶
                        buff.AddHierarchy();
                        buff.duration = config.Duration;
                        break;
                    case E_BuffStackType.OnlyStackTime:
                        buff.duration += config.Duration;
                        break;
                    case E_BuffStackType.StackTimeAndLevel:
                        buff.AddHierarchy();
                        buff.duration += config.Duration;
                        break;
                    case E_BuffStackType.NotStack:
                        // Ë¢ÐÂÊ±¼ä
                        buff.duration = config.Duration;
                        break;
                }
                owner.SendUpdateBuffEvent();
            }
        }
        public void RemoveBuff(string buffName, IBuffOwner owner)
        {
            // ÏÈÅÐ¶Ïµ±Ç°¶ÔÏóÊÇ·ñÓÐÄ³¸öbuff
            if (mBuffs.TryGetValue(owner, out var list))
            {
                int index = list.FindIndex(buff => buff.name == buffName);
                if (index >= 0) // ËµÃ÷ÁÐ±íÖÐ´æÔÚ¸Ã¶ÔÏó
                {
                    owner.SendRemoveBuffEvent(buffName);
                    // var buff = list[index];
                    // ÒÆ³ý¸ÃÎ»ÖÃ¶ÔÏó
                    list.RemoveAt(index);
                }
            }
        }
        /// <summary>
        /// ¼ÆËãÒ»×é±»¶¯
        /// </summary>
        /// <param name="owner">Buff³ÖÓÐÕß</param>
        /// <param name="group">BuffµÄ·Ö×é</param>
        /// <param name="source">BuffµÄÔªÊý¾Ý</param>
        /// <returns>¼ÆËãºóµÄ½á¹û</returns>
        double IBuffSystem.CalcPassive(IBuffOwner owner, E_BuffGroup group, float source)
        {
            // ²éÑ¯µ±Ç°buff³ÖÓÐÕßÉíÉÏµÄbuffÁÐ±í
            if (mBuffs.TryGetValue(owner, out var list))
            {
                // Èç¹ûÃ»ÓÐbuff·µ»ØÔªÊý¾Ý
                if (list.Count == 0) return source;
                // Ò»¸öÊÇ¼Ó¼õÀàÐÍµÄbuff  Ò»¸öÊÇ³Ë³ýÀàÐÍµÄbuff Õý³£À´Ëµ
                // ÎÒÃÇµÄbuff×îºÃ×ñÑ­ÏÈ³Ë³ýºó¼Ó¼õµÄÔ­Ôò ÕâÑùµÃµ½µÄÊý¾ÝÏà¶ÔÎÈ¶¨
                var stack = new Stack<(float, E_Operator)>();
                // ¶ÔbuffÁÐ±í½øÐÐ±éÀú
                for (int i = 0; i < list.Count; i++)
                {
                    // ÕÒµ½µ±Ç°buffµÄÅäÖÃÐÅÏ¢
                    if (TryGetConfig(list[i].name, out var config))
                    {
                        // ÕÒµ½¶ÔÓ¦µÄ±»¶¯buff ²¢ÇÒ ·Ö×éÏàÍ¬µÄ»° ¾Í¿ÉÒÔÈÃ¸Ãbuff½øÐÐ±»¶¯¼ÆËã
                        if (config.IsPassive && config.Group == group)
                        {
                            // »ñÈ¡µ±Ç°buffÐèÒª²ÎÓë¼ÆËãµÄÔöÁ¿ ÔªÊý¾Ý ÐèÒª³ËÉÏµ±Ç°buffµÄµÈ¼¶
                            float delta = owner.DataProcess(config.Name, config.Value * list[i].Level);
                            // ¸ù¾Ý²»Í¬µÄÔËËã·û ½øÐÐ²»Í¬µÄ¼ÆËã²Ù×÷
                            switch (config.Operator)
                            {
                                case E_Operator.Add:
                                case E_Operator.Sub:
                                    stack.Push((delta, config.Operator));
                                    break;
                                // ³Ë·¨ºÍ³ý·¨´ó²¿·Ö¶¼ÊÇ°Ù·Ö±È ËùÒÔ ÎÒÃÇÐèÒª¼ÓÈëÒ»¸öÌØÊâµÄ¼ÆËã·½Ê½
                                // ÀýÈçÔö¼Ó°Ù·ÖÖ® 20 ¾ÍÊÇ ³ËÒÔ 1 + 0.2f
                                // ÀýÈç¼õÉÙ°Ù·ÖÖ® 20 ¾ÍÊÇ ³ËÒÔ 1 - 0.2f
                                case E_Operator.Mul:
                                    source *= 1 + delta;
                                    break;
                                case E_Operator.Div:
                                    source *= 1 - delta;
                                    break;
                            }
                        }
                    }
                }
                // ¿´¿´ÓÐÃ»ÓÐ¼Ó·¨µÄbuff
                while (stack.Count > 0)
                {
                    var info = stack.Pop();
                    switch (info.Item2)
                    {
                        case E_Operator.Add:
                            source += info.Item1;
                            break;
                        case E_Operator.Sub:
                            source -= info.Item1;
                            break;
                    }
                }
            }
            return source;
        }
        // ¼ÆËãµ¥¸ö±»¶¯
        float IBuffSystem.CalcPassive(IBuffOwner owner, string buffName, float source)
        {
            // ²éÑ¯buff ÁÐ±í
            if (mBuffs.TryGetValue(owner, out var list))
            {
                if (list.Count == 0) return source;
                // ²éÑ¯ÁÐ±íÖÐÊÇ·ñ´æÔÚbuff
                int index = list.FindIndex(info => info.name == buffName);
                // ÐèÒªÕÒµ½¶ÔÓ¦buff ²¢ÇÒ ÄÜ¹»ÕÒµ½ÅäÖÃ ²¢ÇÒ buffÊÇ±»¶¯buff
                if (index >= 0 && TryGetConfig(buffName, out var config) && config.IsPassive)
                {
                    owner.CalcPassive(config, list[index].Level, ref source);
                }
            }
            return source;
        }

        public bool ContainsBuff(string buffName, IBuffOwner owner)
        {
            return mBuffs.TryGetValue(owner, out var list) ? list.Find(buff => buff.name == buffName) != null : false;
        }
    }
}