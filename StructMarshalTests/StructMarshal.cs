using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using StructMarshal;

namespace StructMarshalTests
{
    [SuppressMessage("Assertion", "NUnit2045:Use Assert.Multiple")]
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Read_To_Larger_Struct()
        {
            Vector2 v2 = new(1, 2);

            var v4 = StructMarshal.StructMarshal.Read<Vector2, Vector4>(ref v2);

            Assert.That(Unsafe.AreSame(ref Unsafe.As<Vector2, Vector4>(ref v2), ref v4), Is.False);
            Assert.That(v4, Is.EqualTo(new Vector4(1, 2, 0, 0)));

            Span<float> floats = stackalloc float[] { 1, 2 };
            v4 = StructMarshal.StructMarshal.Read<float, Vector4>(floats);

            Assert.That(Unsafe.AreSame(ref Unsafe.As<float, Vector4>(ref MemoryMarshal.GetReference(floats)), ref v4),
                Is.False);
            Assert.That(v4, Is.EqualTo(new Vector4(1, 2, 0, 0)));
        }

        [Test]
        public void Read_To_Smaller_Struct()
        {
            Vector4 v4 = new(1, 2, 3, 4);

            var v2 = StructMarshal.StructMarshal.Read<Vector4, Vector2>(ref v4);

            Assert.That(Unsafe.AreSame(ref Unsafe.As<Vector4, Vector2>(ref v4), ref v2), Is.False);
            Assert.That(v2, Is.EqualTo(new Vector2(1, 2)));

            Span<float> floats = stackalloc float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            v2 = StructMarshal.StructMarshal.Read<float, Vector2>(floats);

            Assert.That(Unsafe.AreSame(ref Unsafe.As<float, Vector2>(ref MemoryMarshal.GetReference(floats)), ref v2),
                Is.False);
            Assert.That(v2, Is.EqualTo(new Vector2(1, 2)));
        }

        [Test]
        public void Cast_To_Smaller_Struct()
        {
            Vector4 v4 = new(1, 2, 3, 4);

            ref var v2 = ref StructMarshal.StructMarshal.Cast<Vector4, Vector2>(ref v4);

            Assert.That(Unsafe.AreSame(ref Unsafe.As<Vector4, Vector2>(ref v4), ref v2));
            Assert.That(v2, Is.EqualTo(new Vector2(1, 2)));

            Span<float> floats = stackalloc float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            v2 = ref StructMarshal.StructMarshal.Cast<float, Vector2>(floats);

            Assert.That(Unsafe.AreSame(ref Unsafe.As<float, Vector2>(ref MemoryMarshal.GetReference(floats)), ref v2));
            Assert.That(v2, Is.EqualTo(new Vector2(1, 2)));
        }

        [Test]
        public void Cast_To_Larger_Struct()
        {
            Vector2 v2 = new(1, 2);

            void V4() => StructMarshal.StructMarshal.Cast<Vector2, Vector4>(ref v2);

            Assert.Throws<InvalidCastException>(V4);

            Span<float> floats = stackalloc float[] { 1, 2 };

            try {
                StructMarshal.StructMarshal.Cast<float, Vector4>(floats);
            }
            catch (InvalidCastException) {
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public unsafe void Syntax_Sugar()
        {
            Assert.Throws<NullReferenceException>(() => new ReinterpretCastDecorator<Vector2>().As<Vector4>());

            var v2 = new Vector2(1, 2);

            Assert.Throws<InvalidCastException>(() => Reinterpret.Cast(v2).AsRef<Vector4>());
            Assert.Throws<InvalidCastException>(() => Reinterpret.Cast(v2).AsPointer<Vector4>());
            Assert.DoesNotThrow(() => Reinterpret.Cast(v2).AsSpan<Vector4>().ToArray());
            Assert.That(Reinterpret.Cast(v2).AsSpan<Vector4>().ToArray(), Has.Length.EqualTo(0));

            Assert.That(Reinterpret.Cast(v2).As<Vector4>().Equals(new Vector4(1,2,0,0)));
            Assert.That(Reinterpret.Cast(v2).AsPointer<float>()->Equals(1));
            Assert.That(Reinterpret.Cast(v2).AsRef<float>().Equals(1));
        }
    }
}