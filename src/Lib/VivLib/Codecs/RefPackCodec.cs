/*
Copyright (c) 2024 César Morgan
Copyright (c) 2023 Nicholas Hayes
SPDX-License-Identifier: MIT

Portions of this file have been adapted from zlib version 1.2.3

zlib.h -- interface of the 'zlib' general purpose compression library
version 1.2.3, July 18th, 2005

Copyright (C) 1995-2005 Jean-loup Gailly and Mark Adler

This software is provided 'as-is', without any express or implied
warranty.  In no event will the authors be held liable for any damages
arising from the use of this software.

Permission is granted to anyone to use this software for any purpose,
including commercial applications, and to alter it and redistribute it
freely, subject to the following restrictions:

1. The origin of this software must not be misrepresented; you must not
   claim that you wrote the original software. If you use this software
   in a product, an acknowledgment in the product documentation would be
   appreciated but is not required.
2. Altered source versions must be plainly marked as such, and must not be
   misrepresented as being the original software.
3. This notice may not be removed or altered from any source distribution.

Jean-loup Gailly        Mark Adler
jloup@gzip.org          madler@alumni.caltech.edu
*/

using St = TheXDS.Vivianne.Resources.Strings.QfsCodec;

namespace TheXDS.Vivianne.Codecs;

/// <summary>
/// Implements a generic codec that can read and write data using LZ
/// compression.
/// </summary>
/// <remarks>
/// Portions of this file have been adapted from zlib 1.2.3 <br/>
/// Original zlib implementation by:
/// <list type="bullet">
/// <item>Jean-loup Gailly (Mark Adler)</item>
/// <item>jloup@gzip.org (madler@alumni.caltech.edu)</item>
/// </list>
/// QFS compression and decompression routines by:
/// <list type="bullet">
/// <item>Nicholas Hayes</item>
/// </list>
/// Minor cleanup / Need For Speed compatibility by:
/// <list type="bullet">
/// <item>César Morgan (xds_xps_ivx@hotmail.com)</item>
/// </list>
/// </remarks>
public static class RefPackCodec
{
    private const ushort LZ_Signature = 0xFB10;
    private static readonly IEnumerable<KeyValuePair<int?, BlockDecompression>> BlockDecompressors = [
        new(0x80, ReadSmallBlock),
        new(0x40, ReadMediumBlock),
        new(0x20, ReadLargeBlock),
        new(null, ReadRawBlock)
        ];
    private static readonly IEnumerable<KeyValuePair<(int bestLength, int bestOffset), BlockCompression>> BlockCompressors = [
        new((10, 1024), WriteSmallBlock),
        new((67, 16384), WriteMediumBlock),
        new((1028, 1 << 17), WriteLargeBlock)
        ];

    /// <summary>
    /// Gets a value that indicates if the raw file contents are compressed.
    /// </summary>
    /// <param name="entryData">Contents to be verified.</param>
    /// <returns>
    /// <see langword="true"/> if the raw byte array appears to contain
    /// compressed data; <see langword="false"/> otherwise.
    /// </returns>
    public static bool IsCompressed(byte[] entryData)
    {
        return entryData.Length >= 2 && BitConverter.ToUInt16(entryData, 0) == LZ_Signature;
    }

    /// <summary>
    /// Gets a value that indicates if the raw file contents are compressed.
    /// </summary>
    /// <param name="stream">Stream with the contents to be verified.</param>
    /// <returns>
    /// <see langword="true"/> if the stream appears to contain
    /// compressed data; <see langword="false"/> otherwise.
    /// </returns>
    public static bool IsCompressed(Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        byte[] magic = [(byte)stream.ReadByte(), (byte)stream.ReadByte()];
        var result = stream.Length > 2 && IsCompressed(magic);
        stream.Seek(0, SeekOrigin.Begin);
        return result;
    }

    /// <summary>
    /// Decompress data compressed with RefPack compression.
    /// </summary>
    /// <param name="sourceBytes">Compressed data array</param>
    /// <returns>Decompressed data array</returns>
    /// <example>
    /// <c>
    /// // Decompress data (This file will normally be compressed, should ideally check before decompressing)
    /// byte[] decompressedData = LzCodec.Decompress(data);
    /// </c>
    /// </example>
    public static byte[] Decompress(byte[] sourceBytes)
    {
        if (!IsCompressed(sourceBytes)) return sourceBytes;
        int destinationPosition = 0;
        byte[] destinationBytes = CreateDecompressArray(sourceBytes);
        int sourcePosition = 5;
        while (sourcePosition < sourceBytes.Length && sourceBytes[sourcePosition] < 0xFC)
        {
            DecompressBlock(ref sourceBytes, ref sourcePosition, ref destinationBytes, ref destinationPosition);
        }
        if (sourcePosition < sourceBytes.Length && destinationPosition < destinationBytes.Length)
        {
            int length = sourceBytes[sourcePosition] & 3;
            LZCompliantCopy(ref sourceBytes, sourcePosition + 1, ref destinationBytes, destinationPosition, length);
            destinationPosition += length;
        }
        return destinationBytes;
    }

