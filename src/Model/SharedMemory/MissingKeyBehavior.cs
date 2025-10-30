namespace Morph.Server.Sdk.Model.SharedMemory
{
    /// <summary>
    /// What to do if value does not exist
    /// </summary>
    public enum MissingKeyBehavior
    {
        /// <summary>
        /// Throws an exception if key does not exist
        /// </summary>
        Throw,

        /// <summary>
        /// Uses default type value instead, if key does not exist
        /// </summary>
        UseDefault,
    }
}
