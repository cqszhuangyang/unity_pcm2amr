using UnityEngine;
using System.Collections;

public class NSpeexLib  {

	public static NSpeex.SpeexEncoder speexEnc = new NSpeex.SpeexEncoder(NSpeex.BandMode.Narrow);
    public static NSpeex.SpeexDecoder speexDec = new NSpeex.SpeexDecoder(NSpeex.BandMode.Narrow);
    public static byte[] SpeexCompress(float[] input, out int length)
    {
        short[] shortBuffer = new short[input.Length];
        byte[] encoded = new byte[input.Length];
        input.ToShortArray(shortBuffer);
        length = speexEnc.Encode(shortBuffer, 0, input.Length, encoded, 0, encoded.Length);
       
        return encoded;
    }

    public static float[] SpeexDecompress(byte[] data,int frameSize)
    {

        short[] shortBuffer = new short[frameSize];
        int len = speexDec.Decode(data, 0, data.Length, shortBuffer, 0, false);
        float[] decoded = new float[len];
        shortBuffer.ToFloatArray(decoded, len);
        
        return decoded;
    }
}