    /// <summary>
    /// Compress data with RefPack compression.
    /// </summary>
    /// <param name="dData">Data to compress</param>
    /// <returns>Compressed data array</returns>
    public static byte[] Compress(byte[] dData)
    {
        if (IsCompressed(dData)) return dData;
        const int QFS_MAXITER = 50;
        const int WINDOW_SIZE = 1 << 17;
        const int WINDOW_MASK = WINDOW_SIZE - 1;
        int[] rev_similar = new int[WINDOW_SIZE];
        int[,] rev_last = new int[256, 256];
        byte[] cData = InitializeCompressedData(dData);
        int cPos = 5;
        int lastwrote = 0;
        for (int dPos = 0; dPos < dData.Length - 1; dPos++)
        {
            UpdateReverseLast(rev_last, dData, dPos);
            int offset = rev_last[dData[dPos], dData[dPos + 1]] - 1;
            rev_similar[dPos & WINDOW_MASK] = offset + 1;

            if (dPos >= lastwrote)
            {
                (int bestLength, int bestOffset) = FindBestMatch(dData, rev_similar, dPos, offset, QFS_MAXITER);
                ProcessBestMatch(cData, ref cPos, dData, ref lastwrote, bestLength, bestOffset, dPos);
            }
        }
        FinalizeCompression(cData, dData, ref lastwrote, ref cPos);
        return cData;
    }

    private static byte[] InitializeCompressedData(byte[] dData)
    {
        byte[] cData = new byte[dData.Length + 1028];
        cData[0] = 0x10;
        cData[1] = 0xFB;
        cData[2] = (byte)((dData.Length >> 16) & 0xff);
        cData[3] = (byte)((dData.Length >> 8) & 0xff);
        cData[4] = (byte)(dData.Length & 0xff);
        return cData;
    }

    private static void UpdateReverseLast(int[,] rev_last, byte[] dData, int dPos)
    {
        rev_last[dData[dPos], dData[dPos + 1]] = dPos + 1;
    }

    private static (int bestLength, int bestOffset) FindBestMatch(byte[] dData, int[] rev_similar, int dPos, int offset, int QFS_MAXITER)
    {
        int bestLength = 0;
        int bestOffset = default;
        int idx = 0;

        while (offset >= 0 && dPos - offset < rev_similar.Length && idx < QFS_MAXITER)
        {
            int matchLength = FindMatchLength(dData, dPos, offset);
            if (matchLength > bestLength)
            {
                bestLength = matchLength;
                bestOffset = dPos - offset;
            }
            offset = rev_similar[offset & (rev_similar.Length - 1)] - 1;
            idx++;
        }

        return (bestLength, bestOffset);
    }

    private static int FindMatchLength(byte[] dData, int dPos, int offset)
    {
        int matchLength = 2;
        while (dData[dPos + matchLength] == dData[offset + matchLength] && matchLength < 1028)
        {
            matchLength++;
            if (dPos + matchLength >= dData.Length)
            {
                break;
            }
        }
        return matchLength;
    }

    private static void ProcessBestMatch(byte[] cData, ref int cPos, byte[] dData, ref int lastwrote, int bestLength, int bestOffset, int dPos)
    {
        if (bestLength > dData.Length - dPos || bestLength <= 2) return;
        while (dPos - lastwrote >= 4)
        {
            WriteRawBlock(ref dPos, ref cData, ref cPos, ref dData, ref lastwrote);
        }
        int remainingMatchLength = dPos - lastwrote;
        if (BlockCompressors.FirstOrDefault(p => bestLength <= p.Key.bestLength && bestOffset <= p.Key.bestOffset) is { Value: var compressor })
        {
            compressor(ref cData, ref cPos, ref dData, ref lastwrote, ref bestLength, ref bestOffset, ref remainingMatchLength);
        }
        else
        {
            throw new InvalidOperationException(St.CompressionBlockSizeError);
        }
    }

