using System.Buffers.Binary;
using System.Net.Sockets;
using System.Text.Json;
using SchemaIPC.Models;

namespace SchemaIPC;

public abstract class BaseClient {
    private const int MinVersion = 0;
    private const int ClientVersion = 0;

    private CancellationTokenSource? CloseCts;
    private CancellationToken? CloseToken;
    
    private NetworkStream? Stream;
    
    private bool _running = false;

    private async ValueTask<PacketHeader> ReadHeader() {
        if (Stream == null) throw new NullReferenceException("Network Stream is null");

        var bytes = new byte[8];

        await Stream.ReadExactlyAsync(bytes, 0, 8);

        return new PacketHeader {
            PacketLength = BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(0, 4)),
            MessageType = BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(4, 4)),
        };
    }
    
    private async ValueTask<byte[]> ReadPayload(uint length) {
        if (Stream == null) throw new NullReferenceException("Network Stream is null");

        var bytes = new byte[length];

        await Stream.ReadExactlyAsync(bytes, 0, (int)length);

        return bytes;
    }
    
    protected abstract Task<Socket> ConnectAsync();

    public async Task Connect() {
        if (_running) throw new InvalidOperationException("Client is already running");
        _running = true;
        
        // Send Hello Packet to Server
        
        for (;;) {
            // Handle disconnection
            using var sock = await ConnectAsync();
            Stream = new NetworkStream(sock);

            for (;;) {
                var header = await ReadHeader();
                var payload = await ReadPayload(header.PacketLength);
        
                Console.WriteLine(JsonSerializer.Serialize(header, new JsonSerializerOptions {
                    WriteIndented = true,
                    IncludeFields = true,
                }));
                Console.WriteLine(Convert.ToHexString(payload));
            }

            await Task.Delay(5000);
        }
    }
}