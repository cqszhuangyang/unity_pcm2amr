package com.game.amrlib;
import android.util.Log;

import java.nio.ShortBuffer;
import java.sql.Array;
import java.util.Arrays;

import io.kvh.media.amr.*;
/**
 * Created by pc on 2017/3/2.
 */

public class AMRTool {

    public static byte[] Encoding(short[] allin)
    {

        AmrEncoder.init(0);
          int mode = AmrEncoder.Mode.MR122.ordinal();

          int count = allin.length/160;
          count = allin.length%160>0?count+1:count;
//          byte[] outall =null;
         byte[] outall =new byte[count*32];
         int byteEncoded = AmrEncoder.encode(mode, allin, outall);

//        for (int i =0; i <count; i++) {
//
//            short[] in = new short[160];//short array read from AudioRecorder, length 160
//            System.arraycopy(allin,i*160,in,0,160);
//            byte[] out = new byte[32];//output amr frame, length 32
//            int byteEncoded = AmrEncoder.encode(mode, in, out);
//            //encode done
//            if(outall==null){
//                outall=new byte[out.length];
//                //copy it
//                System.arraycopy(out,0,outall,0,outall.length);
//            }else {
//                byte[]temp=new byte[outall.length+out.length];
//                System.arraycopy(outall,0,temp,0,outall.length);//copy out all
//                System.arraycopy(out,0,temp,outall.length,out.length);
//                outall=temp;
//            }
//        }
        //copy done

        AmrEncoder.exit();

        return outall;
    }

    public static short[] Decoding( byte[] allamrframe )
    {
        long state = AmrDecoder.init();
        int count = allamrframe.length/32;
        count = allamrframe.length%32>0?count++:count;
        short[] allpcmframs=new short[count*160];
        AmrDecoder.decode(state, allamrframe, allpcmframs);
//        short[] allpcmframs=null;
//        for(int i = 0 ; i <count;i++)
//        {
//            byte[] amrframe = new byte[32];//amr frame 32 bytes
//            short[] pcmframs = new short[160];//pcm frame 160 shorts
//            System.arraycopy(allamrframe,i*32,amrframe,0,32);
//            AmrDecoder.decode(state, amrframe, pcmframs);
//
////            //encode done
//            if(allpcmframs==null){
//                allpcmframs=new short[pcmframs.length];
//                //copy it
//                System.arraycopy(pcmframs,0,allpcmframs,0,allpcmframs.length);
//            }else {
//                short[]temp=new short[allpcmframs.length+pcmframs.length];
//                System.arraycopy(allpcmframs,0,temp,0,allpcmframs.length);//copy out all
//                System.arraycopy(pcmframs,0,temp,allpcmframs.length,pcmframs.length);
//                allpcmframs=temp;
//            }
//        }
        AmrDecoder.exit(state);
        return allpcmframs;
    }



}
