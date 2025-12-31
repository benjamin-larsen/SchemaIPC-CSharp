using System.Net.Sockets;

namespace SchemaIPC;

public abstract class BaseClient {
    protected CancellationTokenSource? _closeCts;
    protected CancellationToken? _closeToken;

    public abstract Task<Socket> ConnectAsync();
}