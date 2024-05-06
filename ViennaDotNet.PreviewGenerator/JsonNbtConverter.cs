using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using ViennaDotNet.Common.Excceptions;
using ViennaDotNet.PreviewGenerator.NBT;

namespace ViennaDotNet.PreviewGenerator
{
    internal class JsonNbtConverter
    {
        public static JsonNbtTag convert(NbtMap tag)
        {
            Dictionary<string, JsonNbtTag> value = new();
            foreach (var entry in tag.entrySet())
                value[entry.Key] = convert(entry.Value);

            return new CompoundJsonNbtTag(value);
        }

        public static JsonNbtTag convert(NbtList tag)
        {
            LinkedList<JsonNbtTag> value = new();
            foreach (object item in tag)
                value.AddLast(convert(item));

            return new ListJsonNbtTag(value.ToArray());
        }

        private static JsonNbtTag convert(object tag)
        {
            if (tag is NbtMap map)
                return convert(map);
            else if (tag is NbtList list)
                return convert(list);
            else if (tag is int i)
                return new IntJsonNbtTag(i);
            else if (tag is byte b)
                return new ByteJsonNbtTag(b);
            else if (tag is float f)
                return new FloatJsonNbtTag(f);
            else if (tag is string s)
                return new StringJsonNbtTag(s);
            else
                throw new UnsupportedOperationException($"Cannot convert tag of type {tag.GetType().Name}");
        }

        public abstract class JsonNbtTag
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public enum Type
            {
                [EnumMember(Value = "compound")] COMPOUND,
                [EnumMember(Value = "list")] LIST,
                [EnumMember(Value = "int")] INT,
                [EnumMember(Value = "byte")] BYTE,
                [EnumMember(Value = "float")] FLOAT,
                [EnumMember(Value = "string")] STRING
            }

            public readonly Type type;
            public readonly object value;

            public JsonNbtTag(Type type, object value)
            {
                this.type = type;
                this.value = value;
            }
        }

        public sealed class CompoundJsonNbtTag : JsonNbtTag
        {
            public CompoundJsonNbtTag(Dictionary<string, JsonNbtTag> value)
                : base(Type.COMPOUND, value)
            {
            }
        }

        public sealed class ListJsonNbtTag : JsonNbtTag
        {
            public ListJsonNbtTag(JsonNbtTag[] value)
                : base(Type.LIST, value)
            {
            }
        }

        public sealed class IntJsonNbtTag : JsonNbtTag
        {
            public IntJsonNbtTag(int value)
                : base(Type.INT, value)
            {
            }
        }

        public sealed class ByteJsonNbtTag : JsonNbtTag
        {
            public ByteJsonNbtTag(byte value)
                : base(Type.BYTE, value)
            {
            }
        }

        public sealed class FloatJsonNbtTag : JsonNbtTag
        {
            public FloatJsonNbtTag(float value)
                : base(Type.FLOAT, value)
            {
            }
        }

        public sealed class StringJsonNbtTag : JsonNbtTag
        {
            public StringJsonNbtTag(string value)
                : base(Type.STRING, value)
            {
            }
        }
    }
}
