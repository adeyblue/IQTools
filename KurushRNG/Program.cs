using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

using System.Diagnostics;
using System.Threading;

namespace KurushRNG
{
    class Program
    {
        #region TRNs
        static class TRNs
        {
            public static int[] g_4x2TRNs = new int[] {4,3,2,2,2,3,2,2,2,2,2,2,2,3,2,2,4,2,2,2,3,2,1,2,1,1,3,
            2,5,5,2,1,4,2,1,1,3,2,2,2,1,3,4,2,1,1,8,1,1,2,3,3,3,3,3,1,1,1,2,2,3,3,1,2,3,1,2,2,1,1,
            1,2,3,1,1,2,1,1,2,4,1,3,1,3,5,1,1,2,1,1,1,1,2,3,5,4,2,1,3,3,4,2,5,5,1,1,1,3,3,2,1,2,2,
            3,1,2,2,2,3,2,4,2,2,1,1,3,1,2,1,4,2,3,1,5,3,2,2,1,3,1,1,2,2,2,3,1,2,1,1,4,1,3,1,2,1,3,
            2,1,1,2,1,2,1,2,2,3,2,5,2,2,2,1,2,2,3,2,4,3,3,1,5,1,3,2,1,1,1,2,1,2,1,1,3,1,2,2,1,3,2,3};
            public static int[] g_4x3TRNs = new int[] {3,3,3,2,1,2,3,3,3,4,3,2,3,1,3,5,3,5,5,2,1,4,2,5,1,2,2,
            5,4,4,2,3,3,3,3,5,1,3,1,2,2,2,3,2,2,3,1,4,1,2,2,2,4,3,3,3,6,3,2,3,4,1,3,3,6,6,3,3,3,5,
            2,3,2,2,2,1,6,2,2,3,2,4,3,4,5,2,3,3,3,3,2,4,3,3,3,2,3,2,2,4,3,5,6,2,2,3,3,4,2,3,4,2,3,
            4,2,4,2,5,2,1,2,3,3,4,3,2,1,2,2,1,2,3,2,5,4,4,1,3,2,2,2,2,2,1,1,1,3,3,3,2,4,4,3,3,2,5,
            4,3,2,2,4,3,3,2,4,6,2,3,4,4,5,4,2,3,2,2,3,2,4,2,3,2,1,4,3,2,2,3,3,3,2,2,2,2,3,1,2,2,4,2};
            public static int[] g_4x4TRNs = new int[] {1,4,2,2,3,5,1,4,5,5,3,7,3,2,4,3,3,2,3,2,4,4,4,3,3,3,3,
            5,6,3,4,3,3,4,3,3,5,4,3,5,4,5,3,3,2,4,6,4,3,3,3,3,1,2,5,4,3,2,3,4,3,4,4,4,4,3,4,4,3,3,
            3,4,5,3,5,3,5,4,2,3,3,3,4,4,4,3,5,3,2,2,2,3,5,2,3,3,3,2,4,4,5,2,2,3,3,3,3,2,4,2,5,3,4,
            2,2,2,3,2,3,4,4,5,2,3,2,3,2,6,4,2,2,3,2,4,3,3,2,5,2,4,4,3,4,6,4,2,2,5,2,2,3,3,3,3,3,3,
            3,3,6,5,4,2,4,3,2,3,2,2,3,2,5,5,4,2,4,3,5,2,4,2,6,3,4,8,6,5,3,3,2,2,2,5,4,6,4,2,3,4,8,3};
            public static int[] g_4x5TRNs = new int[] {5,4,6,4,6,4,7,6,7,7,5,6,5,5,6,8,5,5,6,3,6,5,7,5,5,4,5,
            6,5,4,5,6,5,4,4,6,5,6,4,3,4,6,8,5,6,4,5,4,8,4,3,7,6,6,6,6,6,6,6,5,7,5,6,4,5,6,4,6,5,5,
            5,3,6,5,4,7,6,7,7,7,8,7,7,6,5,6,5,3,4,7,7,4,5,5,6,6,4,6,6,6,5,6,5,8,6,6,6,3,6,8,5,7,5,
            5,5,6,5,7,6,5,6,6,4,7,6,4,5,4,6,8,6,5,5,8,7,8,4,6,5,4,7,5,5,5,5,4,6,5,5,4,5,7,5,6,6,3,
            4,4,7,6,8,4,5,6,4,5,4,3,4,6,4,5,4,5,5,6,5,4,6,4,7,6,5,5,5,4,4,5,5,7,5,5,7,7,5,5,4,4,5,5};
            public static int[] g_4x6TRNs = new int[] {6,7,8,5,9,5,4,3,5,6,7,8,7,4,7,6,4,7,7,7,6,7,8,9,5,0xA,7,
            8,6,0xA,6,7,5,0xA,7,9,9,8,6,7,8,8,6,5,9,8,9,0xA,0xD,0xC,8,5,9,8,0xA,7,7,0xA,8,7,9,7,7,0xA,
            6,0xA,7,8,6,6,8,6,7,6,5,0xA,8,6,8,6,7,9,9,8,6,7,9,8,9,4,5,6,0xC,7,8,7,0xA,7,7,7,8,4,8,5,8,
            7,0xB,9,8,7,7,6,5,7,6,6,9,6,7,6,6,6,9,9,9,7,5,5,7,7,8,9,0xB,7,5,8,8,7,5,4,7,7,8,8,6,9,9,8,
            0xC,0xB,7,9,0xA,0xA,0xA,7,5,8,8,7,7,7,8,0xA,0xA,9,0xB,8,0xA,8,7,8,7,7,8,8,7,9,8,9,7,5,6,8,
            7,6,8,8,9,0xA,7,7,8,5,0xB,6,5,7,8,7};
            public static int[] g_5x4TRNs = new int[] {3,4,5,7,4,7,7,6,6,4,7,6,7,6,7,7,6,6,5,8,7,7,7,6,6,6,4,8,
            6,6,7,5,7,7,6,6,7,6,8,6,6,5,5,0xB,7,8,0xB,6,7,8,6,6,7,6,5,6,6,8,7,5,8,7,4,5,5,6,9,5,6,6,
            5,8,9,8,3,9,7,8,5,0xA,5,8,7,8,8,7,0xA,7,8,8,7,5,8,5,7,6,6,7,4,5,6,4,7,4,7,9,5,6,5,6,4,4,
            8,6,7,6,7,0xA,7,4,7,4,8,7,7,7,4,0xA,7,5,3,7,4,5,8,5,4,8,6,5,4,5,6,8,6,4,6,7,4,5,6,4,7,4,
            6,6,9,6,6,7,6,7,9,4,7,7,7,5,4,5,6,0xA,9,7,7,9,5,5,4,5,7,9,5,9,0xF,7,5,7,9,7,5,7,6,5,7,5,
            7,6,4,5};
            public static int[] g_5x5TRNs = new int[] {1,0xC,8,7,9,8,4,0xA,6,6,6,6,0xE,5,8,7,5,6,5,0xB,5,9,8,7,
            7,9,6,7,4,6,4,8,9,5,9,7,9,9,6,0xB,7,5,5,5,7,6,7,9,6,9,5,7,7,6,8,7,7,8,9,8,8,8,9,8,7,6,6,
            7,0xB,0xB,8,7,0xA,6,0xB,6,7,7,0xA,7,7,0xA,0xB,8,7,5,0xA,8,6,7,4,6,7,6,6,6,8,9,8,7,8,6,6,
            0xC,5,0xB,0xA,8,0xB,6,8,7,0xA,9,8,9,8,8,9,5,9,0xB,8,9,6,0xA,0xB,0xA,7,8,4,6,0xC,8,8,9,7,
            9,8,8,0xB,8,5,6,9,8,0xA,0xA,9,0xA,0xA,7,8,9,8,5,0xA,7,4,0xA,5,7,0xC,9,3,0xA,7,4,3,7,7,8,
            0xA,6,8,5,9,5,6,6,8,8,5,7,6,4,7,6,8,3,4,7,5,5,6,4,6,4,4,4};
            public static int[] g_5x6TRNs = new int[] {1,8,9,5,7,7,7,6,8,8,8,0xA,8,7,6,9,6,6,5,8,9,7,9,6,6,8,5,
            6,5,5,8,6,8,0xA,5,8,5,8,9,7,9,5,7,6,8,6,7,7,6,9,8,0xA,0xA,7,9,0xA,8,7,7,7,0xA,6,9,8,4,7,
            5,5,9,7,5,7,5,6,9,5,8,7,8,7,6,0xA,8,8,9,8,7,9,8,5,6,6,0xC,9,7,6,8,7,7,9,8,8,7,9,8,6,7,7,
            0xA,9,9,6,6,6,8,8,7,6,8,6,9,8,8,0xA,9,9,5,8,7,5,0xA,0xB,0xA,7,7,6,7,5,0xA,7,9,4,0xA,8,6,
            8,8,7,8,6,8,9,7,9,8,8,7,7,9,0xB,7,8,7,9,7,8,7,9,8,8,6,8,8,9,8,8,7,0xA,8,9,7,6,0xA,0xB,8,
            8,0xB,0xA,6,7,7,9,0xA,8,7,5,7,7,7,7};
            public static int[] g_5x7TRNs = new int[] {1,0xA,0xD,0xB,0xC,0xD,0xB,0xB,8,0xA,0xA,0xB,0xB,8,0xB,6,
            9,9,9,6,6,7,0xA,9,8,7,8,8,0xB,0xA,5,0xD,0xB,8,0xA,0xC,8,0xA,7,8,6,5,6,7,0xD,8,6,8,9,7,0xA,
            6,8,0xA,0xB,7,7,0xB,0xB,0xA,6,8,0xB,0xB,0xB,9,9,9,7,9,8,0xE,9,0xC,0xB,0xA,0xB,8,9,9,8,0xB,
            9,0xE,0xA,0xE,0xF,0xA,9,7,8,8,0xA,8,0xA,7,5,9,0xA,0xB,0xA,0xC,0xC,0xE,0xC,8,7,8,0xC,5,0xA,
            0xE,0xA,0xB,9,0xB,0xC,0xB,0xB,0xF,0xA,9,0xD,0xB,0xD,0xC,0xC,0xA,0xD,0xA,0xA,0xA,0xB,0xD,0xD,
            0x10,0xB,0xC,0xE,7,9,0xE,9,8,0xB,0xA,0x12,0xB,8,0xB,9,0xC,0x10,0xA,0xA,0xE,0xB,0xA,0xB,0xA,
            0xA,0xC,0xB,0xB,0xA,0xB,7,0xA,0xB,0xB,9,0xE,0xE,8,0xD,0xA,0xC,0xB,0xA,0xC,8,0xA,0xA,7,4,0xC,
            0xC,0xA,0xB,0xD,8,0xA,9,0xA,0xB,0xC,0xC,9,0x11,0xC};
            public static int[] g_5x8TRNs = new int[] {1,0xB,0xB,0xD,9,0xB,7,0xC,0xB,0xA,9,0xC,7,0xA,0xB,0xB,0xA,0xB,
            0xA,9,0xD,7,8,0xA,0xA,0xA,8,9,0xF,8,9,0xA,0xA,0xA,0xB,0xB,0xB,7,7,0xA,0xB,0xC,0xD,0xE,0xA,9,0xA,
            0xC,0xC,0xE,0xC,0xB,0xE,7,0xB,8,6,9,6,9,0xC,0xD,6,0xB,0xA,0xB,0xB,7,7,0xD,0xD,0xB,0xB,7,0xA,0xA,
            0xB,8,7,0xD,0xC,0xA,0xA,7,8,0xC,9,9,9,9,0xC,0xA,9,8,0xC,8,0xD,0xB,0xC,0xA,8,0xB,0xD,0xD,9,0xE,0xA,
            9,0xE,0xD,0xE,0xB,0xB,0xC,0xA,0xB,0xB,0xA,7,9,0xA,0xC,0xD,0xA,0xA,0xA,0xC,0xB,0xD,0xA,0xE,0xE,0xD,
            0xC,0xA,0xC,8,9,0xA,0xE,7,0xB,0xB,0xC,0xD,8,0xC,0xD,0xA,0xC,0x11,0xA,0xA,0x12,0xC,0xD,9,0x11,0xD,
            0xD,0xA,0x11,0x12,0xC,0xB,0xC,0xB,0xC,0xA,0xB,0xB,0xD,0xA,0xA,0xE,0xB,0x12,0xD,0xD,9,0xD,0xD,0xA,
            0xB,8,0xE,9,0xB,0xE,0x10,0x11,0xB,7,7,0xB,9,0xC,8,9,0xA};
            public static int[] g_6x6TRNs = new int[] {0xB,0xA,8,8,0xB,8,0xA,0xA,0xA,8,8,9,6,7,0xB,7,9,9,8,0xE,6,0xA,0xA,
            0xB,8,0xA,8,6,0xA,9,6,5,5,7,0xB,0xA,0xA,0xA,7,0xD,0xA,0xB,0xA,0xA,9,0xD,6,9,0xB,0xA,8,8,9,6,8,9,
            0xC,9,0xA,0xB,0xB,9,9,5,9,9,9,8,0xB,8,8,0xA,9,0xA,0xB,0xE,7,0xD,8,0x10,7,7,0xC,8,8,9,9,8,0xF,0xD,
            0xB,0xA,9,8,7,8,9,0xC,0xA,0xD,0xB,0xC,0xB,0xB,0xC,0xA,0xC,7,5,8,0xB,0xB,9,0xE,0xA,0xC,8,0xB,9,0xC,
            6,0xD,0xC,0xD,8,7,0xB,0xA,9,7,9,0xD,9,0xA,0xD,0xE,0xC,0xB,7,8,0xA,0xD,0xC,0xD,9,7,0xA,8,9,6,8,8,8,
            0xB,0xC,9,7,9,7,6,0xC,9,8,6,0xA,0xB,9,9,0xB,0xC,0xD,0xB,0xC,8,0xD,0xC,0xA,0xB,0xA,0xB,0xE,8,7,8,0xE,
            0xD,0xA,0xD,0xA,0xD,6,0xA,9,0xC,0xC,0xF,0xE,0xA,0xC,0xD};
            public static int[] g_6x7TRNs = new int[] {0xD,0xF,9,0xA,0xA,9,9,0xB,7,0xC,9,0xA,0xD,0xA,0xC,7,9,0xD,0xE,0xD,9,
            0xC,8,0xC,9,9,9,0xB,0xC,9,0xC,0xC,0xA,0xE,0xB,0xC,0xB,0xB,0xA,6,8,9,0xA,0xB,0xF,0xB,0xE,0xE,0xA,7,0xD,
            0xC,9,0xA,0xD,0xA,0xD,9,7,0xB,0xD,9,0xB,0xA,0xB,8,9,0xA,6,9,0xA,9,7,8,0xA,9,8,9,8,0xB,9,0xE,0xA,8,9,
            0xA,0xE,0xE,7,0xA,0xB,0xB,0xB,0xB,0xC,0xA,0xA,0xB,0xD,9,8,0xB,0xC,0xC,0xE,0xB,0xB,0xA,0xD,0xC,9,0xC,
            0xB,0xE,8,9,0xD,9,9,8,9,8,8,0xE,0xE,7,8,8,7,9,0xE,0xC,9,0xA,6,0xD,0xB,9,0xE,0xB,0xA,0xB,0xA,0xA,0xC,
            0xD,0xB,0xC,0xE,9,0xA,9,9,0xB,8,0xB,0xC,0xE,0xA,9,0xB,9,0xA,0xD,0xA,0xE,0xB,0xB,0xA,0xA,0xC,0xA,9,0xE,
            0xA,8,9,0xA,0xC,0xB,0xB,9,6,0xC,9,0xE,0xB,0xB,0xA,0xC,0xE,0x11,0xA,9,0xD,0xA,0xA,0xF,0x11,0xD};
            public static int[] g_6x8TRNs = new int[] {0xB,0xC,0xD,0xA,0xA,0xC,0xD,0xC,0xF,0xC,0xE,0xB,0xC,0xA,0x12,0xE,6,0xB,
            0x11,0x11,0x11,0xD,0xE,0xE,0x13,0xF,0xD,0x12,0xA,0x11,3,0xA,0xC,0xF,9,0xC,0xC,0xA,0xF,0xC,0xD,0xB,0xF,
            0xB,0xE,0xC,9,9,0xC,8,8,8,7,0xA,0xB,0xA,9,0xA,0xD,0xF,0xC,0xB,0xC,0xF,0xD,0x14,0xE,0xD,0x14,0xF,0x13,
            0xC,0xB,0xD,0xA,0xA,0xB,0x10,0x10,0xC,0xC,0x12,0xA,0xB,0xC,0xF,0xC,0x10,0xD,0xD,0xF,0xE,0xB,0x10,0x14,
            0x10,0xE,0xE,0x10,0xD,0xE,0xD,0xA,0xC,0xE,0x10,0xA,0xB,0xD,0xF,0xD,0xD,0xF,0xC,0x12,0xF,0xC,0xC,0xF,0xD,
            0xF,0xA,8,0xC,0x11,0xE,0x11,0xE,0xE,0x10,0xE,0xE,0x13,0xE,0xF,0x14,0xC,0xB,0xB,0xD,0xE,0xE,0xB,9,0xE,
            0xC,0xE,0x10,0xE,0xF,0x10,0xB,0x10,0xB,0xD,0xD,0x10,0xD,0xB,0x11,0xD,0xD,9,0xF,0xB,0xC,9,0xD,0xF,0xD,
            0x10,0xD,0xD,9,0xF,0xE,0x11,0xA,0x12,0xF,0x10,0xE,0x10,0xE,0x10,0xE,0xE,0xE,0xE,0xC,0x10,0xF,0xD,0x15,
            0x14,0xF,9,0x11,0x12,0xD};
            public static int[] g_6x9TRNs = new int[] {0xD,9,0xD,0xB,0x10,0xD,0xE,0xC,0xA,0xA,0xB,0xA,0xC,0xB,0xD,0xC,0xC,0xB,
            0xD,0xD,0xA,0xC,0xC,0x12,0xA,0xB,0xE,0xA,8,0xF,0xF,0xB,0x11,8,0xD,0xA,0xD,0xC,0x11,0xD,0xA,0xA,0x10,0x11
            ,0xA,0xA,0xA,0xD,0xB,0xE,8,0x10,0xD,0x11,0x10,0xC,0xC,0xB,0xC,0xF,0x10,0x10,0xE,0xC,0xC,0xB,0xB,0xB,0xE,
            0xE,0xA,0xF,9,0xD,0xA,0xF,0xF,0xF,0xE,0xB,0xA,7,0xC,0xB,0xF,0xE,0xD,0x11,0xE,0x10,0xF,0xD,0xD,0x10,0xF,
            0x10,0x13,0xE,9,0xB,0xA,9,0xE,0xC,0xA,0xF,0x11,0xF,0xA,0xE,0x10,0x11,0x12,0xC,0xD,0xC,0xB,0xB,0xC,0x14,
            0x13,9,0xD,0x10,0xA,7,9,0xC,0xF,7,8,0x11,0xC,0x13,0xB,0xB,0xC,0xD,0xC,0x13,0x11,0xD,0x12,0xD,0xF,8,0xD,
            0xE,0xB,0xC,0x10,0x12,0x15,0xC,0xD,0xF,0xD,0xA,0x11,0xE,0x11,0xC,0x10,0xD,0xD,0xD,0xB,0xB,0xD,0x11,0xD,
            0xD,0xB,0xD,0xD,0x10,0xD,0x10,0xC,0xC,0xE,0xA,0xD,0x12,0xF,8,0xD,0xE,0xB,0xD,0x12,0x11,0xA,0xC,0xF,0x11,
            0xC,0x15,0xD,0xC};
            public static int[] g_7x7TRNs = new int[] {0x10,0xA,6,0xC,0xC,0xE,0xB,0xB,0xA,0xD,0xE,0xB,0xD,0xB,0xC,0xC,0xD,9,0xA,
            0xB,0xC,0xC,8,0xD,8,8,0xA,3,0xA,0xE,9,0xB,0xC,0xD,0xA,0xE,8,0xB,0xF,0xC,0xA,9,0xE,0xB,0xB,0xF,0xD,7,0xD,
            0xC,0xC,0xC,0xF,0xE,0xC,0xC,9,0xE,0xA,9,0xA,0xC,0xE,0xD,0xF,0xC,0xD,0xC,0xE,0xD,0xB,0xD,0xB,0xD,0xD,0xD,
            9,0xD,0xB,0xE,0xC,0xA,0xD,0xA,0xD,0xC,0xF,0xA,0xB,0xC,0x10,0xB,8,0xB,0xC,0xB,0xA,0xE,0xE,0xD,0xE,0xE,0xC,
            0xD,0xB,0xF,0xC,0xF,0xE,0xC,0xB,0xB,0xD,0xA,0xB,0x10,0xD,0xF,0xC,0xD,0x10,0xE,0xD,0xD,6,0xF,0xE,0xD,0xC,
            0xC,0xD,0xB,0xC,0xE,0xD,0xC,0x10,0xC,0xF,0xC,0xC,0x10,0xE,0xA,0x10,0x10,0xA,0xA,0xC,0xA,0xA,0xD,0xB,0xE,
            0xE,0xC,6,0xA,0xB,0xC,8,7,0xC,0xB,0xC,0xE,9,0xE,0xD,0xE,0xC,0xF,0xB,0xD,0xA,9,0xA,0xA,9,8,9,0xB,0xB,9,8,
            9,0xA,0xA,9,7,9,0xB,0xD,0xB,8,0xA,0xC,8,0xB,9};
            public static int[] g_7x8TRNs = new int[] {8,9,0x11,0xC,0xC,0xF,0xD,0xC,0xD,9,0xA,0xA,9,9,0x10,0xD,0xB,0xE,0xC,0xE,
            0xE,0xD,0xF,0xB,0xD,9,0x12,0xB,0xC,0xC,0xB,0xC,0xD,0xE,8,0xD,0xF,0x11,0xD,0xE,0x12,0xE,0x17,0x17,0x16,
            0x17,0xD,0xE,0x12,0x11,0x13,0xF,0x11,0x14,0x13,0xD,0x14,0x11,0x15,0xD,0xF,0x14,0xD,0x11,0xE,0x12,0x15,
            0x17,0xE,0x12,0x17,0xB,0x14,0x16,0x14,0x15,0x13,0x14,0x18,0xE,0xD,0x15,0x11,0x12,0x11,0xC,0x12,0x10,0x11,
            0x13,0x13,0x12,0x13,0x11,0x16,0xC,0x12,0x15,0x17,0x12,0x13,0x15,0x12,0x16,0x18,0x15,0x15,0x13,0x13,0x14,
            0x15,0x13,0x18,0xD,0x15,0x15,0x17,0x13,0x15,0x16,0x17,0x19,0x15,0x14,0x11,0x17,0x14,0x17,0x11,0xA,0x14,
            0x14,0x11,0x11,0xD,0x11,0xF,0x15,0x13,0x14,0x11,0xD,0xE,0x17,0x17,0x14,0x12,0x12,0x16,0x11,0x11,0x13,0x10,
            0x11,0x12,0x13,0x12,0x14,0x13,0x15,0x14,0x13,0x12,0x10,0x12,0xF,0x17,0x11,0x12,0xF,0x12,0x15,0x13,0x14,0xE,
            0xF,0x13,0x13,0x15,0x14,0xB,0x15,0x15,0x12,0x15,0x11,0x10,0x12,0x10,0x14,0xD,0x13,0x14,0x15,0xF,0xE,0x11,
            0x10,0x15,0x13};
            public static int[] g_7x9TRNs = new int[] {0x12,0xE,0x13,0x13,0x15,0xC,0x16,0x14,0x10,0xD,0x10,0xE,0x12,0x14,0x10,0x11,
            0xB,0xC,0x12,0x14,0x13,0x10,0x12,0xC,0x10,0xE,0xF,0x10,0x11,0x10,0xF,0x10,0xF,0x11,0x14,0xD,0x12,0xF,0x13,
            0xF,0x14,0x14,0x11,0x15,0x16,0x14,0xE,0xE,0x10,0xD,0xE,0xF,0x11,0xD,0xC,0xC,0xF,0xD,0x14,0x13,0x10,0x12,
            0x10,0x10,0x12,0x10,0x11,0xE,0x12,0x15,0x12,0x15,0xF,0x10,0x13,0x10,0x12,0xE,0x10,0xD,0xF,0x11,0x10,0x12,
            0x15,0x11,0x14,0x12,0x11,0x11,0x10,0x13,0xF,0x14,0x11,0xE,0x14,0xF,0x10,0x15,0x13,0x10,0x12,0x12,0x13,0x10,
            0xF,0x10,0xE,0x11,0x12,0x11,0xA,0x10,0x11,0x12,0x12,0x13,0x11,0xD,0xD,0x10,0x14,0x15,0x11,0x11,0x11,0x11,
            0x11,0xF,0x12,0xE,0xD,0xF,0xF,0xE,0x11,0xD,0xD,0xE,0x10,0x11,0xE,0x13,0x12,0x10,0x14,0x11,0x12,0x12,0x12,
            0xF,0x13,0x14,0x11,0x10,0x10,0xF,0xF,0xB,0xA,0x14,0x13,0x13,0x12,0x14,0x14,0x15,0x13,0x17,0x13,0x14,0x12,
            0x13,0x12,0xE,0x14,0x14,0x11,0x15,0x12,0x16,0x16,0x16,0x15,0x15,0x16,0x15,0x16,0x14,0x16,0x13,0x10,0x12,
            0x12,0x13,0x16,0x12,0x18,0x18};
        }
        #endregion
        static public int seed = 1;
        static public int Rand()
        {
            int s = seed;
            long mul = Math.BigMul(s, 0x41C64E6D);
            int loRes = (int)(mul & 0xFFFFFFFF);
            loRes += 0x3039;
            int shRes = loRes >> 16;
            seed = loRes;
            return shRes & 0x7FFF;
        }

