using System;
using System.Collections.Generic;
using System.Text;

namespace ZombieSmashers.net
{
    class NetPacker
    {
        public static byte TinyFloatToByte(float f)
        {
            f *= 255f;
            if (f > 255f) f = 255f;
            if (f < 0f) f = 0f;
            return (byte)f;
        }

        public static float ByteToTinyFloat(byte b)
        {
            float f = (float)b;
            return f / 255f;
        }

        public static short IntToShort(int i)
        {
            if (i > short.MaxValue) i = short.MaxValue;
            if (i < short.MinValue) i = short.MinValue;
            return (short)i;
        }

        public static int ShortToInt(short s)
        {
            return (int)s;
        }

        public static sbyte IntToSbyte(int i)
        {
            if (i > sbyte.MaxValue) i = sbyte.MaxValue;
            if (i < sbyte.MinValue) i = sbyte.MinValue;
            return (sbyte)i;
        }

        public static int SbyteToInt(sbyte s)
        {
            return (int)s;
        }

        public static short BigFloatToShort(float f)
        {
            if (f > short.MaxValue) f = short.MaxValue;
            if (f < short.MinValue) f = short.MinValue;
            return (short)f;
        }

        public static float ShortToBigFloat(short s)
        {
            return (float)s;
        }

        public static short MidFloatToShort(float f)
        {
            f *= 5f;
            if (f > short.MaxValue) f = short.MaxValue;
            if (f < short.MinValue) f = short.MinValue;
            return (short)f;
        }

        public static float ShortToMidFloat(short s)
        {
            return (float)(s) / 5f;
        }

        public static short SmallFloatToShort(float f)
        {
            f *= 20f;
            if (f > short.MaxValue) f = short.MaxValue;
            if (f < short.MinValue) f = short.MinValue;
            return (short)f;
        }

        public static float ShortToSmallFloat(short s)
        {
            return (float)(s) / 20f;
        }
    }
}
