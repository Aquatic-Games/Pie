namespace Pie.Spirv.Cross.Native;

public enum SpvMemorySemanticsShift_
{
    SpvMemorySemanticsAcquireShift = 1,
    SpvMemorySemanticsReleaseShift = 2,
    SpvMemorySemanticsAcquireReleaseShift = 3,
    SpvMemorySemanticsSequentiallyConsistentShift = 4,
    SpvMemorySemanticsUniformMemoryShift = 6,
    SpvMemorySemanticsSubgroupMemoryShift = 7,
    SpvMemorySemanticsWorkgroupMemoryShift = 8,
    SpvMemorySemanticsCrossWorkgroupMemoryShift = 9,
    SpvMemorySemanticsAtomicCounterMemoryShift = 10,
    SpvMemorySemanticsImageMemoryShift = 11,
    SpvMemorySemanticsOutputMemoryShift = 12,
    SpvMemorySemanticsOutputMemoryKHRShift = 12,
    SpvMemorySemanticsMakeAvailableShift = 13,
    SpvMemorySemanticsMakeAvailableKHRShift = 13,
    SpvMemorySemanticsMakeVisibleShift = 14,
    SpvMemorySemanticsMakeVisibleKHRShift = 14,
    SpvMemorySemanticsVolatileShift = 15,
    SpvMemorySemanticsMax = 0x7fffffff,
}
