using QFramework;
using UnityEngine;

namespace CodeTao
{
    /// <summary>
    /// Spawns meteors and explosion
    /// 天外来客（金）Celestial Visitor
    /// 召唤陨石（造物，参与共振，一段时间后，或受到一定伤害后移除），砸向大地（砸的一下推开单位，有伤害）。
    /// </summary>
    public class MeteorSpawner : BuildingSpawner<Meteor>
    {
        public override Meteor SpawnUnit(Vector2 localPos)
        {
            Meteor obj = base.SpawnUnit(localPos);
            Vector3 spawnPosition = transform.position + (Vector3)localPos;
            obj.onFall += () => SpawnExplosion(spawnPosition)?.Init();
            
            return obj;
        }
        
        Explosion SpawnExplosion(Vector2 spawnPosition)
        {
            Explosion explosion = ExplosionManager.Instance.Get();
            explosion.Position(spawnPosition);
            explosion.damager.DamageElementType = elementType;
            explosion.damager.DealDamageAfter += damager.DealDamageAfter;
            return explosion;
        }
    }
}