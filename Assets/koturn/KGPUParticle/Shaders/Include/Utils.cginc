#ifndef UTILS_INCLUDED
#define UTILS_INCLUDED


#if defined(SHADER_API_GLCORE) || defined(SHADER_API_GLES) || defined(SHADER_API_D3D9)
typedef fixed FaceType;
#    define FACE_SEMANTICS VFACE
#else
typedef bool FaceType;
#    define FACE_SEMANTICS SV_IsFrontFace
#endif  // defined(SHADER_API_GLCORE) || defined(SHADER_API_GLES) || defined(SHADER_API_D3D9)


bool isFacing(FaceType face);
float fmodglsl(float x, float y);
float2 fmodglsl(float2 x, float2 y);
float3 fmodglsl(float3 x, float3 y);
float4 fmodglsl(float4 x, float4 y);
float rand(float co1, float co2);
float2 rand(float2 co1, float2 co2);
float3 rand(float3 co1, float3 co2);
float rand(float co1, float co2, float a, float b);
float2 rand(float2 co1, float2 co2, float2 a, float2 b);
float3 rand(float3 co1, float3 co2, float3 a, float3 b);
float rand(float2 co);
float rand(float2 co, float a, float b);
float2 rotate2D(float2 pos, float angle);
float2 rotate2D(float2 pos, float2 pivot, float angle);
float3 rotate3D(float3 pos, float3 angles);
float3 rotate3D(float3 pos, float3 axis, float angle);
float3 rotate3D(float3 pos, float3 pivot, float3 axis, float angle);
float3 rgb2hsv(float3 rgb);
float3 hsv2rgb(float3 hsv);
float3 rgbAddHue(float3 rgb, float hue);


/*!
 * @brief Identify whether surface is facing the camera or facing away from the camera.
 * @param [in] face  Facing variable (VFACE or SV_IsFrontFace).
 * @return True if surface facing the camera, otherwise false.
 */
bool isFacing(FaceType face)
{
#if defined(SHADER_API_GLCORE) || defined(SHADER_API_GLES) || defined(SHADER_API_D3D9)
    return face >= 0.0;
#else
    return face;
#endif  // defined(SHADER_API_GLCORE) || defined(SHADER_API_GLES) || defined(SHADER_API_D3D9)
}


/*!
 * @brief Returns the remainder of x divided by y with the same sign as y.
 * @param [in] x  Scalar numerator.
 * @param [in] y  Scalar denominator.
 * @return Remainder of x / y with the same sign as y.
 */
float fmodglsl(float x, float y)
{
    return x - y * floor(x / y);
}


/*!
 * @brief Returns the remainder of x divided by y with the same sign as y.
 * @param [in] x  Vector numerator.
 * @param [in] y  Vector denominator.
 * @return Remainder of x / y with the same sign as y.
 */
float2 fmodglsl(float2 x, float2 y)
{
    return x - y * floor(x / y);
}


/*!
 * @brief Returns the remainder of x divided by y with the same sign as y.
 * @param [in] x  Vector numerator.
 * @param [in] y  Vector denominator.
 * @return Remainder of x / y with the same sign as y.
 */
float3 fmodglsl(float3 x, float3 y)
{
    return x - y * floor(x / y);
}


/*!
 * @brief Returns the remainder of x divided by y with the same sign as y.
 * @param [in] x  Vector numerator.
 * @param [in] y  Vector denominator.
 * @return Remainder of x / y with the same sign as y.
 */
float4 fmodglsl(float4 x, float4 y)
{
    return x - y * floor(x / y);
}


/*!
 * @brief Get random value between [0, 1].
 * @param [in] co1  First seed.
 * @param [in] co2  Second seed.
 * @return Random value between [0, 1].
 */
float rand(float co1, float co2)
{
    return frac(sin(co1 * 12.9898 + co2 * 78.233) * 43758.5453);
}


/*!
 * @brief Get random value vector between [0, 1].
 * @param [in] co1  First seed vector.
 * @param [in] co2  Second seed vector.
 * @return Random value vector between [0, 1].
 */
float2 rand(float2 co1, float2 co2)
{
    return frac(sin(co1 * 12.9898 + co2 * 78.233) * 43758.5453);
}


/*!
 * @brief Get random value vector between [0, 1].
 * @param [in] co1  First seed vector.
 * @param [in] co2  Second seed vector.
 * @return Random value vector between [0, 1].
 */
float3 rand(float3 co1, float3 co2)
{
    return frac(sin(co1 * 12.9898 + co2 * 78.233) * 43758.5453);
}


/*!
 * @brief Get random value between [0, 1].
 * @param [in] co  Seed vector.
 * @return Random value between [0, 1].
 */
float rand(float2 co)
{
    return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
}


/*!
 * @brief Get random value between [a, b].
 * @param [in] co1  First seed.
 * @param [in] co2  Second seed.
 * @param [in] a  Minimum output value.
 * @param [in] b  Maximum output value.
 * @return Random value between [a, b].
 */
float rand(float co1, float co2, float a, float b)
{
    return lerp(a, b, rand(co1, co2));
}


/*!
 * @brief Get random value vector between [a, b].
 * @param [in] co1  First seed vector.
 * @param [in] co2  Second seed vector.
 * @param [in] a  Minimum output value vector.
 * @param [in] b  Maximum output value vector.
 * @return Random value vector between [a, b].
 */
float2 rand(float2 co1, float2 co2, float2 a, float2 b)
{
    return lerp(a, b, rand(co1, co2));
}


/*!
 * @brief Get random value vector between [a, b].
 * @param [in] co1  First seed vector.
 * @param [in] co2  Second seed vector.
 * @param [in] a  Minimum output value vector.
 * @param [in] b  Maximum output value vector.
 * @return Random value vector between [a, b].
 */
