// Copyright (c) 2024 César Morgan
// Copyright (c) 2023 Nicholas Hayes
// SPDX-License-Identifier: MIT
//
// Portions of this file have been adapted from zlib version 1.2.3
/*
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

namespace TheXDS.Vivianne;

/// <summary>
/// Implements a codec that can read and write QFS files.
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
public class QfsCodec
{
    private const ushort QFS_Signature = 0xFB10;

    /// <summary>
    /// Gets a value that indicates if the raw file contents are compressed.
    /// </summary>
    /// <param name="entryData"></param>
    /// <returns></returns>
    public static bool IsCompressed(byte[] entryData)
    {
        return entryData.Length > 2 && BitConverter.ToUInt16(entryData, 0) == QFS_Signature;
    }

    /// <summary>
    /// Decompress data compressed with QFS/RefPack compression.
    /// </summary>
    /// <param name="sourceBytes">Compressed data array</param>
    /// <returns>Decompressed data array</returns>
    /// <example>
    /// <c>
    /// // Load save game
    /// SC4SaveFile savegame = new SC4SaveFile(@"C:\Path\To\Save\Game.sc4");
    ///
    /// // Read raw data for Region View Subfile from save
    /// byte[] data = sc4Save.LoadIndexEntryRaw(REGION_VIEW_SUBFILE_TGI);
    ///
    /// // Decompress data (This file will normally be compressed, should ideally check before decompressing)
    /// byte[] decompressedData = QFS.UncompressData(data);
    /// </c>
    /// </example>
    /// <exception cref="System.IndexOutOfRangeException">
    /// Thrown when the compression algorithm tries to access an element that is out of bounds in the array
    /// </exception>
    public static byte[] Decompress(byte[] sourceBytes)
    {
        if (!IsCompressed(sourceBytes)) return sourceBytes;        
        int destinationPosition = 0;
        byte[] header = new byte[5];
        Buffer.BlockCopy(sourceBytes, 0, header, 0, 5);
        uint uncompressedSize = Convert.ToUInt32((long)(header[2] << 16) + (header[3] << 8) + header[4]);
        byte[] destinationBytes = new byte[uncompressedSize];

        int sourcePosition = 5;

        byte ctrlByte1;
        byte ctrlByte2;
        byte ctrlByte3;
        byte ctrlByte4;
        int length;
        int offset;

        while ((sourcePosition < sourceBytes.Length) && (sourceBytes[sourcePosition] < 0xFC))
        {
            ctrlByte1 = sourceBytes[sourcePosition];
            ctrlByte2 = sourceBytes[sourcePosition + 1];
            ctrlByte3 = sourceBytes[sourcePosition + 2];
            if ((ctrlByte1 & 0x80) == 0)
            {
                length = ctrlByte1 & 3;
                LZCompliantCopy(ref sourceBytes, sourcePosition + 2, ref destinationBytes, destinationPosition, length);
                sourcePosition += length + 2;
                destinationPosition += length;
                length = ((ctrlByte1 & 0x1C) >> 2) + 3;
                offset = ((ctrlByte1 >> 5) << 8) + ctrlByte2 + 1;
                LZCompliantCopy(ref destinationBytes, destinationPosition - offset, ref destinationBytes, destinationPosition, length);

                destinationPosition += length;
            }
            else if ((ctrlByte1 & 0x40) == 0)
            {
                length = (ctrlByte2 >> 6) & 3;
                LZCompliantCopy(ref sourceBytes, sourcePosition + 3, ref destinationBytes, destinationPosition, length);
                sourcePosition += length + 3;
                destinationPosition += length;
                length = (ctrlByte1 & 0x3F) + 4;
                offset = ((ctrlByte2 & 0x3F) * 256) + ctrlByte3 + 1;
                LZCompliantCopy(ref destinationBytes, destinationPosition - offset, ref destinationBytes, destinationPosition, length);

                destinationPosition += length;
            }
            else if ((ctrlByte1 & 0x20) == 0)
            {
                ctrlByte4 = sourceBytes[sourcePosition + 3];
                length = ctrlByte1 & 3;
                LZCompliantCopy(ref sourceBytes, sourcePosition + 4, ref destinationBytes, destinationPosition, length);
                sourcePosition += length + 4;
                destinationPosition += length;
                length = (((ctrlByte1 >> 2) & 3) * 256) + ctrlByte4 + 5;
                offset = ((ctrlByte1 & 0x10) << 12) + (256 * ctrlByte2) + ctrlByte3 + 1;
                LZCompliantCopy(ref destinationBytes, destinationPosition - offset, ref destinationBytes, destinationPosition, length);
                destinationPosition += length;
            }
            else
            {
                length = ((ctrlByte1 & 0x1F) * 4) + 4;
                LZCompliantCopy(ref sourceBytes, sourcePosition + 1, ref destinationBytes, destinationPosition, length);

                sourcePosition += length + 1;
                destinationPosition += length;
            }
        }
        if ((sourcePosition < sourceBytes.Length) && (destinationPosition < destinationBytes.Length))
        {
            LZCompliantCopy(ref sourceBytes, sourcePosition + 1, ref destinationBytes, destinationPosition, sourceBytes[sourcePosition] & 3);
            _ = sourceBytes[sourcePosition] & 3;
        }
        return destinationBytes;
    }

    /// <summary>
    /// Compress data with QFS/RefPack compression.
    /// </summary>
    /// <param name="dData">Data to compress</param>
    /// <returns>Compressed data array</returns>
    /// <exception cref="Exception">If error occurred during compression</exception>
    public static byte[] Compress(byte[] dData)
    {
        if (IsCompressed(dData))
        {
            return dData;
        }
        const int QFS_MAXITER = 50;
        const int WINDOW_SIZE = 1 << 17;
        const int WINDOW_MASK = WINDOW_SIZE - 1;
        int[] rev_similar = new int[WINDOW_SIZE];
        int[,] rev_last = new int[256, 256];
        int dPos;
        byte[] cData = new byte[dData.Length + 1028];
        cData[0] = 0x10;
        cData[1] = 0xFB;
        cData[2] = (byte)((dData.Length >> 16) & 0xff);
        cData[3] = (byte)((dData.Length >> 8) & 0xff);
        cData[4] = (byte)(dData.Length & 0xff);
        int cPos = 5;
        int bestLength, matchLength, offset, bestoffset = default;
        int lastwrote = 0;
        int x, idx;
        int tmpx, tmpy;
        for (dPos = 0; dPos < dData.Length - 1; dPos++)
        {
            x = rev_last[dData[dPos], dData[dPos + 1]] - 1;
            rev_similar[dPos & WINDOW_MASK] = x + 1;
            tmpx = dData[dPos];
            tmpy = dData[dPos + 1];
            rev_last[tmpx, tmpy] = dPos + 1;
            offset = x;
            if (dPos >= lastwrote)
            {
                bestLength = 0;
                idx = 0;
                while (offset >= 0 & dPos - offset < WINDOW_SIZE & idx < QFS_MAXITER)
                {
                    matchLength = 2;
                    while (dData[dPos + matchLength] == dData[offset + matchLength] && matchLength < 1028)
                    {
                        matchLength++;
                        if (dPos + matchLength >= dData.Length)
                        {
                            break;
                        }
                    }
                    if (matchLength > bestLength)
                    {
                        bestLength = matchLength;
                        bestoffset = dPos - offset;
                    }
                    offset = rev_similar[offset & WINDOW_MASK] - 1;
                    idx++;
                }
                if (bestLength > dData.Length - dPos)
                    bestLength = 0;
                if (bestLength <= 2)
                {
                    bestLength = 0;
                }
                else if (bestLength == 3 & bestoffset > 1024)
                {
                    bestLength = 0;
                }
                else if (bestLength == 4 & bestoffset > 16384)
                {
                    bestLength = 0;
                }
                if (bestLength != 0)
                {
                    while (dPos - lastwrote >= 4)
                    {
                        matchLength = ((dPos - lastwrote) / 4) - 1;
                        if (matchLength > 0x1B)
                            matchLength = 0x1B;
                        cData[cPos] = (byte)(0xE0 + matchLength);
                        cPos++;
                        matchLength = (4 * matchLength) + 4;
                        SlowMemCopy(cData, cPos, dData, lastwrote, matchLength);
                        lastwrote += matchLength;
                        cPos += matchLength;
                    }
                    matchLength = dPos - lastwrote;
                    if (bestLength <= 10 && bestoffset <= 1024)
                    {
                        cData[cPos] = (byte)(((bestoffset - 1) / 256 * 32) + ((bestLength - 3) * 4) + matchLength);
                        cPos++;
                        cData[cPos] = (byte)((bestoffset - 1) & 0xFF);
                        cPos++;
                        SlowMemCopy(cData, cPos, dData, lastwrote, matchLength);
                        lastwrote += matchLength;
                        cPos += matchLength;
                        lastwrote += bestLength;
                    }
                    else if (bestLength <= 67 && bestoffset <= 16384)
                    {
                        cData[cPos] = (byte)(0x80 + (bestLength - 4));
                        cPos++;
                        cData[cPos] = (byte)((matchLength * 64) + ((bestoffset - 1) / 256));
                        cPos++;
                        cData[cPos] = (byte)((bestoffset - 1) & 0xFF);
                        cPos++;
                        SlowMemCopy(cData, cPos, dData, lastwrote, matchLength);
                        lastwrote += matchLength;
                        cPos += matchLength;
                        lastwrote += bestLength;
                    }
                    else if (bestLength <= 1028 && bestoffset < WINDOW_SIZE)
                    {
                        bestoffset--;
                        cData[cPos] = (byte)(0xC0 + (bestoffset / 65536 * 16) + ((bestLength - 5) / 256 * 4) + matchLength);
                        cPos++;
                        cData[cPos] = (byte)((bestoffset / 256) & 0xFF);
                        cPos++;
                        cData[cPos] = (byte)(bestoffset & 0xFF);
                        cPos++;
                        cData[cPos] = (byte)((bestLength - 5) & 0xFF);
                        cPos++;
                        SlowMemCopy(cData, cPos, dData, lastwrote, matchLength);
                        lastwrote += matchLength;
                        cPos += matchLength;
                        lastwrote += bestLength;
                    }
                }
            }
        }
        dPos = dData.Length;
        while (dPos - lastwrote >= 4)
        {
            matchLength = (dPos - lastwrote) / (4 - 1);
            if (matchLength > 0x1B)
            {
                matchLength = 0x1B;
            }
            cData[cPos] = (byte)(0xE0 + matchLength);
            cPos++;
            matchLength = (4 * matchLength) + 4;
            SlowMemCopy(cData, cPos, dData, lastwrote, matchLength);
            lastwrote += matchLength;
            cPos += matchLength;
        }
        matchLength = dPos - lastwrote;
        cData[cPos] = (byte)(0xFC + matchLength);
        cPos++;
        SlowMemCopy(cData, cPos, dData, lastwrote, matchLength);
        lastwrote += matchLength;
        cPos += matchLength;
        if (lastwrote != dData.Length)
        {
            throw new Exception("Something strange happened at the end of QFS compression!");
        }
        Array.Resize(ref cData, cPos);
        return cData;
    }

    private static void SlowMemCopy(byte[] dst, int dstptr, byte[] src, int srcptr, int nbytes)
    {
        // NOTE:: DO NOT change this into a system call, the nature of QFS means that it MUST work byte for byte (internal overlaps possible) and in any case this is fast
        while (nbytes > 0)
        {
            dst[dstptr] = src[srcptr];
            dstptr++;
            srcptr++;
            nbytes--;
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
}
