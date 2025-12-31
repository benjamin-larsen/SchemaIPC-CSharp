using System.Net.Sockets;

namespace SchemaIPC;

public class ClientUnix(string path) : BaseClient {
    public override async Task<Socket> ConnectAsync() {
        var sock = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
        await sock.ConnectAsync(new UnixDomainSocketEndPoint(path));
        
        return sock;
    }
}