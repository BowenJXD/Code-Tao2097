namespace CodeTao
{
    public class DeathSpawnBehaviour : SpawnBehaviour
    {
        public override void Deinit()
        {
            base.Deinit();
            Spawn();
        }
    }
}