using System;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;

namespace Pie.Tests.Tests.Utils;

public class DDS
{
    /// <summary>
    /// The number of mipmap levels in this texture.
    /// </summary>
    public readonly int MipLevels;

    public readonly Size Size;

    public readonly byte[][][] Bitmaps;
    
    public DDS(byte[] file)
    {
        using MemoryStream stream = new MemoryStream(file);
        using BinaryReader reader = new BinaryReader(stream);
        
        // 'DDS '
        if (reader.ReadUInt32() != 0x20534444)
            throw new InvalidDataException("Given file is not a DDS file (identifier missing).");

        if (reader.ReadUInt32() != 124)
            throw new InvalidDataException("Invalid DDS file (size of structure is not 124).");

        uint flags = reader.ReadUInt32();
        if ((flags & 0x1) != 0x1 || (flags & 0x2) != 0x2 || (flags & 0x4) != 0x4 || (flags & 0x1000) != 0x1000)
        {
            throw new InvalidDataException(
                "Malformed DDS file, flags did not contain required flags (flags were actually " +
                Convert.ToString(flags, 2) + ")");
        }

        bool containsMipmaps = (flags & 0x20000) == 0x20000;
        bool usePitch = (flags & 0x8) == 0x8;

        uint height = reader.ReadUInt32();
        uint width = reader.ReadUInt32();

        uint pitchOrLinearSize = reader.ReadUInt32();

        reader.ReadUInt32(); // depth, not supported

        uint numMipmaps = reader.ReadUInt32();

        #region DDS_PIXELFORMAT
        
        reader.ReadBytes(11 * sizeof(uint)); // 11 byte DWORD reserved

        if (reader.ReadUInt32() != 32)
            throw new InvalidDataException("An error occurred while reading the DDS file (invalid pixel format).");

        uint fFlags = reader.ReadUInt32();
        bool validFourCc = (fFlags & 0x4) == 0x4;
        
        uint fourCc = reader.ReadUInt32();

        uint rgbBitCount = reader.ReadUInt32();
        uint rBitMask = reader.ReadUInt32();
        uint gBitMask = reader.ReadUInt32();
        uint bBitMask = reader.ReadUInt32();
        uint aBitMask = reader.ReadUInt32();

        // TODO: The rest of the header.
        reader.ReadBytes(5 * sizeof(uint));

        Format format = Format.R8G8B8A8_UNorm;

        if (validFourCc)
        {
            FourCCType fourCcType = (FourCCType) fourCc;
            switch (fourCcType)
            {
                case FourCCType.Dxt1:
                    format = Format.BC1_UNorm;
                    break;
                case FourCCType.Dxt2:
                    throw new NotSupportedException("DXT2 compressed textures are not supported.");
                    break;
                case FourCCType.Dxt3:
                    format = Format.BC2_UNorm;
                    break;
                case FourCCType.Dxt4:
                    throw new NotSupportedException("DXT4 compressed textures are not supported.");
                    break;
                case FourCCType.Dxt5:
                    format = Format.BC3_UNorm;
                    break;
                case FourCCType.Bc4U:
                    format = Format.BC4_UNorm;
                    break;
                case FourCCType.Bc4S:
                    format = Format.BC4_SNorm;
                    break;
                case FourCCType.Bc5U:
                    format = Format.BC5_UNorm;
                    break;
                case FourCCType.Bc5S:
                    format = Format.BC5_SNorm;
                    break;
                case FourCCType.Dx10:
                    uint dxgiFormat = reader.ReadUInt32();

                    format = dxgiFormat switch
                    {
                        2 => Format.R32G32B32A32_Float,
                        3 => Format.R32G32B32A32_UInt,
                        4 => Format.R32G32B32A32_SInt,
                        6 => Format.R32G32B32_Float,
                        7 => Format.R32G32B32_UInt,
                        8 => Format.R32G32B32_SInt,
                        10 => Format.R16G16B16A16_Float,
                        11 => Format.R16G16B16A16_UNorm,
                        12 => Format.R16G16B16A16_UInt,
                        13 => Format.R16G16B16A16_SNorm,
                        14 => Format.R16G16B16A16_SInt,
                        16 => Format.R32G32_Float,
                        17 => Format.R32G32_UInt,
                        18 => Format.R32G32_SInt,
                        28 => Format.R8G8B8A8_UNorm,
                        29 => Format.R8G8B8A8_UNorm_SRgb,
                        30 => Format.R8G8B8A8_UInt,
                        31 => Format.R8G8B8A8_SNorm,
                        32 => Format.R8G8B8A8_SInt,
                        34 => Format.R16G16_Float,
                        35 => Format.R16G16_UNorm,
                        36 => Format.R16G16_UInt,
                        37 => Format.R16G16_SNorm,
                        38 => Format.R16G16_SInt,
                        40 => Format.D32_Float,
                        41 => Format.R32_Float,
                        42 => Format.R32_UInt,
                        43 => Format.R32_SInt,
                        45 => Format.D24_UNorm_S8_UInt,
                        49 => Format.R8G8_UNorm,
                        50 => Format.R8G8_UInt,
                        51 => Format.R8G8_SNorm,
                        52 => Format.R8G8_SInt,
                        54 => Format.R16_Float,
                        55 => Format.D16_UNorm,
                        56 => Format.R16_UNorm,
                        57 => Format.R16_UInt,
                        58 => Format.R16_SNorm,
                        59 => Format.R16_SInt,
                        61 => Format.R8_UNorm,
                        62 => Format.R8_UInt,
                        63 => Format.R8_SNorm,
                        64 => Format.R8_SInt,
                        71 => Format.BC1_UNorm,
                        72 => Format.BC1_UNorm_SRgb,
                        74 => Format.BC2_UNorm,
                        75 => Format.BC2_UNorm_SRgb,
                        77 => Format.BC3_UNorm,
                        78 => Format.BC3_UNorm_SRgb,
                        80 => Format.BC4_UNorm,
                        81 => Format.BC4_SNorm,
                        83 => Format.BC5_UNorm,
                        84 => Format.BC5_SNorm,
                        87 => Format.B8G8R8A8_UNorm,
                        91 => Format.B8G8R8A8_UNorm_SRgb,
                        95 => Format.BC6H_UF16,
                        96 => Format.BC6H_SF16,
                        98 => Format.BC7_UNorm,
                        99 => Format.BC7_UNorm_SRgb,
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    uint resourceDimension = reader.ReadUInt32();
                    uint miscFlag = reader.ReadUInt32();
                    uint arraySize = reader.ReadUInt32();
                    uint miscFlags2 = reader.ReadUInt32();
                    break;
                case FourCCType.Rgba16UNorm:
                    format = Format.R16G16B16A16_UNorm;
                    break;
                case FourCCType.Rgba16SNorm:
                    format = Format.R16G16B16A16_SNorm;
                    break;
                case FourCCType.R16Float:
                    format = Format.R16_Float;
                    break;
                case FourCCType.R16G16Float:
                    format = Format.R16G16_Float;
                    break;
                case FourCCType.R16G16B16A16Float:
                    format = Format.R16G16B16A16_Float;
                    break;
                case FourCCType.R32Float:
                    format = Format.R32_Float;
                    break;
                case FourCCType.R32G32Float:
                    format = Format.R32G32_Float;
                    break;
                case FourCCType.R32G32B32A32Float:
                    format = Format.R32G32B32A32_Float;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("fourCcType", fourCcType, null);
            }
        }
        else
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            bool IsBitmask(uint r, uint g, uint b, uint a) =>
                rBitMask == r && gBitMask == g && bBitMask == b && aBitMask == a;

            NotSupportedException InvalidRgbException() => new NotSupportedException(
                $"{rgbBitCount} bpp bitmask R: {Convert.ToString(rBitMask, 16)}, G: {Convert.ToString(bBitMask, 16)}, B: {Convert.ToString(gBitMask, 16)}, A: {Convert.ToString(aBitMask, 16)} is not supported.");
            
            switch (rgbBitCount)
            {
                case 32:
                    if (IsBitmask(0xFF, 0xFF00, 0xFF0000, 0xFF000000))
                        format = Format.R8G8B8A8_UNorm;
                    else if (IsBitmask(0xFF0000, 0xFF00, 0xFF, 0xFF000000))
                        format = Format.B8G8R8A8_UNorm;
                    else
                        throw InvalidRgbException();

                    break;
                default:
                    throw InvalidRgbException();
            }
        }

        //bool isCompressed = format is >= Format.BC1_UNorm and <= Format.BC7_UNorm_SRgb;
        
        #endregion

        uint size = uint.Max(1, (width + 3) / 4) * uint.Max(1, (height + 3) / 4) * 16;

        MipLevels = (int) numMipmaps;
        Size = new Size((int) width, (int) height);
        
        Bitmaps = new byte[1][][];

        for (int i = 0; i < Bitmaps.Length; i++)
        {
            Bitmaps[i] = new byte[numMipmaps][];

            uint w = width;
            uint h = height;
            uint pLs = usePitch ? pitchOrLinearSize * width : pitchOrLinearSize;
            for (int m = 0; m < numMipmaps; m++)
            {
                Bitmaps[i][m] = reader.ReadBytes((int) pLs);

                w /= 2;
                h /= 2;
                pLs /= 4;
            }
        }
    }

    private enum FourCCType : uint
    {
        Dxt1 = 0x31545844,
        
        Dxt2 = 0x32545844,
        
        Dxt3 = 0x33545844,
        
        Dxt4 = 0x34545844,
        
        Dxt5 = 0x35545844,
        
        Bc4U = 0x55344342,
        
        Bc4S = 0x53344342,
        
        Bc5U = 0x55354342,
        
        Bc5S = 0x53354342,

        Dx10 = 0x30315844,
        
        Rgba16UNorm = 36,
        
        Rgba16SNorm = 110,
        
        R16Float = 111,
        
        R16G16Float = 112,
        
        R16G16B16A16Float = 113,
        
        R32Float = 114,
        
        R32G32Float = 115,
        
        R32G32B32A32Float = 116
    }
}