using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Koturn.KGPUParticle.Inspectors.Enums;


namespace Koturn.KGPUParticle.Inspectors
{
    /// <summary>
    /// Custom editor of KGPUParticle shaders.
    /// </summary>
    public class KGPUParticleBaseGUI : ShaderGUI
    {
        /// <summary>
        /// Property name of "_MainTex".
        /// </summary>
        private const string PropNameMainTex = "_MainTex";
        /// <summary>
        /// Property name of "_Color".
        /// </summary>
        private const string PropNameColor = "_Color";
        /// <summary>
        /// Property name of "_LightingMethod".
        /// </summary>
        private const string PropNameLightingMethod = "_LightingMethod";
        /// <summary>
        /// Property name of "_SpecColor".
        /// </summary>
        private const string PropNameSpecColor = "_SpecColor";
        /// <summary>
        /// Property name of "_SpecPower".
        /// </summary>
        private const string PropNameSpecPower = "_SpecPower";
        /// <summary>
        /// Property name of "_EnableRefProbe".
        /// </summary>
        private const string PropNameEnableReflectionProbe = "_EnableReflectionProbe";
        /// <summary>
        /// Property name of "_Glossiness".
        /// </summary>
        private const string PropNameGlossiness = "_Glossiness";
        /// <summary>
        /// Property name of "_Metallic".
        /// </summary>
        private const string PropNameMetallic = "_Metallic";
        /// <summary>
        /// Property name of "_DiffuseMode".
        /// </summary>
        private const string PropNameDiffuseMode = "_DiffuseMode";
        /// <summary>
        /// Property name of "_SpecularMode".
        /// </summary>
        private const string PropNameSpecularMode = "_SpecularMode";
        /// <summary>
        /// Property name of "_AmbientMode".
        /// </summary>
        private const string PropNameAmbientMode = "_AmbientMode";
        /// <summary>
        /// Property name of "_NormalCalcMode".
        /// </summary>
        private const string PropNameNormalCalcMethod = "_NormalCalcMethod";
        /// <summary>
        /// Property name of "_NormalCalcOptimize".
        /// </summary>
        private const string PropNameNormalCalcOptimize = "_NormalCalcOptimize";

        /// <summary>
        /// Property name of "_Cull".
        /// </summary>
        private const string PropNameCull = "_Cull";
        /// <summary>
        /// Property name of "_FlipBackNormal".
        /// </summary>
        private const string PropNameFlipBackNormal = "_FlipBackNormal";
        /// <summary>
        /// Property name of "__RenderingMode".
        /// </summary>
        private const string PropNameRenderingMode = "__RenderingMode";
        /// <summary>
        /// Property name of "_AlphaTest".
        /// </summary>
        private const string PropNameAlphaTest = "_AlphaTest";
        /// <summary>
        /// Property name of "_Cutoff".
        /// </summary>
        private const string PropNameCutoff = "_Cutoff";
        /// <summary>
        /// Property name of "_SrcBlend".
        /// </summary>
        private const string PropNameSrcBlend = "_SrcBlend";
        /// <summary>
        /// Property name of "_DstBlend".
        /// </summary>
        private const string PropNameDstBlend = "_DstBlend";
        /// <summary>
        /// Property name of "_SrcBlendAlpha".
        /// </summary>
        private const string PropNameSrcBlendAlpha = "_SrcBlendAlpha";
        /// <summary>
        /// Property name of "_DstBlendAlpha".
        /// </summary>
        private const string PropNameDstBlendAlpha = "_DstBlendAlpha";
        /// <summary>
        /// Property name of "_BlendOp".
        /// </summary>
        private const string PropNameBlendOp = "_BlendOp";
        /// <summary>
        /// Property name of "_BlendOpAlpha".
        /// </summary>
        private const string PropNameBlendOpAlpha = "_BlendOpAlpha";
        /// <summary>
        /// Property name of "_ZWrite".
        /// </summary>
        private const string PropNameZWrite = "_ZWrite";
        /// <summary>
        /// Property name of "_ZTest".
        /// </summary>
        private const string PropNameZTest = "_ZTest";
        /// <summary>
        /// Property name of "_ZClip".
        /// </summary>
        private const string PropNameZClip = "_ZClip";
        /// <summary>
        /// Property name of "_OffsetFact".
        /// </summary>
        private const string PropNameOffsetFact = "_OffsetFact";
        /// <summary>
        /// Property name of "_OffsetUnit".
        /// </summary>
        private const string PropNameOffsetUnit = "_OffsetUnit";
        /// <summary>
        /// Property name of "_ColorMask".
        /// </summary>
        private const string PropNameColorMask = "_ColorMask";
        /// <summary>
        /// Property name of "_AlphaToMask".
        /// </summary>
        private const string PropNameAlphaToMask = "_AlphaToMask";
        /// <summary>
        /// Property name of "_StencilRef".
        /// </summary>
        private const string PropNameStencilRef = "_StencilRef";
        /// <summary>
        /// Property name of "_StencilReadMask".
        /// </summary>
        private const string PropNameStencilReadMask = "_StencilReadMask";
        /// <summary>
        /// Property name of "_StencilWriteMask".
        /// </summary>
        private const string PropNameStencilWriteMask = "_StencilWriteMask";
        /// <summary>
        /// Property name of "_StencilCompFunc".
        /// </summary>
        private const string PropNameStencilCompFunc = "_StencilCompFunc";
        /// <summary>
        /// Property name of "_StencilPass".
        /// </summary>
        private const string PropNameStencilPass = "_StencilPass";
        /// <summary>
        /// Property name of "_StencilFail".
        /// </summary>
        private const string PropNameStencilFail = "_StencilFail";
        /// <summary>
        /// Property name of "_StencilZFail".
        /// </summary>
        private const string PropNameStencilZFail = "_StencilZFail";
        /// <summary>
        /// Tag name of "RenderType".
        /// </summary>
        private const string TagRenderType = "RenderType";
        /// <summary>
        /// Tag name of "VRCFallback".
        /// </summary>
        private const string TagVRCFallback = "VRCFallback";