        static private int g_forcePuzzlePattern = 200;
        static private int g_currentPuzzle;

        static private int GetNextPuzzlePattern(out bool isFlipped)
        {
            int nextVal = g_forcePuzzlePattern;
            if (nextVal == 200)
            {
                int r = Rand();
                int rDiv20 = r / 200;
                int rShift31 = r >> 31;
                nextVal = rDiv20 - rShift31;
                nextVal *= 200;
                nextVal = r - nextVal;
            }
            int currentPuzzle = nextVal & 0xff;
            g_currentPuzzle = currentPuzzle;
            isFlipped = ((Rand() % 100) < 30);
            return currentPuzzle;
        }

        public struct TRNStats
        {
            public int lowest;
            public int[] lowPuz;
            public int highest;
            public int[] highPuz;
            public float average;

            public TRNStats(int low, int[] inLowPuz, int high, int[] inHighPuz, float avg)
            {
                lowest = low;
                lowPuz = inLowPuz;
                highest = high;
                highPuz = inHighPuz;
                average = avg;
            }
        }

        static public TRNStats ComputeTRNStats(int[] trns)
        {
            List<int> lowestPuzzles = new List<int>();
            List<int> highestPuzzles = new List<int>();
            int lowest = Int32.MaxValue;
            int highest = Int32.MinValue;
            int total = 0;
            for(int i = 0; i < trns.Length; ++i)
            {
                int trn = trns[i];
                total += trn;
                if (trn < lowest)
                {
                    lowest = trn;
                }
                if (trn > highest)
                {
                    highest = trn;
                }
            }
            for (int i = 0; i < trns.Length; ++i)
            {
                int trn = trns[i];
                if (trn == lowest)
                {
                    lowestPuzzles.Add(i);
                }
                if (trn == highest)
                {
                    highestPuzzles.Add(i);
                }
            }
            return new TRNStats(lowest, lowestPuzzles.ToArray(), highest, highestPuzzles.ToArray(), ((float)total) / trns.Length);
            //output.WriteLine("Stats for {0}", name);
            //output.WriteLine("Shortest puzzle: {0} turns (puzzle {1})", lowest, lowPuz);
            //output.WriteLine("Longest puzzle: {0} turns (puzzle {1})", highest, highPuz);
            //output.WriteLine("Average TRN = {0}", ((float)total) / trns.Length);
            //output.WriteLine();
        }

