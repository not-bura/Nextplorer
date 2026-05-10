using SharpDX.D3DCompiler;
using System.IO;
using System.Windows.Media.Effects;

namespace Nextplorer.Presentation
{
    public class ColorPickerShaderEffect : ShaderEffect
    {
        private static PixelShader s_triangleShader = null!;
        private static PixelShader s_rectangleShader = null!;
        private static PixelShader s_colorRingShader = null!;

        private static PixelShader triangleShader
        {
            get
            {
                s_triangleShader ??= LoadTriangle();
                return s_triangleShader;
            }
        }

        private static PixelShader rectangleShader
        {
            get
            {
                s_rectangleShader ??= LoadRectangle();
                return s_rectangleShader;
            }
        }

        private static PixelShader colorRingShader
        {
            get
            {
                s_colorRingShader ??= LoadColorRing();
                return s_colorRingShader;
            }
        }

        public static ColorPickerShaderEffect Triangle()
        {
            return new(triangleShader);
        }

        public static ColorPickerShaderEffect Rectangle()
        {
            return new(rectangleShader);
        }

        public static ColorPickerShaderEffect ColorRing()
        {
            return new(colorRingShader);
        }

        private ColorPickerShaderEffect(PixelShader shader)
        {
            PixelShader = shader;
        }

        private static PixelShader LoadTriangle()
        {
            var _shader = new PixelShader();

            //var _directory = "Nextplorer/shader";
            //var _name = "triangle";

            //var _path = Path.Combine(_directory, _name);

            //if (File.Exists(_path))
            //{
            //    using var _fs = File.OpenRead(_path);
            //    _shader.SetStreamSource(_fs);

            //    return _shader;
            //}

            const string SHADER_CODE = @"
float4 main(float2 uv : TEXCOORD) : COLOR
{
    float2 p1 = float2(0, 0);
    float2 p2 = float2(0, 1);
    float2 p3 = float2(1, 0.5);

    float det = (p2.y - p3.y) * (p1.x - p3.x) + (p3.x - p2.x) * (p1.y - p3.y);
    float w1 = ((p2.y - p3.y) * (uv.x - p3.x) + (p3.x - p2.x) * (uv.y - p3.y)) / det;
    float w2 = ((p3.y - p1.y) * (uv.x - p3.x) + (p1.x - p3.x) * (uv.y - p3.y)) / det;
    float w3 = 1.0 - w1 - w2;

    float3 c1 = float3(1, 1, 1);
    float3 c2 = float3(0, 0, 0);
    float3 c3 = float3(1, 0, 0);

    if (w1 < 0 || w2 < 0 || w3 < 0) discard;

    float3 f = c1 * w1 + c2 * w2 + c3 * w3;

    return float4(f, 1);
}";

            var _result = ShaderBytecode.Compile(SHADER_CODE, "main", "ps_3_0", ShaderFlags.OptimizationLevel3);

            //if (false == Directory.Exists(_directory))
            //{
            //    Directory.CreateDirectory(_directory);
            //}

            //using (var _fs = File.Create(_path))
            //{
            //    _fs.Write(_result.Bytecode);
            //}

            using (var _ms = new MemoryStream(_result.Bytecode))
            {
                _shader.SetStreamSource(_ms);
            }

            return _shader;
        }

        private static PixelShader LoadRectangle()
        {
            var _shader = new PixelShader();

            var _directory = "Nextplorer/shader";
            var _name = "triangle";

            var _path = Path.Combine(_directory, _name);

            if (File.Exists(_path))
            {
                using var _fs = File.OpenRead(_path);
                _shader.SetStreamSource(_fs);

                return _shader;
            }

            const string SHADER_CODE = @"
float4 main(float2 uv : TEXCOORD) : COLOR
{
    float3 white = float3(1, 1, 1);
    float3 hue = float3(1, 0, 0);

    float3 x = lerp(white, hue, uv.x);
    float3 black = float3(0, 0, 0);

    float3 color = lerp(x, black, uv.y);

    return float4(color.r, color.g, color.b, 1);
}";
            var _result = ShaderBytecode.Compile(SHADER_CODE, "main", "ps_3_0", ShaderFlags.OptimizationLevel3);

            if (false == Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }

            using (var _fs = File.Create(_path))
            {
                _fs.Write(_result.Bytecode);
            }

            using (var _ms = new MemoryStream(_result.Bytecode))
            {
                _shader.SetStreamSource(_ms);
            }

            return _shader;
        }

        private static PixelShader LoadColorRing()
        {
            var _shader = new PixelShader();

            //var _directory = "Nextplorer/shader";
            //var _name = "color_ring";

            //var _path = Path.Combine(_directory, _name);

            //if (File.Exists(_path))
            //{
            //    using var _fs = File.OpenRead(_path);
            //    _shader.SetStreamSource(_fs);

            //    return _shader;
            //}

            const string SHADER_CODE = @"
float3 HSV2RGB(float3 color)
{
    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 P = abs(frac(color.xxx + K.xyz) * 6.0 - K.www);
    return color.z * lerp(K.xxx, clamp(P - K.xxx, 0.0, 1.0), color.y);
}

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float2 pos = uv - 0.5;

    float distance = length(pos);
    if (distance > 0.5 || distance < 0.4) discard;

    float angle = atan2(pos.y, pos.x);
    float hue = (angle / (2.0 * 3.14159265)) + 0.4;

    float3 rgb = HSV2RGB(float3(hue, 1, 1));

    return float4(rgb, 1);
}
";

            var _result = ShaderBytecode.Compile(SHADER_CODE, "main", "ps_3_0", ShaderFlags.OptimizationLevel3);

            //if (false == Directory.Exists(_directory))
            //{
            //    Directory.CreateDirectory(_directory);
            //}

            //using (var _fs = File.Create(_path))
            //{
            //    _fs.Write(_result.Bytecode);
            //}

            using (var _ms = new MemoryStream(_result.Bytecode))
            {
                _shader.SetStreamSource(_ms);
            }

            return _shader;
        }
    }
}
