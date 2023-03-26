namespace Koturn.KGPUParticle.Inspectors.Enums
{
    /// <summary>
    /// Rendering types of "VRCFallback".
    /// </summary>
    /// <remarks>
    /// <seealso href="https://docs.vrchat.com/docs/shader-fallback-system"/>
    /// </remarks>
    public enum VRCFallbackRenderType
    {
        /// <summary>
        /// Same as <see cref="VRCFallbackRenderType.Opaque"/> but no string is added to the tag value.
        /// </summary>
        None,
        /// <summary>
        /// Opaque rendering.
        /// </summary>
        Opaque,
        /// <summary>
        /// Cutout rendering.
        /// </summary>
        Cutout,
        /// <summary>
        /// Transparent rendering.
        /// </summary>
        Transparent,
        /// <summary>
        /// Fade rendering.
        /// </summary>
        Fade
    }
}
