using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public class player : MonoBehaviour
{
    // Start is called before the first frame update
    public Material mat;
    public Camera _camera;
    public Shader shader;
    private int FrameNo = 0;
    private Texture2D TexY;
    private Texture2D TexU;
    private Texture2D TexV;

    private string VideoPath;
    private int Width = 1920;
    private int Height = 1080;
    private int FrmaeSize = 1920 * 1080 * 3 / 2;

    private System.Diagnostics.Stopwatch _watch = null;

    private byte[] frameY;
    private byte[] frameU;
    private byte[] frameV;
    FileStream Video;

    /*
    [DllImport("libPlayerStreaming")]
    private static extern int PlayerInit();

    [DllImport("libPlayerStreaming")]
    private static extern int PlayerRun(int num);

    [DllImport("libPlayerStreaming")]
    private static extern int PlayerDeInit();
    */
    void Start()
    {
        //PlayerInit();
        VideoPath = Application.dataPath + "/player/input.yuv";
        TexY = new Texture2D(Width, Height, TextureFormat.Alpha8, false);
        TexU = new Texture2D(Width >> 1, Height >> 1, TextureFormat.Alpha8, false);
        TexV = new Texture2D(Width >> 1, Height >> 1, TextureFormat.Alpha8, false);



        frameY = new byte[Width * Height];

        frameU = new byte[Width * Height >> 2];

        frameV = new byte[Width * Height >> 2];

        Video = new FileStream(VideoPath, FileMode.Open);

    }

    // Update is called once per frame
    void Update()
    {

        float b = 0;
        int a = FrameNo;
        //int a = PlayerRun(FrameNo);
        if(a % 100 == 0 || a % 101 == 0 || a % 102 == 0|| a % 103 == 0 || a % 104 == 0 || a % 105 == 0)
        {
            b = 1f;
        }

        LoadNV12(FrameNo);
        
        mat.SetFloat("_b", b);
        mat.SetTexture("_TexY",TexY);
        mat.SetTexture("_TexU", TexU);
        mat.SetTexture("_TexV", TexV);

        
        FrameNo++;
    }

    void LoadNV12(int frmaeNo)
    {
        try
        {

                
            Video.Read(frameY, 0,FrmaeSize * 2 /3);
            Video.Read(frameU, 0,FrmaeSize/6);
            Video.Read(frameV,0,FrmaeSize/6);

         
            TexY.LoadRawTextureData(frameY);
            TexU.LoadRawTextureData(frameV);
            TexV.LoadRawTextureData(frameU);



            TexY.Apply();                
            TexU.Apply();
            TexV.Apply();
                

        }
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString());
        }
            
       


    }

    void OnDestroy()
    {
        //PlayerDeInit();
        Video.Close();
    }
}