        static public string BuildHrefPuzzleString(int[] puzzles)
        {
            StringBuilder sb = new StringBuilder();
            foreach (int puz in puzzles)
            {
                sb.AppendFormat("<a href=\"#{0}\">Puzzle {1}</a>, ", puz, puz + 1);
            }
            sb.Length -= 2;
            return sb.ToString();
        }

        public struct PuzzleStats
        {
            public int totalPuzzleCubes;
            public int advanCubes;
            public int forbiddenCubes;
            public int normalCubes;
            public int maxAdvanCubes;
            public int[] maxAdvanCubesPuzzles;
            public int maxForbidCubes;
            public int[] maxForbidCubesPuzzles;
            public int maxNormCubes;
            public int[] maxNormCubesPuzzles;
            public int minAdvanCubes;
            public int[] minAdvanCubesPuzzles;
            public int minForbidCubes;
            public int[] minForbidCubesPuzzles;
            public int minNormCubes;
            public int[] minNormCubesPuzzles;
        }

        public struct PuzzleInfo
        {
            public int forb;
            public int norm;
            public int adv;
        }

        static PuzzleStats ComputePuzzleStats(byte[] puzzles, int width, int height)
        {
            List<int> advMaxPuzzles = new List<int>();
            List<int> forbMaxPuzzles = new List<int>();
            List<int> normMaxPuzzles = new List<int>();
            List<int> advMinPuzzles = new List<int>();
            List<int> forbMinPuzzles = new List<int>();
            List<int> normMinPuzzles = new List<int>();
            List<PuzzleInfo> puzzInfs = new List<PuzzleInfo>();
            int minNorm = Int16.MaxValue, maxNorm = Int16.MinValue;
            int minAdv = Int16.MaxValue, maxAdv = Int16.MinValue;
            int minForb = Int16.MaxValue, maxForb = Int16.MinValue;
            PuzzleStats stats = new PuzzleStats();
            stats.totalPuzzleCubes = (width * height) * 200;
            for(int i = 0; i < 200; ++i)
            {
                byte[] puzzle = new byte[70];
                Array.Copy(puzzles, i * 70, puzzle, 0, 70);
                PuzzleInfo puzzInf = new PuzzleInfo();
                for (int y = 0; y < height; ++y)
                {
                    int rowOffset = 7 * y;
                    for (int x = 0; x < width; ++x)
                    {
                        switch(puzzle[rowOffset + x])
                        {
                            case 0:
                            {
                                ++puzzInf.norm;
                            }
                            break;
                            case 1:
                            {
                                ++puzzInf.adv;
                            }
                            break;
                            case 2:
                            {
                                ++puzzInf.forb;
                            }
                            break;
                        }
                    }
                }
                puzzInfs.Add(puzzInf);
                stats.forbiddenCubes += puzzInf.forb;
                stats.normalCubes += puzzInf.norm;
                stats.advanCubes += puzzInf.adv;
                if (puzzInf.norm < minNorm)
                {
                    minNorm = puzzInf.norm;
                }
                if (puzzInf.norm > maxNorm)
                {
                    maxNorm = puzzInf.norm;
                }
                if (puzzInf.forb < minForb)
                {
                    minForb = puzzInf.forb;
                }
                if (puzzInf.forb > maxForb)
                {
                    maxForb = puzzInf.forb;
                }
                if (puzzInf.adv < minAdv)
                {
                    minAdv = puzzInf.adv;
                }
                if (puzzInf.adv > maxAdv)
                {
                    maxAdv = puzzInf.adv;
                }
            }
            for (int i = 0; i < 200; ++i)
            {
                if (puzzInfs[i].adv == maxAdv)
                {
                    advMaxPuzzles.Add(i);
                }
                if (puzzInfs[i].adv == minAdv)
                {
                    advMinPuzzles.Add(i);
                }
                if (puzzInfs[i].forb == maxForb)
                {
                    forbMaxPuzzles.Add(i);
                }
                if (puzzInfs[i].forb == minForb)
                {
                    forbMinPuzzles.Add(i);
                }
                if (puzzInfs[i].norm == maxNorm)
                {
                    normMaxPuzzles.Add(i);
                }
                if (puzzInfs[i].norm == minNorm)
                {
                    normMinPuzzles.Add(i);
                }
            }
            int totalIndividual = stats.forbiddenCubes + stats.advanCubes + stats.normalCubes;
            if (totalIndividual != stats.totalPuzzleCubes)
            {
                throw new ArithmeticException(String.Format("Total cubes {0} != sum of individual cubes {1}", stats.totalPuzzleCubes, totalIndividual));
            }
            stats.maxAdvanCubes = maxAdv;
            stats.maxAdvanCubesPuzzles = advMaxPuzzles.ToArray();
            stats.minAdvanCubes = minAdv;
            stats.minAdvanCubesPuzzles = advMinPuzzles.ToArray();
            stats.maxNormCubes = maxNorm;
            stats.maxNormCubesPuzzles = normMaxPuzzles.ToArray();
            stats.minNormCubes = minNorm;
            stats.minNormCubesPuzzles = normMinPuzzles.ToArray();
            stats.maxForbidCubes = maxForb;
            stats.maxForbidCubesPuzzles = forbMaxPuzzles.ToArray();
            stats.minForbidCubes = minForb;
            stats.minForbidCubesPuzzles = forbMinPuzzles.ToArray();
            return stats;
        }

