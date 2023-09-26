using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Android;


public class player : MonoBehaviour
{

    struct YUVFrame
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1920*1080)]
        public byte[] data_y;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1920*1080/4)]
        public byte[] data_u;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1920*1080/4)]
        public byte[] data_v;
    };
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

    YUVFrame yuvFrame;

    [DllImport("playerstreaming")]
    private static extern int PlayerInit();

    [DllImport("playerstreaming")]
    private static extern int PlayerRun(int num,ref YUVFrame yuvfrmae);

    [DllImport("playerstreaming")]
    private static extern int PlayerDeInit();
    void Awake(){
        Debug.Log("UnityClientPlayer Awake begin");
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Debug.Log("UnityClientPlayer Awake ExternalStorageWrite");
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Debug.Log("UnityClientPlayer Awake ExternalStorageRead");
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
        }
         
        Debug.Log("UnityClientPlayer Awake finish");

    }

    void Start()
    {
        Debug.Log("UnityClientPlayer Start begin");

            if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                // 权限已授予，执行相应操作
                Debug.Log("UnityClientPlayer Storage write permission granted");
            }
            else
            {
                // 权限被拒绝，执行相应操作
                Debug.Log("UnityClientPlayer Storage write permission denied");
            }
            if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
            {
                // 权限已授予，执行相应操作
                Debug.Log("UnityClientPlayer Storage read permission granted");
            }
            else
            {
                // 权限被拒绝，执行相应操作
                Debug.Log("UnityClientPlayer Storage read permission denied");
            }
    

        
        int ret = PlayerInit();
        Debug.LogFormat("UnityClientPlayer ret = {0}",ret);
        TexY = new Texture2D(Width, Height, TextureFormat.Alpha8, false);
        TexU = new Texture2D(Width >> 1, Height >> 1, TextureFormat.Alpha8, false);
        TexV = new Texture2D(Width >> 1, Height >> 1, TextureFormat.Alpha8, false);



        frameY = new byte[Width * Height];

        frameU = new byte[Width * Height >> 2];

        frameV = new byte[Width * Height >> 2];

        //Video = new FileStream(VideoPath, FileMode.Open);

        yuvFrame = new YUVFrame();

        yuvFrame.data_y =new byte[1920*1080];

        yuvFrame.data_u =new byte[1920*1080/4];

        yuvFrame.data_v =new byte[1920*1080/4];
        Debug.Log("UnityClientPlayer Start finish");

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("UnityClientPlayer Updata begin");

        float b = 0;
        //int a = FrameNo;
        int a = PlayerRun(FrameNo,ref yuvFrame);
        Debug.Log("UnityClientPlayer PlayerRun run finish");
        if(a % 100 == 0 || a % 101 == 0 || a % 102 == 0|| a % 103 == 0 || a % 104 == 0 || a % 105 == 0)
        {
            b = 1f;
        }

        CallbackYUV();
        
        mat.SetFloat("_b", b);
        mat.SetTexture("_TexY",TexY);
        mat.SetTexture("_TexU", TexU);
        mat.SetTexture("_TexV", TexV);

        
        FrameNo++;
        Debug.Log("UnityClientPlayer Updata finish");
    }



    void CallbackYUV(){
        Debug.Log("UnityClientPlayer CallbackYUV begin");
        TexY.LoadRawTextureData(yuvFrame.data_y);
        TexU.LoadRawTextureData(yuvFrame.data_v);
        TexV.LoadRawTextureData(yuvFrame.data_u);

        TexY.Apply();                
        TexU.Apply();
        TexV.Apply();
        Debug.Log("UnityClientPlayer CallbackYUV finish");
    }

    void OnDestroy()
    {
        Debug.Log("UnityClientPlayer OnDestroy begin");
        PlayerDeInit();
        Debug.Log("UnityClientPlayer OnDestroy finish");
    }
}
