using UnityEditor;
using UnityEngine;


namespace Koturn.KGPUParticle.Inspectors
{
    /// <summary>
    /// Custom editor of "koturn/KGPUParticle/StarShower".
    /// </summary>
    public class StarShowerGUI : KGPUParticleBaseGUI
    {
        /// <summary>
        /// Property name of "_PolygonSize".
        /// </summary>
        private const string PropNamePolygonSize = "_PolygonSize";

        /// <summary>
        /// Initialize static members.
        /// </summary>
        static StarShowerGUI()
        {
            PreferToUseDefaultRenderType = true;
        }


        /// <summary>
        /// Draw custom properties.
        /// </summary>
        /// <param name="me">A <see cref="MaterialEditor"/>.</param>
        /// <param name="mps"><see cref="MaterialProperty"/> array.</param>
        protected override void DrawCustomProperties(MaterialEditor me, MaterialProperty[] mps)
        {
            ShaderProperty(me, mps, PropNamePolygonSize);
        }
    }
}
