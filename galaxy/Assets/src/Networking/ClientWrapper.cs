using GalaxyShared;

public class ClientWrapper
{
    public byte[] buffer;
    public MsgType type;
    public int size;

    public ClientWrapper()
    {
        buffer = new byte[NetworkUtils.SERVER_READ_BUFFER_SIZE];
    }
}

