using System.Net.Sockets;

namespace SchemaIPC;

public class ClientTcp(string host, int port) : BaseClient {
    protected override async Task<Socket> ConnectAsync() {
        var sock = new Socket(SocketType.Stream, ProtocolType.Tcp);
        await sock.ConnectAsync(host, port);

        return sock;
    }
}