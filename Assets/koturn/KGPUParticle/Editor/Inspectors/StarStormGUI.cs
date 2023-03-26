using UnityEditor;
using UnityEngine;


namespace Koturn.KGPUParticle.Inspectors
{
    /// <summary>
    /// Custom editor of "koturn/KGPUParticle/StarStorm".
    /// </summary>
    public class StarStormGUI : KGPUParticleBaseGUI
    {
        /// <summary>
        /// Property name of "_PolygonSize".
        /// </summary>
        private const string PropNamePolygonSize = "_PolygonSize";
        /// <summary>
        /// Property name of "_Speed".
        /// </summary>
        private const string PropNameSpeed = "_Speed";
        /// <summary>
        /// Property name of "_SpeedAmp".
        /// </summary>
        private const string PropNameSpeedAmp = "_SpeedAmp";

        /// <summary>
        /// Initialize static members.
        /// </summary>
        static StarStormGUI()
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
            ShaderProperty(me, mps, PropNameSpeed);
            ShaderProperty(me, mps, PropNameSpeedAmp);
        }
    }
}
