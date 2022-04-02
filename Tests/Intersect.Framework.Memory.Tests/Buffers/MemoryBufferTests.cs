using System.Collections;
using System.Runtime.InteropServices;

using Intersect.Framework.Memory.Buffers;
using Intersect.Framework.Randomization;

using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Intersect.Framework.Memory.Tests.Buffers;

[TestFixture]
[TestOf(typeof(MemoryBuffer))]
public class MemoryBufferTests
{
    private MemoryBuffer _memoryBuffer = default!;

    [SetUp]
    public void Setup()
    {
        _memoryBuffer = new MemoryBuffer();
    }

    [TearDown]
    public void TearDown()
    {
        _memoryBuffer?.Dispose();
    }

    public class NumericalTestCase
    {
        public object Data { get; }

        public int SizeOf { get; }

        public NumericalTestCase(object data, int sizeOf)
        {
            Data = data;
            SizeOf = sizeOf;
        }
    }

    public interface ITestData : IEnumerable
    {
        Type Type { get; }

        int SizeOf { get; }
    }

    public interface ITestData<T> : ITestData, IEnumerable<T>
    {
    }

    public unsafe class NumericalTestData<T> : ITestData<TestCaseData> where T : unmanaged
    {
        private readonly int _count;

        public NumericalTestData(int count)
        {
            _count = count;
        }

        public Type Type => typeof(T);

        public int SizeOf => sizeof(T);

        IEnumerator<T> GetDataEnumerator()
        {
            yield return default;
            for (var runs = 0; runs < _count - 1; ++runs)
            {
                var randomValue = (T)(default(T) switch
                {
                    (byte _) => (object)unchecked((byte)(runs & 0xFF)),
                    (char _) => Random.Shared.NextChar(),
                    (double _) => Random.Shared.NextDouble(),
                    (float _) => Random.Shared.NextFloat(),
                    (int _) => Random.Shared.Next(),
                    (long _) => Random.Shared.NextLong(),
                    (sbyte _) => unchecked((sbyte)(runs & 0xFF)),
                    (short _) => Random.Shared.NextShort(),
                    (uint _) => Random.Shared.NextUInt(),
                    (ulong _) => Random.Shared.NextULong(),
                    (ushort _) => Random.Shared.NextUShort(),
                    _ => throw new NotSupportedException(typeof(T).FullName)
                });

                yield return randomValue;
            }
        }

