#ifndef OPT_UNITYCG_INCLUDED
#define OPT_UNITYCG_INCLUDED


#include "UnityCG.cginc"


#define DecodeHDR DecodeHDROpt


half3 DecodeHDROpt(half4 data, half4 hdr);


/*!
 * @brief Decodes HDR textures; handles dLDR, RGBM formats.
 * @param [in] data  HDR color data.
 * @param [in] hdr  Decode instruction.
 * @return Decoded color data.
 */
half3 DecodeHDROpt(half4 data, half4 hdr)
{
#if defined(UNITY_COLORSPACE_GAMMA)
    return (hdr.x * (hdr.w * data.a - hdr.w) + hdr.x) * data.rgb;
#elif defined(UNITY_USE_NATIVE_HDR)
    // Multiplier for future HDRI relative to absolute conversion.
    return hdr.x * data.rgb;
#else
    const half alpha = hdr.w * (data.a - 1.0) + 1.0;
    return (hdr.x * pow(alpha, hdr.y)) * data.rgb;
#endif
}


#endif  // OPT_UNITYCG_INCLUDED
