using QFramework;
using UnityEngine;

namespace CodeTao
{
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