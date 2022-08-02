using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System;

public class UpdateLoad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!Directory.Exists(pPath))  //如果p路径不存在
        {
            Directory.CreateDirectory(pPath); //创建一
            StartCoroutine(copy());
        }
        else
        {               //如果p路径存在
            StartCoroutine(CheckUpdate());
        }
    }
    IEnumerator CheckUpdate() //更新ab包文件夹里边的ab包
    {
        string localVersion = pPath + "version.txt";  //获取到p路径里边的ab包目录 json
        string localVersionContent = File.ReadAllText(localVersion);    //取出内容
        
        VersionData localVersionData = JsonConvert.DeserializeObject<VersionData>(localVersionContent);//json反编译
        Dictionary<string, AssetData> versionDic = new Dictionary<string, AssetData>(); //ab包的字典
        for (int i = 0; i < localVersionData.assetDatas.Count; i++)
        {
            AssetData assetData = localVersionData.assetDatas[i];
            versionDic.Add(assetData.abName,assetData);
        }
        string remoteVersion = localVersionData.downLoadUrl + "version.txt";//获取服务器中的目录
        string remoteVersionContent = "";   //目录内容
        Debug.Log(remoteVersion);
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(remoteVersion);   //webrequest方法获取目录
        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.isNetworkError)
        {
            Debug.Log(unityWebRequest.error);
        }
        else
        {
            remoteVersionContent = unityWebRequest.downloadHandler.text;    //获取到的目录内容
        }
        Debug.Log(remoteVersionContent);
        VersionData remoteVersionData = JsonConvert.DeserializeObject<VersionData>(remoteVersionContent);   //反编译
        List<AssetData> updateList = new List<AssetData>();
        if (localVersionData.versonCode<remoteVersionData.versonCode)   //将新目录和原目录对比  如果服务器上的版本有更新
        {
            for (int i = 0; i < remoteVersionData.assetDatas.Count; i++)
            {
                AssetData assetData = remoteVersionData.assetDatas[i];
                if (versionDic.ContainsKey(assetData.abName))   //如果原目录有此文件
                {
                    if (versionDic[assetData.abName].md5!=assetData.md5)    //如果原目录的此文件和新文件不同
                    {
                        updateList.Add(assetData);  //添加进复制列表
                    }
                }
                else
                {
                    updateList.Add(assetData);//添加进复制列表
                }
            }
        }
        else
        {
            EnterGame();    //如果无须更新进入游戏
            yield break;
        }

        for (int i = 0; i < updateList.Count; i++)  //需要更新的文件列表
        {
            string abName = updateList[i].abName;
            UnityWebRequest updateAsset = UnityWebRequest.Get(remoteVersionData.downLoadUrl+abName);
            yield return updateAsset.SendWebRequest();
            if (updateAsset.isNetworkError)
            {
                Debug.Log(updateAsset.error);
            }
            else
            {
                string perPath = pPath + abName;    //p路径下的文件位置
                string fileName = Path.GetFileName(perPath);    //p路径下的文件名字
                string dir = Path.GetDirectoryName(perPath).Replace("\\","/");  //修改
                if (!Directory.Exists(dir)) //如果第一次创建
                {
                    Directory.CreateDirectory(dir); //直接创建文件
                }
                File.WriteAllBytes(dir+fileName,updateAsset.downloadHandler.data);//写入数据
            }
        }
        Debug.Log(pPath + "version.txt");
        Debug.Log(remoteVersionContent);
        File.WriteAllText(pPath+ "version.txt", remoteVersionContent);  //将新的目录替换本地目录
        yield return null;
        EnterGame();
    }

    private void EnterGame()
    {
        Debug.Log("游戏开始");
    }

    IEnumerator copy()
    {
        string streamingAssetPatgVersion = sPath + "version.txt";   //s路径下的目录
        string versionContent = "";


#if UNITY_ANDROID   //如果是安卓端 读取文件
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(streamingAssetPatgVersion);
        yield return unityWebRequest.SendWebRequest();
        if (unityWebRequest.isNetworkError)
        {
            Debug.Log(unityWebRequest.error);
        }
        else
        {
            versionContent = unityWebRequest.downloadHandler.text;
        }
#else   //其他端 读取文件
        versionContent = File.ReadAllText(streamingAssetPatgVersion);
#endif
        VersionData versionData = JsonConvert.DeserializeObject<VersionData>(versionContent);   //目录的反编译
        Debug.Log(versionData.assetDatas.Count);
        for (int i = 0; i < versionData.assetDatas.Count; i++)  //将目录中的所有ab包集合拆出来
        {
            AssetData assetData = versionData.assetDatas[i];
            string SPath = sPath + assetData.abName;
            string p = pPath + assetData.abName;
            string dir = Path.GetDirectoryName(p);
            dir = dir.Replace(sPath, pPath);
            Debug.Log(dir);
            if (!Directory.Exists(dir))//如果文件夹第一次创建就直接创建
            {
                Directory.CreateDirectory(dir);
            }
            File.Copy(SPath, dir + Path.GetFileName(SPath));   //将文件拷贝到指定目录下
        }
        File.WriteAllText(pPath+"/version.txt",versionContent); //p路径下写入目录
        yield return null;
    }
    // Update is called once per frame
    void Update()
    {

    }
    public static string sPath
    {
        get
        {
#if UNITY_ANDROID
    return "jar:file://"+Application.dataPath + "!/assets/Tools/";
#else
            return Application.streamingAssetsPath + "/Tools/";
#endif
        }
    }
    public static string pPath 
    {
        get
        {
            return Application.persistentDataPath + "/Tools/";
        }
    }
}
