using Mirage.Sockets.Udp;

namespace Mirage.Test
{
    public class MirageCoreTests
    {
        MirageServer _server;

        [SetUp]
        public void Setup()
        {
            _server = StartUDPServer(7777);
        }

        [TearDown]
        public void TearDown()
        {
            _server.Stop();
        }

        [Test]
        public void ServeActiveTest()
        {
            Assert.That(_server.Active);
        }

        private MirageServer StartUDPServer(int port)
        {
            var server = new MirageServer();

            var socketFactory = new UdpSocketFactory();
            var socket = socketFactory.CreateSocket();
            var endpoint = socketFactory.GetBindEndPoint(port);
            server.StartServer(socket, socketFactory.MaxPacketSize, endpoint);

            return server;
        }
    }
}
