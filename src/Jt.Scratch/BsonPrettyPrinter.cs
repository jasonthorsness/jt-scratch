// Copyright 2022 Jason Thorsness
namespace Jt.Scratch;

using System.Buffers.Binary;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

public static class BsonPrettyPrinter
{
    public static string PrettyPrint(string bsonShellString)
    {
        // Parse as Mongo shell string
        BsonDocument parsed = BsonDocument.Parse(bsonShellString);

        // Serialize to BSON
        using MemoryStream memoryStream = new();
        using BsonBinaryWriter bsonBinaryWriter = new(memoryStream);
        BsonSerializer.Serialize(bsonBinaryWriter, parsed);
        ReadOnlySpan<byte> bson = memoryStream.ToArray();

        // Pretty-print
        StringBuilder stringBuilder = new();
        SerializeDocument(ref bson, stringBuilder, 0);
        return stringBuilder.ToString();
    }

    private static void SerializeDocument(ref ReadOnlySpan<byte> bson, StringBuilder stringBuilder, int level)
    {
        static void ReadAndPrint(ref ReadOnlySpan<byte> bson, int length, StringBuilder stringBuilder)
        {
            stringBuilder.Append(Convert.ToHexString(bson[..length]));
            bson = bson[length..];
        }

        ReadAndPrint(ref bson, sizeof(int), stringBuilder);

        if (bson[0] == 0)
        {
            bson = bson[1..];
            return;
        }

        stringBuilder.Append('\n');
        level++;

        while (true)
        {
            BsonType bsonType = (BsonType)bson[0];
            stringBuilder.Append(new string(' ', level));
            ReadAndPrint(ref bson, sizeof(byte), stringBuilder);
            stringBuilder.Append(' ');

            int nullIndex = bson.IndexOf((byte)0);
            ReadAndPrint(ref bson, nullIndex + 1, stringBuilder);
            stringBuilder.Append(' ');

            if (bsonType is (BsonType.Array or BsonType.Document))
            {
                SerializeDocument(ref bson, stringBuilder, level);
            }
            else if (bsonType is BsonType.Binary)
            {
                int length = sizeof(int) + 1 + BinaryPrimitives.ReadInt32LittleEndian(bson);
                ReadAndPrint(ref bson, length, stringBuilder);
            }
            else if (bsonType is (BsonType.String or BsonType.JavaScript or BsonType.Symbol))
            {
                int length = sizeof(int) + BinaryPrimitives.ReadInt32LittleEndian(bson);
                ReadAndPrint(ref bson, length, stringBuilder);
            }
            else if (bsonType is BsonType.JavaScriptWithScope)
            {
                int length = BinaryPrimitives.ReadInt32LittleEndian(bson);
                ReadAndPrint(ref bson, length, stringBuilder);
            }
            else if (bsonType is BsonType.RegularExpression)
            {
                nullIndex = bson.IndexOf((byte)0);
                ReadAndPrint(ref bson, nullIndex + 1, stringBuilder);
                nullIndex = bson.IndexOf((byte)0);
                ReadAndPrint(ref bson, nullIndex + 1, stringBuilder);
            }
            else
            {
                int length = bsonType switch
                {
                    BsonType.Null => 0,
                    BsonType.Undefined => 0,
                    BsonType.MinKey => 0,
                    BsonType.MaxKey => 0,
                    BsonType.Boolean => 1,
                    BsonType.Int32 => 4,
                    BsonType.Int64 => 8,
                    BsonType.Double => 8,
                    BsonType.Timestamp => 8,
                    BsonType.DateTime => 8,
                    BsonType.ObjectId => 12,
                    BsonType.Decimal128 => 16,
                    _ => throw new FormatException($"Unexpected BsonType {bsonType:X}"),
                };
                ReadAndPrint(ref bson, length, stringBuilder);
            }

            if (bson[0] == 0)
            {
                bson = bson[1..];
                break;
            }

            stringBuilder.Append('\n');
        }
    }
}
