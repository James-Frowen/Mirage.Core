using Mirage.Tests;
using NUnit.Framework;

namespace Mirage.SocketLayer.Tests.AckSystemTests
{
    /// <summary>
    /// Send is done in setup, and then tests just valid that the sent data is correct
    /// </summary>
    [Category("SocketLayer")]
    public class AckSystemTest_Fragmentation_Receive : AckSystemTestBase
    {
        private AckSystem ackSystem;
        private Config config;
        private byte[] message;
        private byte[] packet1;
        private byte[] packet2;

        [SetUp]
        public void SetUp()
        {
            config = new Config();
            var mtu = MAX_PACKET_SIZE;
            var bigSize = (int)(mtu * 1.5f);

            message = CreateBigData(1, bigSize);

            var sender = new AckTestInstance();
            sender.connection = new SubIRawConnection();
            sender.ackSystem = new AckSystem(sender.connection, config, MAX_PACKET_SIZE, new Time(), bufferPool);
            sender.ackSystem.SendReliable(message);
            packet1 = sender.packet(0);
            packet2 = sender.packet(1);


            var connection = new SubIRawConnection();
            ackSystem = new AckSystem(connection, config, MAX_PACKET_SIZE, new Time(), bufferPool);
        }

        private byte[] CreateBigData(int id, int size)
        {
            var buffer = new byte[size];
            rand.NextBytes(buffer);
            buffer[0] = (byte)id;

            return buffer;
        }


        [Test]
        [TestCase(-2, ExpectedResult = false)]
        [TestCase(-1, ExpectedResult = false)]
        [TestCase(0, ExpectedResult = true, Description = "equal to max is invalid")]
        [TestCase(1, ExpectedResult = true)]
        [TestCase(2, ExpectedResult = true)]
        [TestCase(5, ExpectedResult = true)]
        public bool ShouldBeInvalidIfFragmentIsOverMax(int differenceToMax)
        {
            var max = config.MaxReliableFragments;
            var badPacket = new byte[AckSystem.MIN_RELIABLE_FRAGMENT_HEADER_SIZE];
            var offset = 0;
            // write as if it is normal packet
            ByteUtils.WriteByte(badPacket, ref offset, 0);
            ByteUtils.WriteUShort(badPacket, ref offset, 0);
            ByteUtils.WriteUShort(badPacket, ref offset, 0);
            ByteUtils.WriteULong(badPacket, ref offset, 0);
            ByteUtils.WriteUShort(badPacket, ref offset, 0);
            // write bad index (over max)
            var fragment = max + differenceToMax;
            ByteUtils.WriteByte(badPacket, ref offset, (byte)fragment);

            return ackSystem.InvalidFragment(badPacket);
        }


        [Test]
        public void MessageShouldBeInQueueAfterReceive()
        {
            ackSystem.ReceiveReliable(packet1, packet1.Length, true);

            Assert.IsFalse(ackSystem.NextReliablePacket(out var _));

            ackSystem.ReceiveReliable(packet2, packet2.Length, true);

            var bytesIn1 = MAX_PACKET_SIZE - AckSystem.MIN_RELIABLE_FRAGMENT_HEADER_SIZE;
            var bytesIn2 = message.Length - bytesIn1;

            Assert.IsTrue(ackSystem.NextReliablePacket(out var first));

            Assert.IsTrue(first.IsFragment);
            Assert.That(first.Buffer.array[0], Is.EqualTo(1), "First fragment should have index 1");
            Assert.That(first.Length, Is.EqualTo(bytesIn1 + 1));
            AssertAreSameFromOffsets(message, 0, first.Buffer.array, 1, bytesIn1);

            var second = ackSystem.GetNextFragment();
            Assert.IsTrue(second.IsFragment);
            Assert.That(second.Buffer.array[0], Is.EqualTo(0), "Second fragment should have index 0");
            Assert.That(second.Length, Is.EqualTo(bytesIn2 + 1));
            AssertAreSameFromOffsets(message, bytesIn1, second.Buffer.array, 1, bytesIn2);
        }
    }
}
