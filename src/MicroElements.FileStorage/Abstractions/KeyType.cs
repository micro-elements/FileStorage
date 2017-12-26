namespace MicroElements.FileStorage.Abstractions
{
    /// <summary>
    /// Key type.
    /// </summary>
    public enum KeyType
    {
        /// <summary>
        /// Globally unique id.
        /// <para>Can be generated on client side.</para>
        /// <para>Examples: Guid, MongoId, Flake.</para> 
        /// </summary>
        UniqId = 0,

        /// <summary>
        /// Id that can be got from entity.
        /// <para>Not unique globally. Quazy unique for small data sets.</para> 
        /// <para>For example: full user name and birthdate.</para> 
        /// </summary>
        Semantic,

        /// <summary>
        /// Id unique for one collection. It's a sequence of ordinal numbers.
        /// </summary>
        Identity
    }
}