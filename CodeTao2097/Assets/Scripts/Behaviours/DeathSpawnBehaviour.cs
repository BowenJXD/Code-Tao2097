namespace CodeTao
{
    public class DeathSpawnBehaviour : SpawnUnitBehaviour
    {
        public override void Deinit()
        {
            base.Deinit();
            Spawn();
        }
    }
}