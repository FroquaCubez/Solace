using System.Text;

namespace ViennaDotNet.PreviewGenerator.NBT
{
    public static class NbtUtils
    {
        public static readonly int MAX_DEPTH = 16;
        public static readonly long MAX_READ_SIZE = 0; // Disabled by default

        public static string toString(object o)
        {
            if (o is Byte)
            {
                return ((byte)o) + "b";
            }
            else if (o is short)
            {
                return ((short)o) + "s";
            }
            else if (o is int)
            {
                return ((int)o) + "i";
            }
            else if (o is long)
            {
                return ((long)o) + "l";
            }
            else if (o is float)
            {
                return ((float)o) + "f";
            }
            else if (o is double)
            {
                return ((double)o) + "d";
            }
            else if (o is byte[])
            {
                return "0x" + printHexBinary((byte[])o);
            }
            else if (o is string)
            {
                return "\"" + o + "\"";
            }
            else if (o is int[] intAr)
            {
                List<string> joined = new();
                foreach (int i in intAr)
                    joined.Add(i + "i");

                return "[ " + string.Join(", ", joined) + " ]";
            }
            else if (o is long[] longAr)
            {
                List<string> joined = new();
                foreach (long l in longAr)
                    joined.Add(l + "l");

                return "[ " + string.Join(", ", joined) + " ]";
            }

            return o.ToString()!;
        }

        public static T copy<T>(T val)
        {
            if (val is byte[] bytes)
                return (T)bytes.Clone();
            else if (val is int[] ints)
                return (T)ints.Clone();
            else if (val is long[] longs)
                return (T)longs.Clone();

            return val;
        }

        public static object copyObject(object val)
        {
            if (val is byte[] bytes)
                return bytes.Clone();
            else if (val is int[] ints)
                return ints.Clone();
            else if (val is long[] longs)
                return longs.Clone();

            return val;
        }

        public static string indent(string str)
        {
            StringBuilder builder = new StringBuilder("  " + str);
            for (int i = 2; i < builder.Length; i++)
            {
                if (builder[i] == '\n')
                {
                    builder.Insert(i + 1, "  ");
                    i += 2;
                }
            }
            return builder.ToString();
        }

        private static readonly char[] HEX_CODE = "0123456789ABCDEF".ToArray();

        public static string printHexBinary(byte[] data)
        {
            StringBuilder r = new StringBuilder(data.Length << 1);
            foreach (byte b in data)
            {
                r.Append(HEX_CODE[(b >> 4) & 0xF]);
                r.Append(HEX_CODE[(b & 0xF)]);
            }
            return r.ToString();
        }
    }
}
