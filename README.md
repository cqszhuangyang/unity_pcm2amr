# unity_pcm2amr
##语音编码 pcm2amr and Nspeex for android in unity 
#参考
##AMR编码
####1.0 项目参考
      AMR 安卓编码 https://github.com/kevinho/opencore-amr-Android
####2.0 编码转化
    编码float[] to short[] 互转 http://stackoverflow.com/questions/16529857/converting-16-bit-short-to-32-bit-float
  因为编码采样的到的浮点数是 -1.0f ~ 1.0f 而 signed short is -32767 <= Xn <= +32767， 所以将浮点数转化short 乘32767
  这里编码转换如果是将float拆分为2个short 那么的到的采样值是不对的
##NSpeex编码
####参考 https://github.com/fholm/unityassets/tree/master/VoiceChat
