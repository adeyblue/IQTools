using System;
using System.Collections.Generic;
using System.Drawing;
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

        static public int g_seed = 1;

        static public void SRand(int seed)
        {
            g_seed = seed;
        }

        static public int GetRandomSeed()
        {
            return g_seed;
        }

        static public int Rand()
        {
            int s = g_seed;
            long mul = Math.BigMul(s, 0x41C64E6D);
            int loRes = (int)(mul & 0xFFFFFFFF);
            loRes += 0x3039;
            int shRes = loRes >> 16;
            g_seed = loRes;
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

        class PuzzleSet
        {
            public List<PickedPuzzle>[] waves;
            public PuzzleSet(List<PickedPuzzle>[] inWaves)
            {
                waves = inWaves;
            }
        }

        class PickedPuzzle
        {
            public bool flipped;
            public bool canDupe;
            public bool isDupe;
            public byte[] puzzle;

            public PickedPuzzle(byte[] puzzleData, int inPuzzle, int inPoints, bool inFlipped, bool inCanDupe, bool inIsDupe)
            {
                PuzzleNumber = inPuzzle;
                Points = inPoints;
                flipped = inFlipped;
                canDupe = inCanDupe;
                puzzle = puzzleData;
                isDupe = inIsDupe;
            }

            public PickedPuzzle Clone()
            {
                byte[] newPuzzle = new byte[puzzle.Length];
                Buffer.BlockCopy(puzzle, 0, newPuzzle, 0, puzzle.Length);
                return new PickedPuzzle(newPuzzle, PuzzleNumber, Points, flipped, canDupe, isDupe);
            }

            public PickedPuzzle CloneAsDupe()
            {
                byte[] newPuzzle = new byte[puzzle.Length];
                Buffer.BlockCopy(puzzle, 0, newPuzzle, 0, puzzle.Length);
                return new PickedPuzzle(newPuzzle, PuzzleNumber, Points, flipped, canDupe, true);
            }

            public int Capturable
            {
                get
                {
                    int numCapturable = 0;
                    foreach(byte b in puzzle)
                    {
                        if(b < 2)
                        {
                            ++numCapturable;
                        }
                    }
                    return numCapturable;
                }
            }

            public int Points
            {
                get;
                private set;
            }

            public int PuzzleNumber
            {
                get;
                private set;
            }
        }

        class Wave : IEnumerable<PickedPuzzle>
        {
            public int numPuzzles;
            public int width;
            public int height;
            public List<PickedPuzzle> puzzles;
            public int[] allTrns;
            public byte[] allPuzzleData;

            public Wave(List<PickedPuzzle> inPuzzles, int inWidth, int inHeight, byte[] inAllPuzzles, int[] inAllTrns)
            {
                puzzles = inPuzzles;
                numPuzzles = puzzles.Count;
                width = inWidth;
                height = inHeight;
                allPuzzleData = inAllPuzzles;
                allTrns = inAllTrns;
                Recalculate();
            }

            public void Recalculate()
            {
                int capt = 0;
                int points = 0;
                int turns = 0;
                foreach (PickedPuzzle p in puzzles)
                {
                    capt += p.Capturable;
                    points += p.Points;
                    turns += allTrns[p.PuzzleNumber];
                }
                Capturable = capt;
                Points = points;
                Turns = turns;
            }

            public Wave Clone()
            {
                List<PickedPuzzle> clonedPuzList = new List<PickedPuzzle>();
                puzzles.ForEach(
                    (x) =>
                    {
                        clonedPuzList.Add(x.Clone());
                    }
                );
                return new Wave(clonedPuzList, width, height, allPuzzleData, allTrns);
            }

            public int Points
            {
                get;
                private set;
            }

            public int Capturable
            {
                get;
                private set;
            }

            public int Turns
            {
                get;
                private set;
            }

            public int GetBestScoreAndPosition(out int index)
            {
                int bestScore = 0, bestIndex = 0;
                for (int i = 0; i < puzzles.Count; ++i)
                {
                    int puzzPoints = puzzles[i].Points;
                    if (puzzPoints > bestScore)
                    {
                        bestScore = puzzPoints;
                        bestIndex = i;
                    }
                }
                index = bestIndex;
                return bestScore;
            }

            public PickedPuzzle this[int i]
            {
                get
                {
                    return puzzles[i];
                }
            }

            public IEnumerator<PickedPuzzle> GetEnumerator()
            {
                return puzzles.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return puzzles.GetEnumerator();
            }
        }

        class WaveSet : IEnumerable<Wave>
        {
            public List<Wave> waves;
            public WaveSet()
            {
                waves = new List<Wave>();
            }

            public void Add(Wave w)
            {
                waves.Add(w);
            }

            public int Points
            {
                get
                {
                    int points = 0;
                    foreach (Wave w in waves)
                    {
                        points += w.Points;
                    }
                    return points;
                }
            }

            public int Capturable
            {
                get
                {
                    int num = 0;
                    foreach (Wave w in waves)
                    {
                        num += w.Capturable;
                    }
                    return num;
                }
            }

            public int Turns
            {
                get
                {
                    int num = 0;
                    foreach (Wave w in waves)
                    {
                        num += w.Turns;
                    }
                    return num;
                }
            }

            public Wave this[int i]
            {
                get
                {
                    return waves[i];
                }
            }

            public WaveSet Clone()
            {
                WaveSet ws = new WaveSet();
                foreach (Wave w in waves)
                {
                    ws.Add(w.Clone());
                }
                return ws;
            }

            public IEnumerator<Wave> GetEnumerator()
            {
                return waves.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return waves.GetEnumerator();
            }
        }

        class WaveSetInfo
        {
            public WaveSetInfo(int frame, WaveSet waveSet)
            {
                Frame = frame;
                Set = waveSet;
            }

            public int Frame
            {
                get;
                private set;
            }

            public WaveSet Set
            {
                get;
                private set;
            }

            public int Points
            {
                get
                {
                    return Set.Points;
                }
            }

            public int Capturable
            {
                get
                {
                    return Set.Capturable;
                }
            }

            public int Turns
            {
                get
                {
                    return Set.Turns;
                }
            }
        }

        class StageWaveInfo
        {
            public StageWaveInfo(byte[] puzzles, int[] trns, int width, int height, int numPuzzles)
            {
                PuzzleData = puzzles;
                TRNs = trns;
                Puzzles = numPuzzles;
                Width = width;
                Height = height;
            }

            public int Width
            {
                get;
                private set;
            }

            public int Height
            {
                get;
                private set;
            }

            public int Puzzles
            {
                get;
                private set;
            }

            public int[] TRNs
            {
                get;
                private set;
            }

            public byte[] PuzzleData
            {
                get;
                private set;
            }
        }

        static byte[] CopyPuzzle(byte[] puzzles, int startingPoint, int width, int height, bool flipped)
        {
            byte[] puzzle = new byte[width * height];
            for (int y = 0; y < height; ++y)
            {
                Array.Copy(puzzles, startingPoint + (y * 7), puzzle, y * width, width);
                if (flipped)
                {
                    Array.Reverse(puzzle, y * width, width);
                }
            }
            return puzzle;
        }

        static List<WaveSetInfo> Find10BestInRNGRange(int rngStartSeed, int rngEndSeed, List<StageWaveInfo> wavesToGenerate)
        {
            int numGood = 0;
            List<WaveSetInfo> best10Sets = new List<WaveSetInfo>();
            // i = number of frames rendered 
            for (; rngStartSeed <= rngEndSeed; ++rngStartSeed)
            {
                SRand(rngStartSeed);
                WaveSet noDupeSet = new WaveSet();
                foreach (StageWaveInfo swi in wavesToGenerate)
                {
                    Wave w = GetPuzzleSet(swi);
                    if (w == null) break;
                    //if (wave1Pts == 0)
                    //{
                    //    continue;
                    //}
                    noDupeSet.Add(w);
                }
                if (noDupeSet.waves.Count != 4) continue;
                ++numGood;

                WaveSet bestSet = CheckSquashedScores(noDupeSet);
                int bestPoints = bestSet.Points;
                WaveSetInfo wsi = new WaveSetInfo(rngStartSeed, bestSet);
                int numCaptures = bestSet.Capturable;
                int turns = bestSet.Turns;
#if CAPTURES
                int insertIndex = best10Sets.FindIndex((x) => { return numCaptures < x.Capturable; });
#elif POINTS
                int insertIndex = best10Sets.FindIndex((x) => { return bestPoints > x.Points; });
#elif TURNS
                int insertIndex = best10Sets.FindIndex((x) => { return turns < x.Turns; });
#endif
                if (insertIndex == -1)
                {
                    if (best10Sets.Count < 10)
                    {
                        best10Sets.Add(wsi);
                    }
                }
                else
                {
                    best10Sets.Insert(insertIndex, wsi);
                    if (best10Sets.Count > 10)
                    {
                        best10Sets.RemoveAt(10);
                    }
                }
            }
            return best10Sets;
        }

        static void FindBestOverallCaptures(List<StageWaveInfo>[] stages)
        {
            int rngStart = 940;
            int rngEnd = 0x800;
            List<WaveSetInfo> selectedWaves = new List<WaveSetInfo>(stages.Length);
            foreach (List<StageWaveInfo> stage in stages)
            {
                List<WaveSetInfo> best10 = Find10BestInRNGRange(rngStart, rngEnd, stage);
                best10.Sort(
                    (x, y) => {
                        if (x.Capturable != y.Capturable)
                        {
                            return x.Capturable.CompareTo(y.Capturable);
                        }
                        else return x.Frame.CompareTo(y.Frame);
                    }
                );
                WaveSetInfo best1 = best10[0];
                selectedWaves.Add(best1);
                rngStart = best1.Frame + 3600;
                rngEnd = rngStart + (2 * 3600);
            }
            int capturable = 0;
            foreach (WaveSetInfo wsi in selectedWaves)
            {
                capturable += wsi.Capturable;
                DrawPuzzles("T:\\IQ\\best", wsi);
            }
            Console.WriteLine("Best of the best of each stage totals {0}/3259 capturable cubes ({1:F1}%)", capturable, (capturable / 3259.0f) * 100);
        }

        static void Main(string[] args)
        {
            string puzzleDiagramsOutputDir = "T:\\IQ";
            byte[] puzzles = File.ReadAllBytes(@"..\..\..\KurushRNG\IQ-Group.new");
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

            List<StageWaveInfo> stage1 = new List<StageWaveInfo>(new StageWaveInfo[]
            {
                new StageWaveInfo(fourBy2Puzzles, TRNs.g_4x2TRNs, 4, 2, 3),
                new StageWaveInfo(fourBy2Puzzles, TRNs.g_4x2TRNs, 4, 2, 3),
                new StageWaveInfo(fourBy3Puzzles, TRNs.g_4x3TRNs, 4, 3, 3),
                new StageWaveInfo(fourBy4Puzzles, TRNs.g_4x4TRNs, 4, 4, 3)
            });
            List<StageWaveInfo> stage2 = new List<StageWaveInfo>(new StageWaveInfo[]
            {
                new StageWaveInfo(fourBy5Puzzles, TRNs.g_4x5TRNs, 4, 5, 3),
                new StageWaveInfo(fourBy5Puzzles, TRNs.g_4x5TRNs, 4, 5, 3),
                new StageWaveInfo(fourBy6Puzzles, TRNs.g_4x6TRNs, 4, 6, 3),
                new StageWaveInfo(fourBy6Puzzles, TRNs.g_4x6TRNs, 4, 6, 3)
            });
            List<StageWaveInfo> stage3 = new List<StageWaveInfo>(new StageWaveInfo[]
            {
                new StageWaveInfo(fiveBy4Puzzles, TRNs.g_5x4TRNs, 5, 4, 3),
                new StageWaveInfo(fiveBy5Puzzles, TRNs.g_5x5TRNs, 5, 5, 3),
                new StageWaveInfo(fiveBy6Puzzles, TRNs.g_5x6TRNs, 5, 6, 3),
                new StageWaveInfo(fiveBy6Puzzles, TRNs.g_5x6TRNs, 5, 6, 3)
            });
            List<StageWaveInfo> stage4 = new List<StageWaveInfo>(new StageWaveInfo[]
            {
                new StageWaveInfo(fiveBy7Puzzles, TRNs.g_5x7TRNs, 5, 7, 2),
                new StageWaveInfo(fiveBy7Puzzles, TRNs.g_5x7TRNs, 5, 7, 2),
                new StageWaveInfo(fiveBy8Puzzles, TRNs.g_5x8TRNs, 5, 8, 2),
                new StageWaveInfo(fiveBy8Puzzles, TRNs.g_5x8TRNs, 5, 8, 2)
            });
            List<StageWaveInfo> stage5 = new List<StageWaveInfo>(new StageWaveInfo[]
            {
                new StageWaveInfo(sixBy6Puzzles, TRNs.g_6x6TRNs, 6, 6, 3),
                new StageWaveInfo(sixBy6Puzzles, TRNs.g_6x6TRNs, 6, 6, 3),
                new StageWaveInfo(sixBy7Puzzles, TRNs.g_6x7TRNs, 6, 7, 3),
                new StageWaveInfo(sixBy7Puzzles, TRNs.g_6x7TRNs, 6, 7, 3)
            });
            List<StageWaveInfo> stage6 = new List<StageWaveInfo>(new StageWaveInfo[]
            {
                new StageWaveInfo(sixBy8Puzzles, TRNs.g_6x8TRNs, 6, 8, 2),
                new StageWaveInfo(sixBy8Puzzles, TRNs.g_6x8TRNs, 6, 8, 2),
                new StageWaveInfo(sixBy9Puzzles, TRNs.g_6x9TRNs, 6, 9, 2),
                new StageWaveInfo(sixBy9Puzzles, TRNs.g_6x9TRNs, 6, 9, 2)
            });
            List<StageWaveInfo> stage7 = new List<StageWaveInfo>(new StageWaveInfo[]
            {
                new StageWaveInfo(sevenBy7Puzzles, TRNs.g_7x7TRNs, 7, 7, 3),
                new StageWaveInfo(sevenBy7Puzzles, TRNs.g_7x7TRNs, 7, 7, 3),
                new StageWaveInfo(sevenBy8Puzzles, TRNs.g_7x8TRNs, 7, 8, 3),
                new StageWaveInfo(sevenBy8Puzzles, TRNs.g_7x8TRNs, 7, 8, 3)
            });
            List<StageWaveInfo> stage8 = new List<StageWaveInfo>(new StageWaveInfo[]
            {
                new StageWaveInfo(sevenBy8Puzzles, TRNs.g_7x8TRNs, 7, 8, 2),
                new StageWaveInfo(sevenBy9Puzzles, TRNs.g_7x9TRNs, 7, 9, 2),
                new StageWaveInfo(sevenBy9Puzzles, TRNs.g_7x9TRNs, 7, 9, 2),
                new StageWaveInfo(sevenBy9Puzzles, TRNs.g_7x9TRNs, 7, 9, 2)
            });
            List<StageWaveInfo> stageF = new List<StageWaveInfo>(new StageWaveInfo[]
            {
                new StageWaveInfo(sevenBy9Puzzles, TRNs.g_7x9TRNs, 7, 9, 1),
                new StageWaveInfo(sevenBy9Puzzles, TRNs.g_7x9TRNs, 7, 9, 1),
                new StageWaveInfo(sevenBy9Puzzles, TRNs.g_7x9TRNs, 7, 9, 1),
                new StageWaveInfo(sevenBy9Puzzles, TRNs.g_7x9TRNs, 7, 9, 1)
            });
            //List<StageWaveInfo>[] stages = new List<StageWaveInfo>[9] {
            //    stage1, stage2, stage3, stage4, stage5, stage6, stage7, stage8, stageF
            //};

            //FindBestOverallCaptures(stages);

            int startPoint = 0xb9be;
            int endPoint = 0xb9d2;
            //int endPoint = 0x3c41;
            List<WaveSetInfo> best10Sets = Find10BestInRNGRange(startPoint, endPoint, stageF);
            
            //Console.WriteLine("Found {0} candidates out of a total of {1}", numGood, (endPoint - startPoint) + 1);
            Console.WriteLine("Best 10 shortest turn sequences found between frames {0}-{1} are", startPoint, endPoint);
            foreach (WaveSetInfo result in best10Sets)
            {
                Console.WriteLine("Frame {0} - {1} points, {2} captures, {3} turns", result.Frame, result.Points, result.Capturable, result.Turns);
                PrintPuzzles(result.Set);
                DrawPuzzles(puzzleDiagramsOutputDir, result);
            }
        }

        private static void DrawPuzzles(string outDir, WaveSetInfo puzzleSetInfo)
        {
            int maxTileWidth = 6;
            int numRows = 0;
            foreach (Wave w in puzzleSetInfo.Set)
            {
                numRows += (w.height + 2);
                maxTileWidth = Math.Max(maxTileWidth, (w.width + 1) * 3);
            }
            numRows += 2; // for header & footer
            const int rectWidth = 40;
            const int rectHeight = 40;
            using (Bitmap bm = new Bitmap((maxTileWidth * rectWidth) + 20, (numRows * rectHeight) + 10, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
            {
                Brush textColour = Brushes.Black;
                Pen p = Pens.Black;
                using(Font f = new Font(FontFamily.GenericSansSerif, 16))
                using (Graphics g = Graphics.FromImage(bm))
                {
                    g.Clear(Color.DarkGray);
                    string header = String.Format("Frame {0} - Points {1} - Captures {2} - Turns {3} - flipped already flipped", puzzleSetInfo.Frame, puzzleSetInfo.Points, puzzleSetInfo.Capturable, puzzleSetInfo.Turns);
                    g.DrawString(header, f, textColour, new PointF(0, 0));
                    int wave = 1;
                    int yLoc = 40;
                    int[] qubes = new int[3]{0, 0, 0};
                    Brush[] qubeBrushes = new Brush[3] {Brushes.BurlyWood, Brushes.Green, Brushes.Black};
                    foreach(Wave w in puzzleSetInfo.Set)
                    {
                        string waveHeaderText = String.Format("Wave {0}: {1}x{2}", wave, w.width, w.height);
                        g.DrawString(waveHeaderText, f, textColour, new PointF(0, yLoc));
                        yLoc += rectHeight;
                        int xLoc = 0;
                        string puzzleHeaderFormat = "Puzzle {0} - TRN {1}{2}{3}";
                        int puzzle = 0;
                        PickedPuzzle pp = w[puzzle];
                        g.DrawString(
                            String.Format(
                                puzzleHeaderFormat, 
                                pp.PuzzleNumber + 1, 
                                w.allTrns[pp.PuzzleNumber], 
                                pp.flipped ? " (f)" : String.Empty,
                                pp.isDupe ? " (s)" : String.Empty
                            ),
                            f,
                            textColour,
                            new PointF(0, yLoc)
                        );
                        yLoc += rectHeight;
                        for (int y = 0; y < w.height; ++y)
                        {
                            puzzle = 0;
                            pp = w[puzzle];
                            for (int x = 0, puzzX = 0; x < (w.width * w.numPuzzles); ++x, ++puzzX)
                            {
                                if ((x != 0) && ((x % w.width) == 0))
                                {
                                    ++puzzle;
                                    pp = w[puzzle];
                                    puzzX = 0;
                                    xLoc += rectWidth;
                                    if (y == 0)
                                    {
                                        g.DrawString(
                                            String.Format(
                                                puzzleHeaderFormat, 
                                                pp.PuzzleNumber + 1, 
                                                w.allTrns[pp.PuzzleNumber], 
                                                pp.flipped ? " (f)" : String.Empty,
                                                pp.isDupe ? " (s)" : String.Empty
                                            ),
                                            f,
                                            textColour,
                                            new PointF(xLoc, yLoc - rectHeight)
                                        );
                                    }
                                }
                                byte qube = pp.puzzle[(y * w.width) + puzzX];
                                Brush b = qubeBrushes[qube];
                                ++qubes[qube];
                                Rectangle r = new Rectangle(xLoc, yLoc, rectWidth, rectHeight);
                                g.FillRectangle(b, r);
                                g.DrawRectangle(p, r);
                                xLoc += rectWidth;
                            }
                            yLoc += rectHeight;
                            xLoc = 0;
                        }
                        ++wave;
                    }
                    int totalCubes = qubes[0] + qubes[1] + qubes[2];
                    float totalCubesF = (float)totalCubes;
                    g.DrawString(
                        String.Format(
                            "Normal {0}/{3}({4:F1}%), Adv {1}/{3}({5:F1}%) Forb {2}/{3}({6:F1}%)",
                            qubes[0],
                            qubes[1],
                            qubes[2],
                            totalCubes,
                            (qubes[0] / totalCubesF) * 100,
                            (qubes[1] / totalCubesF) * 100,
                            (qubes[2] / totalCubesF) * 100
                        ),
                        f,
                        textColour,
                        new PointF(0, yLoc)
                    );
                }
#if CAPTURES
                string fileNameFormat = "{2}captures-{1}points-frame{0}.png";
#elif POINTS
                string fileNameFormat = "{1}points-{2}captures-frame{0}.png";
#elif TURNS
                string fileNameFormat = "{3}turns-{2}captures-frame{0}.png";
#endif
                string fileName = String.Format(fileNameFormat, puzzleSetInfo.Frame, puzzleSetInfo.Points, puzzleSetInfo.Capturable, puzzleSetInfo.Turns);
                bm.Save(Path.Combine(outDir, fileName), System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private static void PrintPuzzles(WaveSet puzzleSet)
        {
            const string flipped = "(f) ";
            foreach (Wave w in puzzleSet)
            {
                Console.Write("{0}x{1}: ", w.width, w.height);
                foreach (PickedPuzzle p in w)
                {
                    Console.Write("{0} {1}{2}", p.PuzzleNumber + 1, p.flipped ? flipped : String.Empty, p.isDupe ? "(s)" : String.Empty);
                }
                Console.WriteLine();
            }
        }

        private static Wave GetPuzzleSet(StageWaveInfo waveInfo)
        {
            int points = 0;
            int width = waveInfo.Width;
            int height = waveInfo.Height;
            int area = width * height;
            int numPuzzles = waveInfo.Puzzles;
            List<PickedPuzzle> wavePuzzles = new List<PickedPuzzle>(numPuzzles);
            bool flipped;
            int[] trns = waveInfo.TRNs;
            for (int i = 0; i < numPuzzles; ++i)
            {
                int nextPuzzle = GetNextPuzzlePattern(out flipped);
                Debug.Assert((nextPuzzle >= 0) && (nextPuzzle < 200));
                int puzTrn = trns[nextPuzzle];
                //if (puzTrn == 1) return null;
                byte[] puzzle = CopyPuzzle(waveInfo.PuzzleData, nextPuzzle * 70, width, height, flipped);
                bool hasForbidden;
                int thisPuzzlePoints = CalcPuzzlePoints(puzzle, out hasForbidden);
                PickedPuzzle puzz = new PickedPuzzle(puzzle, nextPuzzle, thisPuzzlePoints, flipped, !hasForbidden, false);
                wavePuzzles.Add(puzz);
                points += thisPuzzlePoints;
            }
            return new Wave(wavePuzzles, width, height, waveInfo.PuzzleData, trns);
        }

        private static PickedPuzzle GetPuzzlePoints(byte[] puzzles, int[] trns, int width, int height, PickedPuzzle pp)
        {
            int puzTrn = trns[pp.PuzzleNumber];
            byte[] puzzle = CopyPuzzle(puzzles, pp.PuzzleNumber * 70, width, height, pp.flipped);
            bool hasForbidden;
            int thisPuzzlePoints = CalcPuzzlePoints(puzzle, out hasForbidden);
            return new PickedPuzzle(puzzle, pp.PuzzleNumber, (puzTrn == 1) ? 0 : thisPuzzlePoints, pp.flipped, !hasForbidden, pp.isDupe);
        }

        private static WaveSet CheckSquashedScores(WaveSet waves)
        {
            return DoWaveDupeRecursion(waves, 0, 0);
        }

        private static WaveSet DoWaveDupeRecursion(WaveSet baseSet, int wave, int puzzle)
        {
            int numWaves = baseSet.waves.Count;
            if (
                (wave >= numWaves) ||
                ((puzzle >= baseSet[wave].numPuzzles)) && (wave == (numWaves - 1)))
            {
                return baseSet;
            }
            Wave w = baseSet[wave];
            if (puzzle >= w.numPuzzles)
            {
                ++wave;
                w = baseSet[wave]; 
                puzzle = 0;
            }
            WaveSet afterDupingThisPuzzle;
            // we can't dupe the last puzzle of each wave
            // or if it has a forbidden cube
            if ((puzzle == (w.numPuzzles - 1)) || (!w[puzzle].canDupe))
            {
                afterDupingThisPuzzle = baseSet;
            }
            else
            {
                afterDupingThisPuzzle = CalculateKnockOnEffect(baseSet, wave, puzzle);
                // if this change would've introduced a puzzle with a TRN of 1
                // discard this dupe
                if (afterDupingThisPuzzle == null) afterDupingThisPuzzle = baseSet;
            }
            WaveSet noDupeSet = DoWaveDupeRecursion(baseSet, wave, puzzle + 1);
            WaveSet dupeSet = noDupeSet;
            if (afterDupingThisPuzzle != baseSet)
            {
                dupeSet = DoWaveDupeRecursion(afterDupingThisPuzzle, wave, puzzle + 2);
            }
#if POINTS
            int dupePoints = dupeSet.Points;
            int noDupePoints = noDupeSet.Points;
            WaveSet ret = dupePoints > noDupePoints ? dupeSet : noDupeSet;
#elif TURNS
            int dupeTurns = dupeSet.Turns;
            int noDupeTurns = noDupeSet.Turns;
            WaveSet ret = dupeTurns < noDupeTurns ? dupeSet : noDupeSet;
#elif CAPTURES
            int dupeCaptures = dupeSet.Capturable;
            int noDupeCaptures = noDupeSet.Capturable;
            WaveSet ret = dupeCaptures > noDupeCaptures ? dupeSet : noDupeSet;
#endif
            return ret;
        }

        private static WaveSet CalculateKnockOnEffect(WaveSet originalSet, int startWaveIdx, int startPoint)
        {
            WaveSet newSet = originalSet.Clone();
            Debug.Assert(startPoint < (newSet[startWaveIdx].numPuzzles - 1));
            // for these waves, ditch the last puzzle score and calculate the score of
            // the last puzzle of the previous wave as if that puzzle number had happened in this wave
            for (int i = 3; i > startWaveIdx; --i)
            {
                Wave w = newSet[i];
                List<PickedPuzzle> wavePuzzles = w.puzzles;
                wavePuzzles.RemoveAt(wavePuzzles.Count - 1);
                Wave prevWave = newSet[i - 1];
                List<PickedPuzzle> prevWavePuzzles = prevWave.puzzles;
                PickedPuzzle lastPrevWave = prevWavePuzzles[prevWavePuzzles.Count - 1];
                PickedPuzzle prevLastInThisWave = GetPuzzlePoints(w.allPuzzleData, w.allTrns, w.width, w.height, lastPrevWave);
                // puzzles with a TRN of 1 are no good, so we cancel
                if (prevLastInThisWave.Points == 0) return null;
                prevLastInThisWave.flipped = lastPrevWave.flipped;
                wavePuzzles.Insert(0, prevLastInThisWave);
                w.Recalculate();
            }
            // on this wave, we just need to insert the highscore at the same position (ie duplicate it)
            // then remove the last puzzle
            Wave startWave = newSet[startWaveIdx];
            List<PickedPuzzle> startWavePuzzles = startWave.puzzles;
            startWavePuzzles.Insert(startPoint, startWavePuzzles[startPoint].CloneAsDupe());
            startWavePuzzles.RemoveAt(startWavePuzzles.Count - 1);
            startWave.Recalculate();
//#if DEBUG
//            Debug.WriteLine("Orig puzzle set:");
//            for (int i = 0; i < originalSet.waves.Count; ++i)
//            {
//                Wave oldWave = originalSet[i];
//                for (int j = 0; j < oldWave.numPuzzles; ++j)
//                {
//                    PickedPuzzle pp = oldWave.puzzles[j];
//                    Debug.Write(String.Format("{0}={1} ", pp.puzzleNumber, pp.points));
//                }
//            }
//            Debug.WriteLine(String.Format("{0}New puzzle set:", Environment.NewLine));
//            for (int i = 0; i < newSet.waves.Count; ++i)
//            {
//                Wave newWave = newSet[i];
//                for (int j = 0; j < newWave.numPuzzles; ++j)
//                {
//                    PickedPuzzle pp = newWave.puzzles[j];
//                    Debug.Write(String.Format("{0}={1} ", pp.puzzleNumber, pp.points));
//                }
//            }
//            Debug.WriteLine("");
//#endif
            return newSet;
        }

        private static int CalcPuzzlePoints(byte[] puzzle, out bool hasForbidden)
        {
            int points = 10000;
            int extraPoints = 0;
            hasForbidden = false;
            foreach (byte b in puzzle)
            {
                if (b < 2)
                {
                    points += ((b + 1) * 100);
                    // add on extra points for advantage cubes
                    if (b == 1)
                    {
                        extraPoints += 100;
                    }
                }
                else
                {
                    // subtract extra points for forbidden cubes
                    extraPoints -= 100;
                    hasForbidden = true;
                }
            }
            return points + extraPoints;
        }
    }
}
