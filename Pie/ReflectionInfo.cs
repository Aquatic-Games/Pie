using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pie;

/// <summary>
/// Shader reflection info.
/// </summary>
public struct ReflectionInfo
{
    public ShaderStage Stage;
    
    [JsonPropertyName("entryPoints")] public EntryPoint[] EntryPoints;
    
    [JsonPropertyName("types")] public Dictionary<string, ShaderType> Types;

    [JsonPropertyName("inputs")] public ShaderAttribute[] Inputs;

    [JsonPropertyName("outputs")] public ShaderAttribute[] Outputs;

    [JsonPropertyName("ubos")] public UBOInfo[] Ubos;

    public ReflectionInfo(EntryPoint[] entryPoints, Dictionary<string, ShaderType> types, ShaderAttribute[] inputs, ShaderAttribute[] outputs, UBOInfo[] ubos)
    {
        EntryPoints = entryPoints;
        Types = types;
        Inputs = inputs;
        Outputs = outputs;
        Ubos = ubos;
        Stage = ShaderStage.Vertex;
    }

    public static ReflectionInfo FromJson(string json, ShaderStage stage)
    {
        ReflectionInfo info = JsonSerializer.Deserialize<ReflectionInfo>(json, new JsonSerializerOptions() { IncludeFields = true, IgnoreReadOnlyFields = false });
        info.Stage = stage;
        return info;
    }

    public struct EntryPoint
    {
        [JsonPropertyName("name")] public string Name;
        
        [JsonPropertyName("mode")] public string Mode;

        public EntryPoint(string name, string mode)
        {
            Name = name;
            Mode = mode;
        }
    }

    public struct ShaderType
    {
        [JsonPropertyName("name")] public string Name;
        
        [JsonPropertyName("members")] public MemberInfo[] Members;

        public ShaderType(string name, MemberInfo[] members)
        {
            Name = name;
            Members = members;
        }

        public struct MemberInfo
        {
            [JsonPropertyName("name")] public string Name;
            
            [JsonPropertyName("type")] public string Type;

            [JsonPropertyName("array")] public int[] Array;
            
            [JsonPropertyName("array_size_is_literal")] public bool[] ArraySizeIsLiteral;

            [JsonPropertyName("offset")] public int Offset;

            [JsonPropertyName("matrix_stride")] public int MatrixStride;

            public MemberInfo(string name, string type, int[] array, bool[] arraySizeIsLiteral, int offset, int matrixStride)
            {
                Name = name;
                Type = type;
                Array = array;
                ArraySizeIsLiteral = arraySizeIsLiteral;
                Offset = offset;
                MatrixStride = matrixStride;
            }
        }
    }

    public struct ShaderAttribute
    {
        [JsonPropertyName("type")] public string Type;

        [JsonPropertyName("name")] public string Name;

        [JsonPropertyName("location")] public int Location;

        public ShaderAttribute(string type, string name, int location)
        {
            Type = type;
            Name = name;
            Location = location;
        }
    }

    public struct UBOInfo
    {
        [JsonPropertyName("type")] public string Type;

        [JsonPropertyName("name")] public string Name;

        [JsonPropertyName("block_size")] public int BlockSize;

        [JsonPropertyName("set")] public int Set;

        [JsonPropertyName("binding")] public int Binding;

        public UBOInfo(string type, string name, int blockSize, int set, int binding)
        {
            Type = type;
            Name = name;
            BlockSize = blockSize;
            Set = set;
            Binding = binding;
        }
    }
}