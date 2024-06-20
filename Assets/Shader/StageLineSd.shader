Shader "Shader Graphs/StageLine_ShaderGraph"
{
    Properties
    {
        _ScrollTime("ScrollTime", Float) = -0.05
        _Color("Color", Color) = (1, 1, 1, 0)
        [NoScaleOffset]_Sprite_ColorMap("Sprite_ColorMap", 2D) = "white" {}
        [NoScaleOffset]_Sprite_Main("Sprite_Main", 2D) = "white" {}
        [NoScaleOffset]_Sprite_Line("Sprite_Line", 2D) = "white" {}
        [HideInInspector]_QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector]_QueueControl("_QueueControl", Float) = -1
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
        SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue" = "Transparent"
            "DisableBatching" = "False"
            "ShaderGraphShader" = "true"
            "ShaderGraphTargetId" = "UniversalUnlitSubTarget"
        }
        Pass
        {
            Name "Universal Forward"
            Tags
            {
            // LightMode: <None>
        }

        // Render State
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 2.0
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma instancing_options renderinglayer
        #pragma vertex vert
        #pragma fragment frag

        // Keywords
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma shader_feature _ _SAMPLE_GI
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
        // GraphKeywords: <None>

        // Defines

        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_UNLIT
        #define _FOG_FRAGMENT 1
        #define _SURFACE_TYPE_TRANSPARENT 1
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

        // --------------------------------------------------
        // Structs and Packing

        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float3 positionWS : INTERP1;
             float3 normalWS : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

        PackedVaryings PackVaryings(Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.positionWS.xyz = input.positionWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

        Varyings UnpackVaryings(PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.positionWS = input.positionWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }


        // --------------------------------------------------
        // Graph

        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _ScrollTime;
        float4 _Color;
        float4 _Sprite_Main_TexelSize;
        float4 _Sprite_Line_TexelSize;
        float4 _Sprite_ColorMap_TexelSize;
        CBUFFER_END


            // Object and Global properties
            SAMPLER(SamplerState_Linear_Repeat);
            TEXTURE2D(_Sprite_Main);
            SAMPLER(sampler_Sprite_Main);
            TEXTURE2D(_Sprite_Line);
            SAMPLER(sampler_Sprite_Line);
            TEXTURE2D(_Sprite_ColorMap);
            SAMPLER(sampler_Sprite_ColorMap);

            // Graph Includes
            // GraphIncludes: <None>

            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif

            // Graph Functions

            void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
            {
                Out = A * B;
            }

            void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
            {
                Out = A * B;
            }

            void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
            {
                Out = UV * Tiling + Offset;
            }

            void Unity_Add_float(float A, float B, out float Out)
            {
                Out = A + B;
            }

            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                description.Position = IN.ObjectSpacePosition;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;
                return description;
            }

            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
            return output;
            }
            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif

            // Graph Pixel
            struct SurfaceDescription
            {
                float3 BaseColor;
                float Alpha;
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                UnityTexture2D _Property_306c239033e04707a0f1a335c4cffdcf_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Sprite_ColorMap);
                float4 _SampleTexture2D_8160719f218241dabbe321661faaf004_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_306c239033e04707a0f1a335c4cffdcf_Out_0_Texture2D.tex, _Property_306c239033e04707a0f1a335c4cffdcf_Out_0_Texture2D.samplerstate, _Property_306c239033e04707a0f1a335c4cffdcf_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_8160719f218241dabbe321661faaf004_R_4_Float = _SampleTexture2D_8160719f218241dabbe321661faaf004_RGBA_0_Vector4.r;
                float _SampleTexture2D_8160719f218241dabbe321661faaf004_G_5_Float = _SampleTexture2D_8160719f218241dabbe321661faaf004_RGBA_0_Vector4.g;
                float _SampleTexture2D_8160719f218241dabbe321661faaf004_B_6_Float = _SampleTexture2D_8160719f218241dabbe321661faaf004_RGBA_0_Vector4.b;
                float _SampleTexture2D_8160719f218241dabbe321661faaf004_A_7_Float = _SampleTexture2D_8160719f218241dabbe321661faaf004_RGBA_0_Vector4.a;
                float4 _Property_8be6b4469f814ada80c8fe4de7ccc50a_Out_0_Vector4 = _Color;
                float4 _Multiply_543f735c64354421a5153ca645342e95_Out_2_Vector4;
                Unity_Multiply_float4_float4(_SampleTexture2D_8160719f218241dabbe321661faaf004_RGBA_0_Vector4, _Property_8be6b4469f814ada80c8fe4de7ccc50a_Out_0_Vector4, _Multiply_543f735c64354421a5153ca645342e95_Out_2_Vector4);
                UnityTexture2D _Property_84ab372623ff4f098d6988634f5e924c_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Sprite_Main);
                float4 _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_84ab372623ff4f098d6988634f5e924c_Out_0_Texture2D.tex, _Property_84ab372623ff4f098d6988634f5e924c_Out_0_Texture2D.samplerstate, _Property_84ab372623ff4f098d6988634f5e924c_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy));
                _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4.rgb = UnpackNormal(_SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4);
                float _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_R_4_Float = _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4.r;
                float _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_G_5_Float = _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4.g;
                float _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_B_6_Float = _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4.b;
                float _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_A_7_Float = _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4.a;
                UnityTexture2D _Property_3d4c2164e736405faa39cbe26f58ef77_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Sprite_Line);
                float _Property_844d4ae06c6a40c08a85e6074cf895e8_Out_0_Float = _ScrollTime;
                float2 _Vector2_2ccdec8b6c8247ffae430506ca119d8b_Out_0_Vector2 = float2(0, _Property_844d4ae06c6a40c08a85e6074cf895e8_Out_0_Float);
                float2 _Multiply_26470bc1dde54e298caa7dcbc3d14e98_Out_2_Vector2;
                Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_2ccdec8b6c8247ffae430506ca119d8b_Out_0_Vector2, _Multiply_26470bc1dde54e298caa7dcbc3d14e98_Out_2_Vector2);
                float2 _TilingAndOffset_8557ed2455574bfe85b924e3f527b989_Out_3_Vector2;
                Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Multiply_26470bc1dde54e298caa7dcbc3d14e98_Out_2_Vector2, _TilingAndOffset_8557ed2455574bfe85b924e3f527b989_Out_3_Vector2);
                float4 _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_3d4c2164e736405faa39cbe26f58ef77_Out_0_Texture2D.tex, _Property_3d4c2164e736405faa39cbe26f58ef77_Out_0_Texture2D.samplerstate, _Property_3d4c2164e736405faa39cbe26f58ef77_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_8557ed2455574bfe85b924e3f527b989_Out_3_Vector2));
                _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4.rgb = UnpackNormal(_SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4);
                float _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_R_4_Float = _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4.r;
                float _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_G_5_Float = _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4.g;
                float _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_B_6_Float = _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4.b;
                float _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_A_7_Float = _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4.a;
                float _Add_7c0aeeb4a1ea410faadf3e5fcabb7391_Out_2_Float;
                Unity_Add_float(_SampleTexture2D_c6ff558bc98048588292232098ae0d9d_A_7_Float, _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_A_7_Float, _Add_7c0aeeb4a1ea410faadf3e5fcabb7391_Out_2_Float);
                surface.BaseColor = (_Multiply_543f735c64354421a5153ca645342e95_Out_2_Vector4.xyz);
                surface.Alpha = _Add_7c0aeeb4a1ea410faadf3e5fcabb7391_Out_2_Float;
                return surface;
            }

            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                output.ObjectSpaceNormal = input.normalOS;
                output.ObjectSpaceTangent = input.tangentOS.xyz;
                output.ObjectSpacePosition = input.positionOS;

                return output;
            }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

            #ifdef HAVE_VFX_MODIFICATION
            #if VFX_USE_GRAPH_VALUES
                uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
            #endif
                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

            #endif








                #if UNITY_UV_STARTS_AT_TOP
                #else
                #endif


                output.uv0 = input.texCoord0;
                output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
            #else
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            #endif
            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                    return output;
            }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"

            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif

            ENDHLSL
            }
            Pass
            {
                Name "DepthNormalsOnly"
                Tags
                {
                    "LightMode" = "DepthNormalsOnly"
                }

                // Render State
                Cull Back
                ZTest LEqual
                ZWrite On

                // Debug
                // <None>

                // --------------------------------------------------
                // Pass

                HLSLPROGRAM

                // Pragmas
                #pragma target 2.0
                #pragma multi_compile_instancing
                #pragma vertex vert
                #pragma fragment frag

                // Keywords
                #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
                // GraphKeywords: <None>

                // Defines

                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define ATTRIBUTES_NEED_TEXCOORD0
                #define VARYINGS_NEED_NORMAL_WS
                #define VARYINGS_NEED_TEXCOORD0
                #define FEATURES_GRAPH_VERTEX
                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
                #define _SURFACE_TYPE_TRANSPARENT 1
                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                // custom interpolator pre-include
                /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                // Includes
                #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
                #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                // --------------------------------------------------
                // Structs and Packing

                // custom interpolators pre packing
                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                struct Attributes
                {
                     float3 positionOS : POSITION;
                     float3 normalOS : NORMAL;
                     float4 tangentOS : TANGENT;
                     float4 uv0 : TEXCOORD0;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                     float4 positionCS : SV_POSITION;
                     float3 normalWS;
                     float4 texCoord0;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                     float4 uv0;
                     float3 TimeParameters;
                };
                struct VertexDescriptionInputs
                {
                     float3 ObjectSpaceNormal;
                     float3 ObjectSpaceTangent;
                     float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                     float4 positionCS : SV_POSITION;
                     float4 texCoord0 : INTERP0;
                     float3 normalWS : INTERP1;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };

                PackedVaryings PackVaryings(Varyings input)
                {
                    PackedVaryings output;
                    ZERO_INITIALIZE(PackedVaryings, output);
                    output.positionCS = input.positionCS;
                    output.texCoord0.xyzw = input.texCoord0;
                    output.normalWS.xyz = input.normalWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }

                Varyings UnpackVaryings(PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.texCoord0 = input.texCoord0.xyzw;
                    output.normalWS = input.normalWS.xyz;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }


                // --------------------------------------------------
                // Graph

                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float _ScrollTime;
                float4 _Color;
                float4 _Sprite_Main_TexelSize;
                float4 _Sprite_Line_TexelSize;
                float4 _Sprite_ColorMap_TexelSize;
                CBUFFER_END


                    // Object and Global properties
                    SAMPLER(SamplerState_Linear_Repeat);
                    TEXTURE2D(_Sprite_Main);
                    SAMPLER(sampler_Sprite_Main);
                    TEXTURE2D(_Sprite_Line);
                    SAMPLER(sampler_Sprite_Line);
                    TEXTURE2D(_Sprite_ColorMap);
                    SAMPLER(sampler_Sprite_ColorMap);

                    // Graph Includes
                    // GraphIncludes: <None>

                    // -- Property used by ScenePickingPass
                    #ifdef SCENEPICKINGPASS
                    float4 _SelectionID;
                    #endif

                    // -- Properties used by SceneSelectionPass
                    #ifdef SCENESELECTIONPASS
                    int _ObjectId;
                    int _PassValue;
                    #endif

                    // Graph Functions

                    void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
                    {
                        Out = A * B;
                    }

                    void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
                    {
                        Out = UV * Tiling + Offset;
                    }

                    void Unity_Add_float(float A, float B, out float Out)
                    {
                        Out = A + B;
                    }

                    // Custom interpolators pre vertex
                    /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                    // Graph Vertex
                    struct VertexDescription
                    {
                        float3 Position;
                        float3 Normal;
                        float3 Tangent;
                    };

                    VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                    {
                        VertexDescription description = (VertexDescription)0;
                        description.Position = IN.ObjectSpacePosition;
                        description.Normal = IN.ObjectSpaceNormal;
                        description.Tangent = IN.ObjectSpaceTangent;
                        return description;
                    }

                    // Custom interpolators, pre surface
                    #ifdef FEATURES_GRAPH_VERTEX
                    Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                    {
                    return output;
                    }
                    #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                    #endif

                    // Graph Pixel
                    struct SurfaceDescription
                    {
                        float Alpha;
                    };

                    SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                    {
                        SurfaceDescription surface = (SurfaceDescription)0;
                        UnityTexture2D _Property_84ab372623ff4f098d6988634f5e924c_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Sprite_Main);
                        float4 _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_84ab372623ff4f098d6988634f5e924c_Out_0_Texture2D.tex, _Property_84ab372623ff4f098d6988634f5e924c_Out_0_Texture2D.samplerstate, _Property_84ab372623ff4f098d6988634f5e924c_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy));
                        _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4.rgb = UnpackNormal(_SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4);
                        float _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_R_4_Float = _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4.r;
                        float _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_G_5_Float = _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4.g;
                        float _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_B_6_Float = _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4.b;
                        float _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_A_7_Float = _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4.a;
                        UnityTexture2D _Property_3d4c2164e736405faa39cbe26f58ef77_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Sprite_Line);
                        float _Property_844d4ae06c6a40c08a85e6074cf895e8_Out_0_Float = _ScrollTime;
                        float2 _Vector2_2ccdec8b6c8247ffae430506ca119d8b_Out_0_Vector2 = float2(0, _Property_844d4ae06c6a40c08a85e6074cf895e8_Out_0_Float);
                        float2 _Multiply_26470bc1dde54e298caa7dcbc3d14e98_Out_2_Vector2;
                        Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_2ccdec8b6c8247ffae430506ca119d8b_Out_0_Vector2, _Multiply_26470bc1dde54e298caa7dcbc3d14e98_Out_2_Vector2);
                        float2 _TilingAndOffset_8557ed2455574bfe85b924e3f527b989_Out_3_Vector2;
                        Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Multiply_26470bc1dde54e298caa7dcbc3d14e98_Out_2_Vector2, _TilingAndOffset_8557ed2455574bfe85b924e3f527b989_Out_3_Vector2);
                        float4 _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_3d4c2164e736405faa39cbe26f58ef77_Out_0_Texture2D.tex, _Property_3d4c2164e736405faa39cbe26f58ef77_Out_0_Texture2D.samplerstate, _Property_3d4c2164e736405faa39cbe26f58ef77_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_8557ed2455574bfe85b924e3f527b989_Out_3_Vector2));
                        _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4.rgb = UnpackNormal(_SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4);
                        float _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_R_4_Float = _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4.r;
                        float _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_G_5_Float = _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4.g;
                        float _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_B_6_Float = _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4.b;
                        float _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_A_7_Float = _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4.a;
                        float _Add_7c0aeeb4a1ea410faadf3e5fcabb7391_Out_2_Float;
                        Unity_Add_float(_SampleTexture2D_c6ff558bc98048588292232098ae0d9d_A_7_Float, _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_A_7_Float, _Add_7c0aeeb4a1ea410faadf3e5fcabb7391_Out_2_Float);
                        surface.Alpha = _Add_7c0aeeb4a1ea410faadf3e5fcabb7391_Out_2_Float;
                        return surface;
                    }

                    // --------------------------------------------------
                    // Build Graph Inputs
                    #ifdef HAVE_VFX_MODIFICATION
                    #define VFX_SRP_ATTRIBUTES Attributes
                    #define VFX_SRP_VARYINGS Varyings
                    #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                    #endif
                    VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                    {
                        VertexDescriptionInputs output;
                        ZERO_INITIALIZE(VertexDescriptionInputs, output);

                        output.ObjectSpaceNormal = input.normalOS;
                        output.ObjectSpaceTangent = input.tangentOS.xyz;
                        output.ObjectSpacePosition = input.positionOS;

                        return output;
                    }
                    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                    {
                        SurfaceDescriptionInputs output;
                        ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                    #ifdef HAVE_VFX_MODIFICATION
                    #if VFX_USE_GRAPH_VALUES
                        uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                        /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                    #endif
                        /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                    #endif








                        #if UNITY_UV_STARTS_AT_TOP
                        #else
                        #endif


                        output.uv0 = input.texCoord0;
                        output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                    #else
                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                    #endif
                    #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                            return output;
                    }

                    // --------------------------------------------------
                    // Main

                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"

                    // --------------------------------------------------
                    // Visual Effect Vertex Invocations
                    #ifdef HAVE_VFX_MODIFICATION
                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                    #endif

                    ENDHLSL
                    }
                    Pass
                    {
                        Name "ShadowCaster"
                        Tags
                        {
                            "LightMode" = "ShadowCaster"
                        }

                        // Render State
                        Cull Back
                        ZTest LEqual
                        ZWrite On
                        ColorMask 0

                        // Debug
                        // <None>

                        // --------------------------------------------------
                        // Pass

                        HLSLPROGRAM

                        // Pragmas
                        #pragma target 2.0
                        #pragma multi_compile_instancing
                        #pragma vertex vert
                        #pragma fragment frag

                        // Keywords
                        #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
                        // GraphKeywords: <None>

                        // Defines

                        #define ATTRIBUTES_NEED_NORMAL
                        #define ATTRIBUTES_NEED_TANGENT
                        #define ATTRIBUTES_NEED_TEXCOORD0
                        #define VARYINGS_NEED_NORMAL_WS
                        #define VARYINGS_NEED_TEXCOORD0
                        #define FEATURES_GRAPH_VERTEX
                        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                        #define SHADERPASS SHADERPASS_SHADOWCASTER
                        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                        // custom interpolator pre-include
                        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                        // Includes
                        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                        // --------------------------------------------------
                        // Structs and Packing

                        // custom interpolators pre packing
                        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                        struct Attributes
                        {
                             float3 positionOS : POSITION;
                             float3 normalOS : NORMAL;
                             float4 tangentOS : TANGENT;
                             float4 uv0 : TEXCOORD0;
                            #if UNITY_ANY_INSTANCING_ENABLED
                             uint instanceID : INSTANCEID_SEMANTIC;
                            #endif
                        };
                        struct Varyings
                        {
                             float4 positionCS : SV_POSITION;
                             float3 normalWS;
                             float4 texCoord0;
                            #if UNITY_ANY_INSTANCING_ENABLED
                             uint instanceID : CUSTOM_INSTANCE_ID;
                            #endif
                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                            #endif
                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                            #endif
                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                            #endif
                        };
                        struct SurfaceDescriptionInputs
                        {
                             float4 uv0;
                             float3 TimeParameters;
                        };
                        struct VertexDescriptionInputs
                        {
                             float3 ObjectSpaceNormal;
                             float3 ObjectSpaceTangent;
                             float3 ObjectSpacePosition;
                        };
                        struct PackedVaryings
                        {
                             float4 positionCS : SV_POSITION;
                             float4 texCoord0 : INTERP0;
                             float3 normalWS : INTERP1;
                            #if UNITY_ANY_INSTANCING_ENABLED
                             uint instanceID : CUSTOM_INSTANCE_ID;
                            #endif
                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                            #endif
                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                            #endif
                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                            #endif
                        };

                        PackedVaryings PackVaryings(Varyings input)
                        {
                            PackedVaryings output;
                            ZERO_INITIALIZE(PackedVaryings, output);
                            output.positionCS = input.positionCS;
                            output.texCoord0.xyzw = input.texCoord0;
                            output.normalWS.xyz = input.normalWS;
                            #if UNITY_ANY_INSTANCING_ENABLED
                            output.instanceID = input.instanceID;
                            #endif
                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                            #endif
                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                            #endif
                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                            output.cullFace = input.cullFace;
                            #endif
                            return output;
                        }

                        Varyings UnpackVaryings(PackedVaryings input)
                        {
                            Varyings output;
                            output.positionCS = input.positionCS;
                            output.texCoord0 = input.texCoord0.xyzw;
                            output.normalWS = input.normalWS.xyz;
                            #if UNITY_ANY_INSTANCING_ENABLED
                            output.instanceID = input.instanceID;
                            #endif
                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                            #endif
                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                            #endif
                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                            output.cullFace = input.cullFace;
                            #endif
                            return output;
                        }


                        // --------------------------------------------------
                        // Graph

                        // Graph Properties
                        CBUFFER_START(UnityPerMaterial)
                        float _ScrollTime;
                        float4 _Color;
                        float4 _Sprite_Main_TexelSize;
                        float4 _Sprite_Line_TexelSize;
                        float4 _Sprite_ColorMap_TexelSize;
                        CBUFFER_END


                            // Object and Global properties
                            SAMPLER(SamplerState_Linear_Repeat);
                            TEXTURE2D(_Sprite_Main);
                            SAMPLER(sampler_Sprite_Main);
                            TEXTURE2D(_Sprite_Line);
                            SAMPLER(sampler_Sprite_Line);
                            TEXTURE2D(_Sprite_ColorMap);
                            SAMPLER(sampler_Sprite_ColorMap);

                            // Graph Includes
                            // GraphIncludes: <None>

                            // -- Property used by ScenePickingPass
                            #ifdef SCENEPICKINGPASS
                            float4 _SelectionID;
                            #endif

                            // -- Properties used by SceneSelectionPass
                            #ifdef SCENESELECTIONPASS
                            int _ObjectId;
                            int _PassValue;
                            #endif

                            // Graph Functions

                            void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
                            {
                                Out = A * B;
                            }

                            void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
                            {
                                Out = UV * Tiling + Offset;
                            }

                            void Unity_Add_float(float A, float B, out float Out)
                            {
                                Out = A + B;
                            }

                            // Custom interpolators pre vertex
                            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                            // Graph Vertex
                            struct VertexDescription
                            {
                                float3 Position;
                                float3 Normal;
                                float3 Tangent;
                            };

                            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                            {
                                VertexDescription description = (VertexDescription)0;
                                description.Position = IN.ObjectSpacePosition;
                                description.Normal = IN.ObjectSpaceNormal;
                                description.Tangent = IN.ObjectSpaceTangent;
                                return description;
                            }

                            // Custom interpolators, pre surface
                            #ifdef FEATURES_GRAPH_VERTEX
                            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                            {
                            return output;
                            }
                            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                            #endif

                            // Graph Pixel
                            struct SurfaceDescription
                            {
                                float Alpha;
                            };

                            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                            {
                                SurfaceDescription surface = (SurfaceDescription)0;
                                UnityTexture2D _Property_84ab372623ff4f098d6988634f5e924c_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Sprite_Main);
                                float4 _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_84ab372623ff4f098d6988634f5e924c_Out_0_Texture2D.tex, _Property_84ab372623ff4f098d6988634f5e924c_Out_0_Texture2D.samplerstate, _Property_84ab372623ff4f098d6988634f5e924c_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy));
                                _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4.rgb = UnpackNormal(_SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4);
                                float _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_R_4_Float = _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4.r;
                                float _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_G_5_Float = _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4.g;
                                float _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_B_6_Float = _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4.b;
                                float _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_A_7_Float = _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4.a;
                                UnityTexture2D _Property_3d4c2164e736405faa39cbe26f58ef77_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Sprite_Line);
                                float _Property_844d4ae06c6a40c08a85e6074cf895e8_Out_0_Float = _ScrollTime;
                                float2 _Vector2_2ccdec8b6c8247ffae430506ca119d8b_Out_0_Vector2 = float2(0, _Property_844d4ae06c6a40c08a85e6074cf895e8_Out_0_Float);
                                float2 _Multiply_26470bc1dde54e298caa7dcbc3d14e98_Out_2_Vector2;
                                Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_2ccdec8b6c8247ffae430506ca119d8b_Out_0_Vector2, _Multiply_26470bc1dde54e298caa7dcbc3d14e98_Out_2_Vector2);
                                float2 _TilingAndOffset_8557ed2455574bfe85b924e3f527b989_Out_3_Vector2;
                                Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Multiply_26470bc1dde54e298caa7dcbc3d14e98_Out_2_Vector2, _TilingAndOffset_8557ed2455574bfe85b924e3f527b989_Out_3_Vector2);
                                float4 _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_3d4c2164e736405faa39cbe26f58ef77_Out_0_Texture2D.tex, _Property_3d4c2164e736405faa39cbe26f58ef77_Out_0_Texture2D.samplerstate, _Property_3d4c2164e736405faa39cbe26f58ef77_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_8557ed2455574bfe85b924e3f527b989_Out_3_Vector2));
                                _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4.rgb = UnpackNormal(_SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4);
                                float _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_R_4_Float = _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4.r;
                                float _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_G_5_Float = _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4.g;
                                float _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_B_6_Float = _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4.b;
                                float _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_A_7_Float = _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4.a;
                                float _Add_7c0aeeb4a1ea410faadf3e5fcabb7391_Out_2_Float;
                                Unity_Add_float(_SampleTexture2D_c6ff558bc98048588292232098ae0d9d_A_7_Float, _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_A_7_Float, _Add_7c0aeeb4a1ea410faadf3e5fcabb7391_Out_2_Float);
                                surface.Alpha = _Add_7c0aeeb4a1ea410faadf3e5fcabb7391_Out_2_Float;
                                return surface;
                            }

                            // --------------------------------------------------
                            // Build Graph Inputs
                            #ifdef HAVE_VFX_MODIFICATION
                            #define VFX_SRP_ATTRIBUTES Attributes
                            #define VFX_SRP_VARYINGS Varyings
                            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                            #endif
                            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                            {
                                VertexDescriptionInputs output;
                                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                output.ObjectSpaceNormal = input.normalOS;
                                output.ObjectSpaceTangent = input.tangentOS.xyz;
                                output.ObjectSpacePosition = input.positionOS;

                                return output;
                            }
                            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                            {
                                SurfaceDescriptionInputs output;
                                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                            #ifdef HAVE_VFX_MODIFICATION
                            #if VFX_USE_GRAPH_VALUES
                                uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                                /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                            #endif
                                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                            #endif








                                #if UNITY_UV_STARTS_AT_TOP
                                #else
                                #endif


                                output.uv0 = input.texCoord0;
                                output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                            #else
                            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                            #endif
                            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                    return output;
                            }

                            // --------------------------------------------------
                            // Main

                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

                            // --------------------------------------------------
                            // Visual Effect Vertex Invocations
                            #ifdef HAVE_VFX_MODIFICATION
                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                            #endif

                            ENDHLSL
                            }
                            Pass
                            {
                                Name "SceneSelectionPass"
                                Tags
                                {
                                    "LightMode" = "SceneSelectionPass"
                                }

                                // Render State
                                Cull Off

                                // Debug
                                // <None>

                                // --------------------------------------------------
                                // Pass

                                HLSLPROGRAM

                                // Pragmas
                                #pragma target 2.0
                                #pragma vertex vert
                                #pragma fragment frag

                                // Keywords
                                // PassKeywords: <None>
                                // GraphKeywords: <None>

                                // Defines

                                #define ATTRIBUTES_NEED_NORMAL
                                #define ATTRIBUTES_NEED_TANGENT
                                #define ATTRIBUTES_NEED_TEXCOORD0
                                #define VARYINGS_NEED_TEXCOORD0
                                #define FEATURES_GRAPH_VERTEX
                                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                #define SHADERPASS SHADERPASS_DEPTHONLY
                                #define SCENESELECTIONPASS 1
                                #define ALPHA_CLIP_THRESHOLD 1
                                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                                // custom interpolator pre-include
                                /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                // Includes
                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
                                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                                // --------------------------------------------------
                                // Structs and Packing

                                // custom interpolators pre packing
                                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                struct Attributes
                                {
                                     float3 positionOS : POSITION;
                                     float3 normalOS : NORMAL;
                                     float4 tangentOS : TANGENT;
                                     float4 uv0 : TEXCOORD0;
                                    #if UNITY_ANY_INSTANCING_ENABLED
                                     uint instanceID : INSTANCEID_SEMANTIC;
                                    #endif
                                };
                                struct Varyings
                                {
                                     float4 positionCS : SV_POSITION;
                                     float4 texCoord0;
                                    #if UNITY_ANY_INSTANCING_ENABLED
                                     uint instanceID : CUSTOM_INSTANCE_ID;
                                    #endif
                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                    #endif
                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                    #endif
                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                    #endif
                                };
                                struct SurfaceDescriptionInputs
                                {
                                     float4 uv0;
                                     float3 TimeParameters;
                                };
                                struct VertexDescriptionInputs
                                {
                                     float3 ObjectSpaceNormal;
                                     float3 ObjectSpaceTangent;
                                     float3 ObjectSpacePosition;
                                };
                                struct PackedVaryings
                                {
                                     float4 positionCS : SV_POSITION;
                                     float4 texCoord0 : INTERP0;
                                    #if UNITY_ANY_INSTANCING_ENABLED
                                     uint instanceID : CUSTOM_INSTANCE_ID;
                                    #endif
                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                    #endif
                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                    #endif
                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                    #endif
                                };

                                PackedVaryings PackVaryings(Varyings input)
                                {
                                    PackedVaryings output;
                                    ZERO_INITIALIZE(PackedVaryings, output);
                                    output.positionCS = input.positionCS;
                                    output.texCoord0.xyzw = input.texCoord0;
                                    #if UNITY_ANY_INSTANCING_ENABLED
                                    output.instanceID = input.instanceID;
                                    #endif
                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                    #endif
                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                    #endif
                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                    output.cullFace = input.cullFace;
                                    #endif
                                    return output;
                                }

                                Varyings UnpackVaryings(PackedVaryings input)
                                {
                                    Varyings output;
                                    output.positionCS = input.positionCS;
                                    output.texCoord0 = input.texCoord0.xyzw;
                                    #if UNITY_ANY_INSTANCING_ENABLED
                                    output.instanceID = input.instanceID;
                                    #endif
                                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                    #endif
                                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                    #endif
                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                    output.cullFace = input.cullFace;
                                    #endif
                                    return output;
                                }


                                // --------------------------------------------------
                                // Graph

                                // Graph Properties
                                CBUFFER_START(UnityPerMaterial)
                                float _ScrollTime;
                                float4 _Color;
                                float4 _Sprite_Main_TexelSize;
                                float4 _Sprite_Line_TexelSize;
                                float4 _Sprite_ColorMap_TexelSize;
                                CBUFFER_END


                                    // Object and Global properties
                                    SAMPLER(SamplerState_Linear_Repeat);
                                    TEXTURE2D(_Sprite_Main);
                                    SAMPLER(sampler_Sprite_Main);
                                    TEXTURE2D(_Sprite_Line);
                                    SAMPLER(sampler_Sprite_Line);
                                    TEXTURE2D(_Sprite_ColorMap);
                                    SAMPLER(sampler_Sprite_ColorMap);

                                    // Graph Includes
                                    // GraphIncludes: <None>

                                    // -- Property used by ScenePickingPass
                                    #ifdef SCENEPICKINGPASS
                                    float4 _SelectionID;
                                    #endif

                                    // -- Properties used by SceneSelectionPass
                                    #ifdef SCENESELECTIONPASS
                                    int _ObjectId;
                                    int _PassValue;
                                    #endif

                                    // Graph Functions

                                    void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
                                    {
                                        Out = A * B;
                                    }

                                    void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
                                    {
                                        Out = UV * Tiling + Offset;
                                    }

                                    void Unity_Add_float(float A, float B, out float Out)
                                    {
                                        Out = A + B;
                                    }

                                    // Custom interpolators pre vertex
                                    /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                    // Graph Vertex
                                    struct VertexDescription
                                    {
                                        float3 Position;
                                        float3 Normal;
                                        float3 Tangent;
                                    };

                                    VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                    {
                                        VertexDescription description = (VertexDescription)0;
                                        description.Position = IN.ObjectSpacePosition;
                                        description.Normal = IN.ObjectSpaceNormal;
                                        description.Tangent = IN.ObjectSpaceTangent;
                                        return description;
                                    }

                                    // Custom interpolators, pre surface
                                    #ifdef FEATURES_GRAPH_VERTEX
                                    Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                    {
                                    return output;
                                    }
                                    #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                    #endif

                                    // Graph Pixel
                                    struct SurfaceDescription
                                    {
                                        float Alpha;
                                    };

                                    SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                    {
                                        SurfaceDescription surface = (SurfaceDescription)0;
                                        UnityTexture2D _Property_84ab372623ff4f098d6988634f5e924c_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Sprite_Main);
                                        float4 _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_84ab372623ff4f098d6988634f5e924c_Out_0_Texture2D.tex, _Property_84ab372623ff4f098d6988634f5e924c_Out_0_Texture2D.samplerstate, _Property_84ab372623ff4f098d6988634f5e924c_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy));
                                        _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4.rgb = UnpackNormal(_SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4);
                                        float _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_R_4_Float = _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4.r;
                                        float _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_G_5_Float = _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4.g;
                                        float _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_B_6_Float = _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4.b;
                                        float _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_A_7_Float = _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4.a;
                                        UnityTexture2D _Property_3d4c2164e736405faa39cbe26f58ef77_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Sprite_Line);
                                        float _Property_844d4ae06c6a40c08a85e6074cf895e8_Out_0_Float = _ScrollTime;
                                        float2 _Vector2_2ccdec8b6c8247ffae430506ca119d8b_Out_0_Vector2 = float2(0, _Property_844d4ae06c6a40c08a85e6074cf895e8_Out_0_Float);
                                        float2 _Multiply_26470bc1dde54e298caa7dcbc3d14e98_Out_2_Vector2;
                                        Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_2ccdec8b6c8247ffae430506ca119d8b_Out_0_Vector2, _Multiply_26470bc1dde54e298caa7dcbc3d14e98_Out_2_Vector2);
                                        float2 _TilingAndOffset_8557ed2455574bfe85b924e3f527b989_Out_3_Vector2;
                                        Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Multiply_26470bc1dde54e298caa7dcbc3d14e98_Out_2_Vector2, _TilingAndOffset_8557ed2455574bfe85b924e3f527b989_Out_3_Vector2);
                                        float4 _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_3d4c2164e736405faa39cbe26f58ef77_Out_0_Texture2D.tex, _Property_3d4c2164e736405faa39cbe26f58ef77_Out_0_Texture2D.samplerstate, _Property_3d4c2164e736405faa39cbe26f58ef77_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_8557ed2455574bfe85b924e3f527b989_Out_3_Vector2));
                                        _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4.rgb = UnpackNormal(_SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4);
                                        float _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_R_4_Float = _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4.r;
                                        float _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_G_5_Float = _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4.g;
                                        float _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_B_6_Float = _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4.b;
                                        float _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_A_7_Float = _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4.a;
                                        float _Add_7c0aeeb4a1ea410faadf3e5fcabb7391_Out_2_Float;
                                        Unity_Add_float(_SampleTexture2D_c6ff558bc98048588292232098ae0d9d_A_7_Float, _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_A_7_Float, _Add_7c0aeeb4a1ea410faadf3e5fcabb7391_Out_2_Float);
                                        surface.Alpha = _Add_7c0aeeb4a1ea410faadf3e5fcabb7391_Out_2_Float;
                                        return surface;
                                    }

                                    // --------------------------------------------------
                                    // Build Graph Inputs
                                    #ifdef HAVE_VFX_MODIFICATION
                                    #define VFX_SRP_ATTRIBUTES Attributes
                                    #define VFX_SRP_VARYINGS Varyings
                                    #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                                    #endif
                                    VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                    {
                                        VertexDescriptionInputs output;
                                        ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                        output.ObjectSpaceNormal = input.normalOS;
                                        output.ObjectSpaceTangent = input.tangentOS.xyz;
                                        output.ObjectSpacePosition = input.positionOS;

                                        return output;
                                    }
                                    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                    {
                                        SurfaceDescriptionInputs output;
                                        ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                    #ifdef HAVE_VFX_MODIFICATION
                                    #if VFX_USE_GRAPH_VALUES
                                        uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                                        /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                                    #endif
                                        /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                                    #endif








                                        #if UNITY_UV_STARTS_AT_TOP
                                        #else
                                        #endif


                                        output.uv0 = input.texCoord0;
                                        output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                    #else
                                    #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                    #endif
                                    #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                            return output;
                                    }

                                    // --------------------------------------------------
                                    // Main

                                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"

                                    // --------------------------------------------------
                                    // Visual Effect Vertex Invocations
                                    #ifdef HAVE_VFX_MODIFICATION
                                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                                    #endif

                                    ENDHLSL
                                    }
                                    Pass
                                    {
                                        Name "ScenePickingPass"
                                        Tags
                                        {
                                            "LightMode" = "Picking"
                                        }

                                        // Render State
                                        Cull Back

                                        // Debug
                                        // <None>

                                        // --------------------------------------------------
                                        // Pass

                                        HLSLPROGRAM

                                        // Pragmas
                                        #pragma target 2.0
                                        #pragma vertex vert
                                        #pragma fragment frag

                                        // Keywords
                                        // PassKeywords: <None>
                                        // GraphKeywords: <None>

                                        // Defines

                                        #define ATTRIBUTES_NEED_NORMAL
                                        #define ATTRIBUTES_NEED_TANGENT
                                        #define ATTRIBUTES_NEED_TEXCOORD0
                                        #define VARYINGS_NEED_TEXCOORD0
                                        #define FEATURES_GRAPH_VERTEX
                                        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                                        #define SHADERPASS SHADERPASS_DEPTHONLY
                                        #define SCENEPICKINGPASS 1
                                        #define ALPHA_CLIP_THRESHOLD 1
                                        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */


                                        // custom interpolator pre-include
                                        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

                                        // Includes
                                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
                                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
                                        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
                                        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                                        // --------------------------------------------------
                                        // Structs and Packing

                                        // custom interpolators pre packing
                                        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                                        struct Attributes
                                        {
                                             float3 positionOS : POSITION;
                                             float3 normalOS : NORMAL;
                                             float4 tangentOS : TANGENT;
                                             float4 uv0 : TEXCOORD0;
                                            #if UNITY_ANY_INSTANCING_ENABLED
                                             uint instanceID : INSTANCEID_SEMANTIC;
                                            #endif
                                        };
                                        struct Varyings
                                        {
                                             float4 positionCS : SV_POSITION;
                                             float4 texCoord0;
                                            #if UNITY_ANY_INSTANCING_ENABLED
                                             uint instanceID : CUSTOM_INSTANCE_ID;
                                            #endif
                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                            #endif
                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                            #endif
                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                            #endif
                                        };
                                        struct SurfaceDescriptionInputs
                                        {
                                             float4 uv0;
                                             float3 TimeParameters;
                                        };
                                        struct VertexDescriptionInputs
                                        {
                                             float3 ObjectSpaceNormal;
                                             float3 ObjectSpaceTangent;
                                             float3 ObjectSpacePosition;
                                        };
                                        struct PackedVaryings
                                        {
                                             float4 positionCS : SV_POSITION;
                                             float4 texCoord0 : INTERP0;
                                            #if UNITY_ANY_INSTANCING_ENABLED
                                             uint instanceID : CUSTOM_INSTANCE_ID;
                                            #endif
                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                                            #endif
                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                                            #endif
                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                                            #endif
                                        };

                                        PackedVaryings PackVaryings(Varyings input)
                                        {
                                            PackedVaryings output;
                                            ZERO_INITIALIZE(PackedVaryings, output);
                                            output.positionCS = input.positionCS;
                                            output.texCoord0.xyzw = input.texCoord0;
                                            #if UNITY_ANY_INSTANCING_ENABLED
                                            output.instanceID = input.instanceID;
                                            #endif
                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                            #endif
                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                            #endif
                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                            output.cullFace = input.cullFace;
                                            #endif
                                            return output;
                                        }

                                        Varyings UnpackVaryings(PackedVaryings input)
                                        {
                                            Varyings output;
                                            output.positionCS = input.positionCS;
                                            output.texCoord0 = input.texCoord0.xyzw;
                                            #if UNITY_ANY_INSTANCING_ENABLED
                                            output.instanceID = input.instanceID;
                                            #endif
                                            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                                            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                                            #endif
                                            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                                            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                                            #endif
                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                            output.cullFace = input.cullFace;
                                            #endif
                                            return output;
                                        }


                                        // --------------------------------------------------
                                        // Graph

                                        // Graph Properties
                                        CBUFFER_START(UnityPerMaterial)
                                        float _ScrollTime;
                                        float4 _Color;
                                        float4 _Sprite_Main_TexelSize;
                                        float4 _Sprite_Line_TexelSize;
                                        float4 _Sprite_ColorMap_TexelSize;
                                        CBUFFER_END


                                            // Object and Global properties
                                            SAMPLER(SamplerState_Linear_Repeat);
                                            TEXTURE2D(_Sprite_Main);
                                            SAMPLER(sampler_Sprite_Main);
                                            TEXTURE2D(_Sprite_Line);
                                            SAMPLER(sampler_Sprite_Line);
                                            TEXTURE2D(_Sprite_ColorMap);
                                            SAMPLER(sampler_Sprite_ColorMap);

                                            // Graph Includes
                                            // GraphIncludes: <None>

                                            // -- Property used by ScenePickingPass
                                            #ifdef SCENEPICKINGPASS
                                            float4 _SelectionID;
                                            #endif

                                            // -- Properties used by SceneSelectionPass
                                            #ifdef SCENESELECTIONPASS
                                            int _ObjectId;
                                            int _PassValue;
                                            #endif

                                            // Graph Functions

                                            void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
                                            {
                                                Out = A * B;
                                            }

                                            void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
                                            {
                                                Out = UV * Tiling + Offset;
                                            }

                                            void Unity_Add_float(float A, float B, out float Out)
                                            {
                                                Out = A + B;
                                            }

                                            // Custom interpolators pre vertex
                                            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                                            // Graph Vertex
                                            struct VertexDescription
                                            {
                                                float3 Position;
                                                float3 Normal;
                                                float3 Tangent;
                                            };

                                            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                                            {
                                                VertexDescription description = (VertexDescription)0;
                                                description.Position = IN.ObjectSpacePosition;
                                                description.Normal = IN.ObjectSpaceNormal;
                                                description.Tangent = IN.ObjectSpaceTangent;
                                                return description;
                                            }

                                            // Custom interpolators, pre surface
                                            #ifdef FEATURES_GRAPH_VERTEX
                                            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
                                            {
                                            return output;
                                            }
                                            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
                                            #endif

                                            // Graph Pixel
                                            struct SurfaceDescription
                                            {
                                                float Alpha;
                                            };

                                            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                                            {
                                                SurfaceDescription surface = (SurfaceDescription)0;
                                                UnityTexture2D _Property_84ab372623ff4f098d6988634f5e924c_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Sprite_Main);
                                                float4 _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_84ab372623ff4f098d6988634f5e924c_Out_0_Texture2D.tex, _Property_84ab372623ff4f098d6988634f5e924c_Out_0_Texture2D.samplerstate, _Property_84ab372623ff4f098d6988634f5e924c_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy));
                                                _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4.rgb = UnpackNormal(_SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4);
                                                float _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_R_4_Float = _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4.r;
                                                float _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_G_5_Float = _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4.g;
                                                float _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_B_6_Float = _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4.b;
                                                float _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_A_7_Float = _SampleTexture2D_c6ff558bc98048588292232098ae0d9d_RGBA_0_Vector4.a;
                                                UnityTexture2D _Property_3d4c2164e736405faa39cbe26f58ef77_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Sprite_Line);
                                                float _Property_844d4ae06c6a40c08a85e6074cf895e8_Out_0_Float = _ScrollTime;
                                                float2 _Vector2_2ccdec8b6c8247ffae430506ca119d8b_Out_0_Vector2 = float2(0, _Property_844d4ae06c6a40c08a85e6074cf895e8_Out_0_Float);
                                                float2 _Multiply_26470bc1dde54e298caa7dcbc3d14e98_Out_2_Vector2;
                                                Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Vector2_2ccdec8b6c8247ffae430506ca119d8b_Out_0_Vector2, _Multiply_26470bc1dde54e298caa7dcbc3d14e98_Out_2_Vector2);
                                                float2 _TilingAndOffset_8557ed2455574bfe85b924e3f527b989_Out_3_Vector2;
                                                Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Multiply_26470bc1dde54e298caa7dcbc3d14e98_Out_2_Vector2, _TilingAndOffset_8557ed2455574bfe85b924e3f527b989_Out_3_Vector2);
                                                float4 _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_3d4c2164e736405faa39cbe26f58ef77_Out_0_Texture2D.tex, _Property_3d4c2164e736405faa39cbe26f58ef77_Out_0_Texture2D.samplerstate, _Property_3d4c2164e736405faa39cbe26f58ef77_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_8557ed2455574bfe85b924e3f527b989_Out_3_Vector2));
                                                _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4.rgb = UnpackNormal(_SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4);
                                                float _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_R_4_Float = _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4.r;
                                                float _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_G_5_Float = _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4.g;
                                                float _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_B_6_Float = _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4.b;
                                                float _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_A_7_Float = _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_RGBA_0_Vector4.a;
                                                float _Add_7c0aeeb4a1ea410faadf3e5fcabb7391_Out_2_Float;
                                                Unity_Add_float(_SampleTexture2D_c6ff558bc98048588292232098ae0d9d_A_7_Float, _SampleTexture2D_95ecb58a23644707bdc1e4053cbb0b84_A_7_Float, _Add_7c0aeeb4a1ea410faadf3e5fcabb7391_Out_2_Float);
                                                surface.Alpha = _Add_7c0aeeb4a1ea410faadf3e5fcabb7391_Out_2_Float;
                                                return surface;
                                            }

                                            // --------------------------------------------------
                                            // Build Graph Inputs
                                            #ifdef HAVE_VFX_MODIFICATION
                                            #define VFX_SRP_ATTRIBUTES Attributes
                                            #define VFX_SRP_VARYINGS Varyings
                                            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
                                            #endif
                                            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                                            {
                                                VertexDescriptionInputs output;
                                                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                                                output.ObjectSpaceNormal = input.normalOS;
                                                output.ObjectSpaceTangent = input.tangentOS.xyz;
                                                output.ObjectSpacePosition = input.positionOS;

                                                return output;
                                            }
                                            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                                            {
                                                SurfaceDescriptionInputs output;
                                                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                                            #ifdef HAVE_VFX_MODIFICATION
                                            #if VFX_USE_GRAPH_VALUES
                                                uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                                                /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                                            #endif
                                                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                                            #endif








                                                #if UNITY_UV_STARTS_AT_TOP
                                                #else
                                                #endif


                                                output.uv0 = input.texCoord0;
                                                output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                                            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                                            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                                            #else
                                            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                                            #endif
                                            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                                                    return output;
                                            }

                                            // --------------------------------------------------
                                            // Main

                                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
                                            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"

                                            // --------------------------------------------------
                                            // Visual Effect Vertex Invocations
                                            #ifdef HAVE_VFX_MODIFICATION
                                            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
                                            #endif

                                            ENDHLSL
                                            }
    }
        CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
                                                CustomEditorForRenderPipeline "UnityEditor.ShaderGraphUnlitGUI" "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset"
                                                FallBack "Hidden/Shader Graph/FallbackError"
}