    private static void FinalizeCompression(byte[] cData, byte[] dData, ref int lastwrote, ref int cPos)
    {
        int dPos = dData.Length;
        while (dPos - lastwrote >= 4)
        {
            int matchLength = (dPos - lastwrote) / (4 - 1);
            if (matchLength > 0x1B)
            {
                matchLength = 0x1B;
            }
            cData[cPos++] = (byte)(0xE0 + matchLength);
            matchLength = (4 * matchLength) + 4;
            SlowMemCopy(cData, cPos, dData, lastwrote, matchLength);
            lastwrote += matchLength;
            cPos += matchLength;
        }

        int remainingMatchLength = dPos - lastwrote;
        cData[cPos++] = (byte)(0xFC + remainingMatchLength);
        SlowMemCopy(cData, cPos, dData, lastwrote, remainingMatchLength);
        lastwrote += remainingMatchLength;
        cPos += remainingMatchLength;

        if (lastwrote != dData.Length)
        {
            throw new Exception(St.QFSCompressionSizeMismatch);
        }

        Array.Resize(ref cData, cPos);
    }

    private static byte[] CreateDecompressArray(byte[] sourceBytes)
    {
        byte[] header = new byte[5];
        Buffer.BlockCopy(sourceBytes, 0, header, 0, 5);
        uint uncompressedSize = Convert.ToUInt32((long)(header[2] << 16) + (header[3] << 8) + header[4]);
        return new byte[uncompressedSize];
    }

    private static void DecompressBlock(ref byte[] sourceBytes, ref int sourcePosition, ref byte[] destinationBytes, ref int destinationPosition)
    {
        var controlByte = sourceBytes[sourcePosition];
        if (BlockDecompressors.FirstOrDefault(p => (controlByte & p.Key) == 0 || p.Key is null) is { Value: var decompressor })
        {
            decompressor(sourceBytes, ref destinationBytes, ref sourcePosition, ref destinationPosition);
        }
        else
        {
            throw new InvalidDataException(St.Decompress_InvalidControlByte);
        }
    }

    private static void LZCompliantCopy(ref byte[] source, int sourceOffset, ref byte[] destination, int destinationOffset, int length)
    {
        if (length != 0)
        {
            for (int i = 0; i < length; i++)
            {
                Buffer.BlockCopy(source, sourceOffset, destination, destinationOffset, 1);
                sourceOffset++;
                destinationOffset++;
            }
        }
    }

    private static void ReadRawBlock(byte[] sourceBytes, ref byte[] destinationBytes, ref int sourcePosition, ref int destinationPosition)
    {
        byte ctrlByte1 = sourceBytes[sourcePosition++];
        int length = ((ctrlByte1 & 0x1F) * 4) + 4;
        LZCompliantCopy(ref sourceBytes, sourcePosition, ref destinationBytes, destinationPosition, length);
        sourcePosition += length;
        destinationPosition += length;
    }

    private static void ReadLargeBlock(byte[] sourceBytes, ref byte[] destinationBytes, ref int sourcePosition, ref int destinationPosition)
    {
        byte ctrlByte1 = sourceBytes[sourcePosition++];
        byte ctrlByte2 = sourceBytes[sourcePosition++];
        byte ctrlByte3 = sourceBytes[sourcePosition++];
        byte ctrlByte4 = sourceBytes[sourcePosition++];

        int length = ctrlByte1 & 3;
        LZCompliantCopy(ref sourceBytes, sourcePosition, ref destinationBytes, destinationPosition, length);
        sourcePosition += length;
        destinationPosition += length;

        length = (((ctrlByte1 >> 2) & 3) * 256) + ctrlByte4 + 5;
        int offset = ((ctrlByte1 & 0x10) << 12) + (256 * ctrlByte2) + ctrlByte3 + 1;
        LZCompliantCopy(ref destinationBytes, destinationPosition - offset, ref destinationBytes, destinationPosition, length);
        destinationPosition += length;
    }

    private static void ReadMediumBlock(byte[] sourceBytes, ref byte[] destinationBytes, ref int sourcePosition, ref int destinationPosition)
    {
        byte ctrlByte1 = sourceBytes[sourcePosition++];
        byte ctrlByte2 = sourceBytes[sourcePosition++];
        byte ctrlByte3 = sourceBytes[sourcePosition++];

        int length = (ctrlByte2 >> 6) & 3;
        LZCompliantCopy(ref sourceBytes, sourcePosition, ref destinationBytes, destinationPosition, length);
        sourcePosition += length;
        destinationPosition += length;

        length = (ctrlByte1 & 0x3F) + 4;
        int offset = ((ctrlByte2 & 0x3F) * 256) + ctrlByte3 + 1;
        LZCompliantCopy(ref destinationBytes, destinationPosition - offset, ref destinationBytes, destinationPosition, length);
        destinationPosition += length;
    }

