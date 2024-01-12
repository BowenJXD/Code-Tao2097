using QFramework;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 建筑物的基类，有实体，地图上初始就可以有。
    /// </summary>
    public class Building : CombatUnit
    {
        public BindableStat lifeTime = new BindableStat(5f);
        protected LoopTask lifeTask;
        protected Defencer defencer;
        protected Damager damager;
        protected AttributeController attributeController;
        protected Collider2D col2D;
        protected SpriteRenderer sp;
        
        public override void PreInit()
        {
            base.PreInit();
            defencer = this.GetComp<Defencer>();
            damager = this.GetComp<Damager>();
            col2D = this.GetCollider();
            sp = this.GetComponentInDescendants<SpriteRenderer>();
            attributeController = this.GetComp<AttributeController>();
        }
        
        public override void Init()
        {
            base.Init();
            
            // destroy when lifeTime is over
            lifeTask = new LoopTask(this, lifeTime.Value, Deinit);
            lifeTask.SetCountCondition(1);
            lifeTask.Start();
            
            // Change color after taking DMG
            if (defencer)
            {
                defencer.TakeDamageAfter += (damage) =>
                {
                    sp.color = damage.DamageElement.GetColor();

                    ActionKit.Delay(defencer.DMGCD, () =>
                    {
                        if (!this) return;
                        sp.color = Color.white;
                    }).Start(this);
                };
                defencer.OnDeath += damage =>
                {
                    Deinit();
                };
            }

            if (col2D && damager)
            {
                // Attack player when player is in range
                col2D.OnTriggerEnter2DEvent((col) =>
                {
                    UnitController unitController = col.GetComponentInAncestors<UnitController>();
                    Defencer defencer = col.GetComponentInAncestors<Defencer>(1);
                    if (unitController)
                    {
                        if (Util.IsTagIncluded(unitController.tag, damager.damagingTags) && defencer)
                        {
                            DamageManager.Instance.ExecuteDamage(damager, defencer);
                        }
                    }
                }).UnRegisterWhenGameObjectDestroyed(this);
            }
            
            if (attributeController)
            {
                attributeController.onAddAAtModGroup += AddAAtMod;
            }
        }

        public override BindableStat GetAAtMod(EAAt at)
        {
            BindableStat result = null;
            switch (at)
            {
                case EAAt.DEF:
                    result = defencer.DEF;
                    break;
                case EAAt.MaxHP:
                    result = defencer.MaxHP;
                    break;
                case EAAt.AllElementRES:
                    result = defencer.ElementResistances[ElementType.All];
                    break;
                case EAAt.MetalElementRES:
                    result = defencer.ElementResistances[ElementType.Metal];
                    break;
                case EAAt.WoodElementRES:
                    result = defencer.ElementResistances[ElementType.Wood];
                    break;
                case EAAt.WaterElementRES:
                    result = defencer.ElementResistances[ElementType.Water];
                    break;
                case EAAt.FireElementRES:
                    result = defencer.ElementResistances[ElementType.Fire];
                    break;
                case EAAt.EarthElementRES:
                    result = defencer.ElementResistances[ElementType.Earth];
                    break;
            }

            return result;
        }

        public override void Deinit()
        {
            base.Deinit();
            lifeTask.Pause();
        }
    }
}