        /// <summary>
        /// Cache of reflection result of following lambda.
        /// </summary>
        /// <remarks><seealso cref="CreateToggleKeywordDelegate"/></remarks>
        private static Action<Shader, MaterialProperty, bool> _toggleKeyword;

        /// <summary>
        /// Use default RenderType or not.
        /// </summary>
        protected static bool PreferToUseDefaultRenderType { get; set; }


        /// <summary>
        /// Initialize static members.
        /// </summary>
        static KGPUParticleBaseGUI()
        {
            PreferToUseDefaultRenderType = false;
        }


        /// <summary>
        /// Draw property items.
        /// </summary>
        /// <param name="me">The <see cref="MaterialEditor"/> that are calling this <see cref="OnGUI(MaterialEditor, MaterialProperty[])"/> (the 'owner').</param>
        /// <param name="mps">Material properties of the current selected shader.</param>
        public override void OnGUI(MaterialEditor me, MaterialProperty[] mps)
        {
            EditorGUILayout.LabelField("Main Parameters", EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                ShaderProperty(me, mps, PropNameMainTex, false);
                ShaderProperty(me, mps, PropNameColor, false);
                DrawCustomProperties(me, mps);
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Lighting Parameters", EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                var mpLightingMethod = FindAndDrawProperty(me, mps, PropNameLightingMethod, false);
                var lightingMethod = (LightingMethod)(mpLightingMethod == null ? -1 : (int)mpLightingMethod.floatValue);


                var isNeedGM = true;
                var mpEnableReflectionProbe = FindAndDrawProperty(me, mps, PropNameEnableReflectionProbe, false);
                if (mpEnableReflectionProbe != null)
                {
                    isNeedGM = ToBool(mpEnableReflectionProbe.floatValue);
                }

                using (new EditorGUI.DisabledScope(!isNeedGM))
                {
                    using (new EditorGUI.DisabledScope(lightingMethod == LightingMethod.UnityLambert))
                    {
                        ShaderProperty(me, mps, PropNameGlossiness, false);
                    }
                    using (new EditorGUI.DisabledScope(lightingMethod != LightingMethod.UnityStandard))
                    {
                        ShaderProperty(me, mps, PropNameMetallic, false);
                    }
                }

                using (new EditorGUI.DisabledScope(lightingMethod == LightingMethod.UnityLambert))
                {
                    ShaderProperty(me, mps, PropNameSpecColor, false);
                    ShaderProperty(me, mps, PropNameSpecPower, false);
                }

                using (new EditorGUI.DisabledScope(lightingMethod != LightingMethod.Custom))
                {
                    ShaderProperty(me, mps, PropNameDiffuseMode, false);
                    ShaderProperty(me, mps, PropNameSpecularMode, false);
                    ShaderProperty(me, mps, PropNameAmbientMode, false);
                }

                var mpNormalCalcMethod = FindProperty(PropNameNormalCalcMethod, mps, false);
                var mpNormalCalcOptimize = FindProperty(PropNameNormalCalcOptimize, mps, false);
                if (mpNormalCalcMethod != null && mpNormalCalcOptimize != null)
                {
                    EditorGUILayout.LabelField("Normal Calculation Options", EditorStyles.boldLabel);
                    using (new EditorGUI.IndentLevelScope())
                    using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                    {
                        ShaderProperty(me, mpNormalCalcMethod);
                        ShaderProperty(me, mpNormalCalcOptimize);
                    }
                }
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Rendering Options", EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                var isFlipBackNormalEditable = true;
                var isCullChanged = false;
                using (var ccScope = new EditorGUI.ChangeCheckScope())
                {
                    var mpCull = FindProperty(PropNameCull, mps, false);
                    if (mpCull != null)
                    {
                        ShaderProperty(me, mpCull);
                        isFlipBackNormalEditable = (int)mpCull.floatValue != (int)CullMode.Back;
                        isCullChanged = ccScope.changed;
                    }
                }

                var mpFlipBackNormal = FindProperty(PropNameFlipBackNormal, mps, false);
                if (mpFlipBackNormal != null)
                {
                    using (var ccScope = new EditorGUI.ChangeCheckScope())
                    {
                        using (new EditorGUI.DisabledScope(!isFlipBackNormalEditable))
                        {
                            ShaderProperty(me, mps, PropNameFlipBackNormal, false);
                        }
                        if (!ccScope.changed && isCullChanged)
                        {
                            var material = mpFlipBackNormal.targets.Cast<Material>().First();
                            if (isFlipBackNormalEditable && ToBool(mpFlipBackNormal.floatValue))
                            {
                                SetToggleKeyword(material.shader, mpFlipBackNormal, true);
                            }
                            else
                            {
                                SetToggleKeyword(material.shader, mpFlipBackNormal, false);
                            }
                        }
                    }
                }

                DrawRenderingMode(me, mps);
                ShaderProperty(me, mps, PropNameZTest, false);
                ShaderProperty(me, mps, PropNameZClip, false);
                DrawOffsetProperties(me, mps, PropNameOffsetFact, PropNameOffsetUnit);
                ShaderProperty(me, mps, PropNameColorMask, false);
                ShaderProperty(me, mps, PropNameAlphaToMask, false);

                EditorGUILayout.Space();
                DrawBlendProperties(me, mps);
                EditorGUILayout.Space();
                DrawStencilProperties(me, mps);
                EditorGUILayout.Space();
                DrawVRCFallbackGUI(mps.First().targets.Cast<Material>().First());
                EditorGUILayout.Space();
                DrawAdvancedOptions(me, mps);
            }
        }

