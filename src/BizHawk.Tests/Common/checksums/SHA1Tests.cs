using System;
using System.Linq;
using System.Text;
using BizHawk.Common;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BizHawk.Tests.Common.checksums
{
	[TestClass]
	public sealed class SHA1Tests
	{
		[TestMethod]
		public void TestSHA1Empty()
		{
			byte[] data = Array.Empty<byte>(); // empty data
			byte[] expectedSha = { 0xda, 0x39, 0xa3, 0xee, 0x5e, 0x6b, 0x4b, 0x0d, 0x32, 0x55, 0xbf, 0xef, 0x95, 0x60, 0x18, 0x90, 0xaf, 0xd8, 0x07, 0x09 };

			Assert.IsTrue(expectedSha.SequenceEqual(SHA1Checksum.Compute(data)));
		}

		[TestMethod]
		public void TestSHA1Simple()
		{
			byte[] data = { (byte)'h', (byte)'a', (byte)'s', (byte)'h' }; // random short data
			byte[] expectedSha = { 0x23, 0x46, 0xad, 0x27, 0xd7, 0x56, 0x8b, 0xa9, 0x89, 0x6f, 0x1b, 0x7d, 0xa6, 0xb5, 0x99, 0x12, 0x51, 0xde, 0xbd, 0xf2 };

			Assert.IsTrue(expectedSha.SequenceEqual(SHA1Checksum.Compute(data)));
			Assert.IsTrue(expectedSha.SequenceEqual(SHA1Checksum.ComputeConcat(Array.Empty<byte>(), data)));
			Assert.IsTrue(expectedSha.SequenceEqual(SHA1Checksum.ComputeConcat(data, Array.Empty<byte>())));

			data = new[] { (byte)'h', (byte)'a' };
			byte[] data2 = { (byte)'s', (byte)'h' };

			Assert.IsTrue(expectedSha.SequenceEqual(SHA1Checksum.ComputeConcat(data, data2)));
		}

		[TestMethod]
		public void TestSHA1LessSimple()
		{
			const string testString = "The quick brown fox jumps over the lazy dog.";
			byte[] data = Encoding.ASCII.GetBytes(testString);
			byte[] expectedSha1 = { 0x40, 0x8d, 0x94, 0x38, 0x42, 0x16, 0xf8, 0x90, 0xff, 0x7a, 0x0c, 0x35, 0x28, 0xe8, 0xbe, 0xd1, 0xe0, 0xb0, 0x16, 0x21 };

			Assert.IsTrue(expectedSha1.SequenceEqual(SHA1Checksum.Compute(data)));

			data = new byte[65];
			Encoding.ASCII.GetBytes(testString).CopyTo(data, 0);

			byte[] expectedSha2 = { 0x65, 0x87, 0x84, 0xE2, 0x68, 0xBF, 0xB1, 0x67, 0x94, 0x7B, 0xB7, 0xF3, 0xFB, 0x76, 0x69, 0x62, 0x79, 0x3E, 0x8C, 0x46 };
			Assert.IsTrue(expectedSha2.SequenceEqual(SHA1Checksum.Compute(new Span<byte>(data, 0, 64))));

			byte[] expectedSha3 = { 0x34, 0xF3, 0xA2, 0x57, 0xBD, 0x12, 0x5E, 0x6E, 0x0E, 0x28, 0xD0, 0xE5, 0xDA, 0xBE, 0x22, 0x28, 0x97, 0xFA, 0x69, 0x55 };
			Assert.IsTrue(expectedSha3.SequenceEqual(SHA1Checksum.Compute(data)));
		}
	}
}
