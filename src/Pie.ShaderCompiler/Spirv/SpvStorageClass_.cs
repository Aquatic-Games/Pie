namespace Pie.ShaderCompiler.Spirv
{
    [NativeTypeName("unsigned int")]
    public enum SpvStorageClass_ : uint
    {
        SpvStorageClassUniformConstant = 0,
        SpvStorageClassInput = 1,
        SpvStorageClassUniform = 2,
        SpvStorageClassOutput = 3,
        SpvStorageClassWorkgroup = 4,
        SpvStorageClassCrossWorkgroup = 5,
        SpvStorageClassPrivate = 6,
        SpvStorageClassFunction = 7,
        SpvStorageClassGeneric = 8,
        SpvStorageClassPushConstant = 9,
        SpvStorageClassAtomicCounter = 10,
        SpvStorageClassImage = 11,
        SpvStorageClassStorageBuffer = 12,
        SpvStorageClassCallableDataKHR = 5328,
        SpvStorageClassCallableDataNV = 5328,
        SpvStorageClassIncomingCallableDataKHR = 5329,
        SpvStorageClassIncomingCallableDataNV = 5329,
        SpvStorageClassRayPayloadKHR = 5338,
        SpvStorageClassRayPayloadNV = 5338,
        SpvStorageClassHitAttributeKHR = 5339,
        SpvStorageClassHitAttributeNV = 5339,
        SpvStorageClassIncomingRayPayloadKHR = 5342,
        SpvStorageClassIncomingRayPayloadNV = 5342,
        SpvStorageClassShaderRecordBufferKHR = 5343,
        SpvStorageClassShaderRecordBufferNV = 5343,
        SpvStorageClassPhysicalStorageBuffer = 5349,
        SpvStorageClassPhysicalStorageBufferEXT = 5349,
        SpvStorageClassTaskPayloadWorkgroupEXT = 5402,
        SpvStorageClassCodeSectionINTEL = 5605,
        SpvStorageClassDeviceOnlyINTEL = 5936,
        SpvStorageClassHostOnlyINTEL = 5937,
        SpvStorageClassMax = 0x7fffffff,
    }
}