float3 rand(float3 co1, float3 co2, float3 a, float3 b)
{
    return lerp(a, b, rand(co1, co2));
}


/*!
 * @brief Get random value between [a, b].
 * @param [in] co  Seed vector.
 * @param [in] a  Minimum output value.
 * @param [in] b  Maximum output value.
 * @return Random value between [a, b].
 */
float rand(float2 co, float a, float b)
{
    return lerp(a, b, rand(co));
}


/*!
 * @brief Rotate on 2D plane.
 * @param [in] pos  Target position.
 * @param [in] angle  Angle of rotation.
 * @return Rotated vector.
 */
float2 rotate2D(float2 pos, float angle)
{
    float s, c;
    sincos(angle, /* out */ s, /* out */ c);
    return float2(
        pos.x * c - pos.y * s,
        pos.x * s + pos.y * c);
}


/*!
 * @brief Rotate on 2D plane.
 * @param [in] pos  Target position.
 * @param [in] pivot  Pivot of rotation.
 * @param [in] angle  Angle of rotation.
 * @return Rotated vector.
 */
float2 rotate2D(float2 pos, float2 pivot, float angle)
{
    return rotate2D(pos - pivot, angle) + pivot;
}


/*!
 * @brief Rotate around Z-axis, Y-axis and X-axis.
 * @param [in] pos  Position.
 * @param [in] angles  Rotate angle of XYZ.
 * @return Rotated position.
 */
float3 rotate3D(float3 pos, float3 angles)
{
    float3 s3, c3;
    sincos(angles, s3, c3);

    pos.xy = float2(
        pos.x * c3.z - pos.y * s3.z,
        pos.x * s3.z + pos.y * c3.z);
    pos.zx = float2(
        pos.z * c3.y - pos.x * s3.y,
        pos.z * s3.y + pos.x * c3.y);
    pos.yz = float2(
        pos.y * c3.x - pos.z * s3.x,
        pos.y * s3.x + pos.z * c3.x);

    return pos;
}


/*!
 * @brief Rotate around specified axis vector using Rodrigues' rotation formula.
 * @param [in] pos  Position.
 * @param [in] axis  Rotation Axis vector.
 * @param [in] angle  Rotation angle.
 * @return Rotated position.
 */
float3 rotate3D(float3 pos, float3 axis, float angle)
{
    float s, c;
    sincos(angle, /* out */ s, /* out */ c);

    return pos * c + axis * (1.0 - c) * dot(pos, axis) + cross(pos, axis) * s;

    // return pos * c + axis * (dot(pos, axis) - c * dot(pos, axis)) + cross(pos, axis) * s;

    // return lerp(axis * dot(pos, axis), pos, c) + cross(pos, axis) * s;

    // return pos * c + axis * dot(pos, axis) * (1.0 - c) + cross(pos, axis) * s;
    // float3 da = axis * dot(pos, axis);
    // return pos * c + da * (1.0 - c) + cross(pos, axis) * s;
    // return (pos - da) * c + da + cross(pos, axis) * s;
}


/*!
 * @brief Rotate around specified axis vector using Rodrigues' rotation formula.
 * @param [in] pos  Position.
 * @param [in] pivot  Rotation pivot.
 * @param [in] axis  Rotation Axis vector.
 * @param [in] angle  Rotation angle.
 * @return Rotated position.
 * @see rotate3D(float3, float3, float)
 */
float3 rotate3D(float3 pos, float3 pivot, float3 axis, float angle)
{
    return rotate3D(pos - pivot, axis, angle) + pivot;
}


/*!
 * @brief Convert from RGB to HSV.
 * @param [in] rgb  Vector of RGB components.
 * @return Vector of HSV components.
 */
float3 rgb2hsv(float3 rgb)
{
    static const float4 k = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    static const float e = 1.0e-10;
#if 1
    // Optimized version.
    const bool b1 = rgb.g < rgb.b;
    float4 p = float4(b1 ? rgb.bg : rgb.gb, b1 ? k.wz : k.xy);

    const bool b2 = rgb.r < p.x;
    p.xyz = b2 ? p.xyw : p.yzx;
    const float4 q = b2 ? float4(p.xyz, rgb.r) : float4(rgb.r, p.xyz);

    const float d = q.x - min(q.w, q.y);
    const float2 hs = float2(q.w - q.y, d) / float2(6.0 * d + e, q.x + e);

    return float3(abs(q.z + hs.x), hs.y, q.x);
#else
    // Original version
    const float4 p = rgb.g < rgb.b ? float4(rgb.bg, k.wz) : float4(rgb.gb, k.xy);
    const float4 q = rgb.r < p.x ? float4(p.xyw, rgb.r) : float4(rgb.r, p.yzx);
    const float d = q.x - min(q.w, q.y);

    return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
#endif
}


/*!
 * @brief Convert from HSV to RGB.
 * @param [in] hsv  Vector of HSV components.
 * @return Vector of RGB components.
 */
float3 hsv2rgb(float3 hsv)
{
    static const float4 k = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);

    const float3 p = abs(frac(hsv.xxx + k.xyz) * 6.0 - k.www);
    return hsv.z * lerp(k.xxx, saturate(p - k.xxx), hsv.y);
}


/*!
 * @brief Add hue to RGB color.
 * @param [in] rgb  Vector of RGB components.
 * @param [in] hue  Offset of Hue.
 * @return Vector of RGB components.
 */
float3 rgbAddHue(float3 rgb, float hue)
{
    float3 hsv = rgb2hsv(rgb);
    hsv.x += hue;
    return hsv2rgb(hsv);
}


#endif  // UTILS_INCLUDED