        /// <summary>
        /// Draw custom properties.
        /// </summary>
        /// <param name="me">A <see cref="MaterialEditor"/>.</param>
        /// <param name="mps"><see cref="MaterialProperty"/> array.</param>
        protected virtual void DrawCustomProperties(MaterialEditor me, MaterialProperty[] mps)
        {
            // Do nothing.
        }

        /// <summary>
        /// Draw default item of specified shader property.
        /// </summary>
        /// <param name="me">A <see cref="MaterialEditor"/>.</param>
        /// <param name="mps"><see cref="MaterialProperty"/> array.</param>
        /// <param name="propName">Name of shader property.</param>
        /// <param name="isMandatory">If <c>true</c> then this method will throw an exception
        /// if a property with <<paramref name="propName"/> was not found.</param>
        protected static void ShaderProperty(MaterialEditor me, MaterialProperty[] mps, string propName, bool isMandatory = true)
        {
            var prop = FindProperty(propName, mps, isMandatory);
            if (prop != null)
            {
                ShaderProperty(me, prop);
            }
        }

        /// <summary>
        /// Draw default item of specified shader property.
        /// </summary>
        /// <param name="me">A <see cref="MaterialEditor"/>.</param>
        /// <param name="mp">Target <see cref="MaterialProperty"/>.</param>
        protected static void ShaderProperty(MaterialEditor me, MaterialProperty mp)
        {
            if (mp != null)
            {
                me.ShaderProperty(mp, mp.displayName);
            }
        }

        /// <summary>
        /// Draw default item of specified shader property and return the property.
        /// </summary>
        /// <param name="me">A <see cref="MaterialEditor"/>.</param>
        /// <param name="mps"><see cref="MaterialProperty"/> array.</param>
        /// <param name="propName">Name of shader property.</param>
        /// <param name="isMandatory">If <c>true</c> then this method will throw an exception
        /// if a property with <<paramref name="propName"/> was not found.</param>
        /// <return>Found property.</return>
        protected static MaterialProperty FindAndDrawProperty(MaterialEditor me, MaterialProperty[] mps, string propName, bool isMandatory = true)
        {
            var prop = FindProperty(propName, mps, isMandatory);
            if (prop != null)
            {
                ShaderProperty(me, prop);
            }

            return prop;
        }

        /// <summary>
        /// Find properties which has specified names.
        /// </summary>
        /// <param name="propNames">Names of shader property.</param>
        /// <param name="mps"><see cref="MaterialProperty"/> array.</param>
        /// <param name="isMandatory">If <c>true</c> then this method will throw an exception
        /// if one of properties with <<paramref name="propNames"/> was not found.</param>
        /// <return>Found properties.</return>
        protected static List<MaterialProperty> FindProperties(string[] propNames, MaterialProperty[] mps, bool isMandatory = true)
        {
            var mpList = new List<MaterialProperty>(propNames.Length);
            foreach (var propName in propNames)
            {
                var prop = FindProperty(propName, mps, isMandatory);
                if (prop != null)
                {
                    mpList.Add(prop);
                }
            }

            return mpList;
        }

