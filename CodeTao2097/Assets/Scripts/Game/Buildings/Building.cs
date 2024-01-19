using QFramework;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// 建筑物的基类，有实体，地图上初始就可以有。
    /// </summary>
    public class Building : CombatUnit, IWeaponDerivative, IWAtReceiver
    {
        public BindableStat lifeTime = new BindableStat(5f);
        public BindableStat area = new BindableStat(1f);
        protected LoopTask lifeTask;
        protected Defencer defencer;
        protected Damager damager;
        protected AttributeController attributeController;
        protected Collider2D col2D;
        protected SpriteRenderer sp;
        
        public override void PreInit()
        {
            base.PreInit();
            defencer = GetComp<Defencer>();
            damager = GetComp<Damager>();
            col2D = this.GetCollider();
            sp = this.GetComponentInDescendants<SpriteRenderer>();
            attributeController = GetComp<AttributeController>();
        }
        
        public override void Init()
        {
            base.Init();
            
            // destroy when lifeTime is over
            lifeTask = new LoopTask(this, lifeTime.Value, Deinit);
            lifeTask.SetCountCondition(1);
            lifeTask.Start();

            area.RegisterWithInitValue(value =>
            {
                this.LocalScale(new Vector3(value, value));
            }).UnRegisterWhenGameObjectDestroyed(this);
            
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
        }

        public override void Deinit()
        {
            base.Deinit();
            lifeTask.Pause();
        }

        public Weapon weapon { get; set; }
        public void SetWeapon(Weapon newWeapon, Damager newDamager)
        {
            weapon = newWeapon;
            if (!damager) damager = newDamager;
        }

        public void Receive(IWAtSource source)
        {
            lifeTime.InheritStat(source.GetWAt(EWAt.Duration));
            area.InheritStat(source.GetWAt(EWAt.Area));
        }
    }
}