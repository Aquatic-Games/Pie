namespace Pie.Spirv.Cross.Native;

public enum SpvQuantizationModes_
{
    SpvQuantizationModesTRN = 0,
    SpvQuantizationModesTRN_ZERO = 1,
    SpvQuantizationModesRND = 2,
    SpvQuantizationModesRND_ZERO = 3,
    SpvQuantizationModesRND_INF = 4,
    SpvQuantizationModesRND_MIN_INF = 5,
    SpvQuantizationModesRND_CONV = 6,
    SpvQuantizationModesRND_CONV_ODD = 7,
    SpvQuantizationModesMax = 0x7fffffff,
}
