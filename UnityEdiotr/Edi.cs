using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;
public class Edi : Editor
{
    [MenuItem("Tools/ABTool")] 
    static void build()
    {
        string outPath = Application.dataPath + "/Tools/";
        string path = Application.dataPath + "/Res/";
        string[] filepaths = Directory.GetFiles(path,".",SearchOption.AllDirectories); //获取res文件夹中所有的需要变成AB包的东西

        foreach (var file in filepaths)//遍历文件夹
        {
            if (Path.GetExtension(file).Contains("meta")) continue;
            string abName = string.Empty;
            string fileName = file.Replace(Application.dataPath,"Assets");
            Debug.Log(fileName);
            AssetImporter asset = AssetImporter.GetAtPath(fileName);//获取文件
            abName = fileName.Replace("Assets/Res/",string.Empty);
            abName = abName.Replace("\\","/");
            //修改路径
            if (file.Contains("_Comm"))
            {
                abName = abName.Replace("/" + Path.GetFileName(file), string.Empty);
            }
            else
            {
                abName = abName.Replace(Path.GetExtension(file),string.Empty);
            }
            asset.assetBundleName = abName;//将文件的AB名字设置为自己的路径+名字
            Debug.Log(abName);
            asset.assetBundleVariant = "u3d";//设置ab包后缀
        }
        BuildPipeline.BuildAssetBundles(outPath,BuildAssetBundleOptions.ChunkBasedCompression,BuildTarget.StandaloneWindows64);//将所有文件打包成windows所用的方式
        foreach (var item in filepaths)
        {//遍历将所有文件的ab包名初始化
            if (Path.GetExtension(item).Contains("meta")) continue;

            string fileName = item.Replace(Application.dataPath,"Assets");
            AssetImporter assetImporter = AssetImporter.GetAtPath(fileName);
            assetImporter.assetBundleName = string.Empty;
        }
        AssetDatabase.Refresh();//刷新文件
    }

    static VersionData versionData = new VersionData();
    [MenuItem("Tools/MakVersion")]

    static void MakeVersion()
    {
        versionData.downLoadUrl = "http://127.0.0.1/Game/Tools/"; //服务器位置
        versionData.version = "1.0.1"; //服务器版本号
        versionData.versonCode = 1;//服务器版本编号

        if (versionData.assetDatas==null)  //如果第一次创建assetDatas列表
        {
            versionData.assetDatas = new List<AssetData>();//创建新列表
        }
        else
        {
            versionData.assetDatas.Clear();//清空列表
        }
        string abPath = Application.dataPath + "/Tools/"; //ab包路径
        string[] filePaths = Directory.GetFiles(abPath,".",SearchOption.AllDirectories); 
        Debug.Log(filePaths.Length);
        foreach (var file in filePaths)//将所有ab包文件记录到assetData类中
        {
            if (Path.GetExtension(file).Contains("meta") || Path.GetExtension(file).Contains("manifest")) continue;
            string abName = file.Replace("\\", "/");
            abName = abName.Replace(abPath,string.Empty);
            int len = File.ReadAllBytes(file).Length;
            string md5 = FileMD5(file);

            AssetData assetData = new AssetData();
            assetData.abName = abName;  //包名
            assetData.len = len;        //长度
            assetData.md5 = md5;        //md5值

            versionData.assetDatas.Add(assetData);  //添加到集合中
        }
        string version = JsonConvert.SerializeObject(versionData);
        File.WriteAllText(abPath+"/version.txt",version);
    }

    static System.Text.StringBuilder sb = new System.Text.StringBuilder();
    private static string FileMD5(string filePath)  //创建md5值
    {
        FileStream file = new FileStream(filePath,FileMode.Open);
        System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] bytes = md5.ComputeHash(file);
        file.Close();
        sb.Clear();
        for (int i = 0; i < bytes.Length; i++)
        {
            sb.Append(bytes[i].ToString("X2"));
        }
        return sb.ToString();
    }
}


 //AssetImporter asset = AssetImporter.GetAtPath("Assets/Res/Cube.prefab");
//asset.assetBundleName = "Cube";
//asset.assetBundleVariant = "u3d";

//BuildPipeline.BuildAssetBundles(outPath,BuildAssetBundleOptions.ChunkBasedCompression,BuildTarget.StandaloneWindows64);
//AssetDatabase.Refresh();

//Path.GetFileName()