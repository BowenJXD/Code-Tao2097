namespace CodeTao
{
    /// <summary>
    /// 各种瞄准方式，每个武器的瞄准方式都不尽相同
    /// </summary>
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