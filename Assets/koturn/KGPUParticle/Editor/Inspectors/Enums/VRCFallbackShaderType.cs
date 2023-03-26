namespace Koturn.KGPUParticle.Inspectors.Enums
{
    /// <summary>
    /// Shader types of "VRCFallback".
    /// </summary>
    /// <remarks>
    /// <seealso href="https://docs.vrchat.com/docs/shader-fallback-system"/>
    /// </remarks>
    public enum VRCFallbackShaderType
    {
        /// <summary>
        /// Unlit shader.
        /// </summary>
        Unlit,
        /// <summary>
        /// Standard shader.
        /// </summary>
        Standard,
        /// <summary>
        /// VertexLit shader.
        /// </summary>
        VertexLit,
        /// <summary>
        /// Toon shader.
        /// </summary>
        Toon,
        /// <summary>
        /// Particle shader.
        /// </summary>
        Particle,
        /// <summary>
        /// Sprite shader.
        /// </summary>
        Sprite,
        /// <summary>
        /// Matcap shader.
        /// </summary>
        Matcap,
        /// <summary>
        /// MobileToon shader.
        /// </summary>
        MobileToon,
        /// <summary>
        /// Hide the mesh from view, useful for things like raymarching effects.
        /// </summary>
        Hidden
    }
}