        public IEnumerator<TestCaseData> GetEnumerator()
        {
            var enumerator = GetDataEnumerator();
            while (enumerator.MoveNext())
            {
                var numericalTestCase = new NumericalTestCase(enumerator.Current, SizeOf);
                yield return new TestCaseData(numericalTestCase).SetName(typeof(T).Name);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private unsafe void TestWrite<T>(IEnumerable<T> values) where T : unmanaged
    {
        int written;
        byte[] valueBytes;
        foreach (var value in values)
        {
            written = value switch
            {
                (char typedValue) => _memoryBuffer.Write(typedValue),
                (double typedValue) => _memoryBuffer.Write(typedValue),
                (float typedValue) => _memoryBuffer.Write(typedValue),
                (int typedValue) => _memoryBuffer.Write(typedValue),
                (long typedValue) => _memoryBuffer.Write(typedValue),
                (short typedValue) => _memoryBuffer.Write(typedValue),
                (uint typedValue) => _memoryBuffer.Write(typedValue),
                (ulong typedValue) => _memoryBuffer.Write(typedValue),
                (ushort typedValue) => _memoryBuffer.Write(typedValue),
                _ => throw new NotSupportedException(typeof(T).FullName)
            };

            valueBytes = value switch
            {
                (char typedValue) => BitConverter.GetBytes(typedValue),
                (double typedValue) => BitConverter.GetBytes(typedValue),
                (float typedValue) => BitConverter.GetBytes(typedValue),
                (int typedValue) => BitConverter.GetBytes(typedValue),
                (long typedValue) => BitConverter.GetBytes(typedValue),
                (short typedValue) => BitConverter.GetBytes(typedValue),
                (uint typedValue) => BitConverter.GetBytes(typedValue),
                (ulong typedValue) => BitConverter.GetBytes(typedValue),
                (ushort typedValue) => BitConverter.GetBytes(typedValue),
                _ => throw new NotSupportedException(typeof(T).FullName)
            };

            Assert.AreEqual(sizeof(T), written);
            Assert.AreEqual(sizeof(T), _memoryBuffer.Position);
            Assert.AreEqual(valueBytes, _memoryBuffer.Buffer.Span[0..sizeof(T)].ToArray());
            _memoryBuffer.Position = 0;
        }
    }

    static IEnumerable<TestCaseData> ByteData(string testBaseName, int count) => NumericalData<byte>(testBaseName, count);
    static IEnumerable<TestCaseData> CharData(string testBaseName, int count) => NumericalData<char>(testBaseName, count);
    static IEnumerable<TestCaseData> DoubleData(string testBaseName, int count) => NumericalData<double>(testBaseName, count);
    static IEnumerable<TestCaseData> FloatData(string testBaseName, int count) => NumericalData<float>(testBaseName, count);
    static IEnumerable<TestCaseData> IntData(string testBaseName, int count) => NumericalData<int>(testBaseName, count);
    static IEnumerable<TestCaseData> LongData(string testBaseName, int count) => NumericalData<long>(testBaseName, count);
    static IEnumerable<TestCaseData> SByteData(string testBaseName, int count) => NumericalData<sbyte>(testBaseName, count);
    static IEnumerable<TestCaseData> ShortData(string testBaseName, int count) => NumericalData<short>(testBaseName, count);
    static IEnumerable<TestCaseData> UIntData(string testBaseName, int count) => NumericalData<uint>(testBaseName, count);
    static IEnumerable<TestCaseData> ULongData(string testBaseName, int count) => NumericalData<ulong>(testBaseName, count);
    static IEnumerable<TestCaseData> UShortData(string testBaseName, int count) => NumericalData<ushort>(testBaseName, count);

    static IEnumerable<TestCaseData> NumericalData<T>(string testBaseName, int count) where T : unmanaged
    {
        foreach (var value in new NumericalTestData<T>(count))
            yield return value.SetName($"{testBaseName}({value.TestName})");
    }

    //static IEnumerable<TestCaseData> StringData(string testBaseName, int count)
    //{
        
    //}

    [TestCaseSource(nameof(ByteData), new object[] { nameof(TestWrite), 256 })]
    [TestCaseSource(nameof(CharData), new object[] { nameof(TestWrite), 256 })]
    [TestCaseSource(nameof(DoubleData), new object[] { nameof(TestWrite), 256 })]
    [TestCaseSource(nameof(FloatData), new object[] { nameof(TestWrite), 256 })]
    [TestCaseSource(nameof(IntData), new object[] { nameof(TestWrite), 256 })]
    [TestCaseSource(nameof(LongData), new object[] { nameof(TestWrite), 256 })]
    [TestCaseSource(nameof(SByteData), new object[] { nameof(TestWrite), 256 })]
    [TestCaseSource(nameof(ShortData), new object[] { nameof(TestWrite), 256 })]
    [TestCaseSource(nameof(UIntData), new object[] { nameof(TestWrite), 256 })]
    [TestCaseSource(nameof(ULongData), new object[] { nameof(TestWrite), 256 })]
    [TestCaseSource(nameof(UShortData), new object[] { nameof(TestWrite), 256 })]
    [Test]
    public void TestWrite(NumericalTestCase testCase)
    {
        var value = testCase.Data;
        int written;
        byte[] valueBytes;
        written = value switch
        {
            (byte typedValue) => _memoryBuffer.Write(typedValue),
            (char typedValue) => _memoryBuffer.Write(typedValue),
            (double typedValue) => _memoryBuffer.Write(typedValue),
            (float typedValue) => _memoryBuffer.Write(typedValue),
            (int typedValue) => _memoryBuffer.Write(typedValue),
            (long typedValue) => _memoryBuffer.Write(typedValue),
            (sbyte typedValue) => _memoryBuffer.Write(typedValue),
            (short typedValue) => _memoryBuffer.Write(typedValue),
            (uint typedValue) => _memoryBuffer.Write(typedValue),
            (ulong typedValue) => _memoryBuffer.Write(typedValue),
            (ushort typedValue) => _memoryBuffer.Write(typedValue),
            _ => throw new NotSupportedException(value.GetType().FullName)
        };

        valueBytes = value switch
        {
            (byte typedValue) => new[] { typedValue },
            (char typedValue) => BitConverter.GetBytes(typedValue),
            (double typedValue) => BitConverter.GetBytes(typedValue),
            (float typedValue) => BitConverter.GetBytes(typedValue),
            (int typedValue) => BitConverter.GetBytes(typedValue),
            (long typedValue) => BitConverter.GetBytes(typedValue),
            (sbyte typedValue) => new[] { unchecked((byte)typedValue) },
            (short typedValue) => BitConverter.GetBytes(typedValue),
            (uint typedValue) => BitConverter.GetBytes(typedValue),
            (ulong typedValue) => BitConverter.GetBytes(typedValue),
            (ushort typedValue) => BitConverter.GetBytes(typedValue),
            _ => throw new NotSupportedException(value.GetType().FullName)
        };

        Assert.AreEqual(testCase.SizeOf, written);
        Assert.AreEqual(testCase.SizeOf, _memoryBuffer.Position);
        Assert.AreEqual(valueBytes, _memoryBuffer.Buffer.Span[0..testCase.SizeOf].ToArray());
        _memoryBuffer.Position = 0;
    }

    static IEnumerable<TestCaseData> VariableCharData(string testBaseName) =>
        VariableShortData(testBaseName).Select(testCaseData => new TestCaseData(testCaseData.Arguments)
        {
            TestName = $"{testBaseName}{typeof(char).Name}",
            ExpectedResult = testCaseData.ExpectedResult,
        });

    static IEnumerable<TestCaseData> VariableShortData(string testBaseName)
    {
        var testName = $"{testBaseName}({typeof(short).Name})";
        yield return new TestCaseData(0).SetName(testName).Returns(new byte[] { 0 });
        yield return new TestCaseData(-1).SetName(testName).Returns(new byte[] { 1 });
        yield return new TestCaseData(1).SetName(testName).Returns(new byte[] { 2 });
        yield return new TestCaseData(short.MaxValue).SetName(testName).Returns(new byte[] { 0xfe, 0xff, 0x3 });
        yield return new TestCaseData(short.MinValue).SetName(testName).Returns(new byte[] { 0xff, 0xff, 0x3 });
    }

    static IEnumerable<TestCaseData> VariableUShortData(string testBaseName)
    {
        var testName = $"{testBaseName}({typeof(ushort).Name})";
        yield return new TestCaseData(0).SetName(testName).Returns(new byte[] { 0 });
        yield return new TestCaseData(1).SetName(testName).Returns(new byte[] { 2 });
        yield return new TestCaseData(2).SetName(testName).Returns(new byte[] { 4 });
        yield return new TestCaseData(255).SetName(testName).Returns(new byte[] { 0xfe, 0x3 });
        yield return new TestCaseData(ushort.MaxValue).SetName(testName).Returns(new byte[] { 0xff, 0xff, 0x3 });
    }

    //[TestCaseSource(nameof(CharData), new object[] { nameof(TestWriteVariable) })]
    //[TestCaseSource(nameof(IntData), new object[] { nameof(TestWriteVariable) })]
    //[TestCaseSource(nameof(LongData), new object[] { nameof(TestWriteVariable) })]
    [TestCaseSource(nameof(VariableShortData), new object[] { nameof(TestWriteVariable) })]
    //[TestCaseSource(nameof(UIntData), new object[] { nameof(TestWriteVariable) })]
    //[TestCaseSource(nameof(ULongData), new object[] { nameof(TestWriteVariable) })]
    [TestCaseSource(nameof(VariableUShortData), new object[] { nameof(TestWriteVariable) })]
    [Test]
    public byte[] TestWriteVariable(object value)
    {
        var written = value switch
        {
            (char typedValue) => _memoryBuffer.WriteVariable(typedValue),
            (int typedValue) => _memoryBuffer.WriteVariable(typedValue),
            (long typedValue) => _memoryBuffer.WriteVariable(typedValue),
            (short typedValue) => _memoryBuffer.WriteVariable(typedValue),
            (uint typedValue) => _memoryBuffer.WriteVariable(typedValue),
            (ulong typedValue) => _memoryBuffer.WriteVariable(typedValue),
            (ushort typedValue) => _memoryBuffer.WriteVariable(typedValue),
            _ => throw new NotSupportedException(value.GetType().FullName)
        };

        return _memoryBuffer.Buffer.Span[0..written].ToArray();
    }

    static IEnumerable<TestCaseData> WriteAndReadVariableCharData(string testBaseName)
    {
        var testName = $"{testBaseName}({typeof(char).Name})";
        yield return new TestCaseData((char)0).SetName(testName).Returns(0);
        yield return new TestCaseData((char)1).SetName(testName).Returns(1);
        yield return new TestCaseData((char)254).SetName(testName).Returns(254);
        yield return new TestCaseData((char)255).SetName(testName).Returns(255);
        yield return new TestCaseData((char)256).SetName(testName).Returns(256);
        yield return new TestCaseData(char.MaxValue).SetName(testName).Returns(char.MaxValue);
    }

    static IEnumerable<TestCaseData> WriteAndReadVariableIntData(string testBaseName)
    {
        var testName = $"{testBaseName}({typeof(int).Name})";
        yield return new TestCaseData(0).SetName(testName).Returns(0);
        yield return new TestCaseData(1).SetName(testName).Returns(1);
        yield return new TestCaseData(-1).SetName(testName).Returns(-1);
        yield return new TestCaseData(7).SetName(testName).Returns(7);
        yield return new TestCaseData(8).SetName(testName).Returns(8);
        yield return new TestCaseData(15).SetName(testName).Returns(15);
        yield return new TestCaseData(16).SetName(testName).Returns(16);
        yield return new TestCaseData(254).SetName(testName).Returns(254);
        yield return new TestCaseData(255).SetName(testName).Returns(255);
        yield return new TestCaseData(256).SetName(testName).Returns(256);
        yield return new TestCaseData(65535).SetName(testName).Returns(65535);
        yield return new TestCaseData(65536).SetName(testName).Returns(65536);
        yield return new TestCaseData(int.MaxValue).SetName(testName).Returns(int.MaxValue);
        yield return new TestCaseData(int.MinValue).SetName(testName).Returns(int.MinValue);
    }

    static IEnumerable<TestCaseData> WriteAndReadVariableLongData(string testBaseName)
    {
        var testName = $"{testBaseName}({typeof(long).Name})";
        yield return new TestCaseData((short)0).SetName(testName).Returns(0);
        yield return new TestCaseData((short)-1).SetName(testName).Returns(-1);
        yield return new TestCaseData((short)1).SetName(testName).Returns(1);
        yield return new TestCaseData((short)254).SetName(testName).Returns(254);
        yield return new TestCaseData((short)255).SetName(testName).Returns(255);
        yield return new TestCaseData((short)256).SetName(testName).Returns(256);
        yield return new TestCaseData(short.MaxValue).SetName(testName).Returns(short.MaxValue);
        yield return new TestCaseData(short.MinValue).SetName(testName).Returns(short.MinValue);
    }

    static IEnumerable<TestCaseData> WriteAndReadVariableShortData(string testBaseName)
    {
        var testName = $"{testBaseName}({typeof(short).Name})";
        yield return new TestCaseData((short)0).SetName(testName).Returns(0);
        yield return new TestCaseData((short)-1).SetName(testName).Returns(-1);
        yield return new TestCaseData((short)1).SetName(testName).Returns(1);
        yield return new TestCaseData((short)254).SetName(testName).Returns(254);
        yield return new TestCaseData((short)255).SetName(testName).Returns(255);
        yield return new TestCaseData((short)256).SetName(testName).Returns(256);
        yield return new TestCaseData(short.MaxValue).SetName(testName).Returns(short.MaxValue);
        yield return new TestCaseData(short.MinValue).SetName(testName).Returns(short.MinValue);
    }

    static IEnumerable<TestCaseData> WriteAndReadVariableUShortData(string testBaseName)
    {
        var testName = $"{testBaseName}({typeof(ushort).Name})";
        yield return new TestCaseData((ushort)0).SetName(testName).Returns(0);
        yield return new TestCaseData((ushort)1).SetName(testName).Returns(1);
        yield return new TestCaseData((ushort)2).SetName(testName).Returns(2);
        yield return new TestCaseData((ushort)254).SetName(testName).Returns(254);
        yield return new TestCaseData((ushort)255).SetName(testName).Returns(255);
        yield return new TestCaseData((ushort)256).SetName(testName).Returns(256);
        yield return new TestCaseData(ushort.MaxValue).SetName(testName).Returns(ushort.MaxValue);
    }

    [TestCaseSource(nameof(WriteAndReadVariableCharData), new object[] { nameof(TestWriteVariable) })]
    [TestCaseSource(nameof(WriteAndReadVariableIntData), new object[] { nameof(TestWriteVariable) })]
    //[TestCaseSource(nameof(WriteAndReadVariableLongData), new object[] { nameof(TestWriteVariable) })]
    [TestCaseSource(nameof(WriteAndReadVariableShortData), new object[] { nameof(TestWriteAndRead) })]
    //[TestCaseSource(nameof(UIntData), new object[] { nameof(TestWriteVariable) })]
    //[TestCaseSource(nameof(ULongData), new object[] { nameof(TestWriteVariable) })]
    [TestCaseSource(nameof(WriteAndReadVariableUShortData), new object[] { nameof(TestWriteAndRead) })]
    [Test]
    public object TestWriteAndRead(object value)
    {
        switch (value)
        {
            case char typedValue:
                _ = _memoryBuffer.WriteVariable(typedValue);
                _memoryBuffer.Position = 0;
                _ = _memoryBuffer.ReadVariable(out typedValue);
                return typedValue;
            case int typedValue:
                _ = _memoryBuffer.WriteVariable(typedValue);
                _memoryBuffer.Position = 0;
                _ = _memoryBuffer.ReadVariable(out typedValue);
                return typedValue;
            case long typedValue:
                _ = _memoryBuffer.WriteVariable(typedValue);
                _memoryBuffer.Position = 0;
                _ = _memoryBuffer.ReadVariable(out typedValue);
                return typedValue;
            case short typedValue:
                _ = _memoryBuffer.WriteVariable(typedValue);
                _memoryBuffer.Position = 0;
                _ = _memoryBuffer.ReadVariable(out typedValue);
                return typedValue;
            case uint typedValue:
                _ = _memoryBuffer.WriteVariable(typedValue);
                _memoryBuffer.Position = 0;
                _ = _memoryBuffer.ReadVariable(out typedValue);
                return typedValue;
            case ulong typedValue:
                _ = _memoryBuffer.WriteVariable(typedValue);
                _memoryBuffer.Position = 0;
                _ = _memoryBuffer.ReadVariable(out typedValue);
                return typedValue;
            case ushort typedValue:
                _ = _memoryBuffer.WriteVariable(typedValue);
                _memoryBuffer.Position = 0;
                _ = _memoryBuffer.ReadVariable(out typedValue);
                return typedValue;
            default:
                throw new NotSupportedException(value.GetType().FullName);
        }
    }

    [TestCase("abcdef", new byte[] { 12, 97, 98, 99, 100, 101, 102 })]
    [Test]
    public void TestWriteString(string input, byte[] expected)
    {
        var written = _memoryBuffer.Write(input);
        Assert.AreEqual(expected.Length, written);
        Assert.AreEqual(expected.Length, _memoryBuffer.Position);
        Assert.AreEqual(expected, _memoryBuffer.Buffer.Span[0..expected.Length].ToArray());
        _memoryBuffer.Position = 0;
    }
}