        /// <summary>
        /// Draw inspector items of <see cref="RenderingMode"/>.
        /// </summary>
        /// <param name="me">A <see cref="MaterialEditor"/>.</param>
        /// <param name="mps"><see cref="MaterialProperty"/> array.</param>
        private void DrawRenderingMode(MaterialEditor me, MaterialProperty[] mps)
        {
            using (var ccScope = new EditorGUI.ChangeCheckScope())
            {
                var mpRenderingMode = FindProperty(PropNameRenderingMode, mps, false);
                var mode = RenderingMode.Custom;
                if (mpRenderingMode != null)
                {
                    mode = (RenderingMode)EditorGUILayout.EnumPopup(mpRenderingMode.displayName, (RenderingMode)mpRenderingMode.floatValue);
                    mpRenderingMode.floatValue = (float)mode;

                    if (ccScope.changed && mode != RenderingMode.Custom)
                    {
                        var mpAlphaTest = FindProperty(PropNameAlphaTest, mps, false);
                        foreach (var material in mpRenderingMode.targets.Cast<Material>())
                        {
                            ApplyRenderingMode(material, mpAlphaTest, mode);
                        }
                    }
                }

                using (new EditorGUI.DisabledScope(mode != RenderingMode.Cutout && mode != RenderingMode.Custom))
                {
                    var mpAlphaTest = FindAndDrawProperty(me, mps, PropNameAlphaTest, false);
                    if (mpAlphaTest != null)
                    {
                        using (new EditorGUI.IndentLevelScope())
                        using (new EditorGUI.DisabledScope(ToBool(mpAlphaTest.floatValue)))
                        {
                            ShaderProperty(me, mps, PropNameCutoff);
                        }
                    }
                }

                using (new EditorGUI.DisabledScope(mode != RenderingMode.Custom))
                {
                    ShaderProperty(me, mps, PropNameZWrite);
                }
            }
        }