        static List<string> FindSolution(byte[] puzzleBytes, List<Solution> solutions, int width, out bool flipped)
        {
            MD5 hasher = MD5.Create();
            byte[] localPuzzle = new byte[puzzleBytes.Length];
            Array.Copy(puzzleBytes, 0, localPuzzle, 0, puzzleBytes.Length);
            byte[] puzzleHash = hasher.ComputeHash(localPuzzle);
            Solution temp = new Solution(puzzleHash, null);
            int index = solutions.BinarySearch(temp);
            flipped = false;
            // ouldn't find it, try the flipped version
            if (index < 0)
            {
                int bytesReversed = 0;
                while (bytesReversed < localPuzzle.Length)
                {
                    Array.Reverse(localPuzzle, bytesReversed, width);
                    bytesReversed += width;
                }
                temp.puzzleHash = hasher.ComputeHash(localPuzzle);
                index = solutions.BinarySearch(temp);
                // found it, overwrite the orig puzzle with the flipped copy so the instructions match
                if (index >= 0)
                {
                    flipped = true;
                    Array.Copy(localPuzzle, puzzleBytes, localPuzzle.Length);
                }
            }
            return (index < 0) ? null : solutions[index].solution;
        }

        static byte[] CopyPuzzle(byte[] puzzles, int startingPoint, int width, int height)
        {
            byte[] puzzle = new byte[width * height];
            for (int y = 0; y < height; ++y)
            {
                Array.Copy(puzzles, startingPoint + (y * 7), puzzle, y * width, width);
            }
            return puzzle;
        }

