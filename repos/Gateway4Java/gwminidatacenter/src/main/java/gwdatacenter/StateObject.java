package gwdatacenter;

import java.net.Socket;
import java.util.*;

public class StateObject
{
	public static int BufferSize = 1024;
//C# TO JAVA CONVERTER WARNING: Unsigned integer types have no direct equivalent in Java:
//ORIGINAL LINE: public byte[] buffer = new byte[1024];
	public byte[] buffer = new byte[1024];
//C# TO JAVA CONVERTER WARNING: Unsigned integer types have no direct equivalent in Java:
//ORIGINAL LINE: public List<byte[]> bufferList = new List<byte[]>();
	public ArrayList<byte[]> bufferList = new ArrayList<byte[]>();
	public Socket workSocket;
}