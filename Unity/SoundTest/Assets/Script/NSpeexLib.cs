using UnityEngine;
using System.Collections;

public class NSpeexLib  {

	static NSpeex.SpeexEncoder speexEnc = new NSpeex.SpeexEncoder(NSpeex.BandMode.Narrow);
    static NSpeex.SpeexDecoder speexDec = new NSpeex.SpeexDecoder(NSpeex.BandMode.Narrow);
    public static byte[] SpeexCompress(float[] input, out int length)
    {
        short[] shortBuffer = new short[input.Length];
        byte[] encoded = new byte[input.Length];
        input.ToShortArray(shortBuffer);
        length = speexEnc.Encode(shortBuffer, 0, input.Length, encoded, 0, encoded.Length);
       
        return encoded;
    }

    public static float[] SpeexDecompress(byte[] data, int dataLength)
    {
        float[] decoded = new float[data.Length];
        short[] shortBuffer = new short[data.Length];
        speexDec.Decode(data, 0, dataLength, shortBuffer, 0, false);
        shortBuffer.ToFloatArray(decoded, shortBuffer.Length);
        
        return decoded;
    }
}