        static void DumpTextFile(byte[] puzzles, string fileName, int width, int height, int [] trns)
        {
            string textFile = "T:\\kurushi\\" + Path.ChangeExtension(fileName, ".txt");
            StringBuilder sb = new StringBuilder();
            using (StreamWriter sw = new StreamWriter(textFile))
            {
                for(int puzzNum = 0; puzzNum < 200; ++puzzNum)
                {
                    int puzzStart = (puzzNum * 70);
                    sw.WriteLine("Puzzle {0} - TRN {1}", puzzNum + 1, trns[puzzNum]);
                    for (int y = 0; y < height; ++y)
                    {
                        int rowOffset = y * 7;
                        for (int x = 0; x < width; ++x)
                        {
                            sb.AppendFormat("{0} ", puzzles[puzzStart + (rowOffset + x)]);
                        }
                        sb.Length -= 1;
                        sw.WriteLine(sb.ToString());
                        sb.Length = 0;
                    }
                    sw.WriteLine();
                }
            }
        }

        static void DumpPuzzles(byte[] puzzles, string fileName, int width, int height, int[] trns, List<Solution> solutions, out TRNStats trnStats, out PuzzleStats puzStats)
        {
            //PrintTRNStats(fileName, trns);
            //return;
            //DumpTextFile(puzzles, fileName, width, height,trns);
            trnStats = ComputeTRNStats(trns);
            puzStats = ComputePuzzleStats(puzzles, width, height);
        }

