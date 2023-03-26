namespace Koturn.KGPUParticle.Inspectors.Enums
{
    /// <summary>
    /// Culling types of "VRCFallback".
    /// </summary>
    /// <remarks>
    /// <seealso href="https://docs.vrchat.com/docs/shader-fallback-system"/>
    /// </remarks>
    public enum VRCFallbackCullType
    {
        /// <summary>
        /// Same as <see cref="VRCFallbackCullType.Default"/>.
        /// </summary>
        None,
        /// <summary>
        /// Back face culling.
        /// </summary>
        Default,
        /// <summary>
        /// No culling.
        /// </summary>
        DoubleSided
    }
}
