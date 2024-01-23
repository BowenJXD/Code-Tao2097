using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 死亡传染Pandemic（木）
    /// 敌人死亡后，每层木系debuff都有30%的概率传染给附近1名单位，若为玩家则概率固定10%。
    /// </summary>
    public class PandemicBlessing : Blessing
    {
        /// <summary>
        /// 0 - 100
        /// </summary>
        public BindableStat chance = new BindableStat(30f);
        public BindableStat range = new BindableStat(1f);
        public ElementType elementType = ElementType.Wood;

        public override void OnAdd()
        {
            base.OnAdd();
            EnemyManager.Instance.AddOnSpawnAction(OnSpawn);
        }

        void OnSpawn(Enemy enemy)
        {
            enemy.onDeinit += () => TransferBuffToNearbyEnemies(enemy);
        }

        void TransferBuffToNearbyEnemies(Enemy enemy)
        {
            BuffOwner buffOwner = enemy.GetComp<BuffOwner>();

            if (buffOwner && buffOwner.FindAll(buff => buff.elementType == elementType).Count > 0)
            {
                Collider2D[] cols = Physics2D.OverlapCircleAll(enemy.transform.position, range);

                List<UnitController> units = new List<UnitController>();
                foreach (var col in cols)
                {
                    UnitController unit = col.GetUnit();
                    if (enemy == unit) continue;
                    if (unit)
                    {
                        units.Add(unit);
                    }
                }
                
                TransferBuff(buffOwner, units);
            }
        }

        void TransferBuff(BuffOwner source, List<UnitController> targets)
        {
            // 每层木系debuff都有30%的概率传染
            List<Buff> buffs = source.FindAll(buff => buff.elementType == elementType);
            foreach (var buff in buffs)
            {
                for (int i = 0; i < buff.LVL; i++)
                {
                    UnitController target = targets[RandomUtil.RandRange(0, targets.Count)];
                    BuffOwner targetBuffOwner = target.GetComp<BuffOwner>();
                    float chance = target is Player ? 10f : this.chance;
                    if (targetBuffOwner && RandomUtil.Rand100(chance))
                    {
                        ContentPool<Buff> pool = buff.pool;
                        Buff newBuff = pool.Get();
                        if (!newBuff.AddToContainer(targetBuffOwner))
                        {
                            pool.Release(newBuff);
                        }
                        else
                        {
                            newBuff.RemoveAfter += buffRemoved =>
                            {
                                pool.Release(buffRemoved);
                            };
                        }
                    }
                }
            }
        }
    }
}