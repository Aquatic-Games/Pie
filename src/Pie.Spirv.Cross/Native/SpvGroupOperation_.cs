namespace Pie.Spirv.Cross.Native;

public enum SpvGroupOperation_
{
    SpvGroupOperationReduce = 0,
    SpvGroupOperationInclusiveScan = 1,
    SpvGroupOperationExclusiveScan = 2,
    SpvGroupOperationClusteredReduce = 3,
    SpvGroupOperationPartitionedReduceNV = 6,
    SpvGroupOperationPartitionedInclusiveScanNV = 7,
    SpvGroupOperationPartitionedExclusiveScanNV = 8,
    SpvGroupOperationMax = 0x7fffffff,
}
