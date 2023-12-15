namespace CodeTao
{
    public enum EAimWay
    {
        /// <summary>
        /// Shoot to the nearest target
        /// </summary>
        AutoTargeting,
        /// <summary>
        /// Shoot from the owner's moving direction
        /// </summary>
        Owner,
        /// <summary>
        /// Shoot to a random direction
        /// </summary>
        Random,
        /// <summary>
        /// Shoot to the cursor
        /// </summary>
        Cursor,
        /// <summary>
        /// Sequentially shoot to the spawn points
        /// </summary>
        Sequential
    }
}