        /// <summary>
        /// Change blend of <paramref name="material"/>.
        /// </summary>
        /// <param name="material">Target material.</param>
        /// <param name="mpAlphaTest"><see cref="MaterialProperty"/> for "_AlphaTest".</param>
        /// <param name="renderingMode">Rendering mode.</param>
        private static void ApplyRenderingMode(Material material, MaterialProperty mpAlphaTest, RenderingMode renderingMode)
        {
            switch (renderingMode)
            {
                case RenderingMode.Opaque:
                    SetRenderTypeTag(material, RenderType.Opaque);
                    SetAlphaTest(material, mpAlphaTest, false);
                    material.SetInt(PropNameZWrite, 1);
                    material.SetInt(PropNameSrcBlend, (int)BlendMode.One);
                    material.SetInt(PropNameDstBlend, (int)BlendMode.Zero);
                    material.SetInt(PropNameSrcBlendAlpha, (int)BlendMode.One);
                    material.SetInt(PropNameDstBlendAlpha, (int)BlendMode.Zero);
                    material.SetInt(PropNameBlendOp, (int)BlendOp.Add);
                    material.SetInt(PropNameBlendOpAlpha, (int)BlendOp.Add);
                    SetRenderQueue(material, RenderQueue.Geometry);
                    break;
                case RenderingMode.Cutout:
                    SetRenderTypeTag(material, RenderType.TransparentCutout);
                    SetAlphaTest(material, mpAlphaTest, true);
                    material.SetInt(PropNameZWrite, 1);
                    material.SetInt(PropNameSrcBlend, (int)BlendMode.One);
                    material.SetInt(PropNameDstBlend, (int)BlendMode.Zero);
                    material.SetInt(PropNameSrcBlendAlpha, (int)BlendMode.One);
                    material.SetInt(PropNameDstBlendAlpha, (int)BlendMode.Zero);
                    material.SetInt(PropNameBlendOp, (int)BlendOp.Add);
                    material.SetInt(PropNameBlendOpAlpha, (int)BlendOp.Add);
                    SetRenderQueue(material, RenderQueue.AlphaTest);
                    break;
                case RenderingMode.Fade:
                    SetRenderTypeTag(material, RenderType.Transparent);
                    SetAlphaTest(material, mpAlphaTest, false);
                    material.SetInt(PropNameZWrite, 0);
                    material.SetInt(PropNameSrcBlend, (int)BlendMode.SrcAlpha);
                    material.SetInt(PropNameDstBlend, (int)BlendMode.OneMinusSrcAlpha);
                    material.SetInt(PropNameSrcBlendAlpha, (int)BlendMode.SrcAlpha);
                    material.SetInt(PropNameDstBlendAlpha, (int)BlendMode.OneMinusSrcAlpha);
                    material.SetInt(PropNameBlendOp, (int)BlendOp.Add);
                    material.SetInt(PropNameBlendOpAlpha, (int)BlendOp.Add);
                    SetRenderQueue(material, RenderQueue.Transparent);
                    break;
                case RenderingMode.Transparent:
                    SetRenderTypeTag(material, RenderType.Transparent);
                    SetAlphaTest(material, mpAlphaTest, false);
                    material.SetInt(PropNameZWrite, 0);
                    material.SetInt(PropNameSrcBlend, (int)BlendMode.One);
                    material.SetInt(PropNameDstBlend, (int)BlendMode.OneMinusSrcAlpha);
                    material.SetInt(PropNameSrcBlendAlpha, (int)BlendMode.One);
                    material.SetInt(PropNameDstBlendAlpha, (int)BlendMode.OneMinusSrcAlpha);
                    material.SetInt(PropNameBlendOp, (int)BlendOp.Add);
                    material.SetInt(PropNameBlendOpAlpha, (int)BlendOp.Add);
                    SetRenderQueue(material, RenderQueue.Transparent);
                    break;
                case RenderingMode.Additive:
                    SetRenderTypeTag(material, RenderType.Transparent);
                    SetAlphaTest(material, mpAlphaTest, false);
                    material.SetInt(PropNameZWrite, 0);
                    material.SetInt(PropNameSrcBlend, (int)BlendMode.SrcAlpha);
                    material.SetInt(PropNameDstBlend, (int)BlendMode.One);
                    material.SetInt(PropNameSrcBlendAlpha, (int)BlendMode.SrcAlpha);
                    material.SetInt(PropNameDstBlendAlpha, (int)BlendMode.One);
                    material.SetInt(PropNameBlendOp, (int)BlendOp.Add);
                    material.SetInt(PropNameBlendOpAlpha, (int)BlendOp.Add);
                    SetRenderQueue(material, RenderQueue.Transparent);
                    break;
                case RenderingMode.Multiply:
                    SetRenderTypeTag(material, RenderType.Transparent);
                    SetAlphaTest(material, mpAlphaTest, false);
                    material.SetInt(PropNameZWrite, 0);
                    material.SetInt(PropNameSrcBlend, (int)BlendMode.DstColor);
                    material.SetInt(PropNameDstBlend, (int)BlendMode.Zero);
                    material.SetInt(PropNameSrcBlendAlpha, (int)BlendMode.DstColor);
                    material.SetInt(PropNameDstBlendAlpha, (int)BlendMode.Zero);
                    material.SetInt(PropNameBlendOp, (int)BlendOp.Add);
                    material.SetInt(PropNameBlendOpAlpha, (int)BlendOp.Add);
                    SetRenderQueue(material, RenderQueue.Transparent);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(renderingMode), renderingMode, null);
            }
        }

        /// <summary>
        /// Set "_AlphaTest" value.
        /// </summary>
        /// <param name="material">Target material.</param>
        /// <param name="mpAlphaTest"><see cref="MaterialProperty"/> for "_AlphaTest".</param>
        /// <param name="isEnabled"><para>Toggle switch value.</para>
        /// <para>If this value is true, _AlphaTest is set to 1 and define a keyword "_ALPHATEST_ON".</para>
        /// <para>Otherwise _AlphaTest is set to 0 and undefine a keyword "_ALPHATEST_ON".</para>
        /// </param>
        private static void SetAlphaTest(Material material, MaterialProperty mpAlphaTest, bool isEnabled)
        {
            if (mpAlphaTest == null)
            {
                return;
            }

            if (isEnabled)
            {
                material.SetInt(PropNameAlphaTest, 1);
                SetToggleKeyword(material.shader, mpAlphaTest, true);
            }
            else
            {
                material.SetInt(PropNameAlphaTest, 0);
                SetToggleKeyword(material.shader, mpAlphaTest, false);
            }
        }

        /// <summary>
        /// Set render queue value if the value is differ from the default.
        /// </summary>
        /// <param name="material">Target material.</param>
        /// <param name="renderQueue"><see cref="RenderQueue"/> to set.</param>
        private static void SetRenderTypeTag(Material material, RenderType renderType)
        {
            // Set to default and get the default.
            material.SetOverrideTag(TagRenderType, "");
            if (PreferToUseDefaultRenderType)
            {
                return;
            }

            var defaultTagval = material.GetTag(TagRenderType, false, "Transparent");

            // Set specified render type value if the value differs from the default.
            var renderTypeValue = renderType.ToString();
            if (renderTypeValue != defaultTagval)
            {
                material.SetOverrideTag(TagRenderType, renderTypeValue);
            }
        }

