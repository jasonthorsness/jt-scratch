// Copyright 2022 Jason Thorsness
namespace Jt.Scratch.Tests.Unit;

using MongoDB.Bson;
using Xunit;

/// <summary>.</summary>
public class BsonPrettyPrinterTests
{
    /// <summary>.</summary>
    [Fact]
    public void Basic()
    {
        string prettyPrinted = BsonPrettyPrinter.PrettyPrint("{a:1.0}");
        Assert.Equal(
            @"10000000
 01 6100 000000000000F03F",
            prettyPrinted);
    }

    /// <summary>.</summary>
    [Fact]
    public void All()
    {
        BsonDocument bsonDocument = new();
        bsonDocument.Add(new BsonElement("a", new BsonArray()
        {
            new BsonDouble(0),
            new BsonString("baz"),
            new BsonDocument(),
            new BsonArray(),
            new BsonBinaryData(default, GuidRepresentation.Standard),
            BsonUndefined.Value,
            new BsonObjectId(new ObjectId(new byte[12])),
            new BsonBoolean(true),
            new BsonDateTime(0),
            BsonNull.Value,
            new BsonRegularExpression("foo", "i"),
            new BsonJavaScript("foo"),
            BsonSymbol.Create("bog"),
            new BsonJavaScriptWithScope("bar", new BsonDocument()),
            new BsonInt32(0),
            new BsonTimestamp(0, 0),
            new BsonInt64(0),
            new BsonDecimal128(0),
            BsonMinKey.Value,
            BsonMaxKey.Value,
        }));

        string input = bsonDocument.ToString();

        string prettyPrinted = BsonPrettyPrinter.PrettyPrint(input);
        Assert.Equal(
            @"E2000000
 04 6100 DA000000
  01 3000 0000000000000000
  02 3100 0400000062617A00
  03 3200 05000000
  04 3300 05000000
  05 3400 100000000300000000000000000000000000000000
  06 3500 
  07 3600 000000000000000000000000
  08 3700 01
  09 3800 0000000000000000
  0A 3900 
  0B 313000 666F6F006900
  0D 313100 04000000666F6F00
  0E 313200 04000000626F6700
  0F 313300 1100000004000000626172000500000000
  10 313400 00000000
  11 313500 0000000000000000
  12 313600 0000000000000000
  13 313700 00000000000000000000000000004030
  FF 313800 
  7F 313900 ",
            prettyPrinted);
    }
}