        class Solution : IComparable<Solution>
        {
            public byte[] puzzleHash;
            public List<string> solution;

            public Solution(byte[] hash, List<string> steps)
            {
                puzzleHash = hash;
                solution = steps;
            }

            public int CompareTo(Solution other)
            {
                int len = puzzleHash.Length;
                if(len < other.puzzleHash.Length)
                {
                    return -1;
                }
                else if(len > other.puzzleHash.Length)
                {
                    return 1;
                }
 	            for(int i = 0; i < len; ++i)
                {
                    if(puzzleHash[i] < other.puzzleHash[i])
                    {
                        return -1;
                    }
                    else if(puzzleHash[i] > other.puzzleHash[i])
                    {
                        return 1;
                    }
                }
                return 0;
            }
        }

        class Solutions
        {
            public Dictionary<string, List<Solution>> puzzles;
            public Solutions()
            {
                puzzles = new Dictionary<string, List<Solution>>();
            }
        }

        static Solutions ParseSolutions()
        {
            MD5 hasher = MD5.Create();
            List<byte> puzzle = new List<byte>();
            string puzzleWidth = String.Empty, puzzleHeight = String.Empty;
            Solutions solutions = new Solutions();
            List<string> solutionSteps = new List<string>();
            Regex puzzleLine = new Regex("(\\d)\\s*\\[(_|\\*|X)\\]\\[(_|\\*|X)\\]\\[(_|\\*|X)\\]\\[(_|\\*|X)\\](?:\\[(_|\\*|X)\\])?(?:\\[(_|\\*|X)\\])?(?:\\[(_|\\*|X)\\])?", RegexOptions.Compiled);
            Regex solutionStepLine = new Regex("(?:R(\\d{1,2}\\.\\s.*))");
            string[] lines = File.ReadAllLines(@"F:\Dev-Cpp\Projects\CSharp\IQTracker\KurushRNG\solutions.txt");
            for (int i = 0; i < lines.Length; ++i)
            {
                string line = lines[i];
                bool skip = false;
                if (String.IsNullOrEmpty(line)) // blank
                {
                    skip = true;
                }
                else if (
                    (line.IndexOf('~') != -1) || // wave header
                    (line.IndexOf("-=-+-") != -1) || // stage header
                    (line.IndexOf("----") != -1) // puzzle intro
                )
                {
                    i += 3;
                    if (!(puzzle.Count == 0))
                    {
                        string key = puzzleWidth + "x" + puzzleHeight;
                        List<Solution> sols = null;
                        if (!solutions.puzzles.TryGetValue(key, out sols))
                        {
                            sols = new List<Solution>();
                            solutions.puzzles.Add(key, sols);
                        }
                        byte[] hash = hasher.ComputeHash(puzzle.ToArray());
                        Solution sol = new Solution(hash, solutionSteps);
                        sols.Add(sol);
                        solutionSteps = new List<string>();
                        puzzle = new List<byte>();
                        puzzleHeight = puzzleWidth = String.Empty;
                    }
                    skip = true;
                }
                if (!skip)
                {
                    Match m = puzzleLine.Match(line);
                    if (m.Success)
                    {
                        GroupCollection matchGroups = m.Groups;
                        if (String.IsNullOrEmpty(puzzleHeight))
                        {
                            puzzleHeight = matchGroups[1].Value;
                        }
                        for (int group = 2; group < matchGroups.Count; ++group)
                        {
                            if (String.IsNullOrEmpty(matchGroups[group].Value))
                            {
                                if (String.IsNullOrEmpty(puzzleWidth))
                                {
                                    puzzleWidth = (group - 2).ToString();
                                }
                                break;
                            }
                            switch (matchGroups[group].Value[0])
                            {
                                case '_': puzzle.Add(0); break;
                                case '*': puzzle.Add(1); break;
                                case 'X': puzzle.Add(2); break;
                            }
                        }
                        if (String.IsNullOrEmpty(puzzleWidth))
                        {
                            puzzleWidth = "7";
                        }
                    }
                    m = solutionStepLine.Match(line);
                    if (m.Success)
                    {
                        solutionSteps.Add(m.Groups[1].Value);
                    }
                }
            }
            foreach (KeyValuePair<string, List<Solution>> p in solutions.puzzles)
            {
                p.Value.Sort();
            }
            return solutions;
        }