        /// <summary>
        /// Set render queue value if the value is differ from the default.
        /// </summary>
        /// <param name="material">Target material.</param>
        /// <param name="renderQueue"><see cref="RenderQueue"/> to set.</param>
        private static void SetRenderQueue(Material material, RenderQueue renderQueue)
        {
            // Set to default and get the default.
            material.renderQueue = -1;
            var defaultRenderQueue = material.renderQueue;

            // Set specified render queue value if the value differs from the default.
            var renderQueueValue = (int)renderQueue;
            if (defaultRenderQueue != renderQueueValue)
            {
                material.renderQueue = renderQueueValue;
            }
        }

        /// <summary>
        /// Draw inspector items of "Blend".
        /// </summary>
        /// <param name="me">A <see cref="MaterialEditor"/>.</param>
        /// <param name="mps"><see cref="MaterialProperty"/> array.</param>
        private void DrawBlendProperties(MaterialEditor me, MaterialProperty[] mps)
        {
            var mpRenderingMode = FindProperty(PropNameRenderingMode, mps, false);
            using (new EditorGUI.DisabledScope(mpRenderingMode != null && (RenderingMode)mpRenderingMode.floatValue != RenderingMode.Custom))
            {
                var propSrcBlend = FindProperty(PropNameSrcBlend, mps, false);
                var propDstBlend = FindProperty(PropNameDstBlend, mps, false);
                if (propSrcBlend == null || propDstBlend == null)
                {
                    return;
                }

                EditorGUILayout.LabelField("Blend", EditorStyles.boldLabel);
                using (new EditorGUI.IndentLevelScope())
                using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                {
                    ShaderProperty(me, propSrcBlend);
                    ShaderProperty(me, propDstBlend);

                    var propSrcBlendAlpha = FindProperty(PropNameSrcBlendAlpha, mps, false);
                    var propDstBlendAlpha = FindProperty(PropNameDstBlendAlpha, mps, false);
                    if (propSrcBlendAlpha != null && propDstBlendAlpha != null)
                    {
                        ShaderProperty(me, propSrcBlendAlpha);
                        ShaderProperty(me, propDstBlendAlpha);
                    }

                    ShaderProperty(me, mps, PropNameBlendOp, false);
                    ShaderProperty(me, mps, PropNameBlendOpAlpha, false);
                }
            }
        }

        /// <summary>
        /// Draw inspector items of "Offset".
        /// </summary>
        /// <param name="me">A <see cref="MaterialEditor"/>.</param>
        /// <param name="mps"><see cref="MaterialProperty"/> array.</param>
        /// <param name="propNameFactor">Property name for the first argument of "Offset".</param>
        /// <param name="propNameUnit">Property name for the second argument of "Offset".</param>
        private static void DrawOffsetProperties(MaterialEditor me, MaterialProperty[] mps, string propNameFactor, string propNameUnit)
        {
            var propFactor = FindProperty(propNameFactor, mps, false);
            var propUnit = FindProperty(propNameUnit, mps, false);
            if (propFactor == null || propUnit == null)
            {
                return;
            }
            EditorGUILayout.LabelField("Offset");
            using (new EditorGUI.IndentLevelScope())
            {
                ShaderProperty(me, propFactor);
                ShaderProperty(me, propUnit);
            }
        }

        /// <summary>
        /// Draw inspector items of Stencil.
        /// </summary>
        /// <param name="me">A <see cref="MaterialEditor"/>.</param>
        /// <param name="mps"><see cref="MaterialProperty"/> array.</param>
        private static void DrawStencilProperties(MaterialEditor me, MaterialProperty[] mps)
        {
            var stencilProps = FindProperties(new []
            {
                PropNameStencilRef,
                PropNameStencilReadMask,
                PropNameStencilWriteMask,
                PropNameStencilCompFunc,
                PropNameStencilPass,
                PropNameStencilFail,
                PropNameStencilZFail
            }, mps, false);

            if (stencilProps.Count == 0)
            {
                return;
            }

            EditorGUILayout.LabelField("Stencil", EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                foreach (var prop in stencilProps)
                {
                    me.ShaderProperty(prop, prop.displayName);
                }
            }
        }

