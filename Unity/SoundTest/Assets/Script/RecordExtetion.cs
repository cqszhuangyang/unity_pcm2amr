using System;
using Ionic.Zlib;
using UnityEngine;


/// <summary>
/// 录音扩展 插件 ： https://github.com/MarkerMetro/MarkerMetro.Unity.WinLegacy  用于压缩 解压
/// 1.完成音频录音
/// 2.针对录取的音频进行数据进行压缩，返回byte[]
///
/// </summary>
public static class RecordExtetion
{
    /// <summary>
    /// clip 对象  录音设备clip
    /// </summary>
    private static AudioClip _clip;

    /// <summary>
    /// 录音时长
    /// </summary>
    private static int maxRecordTime =15;

    /// <summary>
    /// samplingRate是采样率长度，越长音质越好，录音文件越大
    /// </summary>
    private static int samplingRate = 8000;

    private static IntPtr m_avFormatCtx;


//    public static void InitFFmpeg()
//    {
//        FFmpeg.av_register_all();
//        m_avFormatCtx = FFmpeg.avcodec_alloc_context();
//    }

//    private void EncodeAndWritePacket()
//    {
//        byte[] frameBuffer = new byte[FrameSize];
//        m_buffer.Read(frameBuffer, 0, frameBuffer.Length);
//
//        fixed (byte* pcmSamples = frameBuffer)
//        {
//            if (m_disposed)
//                throw new ObjectDisposedException(this.ToString());
//
//            FFmpeg.AVPacket outPacket = new FFmpeg.AVPacket();
//            FFmpeg.av_init_packet(ref outPacket);
//
//            byte[] buffer = new byte[FFmpeg.FF_MIN_BUFFER_SIZE];
//            fixed (byte* encodedData = buffer)
//            {
//                try
//                {
//                    outPacket.size = FFmpeg.avcodec_encode_audio(ref m_avCodecCtx, encodedData, FFmpeg.FF_MIN_BUFFER_SIZE, (short*)pcmSamples);
//                    outPacket.pts = m_avCodecCtx.coded_frame->pts;
//                    outPacket.flags |= FFmpeg.PKT_FLAG_KEY;
//                    outPacket.stream_index = m_avStream.index;
//                    outPacket.data = (IntPtr)encodedData;
//
//                    if (outPacket.size > 0)
//                    {
//                        if (FFmpeg.av_write_frame(ref m_avFormatCtx, ref outPacket) != 0)
//                            throw new IOException("Error while writing encoded audio frame to file");
//                    }
//                }
//                finally
//                {
//                    FFmpeg.av_free_packet(ref outPacket);
//                }
//            }
//        }
//    }

    /// <summary>
    /// 开始录音
    /// </summary>
    /// <returns>The function returns null if the recording fails to start.</returns>
    public static bool StartRecording()
    {
        try
        {
            Microphone.End(null);
            //寻找默认设备
       
            _clip = Microphone.Start(null, false, maxRecordTime, samplingRate);
           
        }
        catch (Exception e)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 结束录音
    /// </summary>
    /// <param name="length">音频数据长度</param>
    /// <param name="outClip">返回clip对象</param>
    public static void EndRecording(out int length, out AudioClip outClip)
    {
        int lastPos = Microphone.GetPosition(null);

        if (Microphone.IsRecording(null))
        {
            length = lastPos / samplingRate;
        }
        else
        {
            length = maxRecordTime;
        }

        Microphone.End(null);

        if (length < 1.0f)
        {
            outClip = null;
            return;
        }

        outClip = _clip;
    }


    /// <summary>
    ///
    /// </summary>
    public static AudioClip Clip
    {
        get
        {
            if (_clip == null)
            {
                _clip = AudioClip.Create("MyRecordClip", samplingRate * 2, 1, samplingRate, false);
            }

            return _clip;
        }
    }


    /// <summary>
    /// 获取音频数据文件，使用zlib进行压缩处理  已经被压缩处理
    /// </summary>
    /// <param name="clip"></param>
    /// <returns></returns>
    public static byte[] GetData(this AudioClip clip)
    {
        var data = new float[clip.samples * clip.channels];
        clip.GetData(data, 0);
        byte[] bytes = new byte[data.Length * 4];
        Buffer.BlockCopy(data, 0, bytes, 0, bytes.Length);
        return ConvertBytesZlib(bytes, CompressionMode.Compress);
    }

    /// <summary>
    /// 设置 audioclip
    /// </summary>
    /// <param name="clip">音频源</param>
    /// <param name="bytes">音频字节数组</param>
    public static void SetData(this AudioClip clip, byte[] bytes)
    {
        bytes = ConvertBytesZlib(bytes, CompressionMode.Decompress);
        float[] data = new float[bytes.Length / 4];
        Buffer.BlockCopy(bytes, 0, data, 0, data.Length);
        clip.SetData(data, 0);
    }


    /// <summary>
    /// 对数据 进行压缩 ，解压处理
    /// </summary>
    /// <param name="data"></param>
    /// <param name="compressionMode"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static byte[] ConvertBytesZlib(byte[] data, CompressionMode compressionMode)
    {
        if (compressionMode == CompressionMode.Compress)
        {
            Debug.Log("压缩之前长度：" + data.Length);
            data = ZlibStream.CompressBuffer(data);
            Debug.Log("压缩之后长度：" + data.Length);
            return data;
        }
        else
        {
            //uncompress
            return ZlibStream.UncompressBuffer(data);
        }
    }
}