        static void HtmlMain(string[] args)
        {
            byte[] puzzles = File.ReadAllBytes(@"F:\Dev-Cpp\Projects\CSharp\IQTracker\KurushRNG\IQ-Group.new");
            // 70 bytes between puzzles
            // 200 puzzles per size
            // 14,000 bytes between puzzle sizes
            byte[] fourBy2Puzzles = new byte[14000];
            Array.Copy(puzzles, 14000 * 0, fourBy2Puzzles, 0, 14000);
            byte[] fourBy3Puzzles = new byte[14000];
            Array.Copy(puzzles, 14000 * 1, fourBy3Puzzles, 0, 14000);
            byte[] fourBy4Puzzles = new byte[14000];
            Array.Copy(puzzles, 14000 * 2, fourBy4Puzzles, 0, 14000);
            byte[] fourBy5Puzzles = new byte[14000];
            Array.Copy(puzzles, 14000 * 3, fourBy5Puzzles, 0, 14000);
            byte[] fourBy6Puzzles = new byte[14000];
            Array.Copy(puzzles, 14000 * 4, fourBy6Puzzles, 0, 14000);
            byte[] fiveBy4Puzzles = new byte[14000];
            Array.Copy(puzzles, 14000 * 5, fiveBy4Puzzles, 0, 14000);
            byte[] fiveBy5Puzzles = new byte[14000];
            Array.Copy(puzzles, 14000 * 6, fiveBy5Puzzles, 0, 14000);
            byte[] fiveBy6Puzzles = new byte[14000];
            Array.Copy(puzzles, 14000 * 7, fiveBy6Puzzles, 0, 14000);
            byte[] fiveBy7Puzzles = new byte[14000];
            Array.Copy(puzzles, 14000 * 8, fiveBy7Puzzles, 0, 14000);
            byte[] fiveBy8Puzzles = new byte[14000];
            Array.Copy(puzzles, 14000 * 9, fiveBy8Puzzles, 0, 14000);
            byte[] sixBy6Puzzles = new byte[14000];
            Array.Copy(puzzles, 14000 * 10, sixBy6Puzzles, 0, 14000);
            byte[] sixBy7Puzzles = new byte[14000];
            Array.Copy(puzzles, 14000 * 11, sixBy7Puzzles, 0, 14000);
            byte[] sixBy8Puzzles = new byte[14000];
            Array.Copy(puzzles, 14000 * 12, sixBy8Puzzles, 0, 14000);
            byte[] sixBy9Puzzles = new byte[14000];
            Array.Copy(puzzles, 14000 * 13, sixBy9Puzzles, 0, 14000);
            byte[] sevenBy7Puzzles = new byte[14000];
            Array.Copy(puzzles, 14000 * 14, sevenBy7Puzzles, 0, 14000);
            byte[] sevenBy8Puzzles = new byte[14000];
            Array.Copy(puzzles, 14000 * 15, sevenBy8Puzzles, 0, 14000);
            byte[] sevenBy9Puzzles = new byte[14000];
            Array.Copy(puzzles, 14000 * 16, sevenBy9Puzzles, 0, 14000);

            int[][] trns = new int[17][] {
                TRNs.g_4x2TRNs, TRNs.g_4x3TRNs, TRNs.g_4x4TRNs, TRNs.g_4x5TRNs, TRNs.g_4x6TRNs,
                TRNs.g_5x4TRNs, TRNs.g_5x5TRNs, TRNs.g_5x6TRNs, TRNs.g_5x7TRNs, TRNs.g_5x8TRNs,
                TRNs.g_6x6TRNs, TRNs.g_6x7TRNs, TRNs.g_6x8TRNs, TRNs.g_6x9TRNs, TRNs.g_7x7TRNs,
                TRNs.g_7x8TRNs, TRNs.g_7x9TRNs
            };

            Solutions sol = ParseSolutions();

            TRNStats[] trnStats = new TRNStats[17];
            PuzzleStats[] puzStats = new PuzzleStats[17];
            DumpPuzzles(fourBy2Puzzles, "qube/4x2.html", 4, 2, trns[0], sol.puzzles["4x2"], out trnStats[0], out puzStats[0]);
            DumpPuzzles(fourBy3Puzzles, "qube/4x3.html", 4, 3, trns[1], sol.puzzles["4x3"], out trnStats[1], out puzStats[1]);
            DumpPuzzles(fourBy4Puzzles, "qube/4x4.html", 4, 4, trns[2], sol.puzzles["4x4"], out trnStats[2], out puzStats[2]);
            DumpPuzzles(fourBy5Puzzles, "qube/4x5.html", 4, 5, trns[3], sol.puzzles["4x5"], out trnStats[3], out puzStats[3]);
            DumpPuzzles(fourBy6Puzzles, "qube/4x6.html", 4, 6, trns[4], sol.puzzles["4x6"], out trnStats[4], out puzStats[4]);
            DumpPuzzles(fiveBy4Puzzles, "qube/5x4.html", 5, 4, trns[5], sol.puzzles["5x4"], out trnStats[5], out puzStats[5]);
            DumpPuzzles(fiveBy5Puzzles, "qube/5x5.html", 5, 5, trns[6], sol.puzzles["5x5"], out trnStats[6], out puzStats[6]);
            DumpPuzzles(fiveBy6Puzzles, "qube/5x6.html", 5, 6, trns[7], sol.puzzles["5x6"], out trnStats[7], out puzStats[7]);
            DumpPuzzles(fiveBy7Puzzles, "qube/5x7.html", 5, 7, trns[8], sol.puzzles["5x7"], out trnStats[8], out puzStats[8]);
            DumpPuzzles(fiveBy8Puzzles, "qube/5x8.html", 5, 8, trns[9], sol.puzzles["5x8"], out trnStats[9], out puzStats[9]);
            DumpPuzzles(sixBy6Puzzles, "qube/6x6.html", 6, 6, trns[10], sol.puzzles["6x6"], out trnStats[10], out puzStats[10]);
            DumpPuzzles(sixBy7Puzzles, "qube/6x7.html", 6, 7, trns[11], sol.puzzles["6x7"], out trnStats[11], out puzStats[11]);
            DumpPuzzles(sixBy8Puzzles, "qube/6x8.html", 6, 8, trns[12], sol.puzzles["6x8"], out trnStats[12], out puzStats[12]);
            DumpPuzzles(sixBy9Puzzles, "qube/6x9.html", 6, 9, trns[13], sol.puzzles["6x9"], out trnStats[13], out puzStats[13]);
            DumpPuzzles(sevenBy7Puzzles, "qube/7x7.html", 7, 7, trns[14], sol.puzzles["7x7"], out trnStats[14], out puzStats[14]);
            DumpPuzzles(sevenBy8Puzzles, "qube/7x8.html", 7, 8, trns[15], sol.puzzles["7x8"], out trnStats[15], out puzStats[15]);
            DumpPuzzles(sevenBy9Puzzles, "qube/7x9.html", 7, 9, trns[16], sol.puzzles["7x9"], out trnStats[16], out puzStats[16]);

            int totalCubes = 0;
            int advanCubes = 0;
            int forbidCubes = 0;
            foreach (PuzzleStats s in puzStats)
            {
                totalCubes += s.totalPuzzleCubes;
                advanCubes += s.advanCubes;
                forbidCubes += s.forbiddenCubes;
            }
            float totalAverage = 0;
            foreach (TRNStats t in trnStats)
            {
                totalAverage += t.average;
            }
            totalAverage /= 17;
            int normalCubes = totalCubes - advanCubes - forbidCubes;
            Console.WriteLine("IQ Stats:");
            Console.WriteLine("Average TRN of the game: {0:f2}", totalAverage);
            Console.WriteLine("Total advan cubes: {0} ({1:f2}%)", advanCubes, (((float)advanCubes / totalCubes) * 100.0f));
            Console.WriteLine("Total forbid cubes: {0} ({1:f2}%)", forbidCubes, (((float)forbidCubes / totalCubes) * 100.0f));
            Console.WriteLine("Total normal cubes: {0} ({1:f2}%)", normalCubes, (((float)normalCubes / totalCubes) * 100.0f));
            Console.WriteLine();
        }