        /// <summary>
        /// Draw items for the tag, "VRCFallback".
        /// </summary>
        /// <param name="material">A material.</param>
        /// <remarks>
        /// <seealso href="https://docs.vrchat.com/docs/shader-fallback-system"/>
        /// </remarks>
        [System.Diagnostics.Conditional("VRC_SDK_VRCSDK2"), System.Diagnostics.Conditional("VRC_SDK_VRCSDK3")]
        private void DrawVRCFallbackGUI(Material material)
        {
            EditorGUILayout.LabelField(TagVRCFallback, EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                if (GUILayout.Button("Reset to Default"))
                {
                    material.SetOverrideTag(TagVRCFallback, "");
                }
                var tagVal = material.GetTag(TagVRCFallback, false);

                using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                using (var ccScope = new EditorGUI.ChangeCheckScope())
                {
                    var shaderType = VRCFallbackPopupItem<VRCFallbackShaderType>("Shader Type", tagVal);
                    var strHidden = VRCFallbackShaderType.Hidden.ToString();
                    var isHidden = shaderType == strHidden;
                    using (new EditorGUI.DisabledScope(isHidden))
                    {
                        var renderingMode = VRCFallbackPopupItem<VRCFallbackRenderType>("Rendering Mode", tagVal, false);
                        var facing = VRCFallbackPopupItem<VRCFallbackCullType>("Facing", tagVal, false);
                        if (ccScope.changed)
                        {
                            var newTagVal = isHidden ? strHidden : new StringBuilder()
                                .Append(shaderType)
                                .Append(renderingMode)
                                .Append(facing)
                                .ToString();
                            EditorGUILayout.LabelField("Result", '"' + newTagVal + '"');
                            material.SetOverrideTag(TagVRCFallback, newTagVal);
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Result", '"' + tagVal + '"');
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draw one of popup item of VRCFallback shader.
        /// </summary>
        /// <typeparam name="T"><see cref="VRCFallbackShaderType"/>, <see cref="VRCFallbackRenderType"/> or <see cref="VRCFallbackCullType"/>.</typeparam>
        /// <param name="label">Label text.</param>
        /// <param name="tagVal">Value of "VRCFallback".</param>
        /// <param name="allowDefault">Allow default value or not.</param>
        /// <param name="defaultVal">Default value.</param>
        /// <returns>Return name of estimated selected popup item.
        /// If no popup item is found or <paramref name="allowDefault"/> is true and default value is selected,
        /// returns <see cref="string.Empty"/></returns>
        private static string VRCFallbackPopupItem<T>(string label, string tagVal, bool allowDefault = true, int defaultVal = 0)
            where T : Enum
        {
            var type = typeof(T);
            var names = Enum.GetNames(type);
            var val = GetCurrentSelectedValue<T>(tagVal, names, allowDefault);
            val = EditorGUILayout.Popup(label, val, names);
            if (Enum.IsDefined(type, val) && (allowDefault || val != defaultVal))
            {
                return Enum.GetName(type, val);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Estimate and get the current selected popup item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tagVal">Value of "VRCFallback".</param>
        /// <param name="enumNames">Enum names of <see cref="VRCFallbackShaderType"/>, <see cref="VRCFallbackRenderType"/> or <see cref="VRCFallbackCullType"/>.</param>
        /// <param name="allowDefault">Allow default value or not.</param>
        /// <returns>Enum value of selected item.</returns>
        private static int GetCurrentSelectedValue<T>(string tagVal, string[] enumNames, bool allowDefault = true)
            where T : Enum
        {
            var query = enumNames.Zip(Enum.GetValues(typeof(T)).Cast<int>(), (name, val) => (name, val));
            foreach (var (name, val) in allowDefault ? query : query.Skip(1))
            {
                if (tagVal.Contains(name))
                {
                    return val;
                }
            }

            return default;
        }

        /// <summary>
        /// Draw inspector items of advanced options.
        /// </summary>
        /// <param name="me">A <see cref="MaterialEditor"/>.</param>
        /// <param name="mps"><see cref="MaterialProperty"/> array.</param>
        private static void DrawAdvancedOptions(MaterialEditor me, MaterialProperty[] mps)
        {
            EditorGUILayout.LabelField("Advanced Options", EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                me.RenderQueueField();
#if UNITY_5_6_OR_NEWER
                me.EnableInstancingField();
                me.DoubleSidedGIField();
#endif  // UNITY_5_6_OR_NEWER
            }
        }

        /// <summary>
        /// Convert a <see cref="float"/> value to <see cref="bool"/> value.
        /// </summary>
        /// <param name="floatValue">Source <see cref="float"/> value.</param>
        /// <returns>True if <paramref name="floatValue"/> is greater than 0.5, otherwise false.</returns>
        private static bool ToBool(float floatValue)
        {
            return floatValue >= 0.5f;
        }

        /// <summary>
        /// Enable or disable keyword of <see cref="MaterialProperty"/> which has MaterialToggleUIDrawer.
        /// </summary>
        /// <param name="shader">Target <see cref="Shader"/>.</param>
        /// <param name="prop">Target <see cref="MaterialProperty"/>.</param>
        private static void SetToggleKeyword(Shader shader, MaterialProperty prop, bool isOn)
        {
            try
            {
                (_toggleKeyword ?? (_toggleKeyword = CreateSetKeywordDelegate()))(shader, prop, isOn);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }

        /// <summary>
        /// <para>Create delegate of reflection results about UnityEditor.MaterialToggleUIDrawer.</para>
        /// <code>
        /// (Shader shader, MaterialProperty prop, bool isOn) =>
        /// {
        ///     MaterialPropertyHandler mph = UnityEditor.MaterialPropertyHandler.GetHandler(shader, name);
        ///     if (mph is null)
        ///     {
        ///         throw new ArgumentException("Specified MaterialProperty does not have UnityEditor.MaterialPropertyHandler");
        ///     }
        ///     MaterialToggleUIDrawer mpud = mph.propertyDrawer as MaterialToggleUIDrawer;
        ///     if (mpud is null)
        ///     {
        ///         throw new ArgumentException("Specified MaterialProperty does not have UnityEditor.MaterialToggleUIDrawer");
        ///     }
        ///     mpud.SetKeyword(prop, isOn);
        /// }
        /// </code>
        /// </summary>
        private static Action<Shader, MaterialProperty, bool> CreateSetKeywordDelegate()
        {
            // Get assembly from public class.
            var asm = Assembly.GetAssembly(typeof(UnityEditor.MaterialPropertyDrawer));

            // Get type of UnityEditor.MaterialPropertyHandler which is the internal class.
            var typeMph = asm.GetType("UnityEditor.MaterialPropertyHandler")
                ?? throw new InvalidOperationException("Type not found: UnityEditor.MaterialPropertyHandler");
            var typeMtud = asm.GetType("UnityEditor.MaterialToggleUIDrawer")
                ?? throw new InvalidOperationException("Type not found: UnityEditor.MaterialToggleUIDrawer");

            var ciArgumentException = typeof(ArgumentException).GetConstructor(new[] {typeof(string)});

            var pShader = Expression.Parameter(typeof(Shader), "shader");
            var pMaterialPropertyHandler = Expression.Parameter(typeMph, "mph");
            var pMaterialToggleUIDrawer = Expression.Parameter(typeMtud, "mtud");
            var pMaterialProperty = Expression.Parameter(typeof(MaterialProperty), "mp");
            var pBool = Expression.Parameter(typeof(bool), "isOn");

            var cNull = Expression.Constant(null);

            return Expression.Lambda<Action<Shader, MaterialProperty, bool>>(
                Expression.Block(
                    new[]
                    {
                        pMaterialPropertyHandler,
                        pMaterialToggleUIDrawer
                    },
                    Expression.Assign(
                        pMaterialPropertyHandler,
                        Expression.Call(
                            typeMph.GetMethod(
                                "GetHandler",
                                BindingFlags.NonPublic
                                    | BindingFlags.Static)
                                ?? throw new InvalidOperationException("MethodInfo not found: UnityEditor.MaterialPropertyHandler.GetHandler"),
                            pShader,
                            Expression.Property(
                                pMaterialProperty,
                                typeof(MaterialProperty).GetProperty(
                                    "name",
                                    BindingFlags.GetProperty
                                        | BindingFlags.Public
                                        | BindingFlags.Instance)))),
                    Expression.IfThen(
                        Expression.Equal(
                            pMaterialPropertyHandler,
                            cNull),
                        Expression.Throw(
                            Expression.New(
                                ciArgumentException,
                                Expression.Constant("Specified MaterialProperty does not have UnityEditor.MaterialPropertyHandler")))),
                    Expression.Assign(
                        pMaterialToggleUIDrawer,
                        Expression.TypeAs(
                            Expression.Property(
                                pMaterialPropertyHandler,
                                typeMph.GetProperty(
                                    "propertyDrawer",
                                    BindingFlags.GetProperty
                                        | BindingFlags.Public
                                        | BindingFlags.Instance)
                                    ?? throw new InvalidOperationException("PropertyInfo not found: UnityEditor.MaterialPropertyHandler.propertyDrawer")),
                            typeMtud)),
                    Expression.IfThen(
                        Expression.Equal(
                            pMaterialToggleUIDrawer,
                            cNull),
                        Expression.Throw(
                            Expression.New(
                                ciArgumentException,
                                Expression.Constant("Specified MaterialProperty does not have UnityEditor.MaterialToggleUIDrawer")))),
                    Expression.Call(
                        pMaterialToggleUIDrawer,
                        typeMtud.GetMethod(
                            "SetKeyword",
                            BindingFlags.NonPublic
                                | BindingFlags.Instance)
                            ?? throw new InvalidOperationException("MethodInfo not found: UnityEditor.MaterialToggleUIDrawer.SetKeyword"),
                        pMaterialProperty,
                        pBool)),
                "SetKeyword",
                new []
                {
                    pShader,
                    pMaterialProperty,
                    pBool
                }).Compile();
        }
    }
}