    private static void ReadSmallBlock(byte[] sourceBytes, ref byte[] destinationBytes, ref int sourcePosition, ref int destinationPosition)
    {
        byte ctrlByte1 = sourceBytes[sourcePosition++];
        byte ctrlByte2 = sourceBytes[sourcePosition++];

        int length = ctrlByte1 & 3;
        LZCompliantCopy(ref sourceBytes, sourcePosition, ref destinationBytes, destinationPosition, length);
        sourcePosition += length;
        destinationPosition += length;

        length = ((ctrlByte1 & 0x1C) >> 2) + 3;
        int offset = (ctrlByte1 >> 5 << 8) + ctrlByte2 + 1;
        LZCompliantCopy(ref destinationBytes, destinationPosition - offset, ref destinationBytes, destinationPosition, length);
        destinationPosition += length;
    }

    private static void SlowMemCopy(byte[] dst, int dstptr, byte[] src, int srcptr, int nbytes)
    {
        /* NOTE:
         * DO NOT change this into a system call, the nature of this kind of
         * compression means that it MUST work byte for byte (internal overlaps
         * are possible) and in any case this is fast.
         */
        while (nbytes > 0)
        {
            dst[dstptr] = src[srcptr];
            dstptr++;
            srcptr++;
            nbytes--;
        }
    }

    private static void WriteSmallBlock(ref byte[] cData, ref int cPos, ref byte[] dData, ref int lastwrote, ref int bestLength, ref int bestOffset, ref int remainingMatchLength)
    {
        cData[cPos++] = (byte)(((bestOffset - 1) / 256 * 32) + ((bestLength - 3) * 4) + remainingMatchLength);
        cData[cPos++] = (byte)((bestOffset - 1) & 0xFF);
        SlowMemCopy(cData, cPos, dData, lastwrote, remainingMatchLength);
        lastwrote += remainingMatchLength;
        cPos += remainingMatchLength;
        lastwrote += bestLength;
    }

    private static void WriteMediumBlock(ref byte[] cData, ref int cPos, ref byte[] dData, ref int lastwrote, ref int bestLength, ref int bestOffset, ref int remainingMatchLength)
    {
        cData[cPos++] = (byte)(0x80 + (bestLength - 4));
        cData[cPos++] = (byte)((remainingMatchLength * 64) + ((bestOffset - 1) / 256));
        cData[cPos++] = (byte)((bestOffset - 1) & 0xFF);
        SlowMemCopy(cData, cPos, dData, lastwrote, remainingMatchLength);
        lastwrote += remainingMatchLength;
        cPos += remainingMatchLength;
        lastwrote += bestLength;
    }

    private static void WriteLargeBlock(ref byte[] cData, ref int cPos, ref byte[] dData, ref int lastwrote, ref int bestLength, ref int bestOffset, ref int remainingMatchLength)
    {
        bestOffset--;
        cData[cPos++] = (byte)(0xC0 + (bestOffset / 65536 * 16) + ((bestLength - 5) / 256 * 4) + remainingMatchLength);
        cData[cPos++] = (byte)((bestOffset / 256) & 0xFF);
        cData[cPos++] = (byte)(bestOffset & 0xFF);
        cData[cPos++] = (byte)((bestLength - 5) & 0xFF);
        SlowMemCopy(cData, cPos, dData, lastwrote, remainingMatchLength);
        lastwrote += remainingMatchLength;
        cPos += remainingMatchLength;
        lastwrote += bestLength;
    }

    private static void WriteRawBlock(ref int dPos, ref byte[] cData, ref int cPos, ref byte[] dData, ref int lastwrote)
    {
        int matchLength = ((dPos - lastwrote) / 4) - 1;
        if (matchLength > 0x1B) matchLength = 0x1B;
        cData[cPos++] = (byte)(0xE0 + matchLength);
        matchLength = (4 * matchLength) + 4;
        SlowMemCopy(cData, cPos, dData, lastwrote, matchLength);
        lastwrote += matchLength;
        cPos += matchLength;
    }

    private delegate void BlockDecompression(byte[] sourceBytes, ref byte[] destinationBytes, ref int sourcePosition, ref int destinationPosition);
    private delegate void BlockCompression(ref byte[] cData, ref int cPos, ref byte[] dData, ref int lastwrote, ref int bestLength, ref int bestOffset, ref int remainingMatchLength);
}
