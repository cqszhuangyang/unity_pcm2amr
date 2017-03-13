using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Recording : MonoBehaviour
{

    public Button RecodeBtn;
    int minFreq;
    int maxFreq;
    public AudioSource aduioSource;
    static AndroidJavaClass AJ;
    int len;
    AudioClip clip = null; // ref
                           // Use this for initialization
    void Start()
    {
       // AJ = new AndroidJavaClass("com.game.amrlib.AMRTool");
        RecodeBtn.onClick.AddListener(OnClickRecodeBtn);
        RecodeBtn.GetComponentInChildren<Text>().text = "recoding";
    }
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            Debug.Log("游戏运行在后台");
        }
        else
        {
            Debug.Log("游戏运行在前台");
        }
    }

    private void OnClickRecodeBtn()
    {

        if (RecodeBtn.GetComponentInChildren<Text>().text == "recoding")
        {


            Microphone.GetDeviceCaps("", out minFreq, out maxFreq);
            Debug.Log(minFreq + "," + maxFreq);
            RecordExtetion.StartRecording();
            RecodeBtn.GetComponentInChildren<Text>().text = "stop";
            Debug.Log("recodeTime :" + NSpeexLib.speexEnc.SampleRate);
        }
        else if (RecodeBtn.GetComponentInChildren<Text>().text == "stop")
        {

            RecordExtetion.EndRecording(out len, out clip);
            Debug.Log("recodeTime :" + len + " second");
            // byte[] bytes = RecordExtetion.GetData(clip);
            RecodeBtn.GetComponentInChildren<Text>().text = "encodc";
        }
        else if (RecodeBtn.GetComponentInChildren<Text>().text == "encodc")
        {


            var srcBytes = new float[clip.samples * clip.channels];
            clip.GetData(srcBytes, 0);
            compressByte = encode(srcBytes);
            RecodeBtn.GetComponentInChildren<Text>().text = "play";
        }
        else if (RecodeBtn.GetComponentInChildren<Text>().text == "play")
        {
            if (compressByte == null) return;
            float[] dstBytes = decode(compressByte);
            clip = AudioClip.Create("MyRecordClip", dstBytes.Length, clip.channels, NSpeexLib.speexEnc.SampleRate, false);
            clip.SetData(dstBytes, 0);

            aduioSource.clip = clip;
            aduioSource.volume = 1;
            aduioSource.Play();
            compressByte = null;
            RecodeBtn.GetComponentInChildren<Text>().text = "recoding";
        }
    }
    static byte[] compressByte;

    static byte[] encodedData = new byte[0];
    public static float[] TestSpeex(float[] srcBytes)
    {
       // CoroutineManager.Add(StartEncode(srcBytes));
       // int length = 0;
        byte[] compressByte = encode(srcBytes);
        //float[] decompressByte = NSpeexLib.SpeexDecompress(compressByte, length);
        float[] decompressByte = decode(compressByte);
        return decompressByte;
    }
    static int sampleSize = NSpeexLib.speexEnc.FrameSize * 10;
    private static IEnumerator StartEncode(float[] bytes)
    {
        int recordFrequency = 44100;
        int targetFrequency = 8000;
        int targetSampleSize = 320;
        // int sampleSize = recordFrequency / (targetFrequency / targetSampleSize); 
        int sampleSize = 160;
        int sampleCount = bytes.Length / sampleSize;//记录次数
        sampleCount = bytes.Length % sampleSize > 0 ? sampleCount++ : sampleCount;

        int srcLength = bytes.Length;


        int sampleIndex = 0;
        Debug.Log("sampleCount:" + sampleCount + ",sampleSize:" + sampleSize);
        // int encLenCount = 0;
        for (int index = 0; index < sampleCount; index++)
        {
            var tempLen = sampleSize;
            if (sampleIndex == sampleCount - 1)
            {
                tempLen = (bytes.Length - sampleIndex) % sampleSize;
                tempLen = tempLen > 0 ? tempLen : sampleSize;
            }
            var buff = new float[tempLen];
            Buffer.BlockCopy(bytes, index * sampleSize, buff, 0, tempLen);
            sampleIndex += tempLen;
            int encLen = 0;
            Debug.Log("pos:" + index * sampleSize + "tempLen:" + tempLen);
            byte[] compressByte = NSpeexLib.SpeexCompress(buff, out encLen);
            ///设置包头
            ///
            var temp = new byte[8];
            Kit.setInt(temp, 0, encLen);
            Kit.setInt(temp, 4, tempLen);
            compressByte = Kit.getArray(compressByte, 0, encLen);
            var packet = Kit.connectArray(temp, compressByte);
            encodedData = Kit.connectArray(encodedData, packet);
            yield return null;
            //var tempLen = (bytes.Length - sampleIndex) % sampleRate;
            //tempLen = tempLen > 0 ? tempLen : sampleRate;
            //var buff = new float[tempLen];
            //Buffer.BlockCopy(bytes, index * sampleRate, buff, 0, buff.Length);
            //sampleIndex += tempLen;
            //int encLen = 0;
            //byte[] compressByte = NSpeexLib.SpeexCompress(buff, out encLen);
            //encLenCount += encLen;
            /////设置包头
            /////
            //compressByte = Kit.getArray(compressByte, 0, encLen);
            //encodedData = Kit.connectArray(encodedData, compressByte);
            //Debug.Log("包 :" + index);
        }
    }

    static byte[] encode(float[] bytes)
    {
   
       // int sampleSize = recordFrequency / (targetFrequency / targetSampleSize); 
        
        int sampleCount = bytes.Length / sampleSize;//记录次数
        sampleCount = bytes.Length % sampleSize > 0 ? sampleCount++ : sampleCount;

        int srcLength = bytes.Length;
      
        byte[] encodedData = new byte[0];
   
        int sampleIndex = 0;
        Debug.Log("sampleCount:" + sampleCount + ",sampleSize:"+ sampleSize);
        // int encLenCount = 0;
        for (int index = 0; index < sampleCount; index++)
        {
            var tempLen = sampleSize;
            if (sampleIndex == sampleCount - 1)
            {
                tempLen = (bytes.Length - sampleIndex) % sampleSize;
                tempLen = tempLen > 0 ? tempLen : sampleSize;
            }
            var buff = new float[tempLen];
            Buffer.BlockCopy(bytes, index * sampleSize, buff, 0, tempLen);
            sampleIndex += tempLen;
            int encLen = 0;
            
            byte[] compressByte = NSpeexLib.SpeexCompress(buff, out encLen);
            ///设置包头
            ///
            var temp = new byte[4];
            Kit.setInt(temp,0, encLen);
            compressByte = Kit.getArray(compressByte, 0, encLen);
            var packet = Kit.connectArray(temp, compressByte);
            encodedData = Kit.connectArray(encodedData, packet);
            Debug.Log("pos:" + index * sampleSize + "tempLen:" + tempLen+ "encodedData:"+ encodedData.Length);
            //var tempLen = (bytes.Length - sampleIndex) % sampleRate;
            //tempLen = tempLen > 0 ? tempLen : sampleRate;
            //var buff = new float[tempLen];
            //Buffer.BlockCopy(bytes, index * sampleRate, buff, 0, buff.Length);
            //sampleIndex += tempLen;
            //int encLen = 0;
            //byte[] compressByte = NSpeexLib.SpeexCompress(buff, out encLen);
            //encLenCount += encLen;
            /////设置包头
            /////
            //compressByte = Kit.getArray(compressByte, 0, encLen);
            //encodedData = Kit.connectArray(encodedData, compressByte);
            //Debug.Log("包 :" + index);
        }
        //var temp = new byte[8];
        //Kit.setInt(temp, 0, encLenCount);
        //Kit.setInt(temp, 4, srcLength);

        //encodedData = Kit.connectArray(temp, encodedData);
        return encodedData;
    }

    static float[] decode(byte[] bytes)
    {
        if (bytes.Length == 0) return null;
        int postion = 0;
        float[] decodedData = new float[0];
        int index = 0;
        while (postion < bytes.Length)
        {

            var encLen = Kit.getInt(bytes, postion, out postion);
            Debug.Log("bytes.Length :" + bytes.Length + "encLen :" + encLen + "postion:" + postion);
            var packet = Kit.getArray(bytes, postion, encLen, out postion);
            Debug.Log("解包 :" + postion);
            float[] compressByte = NSpeexLib.SpeexDecompress(packet, sampleSize);
            decodedData = Kit.connectArray(compressByte, decodedData);
            Debug.Log("解:" + decodedData.Length);
            index++;
        }
        //var encLen = Kit.getInt(bytes, 0);

        //var srcLen = Kit.getInt(bytes, 4);
        //Debug.Log("解包长 :" + encLen + "原包长 :" + srcLen);
        //int postion = 0;
        //var packet = Kit.getArray(bytes, 8, encLen, out postion);
        //float[] decodedData = NSpeexLib.SpeexDecompress(packet, srcLen);
        return decodedData;
    }
    

    public static float[] TestPCMAMR(float[] srcBytes)
    {
        // Debug.Log("压缩前字节长度:" + srcBytes.Length);
        short[] shortBytes = new short[srcBytes.Length];
        srcBytes.ToShortArray(shortBytes);
        //   PrintBytes("压缩前：", shortBytes);
        var compressBytes = AJ.CallStatic<byte[]>("Encoding", shortBytes);
        // PrintBytes("压缩后：", compressBytes);
        // Debug.Log("AMR:" + compressBytes.Length / 1024 + "kb");
        var unCompressData = AJ.CallStatic<short[]>("Decoding", compressBytes);

        //   PrintBytes("AMR：", unCompressData);
        var dstBytes = new float[unCompressData.Length];
        unCompressData.ToFloatArray(dstBytes, dstBytes.Length);
        //  Debug.Log("压缩后字节长度:" + dstBytes.Length);
        return dstBytes;
    }

    public static bool Approximately(float a, float b)
    {
        return Mathf.Abs(b - a) < Mathf.Max((float)1E-06 * Mathf.Max(Mathf.Abs(a), Mathf.Abs(b)), Mathf.Epsilon * 8);
    }
    public static void PrintBytes(string content, byte[] srcbytes)
    {
        Debug.Log(content + BitConverter.ToString(srcbytes));
    }
    public static void PrintBytes(string content, float[] srcbytes)
    {
        byte[] bytes = new byte[srcbytes.Length << 2];
        System.Buffer.BlockCopy(srcbytes, 0, bytes, 0, bytes.Length);
        Debug.Log(content + BitConverter.ToString(bytes));
        //string result = srcbytes.ToString(",");
        // Debug.Log(content + result);

    }
    public static void PrintBytes(string content, short[] srcbytes)
    {
        byte[] bytes = new byte[srcbytes.Length << 1];
        System.Buffer.BlockCopy(srcbytes, 0, bytes, 0, bytes.Length);
        Debug.Log(content + BitConverter.ToString(bytes));
        //string result = srcbytes.ToString(",");
        //Debug.Log(content + result);

    }
    public static short[] Float2Shorts(float[] arrays)
    {
        //byte[] bytes = new byte[arrays.Length <<2];
        //for (int i = 0; i < arrays.Length; i++)
        //{
        //    System.Buffer.BlockCopy(BitConverter.GetBytes(arrays[i]), 0, bytes, i * 4, 4);
        //}
        //short[] temp = new short[bytes.Length>>1];
        //for (int i = 0; i < temp.Length; i++) {
        //    temp[i] = BitConverter.ToInt16(bytes, i * 2);
        //}
        byte[] bytes = new byte[arrays.Length << 2];
        System.Buffer.BlockCopy(arrays, 0, bytes, 0, bytes.Length);
        short[] temp = new short[bytes.Length >> 1];
        System.Buffer.BlockCopy(bytes, 0, temp, 0, temp.Length);
        return temp;
    }

    public static float[] Short2Floats(short[] arrays)
    {
        //float[] floatArr = new float[arrays.Length >> 1];
        //byte[] bytes = new byte[arrays.Length << 1];
        //for (int i = 0; i < arrays.Length; i++)
        //{
        //    System.Buffer.BlockCopy(BitConverter.GetBytes(arrays[i]), 0, bytes, i*2, 2);
        //  //  bytes BitConverter.GetBytes(arrays[i]);
        //}
        //for (int i = 0; i < floatArr.Length; i++)
        //{
        //    if (BitConverter.IsLittleEndian)
        //        Array.Reverse(bytes, i * 4, 4);
        //    floatArr[i] = BitConverter.ToSingle(bytes, i * 4);
        //}
        //return floatArr;

        byte[] bytes = new byte[arrays.Length << 1];
        System.Buffer.BlockCopy(arrays, 0, bytes, 0, bytes.Length);
        float[] temp = new float[bytes.Length >> 2];
        System.Buffer.BlockCopy(bytes, 0, temp, 0, temp.Length);
        return temp;
    }

}
public static class Exstension
{
    public static string ToString<T>(this T[] array, string delimiter)
    {
        if (array != null)
        {
            // determine if the length of the array is greater than the performance threshold for using a stringbuilder
            // 10 is just an arbitrary threshold value I've chosen
            if (array.Length < 10)
            {
                // assumption is that for arrays of less than 10 elements
                // this code would be more efficient than a StringBuilder.
                // Note: this is a crazy/pointless micro-optimization.  Don't do this.
                string[] values = new string[array.Length];

                for (int i = 0; i < values.Length; i++)
                    values[i] = array[i].ToString();

                return string.Join(delimiter, values);
            }
            else
            {
                // for arrays of length 10 or longer, use a StringBuilder
                System.Text.StringBuilder sb = new StringBuilder();

                sb.Append(array[0]);
                for (int i = 1; i < array.Length; i++)
                {
                    sb.Append(delimiter);
                    sb.Append(array[i]);
                }

                return sb.ToString();
            }
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    /// 考虑为浮动点样本值的范围是 -1.0 <= Xn <= 1.0 和 signed short 是 -32767 <= Xn <= +32767
    /// 不过，确信你你的帧都是单声道吗？如果不是这也可能音频损坏的原因，你只会复制一个通道
    /// </summary>
    /// <param name="input"></param>
    /// <param name="output"></param>
    public static void ToShortArray(this float[] input, short[] output)
    {
        if (output.Length < input.Length)
        {
            throw new System.ArgumentException("in: " + input.Length + ", out: " + output.Length);
        }

        for (int i = 0; i < input.Length; ++i)
        {
            output[i] = (short)Mathf.Clamp((int)(input[i] * 32767.0f), short.MinValue, short.MaxValue);
        }
    }

    public static void ToFloatArray(this short[] input, float[] output, int length)
    {
        if (output.Length < length || input.Length < length)
        {
            throw new System.ArgumentException();
        }

        for (int i = 0; i < length; ++i)
        {
            output[i] = input[i] / (float)short.MaxValue;
        }
    }
}