        public class PuzzleAppearance
        {
            public uint puzzle;
            public uint times;
            public uint timesFlipped;
        }

        static void Main(string[] args)
        {
            HtmlMain(args);
            //IQFinal.Program.DumpPuzzles();
            //IQMania.Program.DumpPuzzles();
            //IQRemix.Program.DumpPuzzles();
            //IQFinal.Program.SeqMain();
        }

        static void SeqMain(string[] args)
        {
            PuzzleAppearance[] puzzles = new PuzzleAppearance[200];
            for (uint i = 0; i < 200; ++i)
            {
                puzzles[i] = new PuzzleAppearance();
                puzzles[i].puzzle = i;
                puzzles[i].times = 0;
                puzzles[i].timesFlipped = 0;
            }
            using (StreamWriter sw = new StreamWriter("PuzzleSequences.txt"))
            {
                int[] needle = new int[] { 67, 66, 40 };
                int[] gen4x2 = new int[6];
                ulong flipped = 0;
                ulong total = 0;
                //int needlesFound = 0;
                for (uint i = 500000; i < 1000000; ++i)
                {
                    StringBuilder sb = new StringBuilder(512);
                    seed = (int)i;
                    sw.WriteLine("Seed = {0}", i);
                    sb.Append("Puzzles: ");
                    int y = 0;
                    bool[] flippedPuzzles = new bool[12];
                    //needlesFound = 0;

                    for (; y < 12; ++y)
                    {
                        int puzzle = GetNextPuzzlePattern(out flippedPuzzles[y]);
                        //gen4x2[y] = puzzle;
                        ++puzzles[puzzle].times;
                        ++total;
                        if (flippedPuzzles[y])
                        {
                            ++flipped;
                            ++puzzles[puzzle].timesFlipped;
                        }
                        sb.AppendFormat("{0:D3}, ", puzzle);
                    }
                    sb.Length -= 2;
                    sb.AppendLine();
                    sb.Append("Flipped: ");
                    for (y = 0; y < 12; ++y)
                    {
                        int v = Convert.ToInt32(flippedPuzzles[y]);
                        sb.AppendFormat("{0:D3}, ", v);
                    }
                    sb.Length -= 2;
                    sb.AppendLine();
                    sw.WriteLine(sb.ToString());
                    //for (intx = 0; x < 3; ++x)
                    //{
                    //    if (gen4x2[x] == needle[x])
                    //    {
                    //        ++needlesFound;
                    //    }
                    //}
                    //if (needlesFound != 3)
                    //{
                    //    continue;
                    //}
                    //sw.WriteLine("Seed = {0}", i);
                    //sb.Append("4x2 puzzles: ");
                    //for (int u = 0; u < 6; ++u)
                    //{
                    //    sb.AppendFormat("{0}, ", gen4x2[u]);
                    //}

                    //sb.AppendLine();
                    //sb.Append("Second wave: ");
                    //for (y = 0; y < 3; ++y)
                    //{
                    //    int puzzle = GetNextPuzzlePattern(out isFlipped);
                    //    ++p4x3[puzzle].times;
                    //    sb.AppendFormat("{0}{1}, ", puzzle, isFlipped ? " (flipped)" : String.Empty);
                    //}
                    //sb.Length -= 2;
                    //sb.AppendLine();
                    //sb.Append("Third wave: ");
                    //for (y = 0; y < 3; ++y)
                    //{
                    //    int puzzle = GetNextPuzzlePattern(out isFlipped);
                    //    ++p4x3[puzzle].times;
                    //    sb.AppendFormat("{0}{1}, ", puzzle, isFlipped ? " (flipped)" : String.Empty);
                    //}
                    //sb.Length -= 2;
                    //sb.AppendLine();
                    //sb.Append("Fourth wave: ");
                    //for (y = 0; y < 3; ++y)
                    //{
                    //    int puzzle = GetNextPuzzlePattern(out isFlipped);
                    //    ++p4x4[puzzle].times;
                    //    sb.AppendFormat("{0}{1}, ", puzzle, isFlipped ? " (flipped)" : String.Empty);
                    //}
                    //sb.Length -= 2;
                    //sb.AppendLine();
                }
                PrintPuzzleAppearances(puzzles, "puzzles", sw, total, flipped);
            }
        }

        static public void PrintPuzzleAppearances(PuzzleAppearance[] appear, string puzzle, StreamWriter sw, ulong total, ulong flipped)
        {
            sw.WriteLine("Stats for {0}", puzzle);
            Array.Sort(appear, new Comparison<PuzzleAppearance>((x, y) => { return y.times.CompareTo(x.times); }));
            sw.WriteLine(
                "Puzzle {0} = {1} ({2}%) - Flipped {3} ({4}%)",
                appear[0].puzzle,
                appear[0].times,
                (appear[0].times / (float)total) * 100,
                appear[0].timesFlipped,
                (appear[0].timesFlipped / (float)appear[0].times) * 100
            );
            PuzzleAppearance last = appear[appear.Length - 1];
            sw.WriteLine(
                "Puzzle {0} = {1} ({2}%) - Flipped {3} ({4}%)",
                last.puzzle,
                last.times,
                (last.times / (float)total) * 100,
                last.timesFlipped,
                (last.timesFlipped / (float)last.times) * 100
            );
            //for (int i = 0; i < appear.Length; ++i)
            //{
            //    sw.WriteLine(
            //        "Puzzle {0} = {1} ({2}%) - Flipped {3} ({4}%)",
            //        appear[i].puzzle,
            //        appear[i].times,
            //        (appear[i].times / (float)total) * 100,
            //        appear[i].timesFlipped,
            //        (appear[i].timesFlipped / (float)appear[i].times) * 100
            //    );
            //}
            sw.WriteLine("Total puzzles seen = {0}", total);
            sw.WriteLine("Total flipped puzzles = {0}", flipped);

            Array.Sort(appear, new Comparison<PuzzleAppearance>((x, y) => { return y.timesFlipped.CompareTo(x.timesFlipped); }));
            sw.WriteLine("Most flipped puzzle: {0} ({1}%)", appear[0].puzzle, (appear[0].timesFlipped / (float)appear[0].times) * 100);
            last = appear[appear.Length - 1];
            sw.WriteLine("Least flipped puzzle: {0} ({1}%)", last.puzzle, (last.timesFlipped / (float)last.times) * 100);
        }
    }
}
