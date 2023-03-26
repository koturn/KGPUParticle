Shader "koturn/KGPUParticle/StarStorm"
{
    Properties
    {
        _Color ("Base color", Color) = (1.0, 0.4, 0.4, 1.0)

        _PolygonSize ("Polygon size", Range(0.0, 1.0)) = 0.1

        _Speed ("Speed", Range(0.0, 5.0)) = 1.0

        _SpeedAmp ("Speed amplitude", Range(0.0, 5.0)) = 0.0


        // [Space(16.0)]
        // [Header(Lighting Parameters)]
        // [Space(8.0)]

        [KeywordEnum(Unity Lambert, Unity Blinn Phong, Unity Standard, Unity Standard Specular, Custom)]
        _LightingMethod ("Lighting method", Int) = 2  // Default: Unity Standard

        _Glossiness ("Smoothness", Range(0.0, 1.0)) = 0.5
        _Metallic ("Metallic", Range(0.0, 1.0)) = 0.0

        _SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1.0)
        _SpecPower ("Specular Power", Range(0.0, 128.0)) = 16.0


        // [Space(16.0)]
        // [Header(Rendering Parameters)]
        // [Space(8.0)]

        [Enum(UnityEngine.Rendering.CullMode)]
        _Cull ("Culling Mode", Int) = 0  // Default: Off

        [Toggle(_FLIP_BACKFACE_NORMAL_ON)]
        _FlipBackNormal ("Flip Backface Normal", Int) = 1

        [HideInInspector]
        __RenderingMode ("Rendering Mode", Int) = 0

        // [Toggle(_ALPHATEST_ON)]
        // _AlphaTest ("Alpha Test", Int) = 0

        // _Cutoff ("Alpha Cutoff", Range (0.0, 1.0)) = 0.5

        [Enum(UnityEngine.Rendering.BlendMode)]
        _SrcBlend ("Blend Source Factor", Int) = 1  // Default: One

        [Enum(UnityEngine.Rendering.BlendMode)]
        _DstBlend ("Blend Destination Factor", Int) = 0  // Default: Zero

        [Enum(UnityEngine.Rendering.BlendMode)]
        _SrcBlendAlpha ("Blend Source Factor for Alpha", Int) = 1  // Default: One

        [Enum(UnityEngine.Rendering.BlendMode)]
        _DstBlendAlpha ("Blend Destination Factor for Alpha", Int) = 0  // Default: Zero

        [Enum(UnityEngine.Rendering.BlendOp)]
        _BlendOp ("Blend Operation", Int) = 0  // Default: Add

        [Enum(UnityEngine.Rendering.BlendOp)]
        _BlendOpAlpha ("Blend Operation for Alpha", Int) = 0  // Default: Add

        [Enum(Off, 0, On, 1)]
        _ZWrite ("ZWrite", Int) = 1  // Default: On

        [Enum(UnityEngine.Rendering.CompareFunction)]
        _ZTest ("ZTest", Int) = 4  // Default: LEqual

        [Enum(Off, 0, On, 1)]
        _ZClip ("ZClip", Int) = 1  // Default: On

        [Enum(2D, 0, 3D, 1)]
        _OffsetFactor ("Offset Factor", Int) = 0

        _OffsetUnits ("Offset Units", Range(-100, 100)) = 0

        [ColorMask]
        _ColorMask ("Color Mask", Int) = 15  // Default: RGBA

        [Enum(Off, 0, On, 1)]
        _AlphaToMask ("Alpha To Mask", Int) = 0  // Default: Off


        // [Space(16.0)]
        // [Header(Stencil Parameters)]
        // [Space(8.0)]

        [IntRange]
        _StencilRef ("Stencil Reference Value", Range(0, 255)) = 0

        [IntRange]
        _StencilReadMask ("Stencil ReadMask Value", Range(0, 255)) = 255

        [IntRange]
        _StencilWriteMask ("Stencil WriteMask Value", Range(0, 255)) = 255

        [Enum(UnityEngine.Rendering.CompareFunction)]
        _StencilCompFunc ("Stencil Compare Function", Int) = 8  // Default: Always

        [Enum (UnityEngine.Rendering.StencilOp)]
        _StencilPass ("Stencil Pass", Int) = 0  // Default: Keep

        [Enum(UnityEngine.Rendering.StencilOp)]
        _StencilFail ("Stencil Fail", Int) = 0  // Default: Keep

        [Enum(UnityEngine.Rendering.StencilOp)]
        _StencilZFail ("Stencil ZFail", Int) = 0  // Default: Keep
    }

    // Rendering Mode | RenderQueue          | RenderType          | ZWrite | SrcBlend     | DstBlend              | AlphaTest
    // ---------------|----------------------|---------------------|--------|--------------|-----------------------|----------
    // Opaque         | "Geometry" (2000)    | "Opaque"            | On     | One (1)      | Zero (0)              | x
    // Cutout         | "AlphaTest" (2450)   | "TransparentCutout" | On     | One (1)      | Zero (0)              | o
    // Fade           | "Transparent" (3000) | "Transparent"       | Off    | SrcAlpha (5) | OneMinusSrcAlpha (10) | x
    // Transparent    | "Transparent"        | "Transparent"       | Off    | One (1)      | OneMinusSrcAlpha (10) | x

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Geometry"
            "IgnoreProjector" = "True"
            "VRCFallback" = "Hidden"
        }

        Cull [_Cull]
        ZClip [_ZClip]
        Offset [_OffsetFactor], [_OffsetUnits]
        ColorMask [_ColorMask]
        AlphaToMask [_AlphaToMask]

        Stencil
        {
            Ref [_StencilRef]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
            Comp [_StencilCompFunc]
            Pass [_StencilPass]
            Fail [_StencilFail]
            ZFail [_StencilZFail]
        }

        CGINCLUDE
        #pragma target 4.0

        #include "Include/OptUnityCG.cginc"
        #include "Include/OptUnityStandardUtils.cginc"

        #include "AutoLight.cginc"
        #include "UnityShadowLibrary.cginc"

        #include "Include/LightingUtils.cginc"
        #include "Include/Utils.cginc"

        #if defined(_FLIP_BACKFACE_NORMAL_ON) && !defined(SHADER_API_D3D11_9X)
        #    define FLIP_BACKFACE_NORMAL_ON
        #endif  // defined(_FLIP_BACKFACE_NORMAL_ON) && !defined(SHADER_API_D3D11_9X)


        //! Base color.
        uniform float4 _Color;
        //! Polygon size.
        uniform float _PolygonSize;
        //! Speed.
        uniform float _Speed;
        //! Speed amplitude.
        uniform float _SpeedAmp;


        /*!
         * @brief Input data type for vertex shader function, vert().
         * @see vert
         */
        struct appdata
        {
            //! Object space position of the vertex.
            float4 vertex : POSITION;
        #ifdef LIGHTMAP_ON
            //! Lightmap coordinate.
            float2 texcoord1 : TEXCOORD1;
        #endif  // defined(LIGHTMAP_ON)
        #ifdef DYNAMICLIGHTMAP_ON
            //! Dynamic Lightmap coordinate.
            float2 texcoord2 : TEXCOORD2;
        #endif  // defined(DYNAMICLIGHTMAP_ON)
            //! Vertex ID.
            uint vertexID : SV_VertexID;
        };


        /*!
         * @brief Output data type of vertex shader function, vert()
         * and input data type of geometry shader function, geom().
         * @see vert
         * @see geom
         */
        struct v2g
        {
            //! Object space position of the vertex.
            float4 vertex : SV_POSITION;
        #ifdef LIGHTMAP_ON
            //! Lightmap coordinate.
            float2 texcoord1 : TEXCOORD1;
        #endif  // defined(LIGHTMAP_ON)
        #ifdef DYNAMICLIGHTMAP_ON
            //! Dynamic Lightmap coordinate.
            float2 texcoord2 : TEXCOORD2;
        #endif  // defined(DYNAMICLIGHTMAP_ON)
            //! Vertex ID.
            uint vertexID : TEXCOORD1;
        };


        /*!
         * @brief Output data type of geometry shader function, geom()
         * and input data type of fragment shader function, frag().
         * @see geom
         * @see frag
         */
        struct g2f
        {
            //! Clip space position of the vertex/fragment.
            float4 pos : SV_POSITION;
            //! World space position of the vertex/fragment.
            float3 worldPos: TEXCOORD1;
            //! World space normal of the vertex/fragment.
            nointerpolation float3 normal : TEXCOORD2;
            //! Color of the vertex/fragment.
            nointerpolation float3 color : TEXCOORD3;
        #if defined(LIGHTMAP_ON)
        #    ifdef DYNAMICLIGHTMAP_ON
            //! Lightmap and dynamic lightmap coordinate.
            float4 lmap: TEXCOORD4;
        #    else
            //! Lightmap coordinate.
            float2 lmap: TEXCOORD4;
        #    endif
        #elif defined(UNITY_SHOULD_SAMPLE_SH)
            //! Ambient light color.
            half3 ambient: TEXCOORD4;
        #endif  // UNITY_SHOULD_SAMPLE_SH
            UNITY_LIGHTING_COORDS(5, 6)
            UNITY_FOG_COORDS(7)
        };


        half4 calcLighting(half4 color, float3 worldPos, float3 worldNormal, half atten, float4 lmap, half3 ambient);


        /*!
         * @brief Vertex shader function
         * @param [in] v  Input data
         * @return Input data for geometry shader function, geom().
         * @see geom
         */
        v2g vert(appdata v)
        {
            v2g o;
            o.vertex = v.vertex;
        #ifdef LIGHTMAP_ON
            o.texcoord1 = v.texcoord1;
        #endif  // defined(LIGHTMAP_ON)
        #ifdef DYNAMICLIGHTMAP_ON
            o.texcoord2 = v.texcoord2;
        #endif  // defined(DYNAMICLIGHTMAP_ON)
            o.vertexID = v.vertexID;
            return o;
        }


        /*!
         * @brief Geometry shader function.
         * @param [in] i  Input data from vertex shader function, vert().
         * @param [in] primitiveID  Primitive ID.
         * @param [in,out] outStream  Output stream for fragment shader function, frag().
         * @see vert
         * @see frag
         */
        [maxvertexcount(9)]
        void geom(triangle v2g gi[3], uint primitiveID : SV_PrimitiveID, inout TriangleStream<g2f> outStream)
        {
            const float4 cpos = (gi[0].vertex + gi[1].vertex + gi[2].vertex) * (1.0 / 3.0);
            const float vidSum = (float)(gi[0].vertexID + gi[1].vertexID + gi[2].vertexID);
            const float3 rotAxis = normalize(rand(cpos.xyz, primitiveID, -1.0, 1.0));

            const float3 localNormal = normalize(cross(gi[1].vertex.xyz - gi[0].vertex.xyz, gi[2].vertex.xyz - gi[0].vertex.xyz));
            const float3 baseStarOffset = (gi[0].vertex.xyz - cpos.xyz) * _PolygonSize;
            const float3 subStarOffset = baseStarOffset * ((sqrt(5.0) - 3.0) * 0.5);
            const float3 color = rgbAddHue(_Color.xyz, rand(primitiveID, vidSum));
            const float rotAngle = _Time.y * (_Speed + _SpeedAmp * rand(vidSum, primitiveID, -1.0, 1.0));

            for (int i = 0; i < 3; i++) {
                float3 vertices[3];

                // Base triangle.
                const float rotOffset = (UNITY_PI * 2.0 / 5.0) * i;
                vertices[0] = cpos + rotate3D(baseStarOffset, localNormal, rotOffset);
                vertices[1] = cpos + rotate3D(subStarOffset, localNormal, (UNITY_PI * 2.0 / 5.0) + rotOffset);
                vertices[2] = cpos + rotate3D(baseStarOffset, localNormal, (UNITY_PI * 4.0 / 5.0) + rotOffset);

                // Rotate the triangle around rotAxis.
                vertices[0] = rotate3D(vertices[0], rotAxis, rotAngle);
                vertices[1] = rotate3D(vertices[1], rotAxis, rotAngle);
                vertices[2] = rotate3D(vertices[2], rotAxis, rotAngle);

                const float3 newLocalNormal = normalize(cross(vertices[1] - vertices[0], vertices[2] - vertices[0]));
                const float3 newWorldNormal = UnityObjectToWorldNormal(newLocalNormal);

                UNITY_UNROLL
                for (int j = 0; j < 3; j++) {
                    v2g v = gi[j];
                    const float4 vertex = float4(vertices[j], 1.0);

                    g2f o;
                    UNITY_INITIALIZE_OUTPUT(g2f, o);
                    o.pos = UnityObjectToClipPos(vertex);
                    o.worldPos = mul(unity_ObjectToWorld, vertex).xyz;
                    o.normal = newWorldNormal;
                    o.color = color;

        #if defined(LIGHTMAP_ON)
                    o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
        #    ifdef DYNAMICLIGHTMAP_ON
                    o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
        #    endif  // DYNAMICLIGHTMAP_ON
        #elif UNITY_SHOULD_SAMPLE_SH
        #    ifdef VERTEXLIGHT_ON
                    // Approximated illumination from non-important point lights
                    o.ambient.rgb = Shade4PointLights(
                        unity_4LightPosX0,
                        unity_4LightPosY0,
                        unity_4LightPosZ0,
                        unity_LightColor[0].rgb,
                        unity_LightColor[1].rgb,
                        unity_LightColor[2].rgb,
                        unity_LightColor[3].rgb,
                        unity_4LightAtten0,
                        o.worldPos,
                        o.normal);
        #    endif  // VERTEXLIGHT_ON
                    o.ambient.rgb = ShadeSHPerVertex(o.normal, o.ambient.rgb);
        #endif  // VERTEXLIGHT_ON

                    UNITY_TRANSFER_FOG(o, o.pos);
                    outStream.Append(o);
                }
                outStream.RestartStrip();
            }
        }


        #ifdef FLIP_BACKFACE_NORMAL_ON
        /*!
         * @brief Fragment shader function.
         * @param [in] fi  Input data from geometry shader.
         * @param [in] face  Facing parameter.
         * @return Color of the fragment.
         * @see geom
         */
        half4 frag(g2f fi, FaceType face : FACE_SEMANTICS) : SV_Target
        #else
        /*!
         * @brief Fragment shader function.
         * @param [in] fi  Input data from vertex shader.
         * @return Color of the fragment.
         * @see geom
         */
        half4 frag(g2f fi) : SV_Target
        #endif  // defined(FLIP_BACKFACE_NORMAL_ON)
        {
        #ifdef FLIP_BACKFACE_NORMAL_ON
            fi.normal = isFacing(face) ? fi.normal : -fi.normal;
        #endif  // defined(FLIP_BACKFACE_NORMAL_ON)

        #if defined(LIGHTMAP_ON)
        #    ifdef DYNAMICLIGHTMAP_ON
            const float4 lmap = fi.lmap;
        #    else
            const float4 lmap = float4(fi.lmap, 0.0, 0.0);
        #    endif  // DYNAMICLIGHTMAP_ON
            const half3 ambient = half3(0.0, 0.0, 0.0);
        #elif defined(UNITY_SHOULD_SAMPLE_SH)
            const float4 lmap = float4(0.0, 0.0, 0.0, 0.0);
            const half3 ambient = fi.ambient;
        #else
            const float4 lmap = float4(0.0, 0.0, 0.0, 0.0);
            const half3 ambient = half3(0.0, 0.0, 0.0);
        #endif  // defined(LIGHTMAP_ON)

            UNITY_LIGHT_ATTENUATION(atten, fi, fi.worldPos);

            half4 color = calcLighting(
                half4(fi.color, 1.0),
                fi.worldPos,
                fi.normal,
                atten,
                lmap,
                ambient);

            UNITY_APPLY_FOG(i.fogCoord, color);
            return color;
        }


        /*!
         * Calculate lighting.
         * @param [in] color  Base color.
         * @param [in] worldPos  World coordinate.
         * @param [in] worldNormal  Normal in world space.
         * @param [in] atten  Light attenuation.
         * @param [in] lmap  Light map parameters.
         * @param [in] ambient  Ambient light color.
         * @return Color with lighting applied.
         */
        half4 calcLighting(half4 color, float3 worldPos, float3 worldNormal, half atten, float4 lmap, half3 ambient)
        {
        #if defined(_LIGHTINGMETHOD_UNITY_LAMBERT)
            return calcLightingUnityLambert(color, worldPos, worldNormal, atten, lmap, ambient);
        #elif defined(_LIGHTINGMETHOD_UNITY_BLINN_PHONG)
            return calcLightingUnityBlinnPhong(color, worldPos, worldNormal, atten, lmap, ambient);
        #elif defined(_LIGHTINGMETHOD_UNITY_STANDARD)
            return calcLightingUnityStandard(color, worldPos, worldNormal, atten, lmap, ambient);
        #elif defined(_LIGHTINGMETHOD_UNITY_STANDARD_SPECULAR)
            return calcLightingUnityStandardSpecular(color, worldPos, worldNormal, atten, lmap, ambient);
        #else
            // Unlit
            return color;
        #endif  // defined(_LIGHTINGMETHOD_LAMBERT)
        }
        ENDCG


        Pass
        {
            Name "FORWARD_BASE"
            Tags
            {
                "LightMode" = "ForwardBase"
            }

            Blend [_SrcBlend] [_DstBlend], [_SrcBlendAlpha] [_DstBlendAlpha]
            BlendOp [_BlendOp], [_BlendOpAlpha]
            ZWrite [_ZWrite]
            ZTest [_ZTest]

            CGPROGRAM
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma shader_feature_local_fragment _ _FLIP_BACKFACE_NORMAL_ON
            #pragma shader_feature_local_fragment _LIGHTINGMETHOD_UNITY_LAMBERT _LIGHTINGMETHOD_UNITY_BLINN_PHONG _LIGHTINGMETHOD_UNITY_STANDARD _LIGHTINGMETHOD_UNITY_STANDARD_SPECULAR _LIGHTINGMETHOD_CUSTOM

            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            ENDCG
        }

        Pass
        {
            Name "FORWARD_ADD"
            Tags
            {
                "LightMode" = "ForwardAdd"
            }

            Blend [_SrcBlend] One, [_SrcBlendAlpha] One
            BlendOp [_BlendOp], [_BlendOpAlpha]
            ZWrite Off
            ZTest LEqual

            CGPROGRAM
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma shader_feature_local_fragment _ _FLIP_BACKFACE_NORMAL_ON
            #pragma shader_feature_local_fragment _LIGHTINGMETHOD_UNITY_LAMBERT _LIGHTINGMETHOD_UNITY_BLINN_PHONG _LIGHTINGMETHOD_UNITY_STANDARD _LIGHTINGMETHOD_UNITY_STANDARD_SPECULAR _LIGHTINGMETHOD_CUSTOM

            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            ENDCG
        }

        Pass
        {
            Name "SHADOW_CASTER"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            Blend One Zero
            BlendOp Add
            ZWrite On
            ZTest LEqual

            CGPROGRAM
            #pragma multi_compile_shadowcaster

            #pragma vertex vertShadowCaster
            #pragma geometry geomShadowCaster
            #pragma fragment fragShadowCaster


            /*!
             * @brief Input of the vertex shader, vertShadowCaster().
             * @see vertShadowCaster
             */
            struct appdata_shadowcaster
            {
                //! Object space position of the vertex.
                float4 vertex : POSITION;
                //! Vertex ID.
                uint vertexID : SV_VertexID;
            };


            /*!
             * @brief Output of the vertex shader, vertShadowCaster()
             * and input of the geometry shader, geomShadowCaster().
             * @see vertShadowCaster
             * @see geomShadowCaster
             */
            struct v2g_shadowcaster
            {
                //! Object space position of the vertex.
                float4 vertex : SV_POSITION;
                //! Vertex ID.
                uint vertexID : TEXCOORD0;
            };


            /*!
             * @brief Output of the geometry shader, geomShadowCaster()
             * and input of the fragment shader, fragShadowCaster().
             * @see geomShadowCaster
             * @see fragShadowCaster
             */
            struct g2f_shadowcaster
            {
                //
                // V2F_SHADOW_CASTER;
                //
                //! Clip space position.
                float4 pos : SV_POSITION;
            #if defined(SHADOWS_CUBE) && !defined(SHADOWS_CUBE_IN_DEPTH_TEX)
                //! World space light vector.
                float3 vec : TEXCOORD0;
            #endif  // defined(SHADOWS_CUBE) && !defined(SHADOWS_CUBE_IN_DEPTH_TEX)
            };


            /*!
             * @brief Vertex shader function for ShadowCaster Pass.
             * @param [in] v  Input data
             * @return Output for fragment shader.
             */
            v2g_shadowcaster vertShadowCaster(appdata_shadowcaster v)
            {
                v2g_shadowcaster o;
                o.vertex = v.vertex;
                o.vertexID = v.vertexID;

                return o;
            }


            /*!
             * @brief Vertex shader function for ShadowCaster Pass.
             * @param [in] v  Input data
             * @return Output for fragment shader.
             */
            [maxvertexcount(9)]
            void geomShadowCaster(triangle v2g_shadowcaster gi[3], uint primitiveID : SV_PrimitiveID, inout TriangleStream<g2f_shadowcaster> outStream)
            {
                const float4 cpos = (gi[0].vertex + gi[1].vertex + gi[2].vertex) * (1.0 / 3.0);
                const float vidSum = (float)(gi[0].vertexID + gi[1].vertexID + gi[2].vertexID);
                const float3 rotAxis = normalize(rand(cpos.xyz, primitiveID, -1.0, 1.0));

                const float3 localNormal = normalize(cross(gi[1].vertex.xyz - gi[0].vertex.xyz, gi[2].vertex.xyz - gi[0].vertex.xyz));
                const float3 baseStarOffset = (gi[0].vertex.xyz - cpos.xyz) * _PolygonSize;
                const float3 subStarOffset = baseStarOffset * ((sqrt(5.0) - 3.0) * 0.5);
                const float rotAngle = _Time.y * (_Speed + _SpeedAmp * rand(vidSum, primitiveID, -1.0, 1.0));

                for (int i = 0; i < 3; i++) {
                    float3 vertices[3];

                    // Base triangle.
                    const float rotOffset = (UNITY_PI * 2.0 / 5.0) * i;
                    vertices[0] = cpos + rotate3D(baseStarOffset, localNormal, rotOffset);
                    vertices[1] = cpos + rotate3D(subStarOffset, localNormal, (UNITY_PI * 2.0 / 5.0) + rotOffset);
                    vertices[2] = cpos + rotate3D(baseStarOffset, localNormal, (UNITY_PI * 4.0 / 5.0) + rotOffset);

                    // Rotate the triangle around rotAxis.
                    vertices[0] = rotate3D(vertices[0], rotAxis, rotAngle);
                    vertices[1] = rotate3D(vertices[1], rotAxis, rotAngle);
                    vertices[2] = rotate3D(vertices[2], rotAxis, rotAngle);

                    const float3 newLocalNormal = normalize(cross(vertices[1] - vertices[0], vertices[2] - vertices[0]));
                    const float3 newWorldNormal = UnityObjectToWorldNormal(newLocalNormal);

                    UNITY_UNROLL
                    for (int j = 0; j < 3; j++) {
                        const float4 vertex = float4(vertices[j], 1.0);

                        g2f_shadowcaster o;
                        //
                        // TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                        //
            #if defined(SHADOWS_CUBE) && !defined(SHADOWS_CUBE_IN_DEPTH_TEX)
                        o.vec = mul(unity_ObjectToWorld, vertex).xyz - _LightPositionRange.xyz;
                        o.pos = UnityObjectToClipPos(vertex);
            #else
                        o.pos = UnityClipSpaceShadowCasterPos(vertex, newLocalNormal);
                        o.pos = UnityApplyLinearShadowBias(o.pos);
            #endif  // defined(SHADOWS_CUBE) && !defined(SHADOWS_CUBE_IN_DEPTH_TEX)
                        outStream.Append(o);
                    }
                    outStream.RestartStrip();
                }
            }


            /*!
             * @brief Fragment shader function for ShadowCaster Pass.
             * @param [in] fi  Input data from vertex shader.
             * @return Color of the fragment.
             */
            fixed4 fragShadowCaster(g2f_shadowcaster fi) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(fi)
            }
            ENDCG
        }
    }

    // Fallback "Diffuse"
    // Fallback "Transparent/Diffuse"
    // Fallback "Transparent/Cutout/Diffuse"
    CustomEditor "Koturn.KGPUParticle.Inspectors.StarStormGUI"
}
