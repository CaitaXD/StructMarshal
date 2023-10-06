// ReSharper disable HeapView.DelegateAllocation
// ReSharper disable HeapView.ClosureAllocation
// ReSharper disable HeapView.BoxingAllocation

using StructMarshal;

namespace StructMarshalTests
{
    [TestFixture]
    public class ReinterpretCastTests
    {
        [Test]
        public void AsBytes_ReturnsASpanOfBytes()
        {
            // Arrange
            int value = 10;

            // Act
            var bytes = ReinterpretCast.AsBytes(ref value).ToArray();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(bytes, Is.Not.Null);           // specifically for ref structs
                Assert.That(bytes, Has.Length.EqualTo(4)); // specifically for ref structs
            });
        }

        [Test]
        public void Read_ReturnsTheCorrectStruct()
        {
            // Arrange
            int value = 10;
            var bytes = ReinterpretCast.AsBytes(ref value).ToArray();

            // Act
            var newValue = ReinterpretCast.Read<byte, int>(bytes);

            // Assert
            Assert.That(newValue, Is.EqualTo(10));
        }

        [Test]
        public void Read_Works_WhenCastingToALargerStruct()
        {
            // Arrange
            byte   value = 10;
            byte[] bytes = ReinterpretCast.AsBytes(ref value).ToArray();

            // Act
            Assert.DoesNotThrow(() => ReinterpretCast.Read<byte, long>(bytes));
        }

        [Test]
        public void Cast_ReturnsTheCorrectStruct()
        {
            // Arrange
            int value = 10;

            // Act
            var newValue = ReinterpretCast.Cast<int, short>(ref value);

            // Assert
            Assert.That(newValue, Is.EqualTo(10));
        }

        [Test]
        public void Cast_ThrowsAnInvalidCastException_WhenCastingToALargerStruct()
        {
            // Arrange
            int value = 10;

            // Act
            Assert.Throws<InvalidCastException>(() => ReinterpretCast.Cast<int, long>(ref value));
        }

        [Test]
        public void AsSpan_ReturnsASpanOfStructs()
        {
            // Arrange
            int value = 10;

            // Act
            var arr = ReinterpretCast.AsSpan<int, short>(ref value).ToArray();

            // Assert
            Assert.That(arr, Is.Not.Null);
            Assert.That(arr, Has.Length.EqualTo(2));
        }

        [Test]
        public unsafe void AsPointer_ReturnsTheCorrectPointer()
        {
            // Arrange
            int value = 10;

            // Act
            var pointer = ReinterpretCast.AsPointer<int, short>(ref value);
            Assert.Multiple(() =>
            {
                // Assert
                Assert.That((nint)pointer, Is.Not.Zero);
                Assert.That(*pointer, Is.EqualTo(10));
            });
        }

        [Test]
        public unsafe void AsPointer_ThrowsAnInvalidCastException_WhenCastingToALargerStruct()
        {
            // Arrange
            int value = 10;

            // Act
            Assert.Throws<InvalidCastException>(() => ReinterpretCast.AsPointer<int, long>(ref value));
        }
    }
}