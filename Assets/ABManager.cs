using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ABManager : Singelton<ABManager>   //ab包管理类
{
    AssetBundleManifest assetBundleManifest;  //ab包的类
    string ABpath;
    public ABManager()
    {
        ABpath = Application.dataPath + "/Tools/"; //ab包地址
        AssetBundle assetBundle = AssetBundle.LoadFromFile(ABpath+ "Tools"); //获取到ab包目录
        assetBundleManifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
    }
    Dictionary<string, BundleData> dicBundles = new Dictionary<string, BundleData>();//ab包文件字典

    public T[] loadAsset<T>(string abName) where T:UnityEngine.Object  //加载ab包文件
    {
        string[] dependencles = assetBundleManifest.GetAllDependencies(abName);  //获取所有的文件的依赖包
        foreach (var item in dependencles)
        {
            if (!dicBundles.ContainsKey(item))//如果字典中没有加载过此物品证明第一次加载
            {
                AssetBundle assetBundle1 = AssetBundle.LoadFromFile(ABpath+item);
                BundleData bundleData = new BundleData(assetBundle1);
                dicBundles.Add(item,bundleData);//添加进字典中
            }
            else
            {
                dicBundles[item].count++; //如果字典中有此物品将类中的加载次数增加
            }
        }
        if (!dicBundles.ContainsKey(abName))//获取文件本体
        {//如果字典中没有加载过此物品证明第一次加载
            AssetBundle assetBundle1 = AssetBundle.LoadFromFile(ABpath + abName);
            BundleData bundleData = new BundleData(assetBundle1);
            dicBundles.Add(abName, bundleData);
        }
        else
        {//添加进字典中
            dicBundles[abName].count++;
        }
        return dicBundles[abName].ab.LoadAllAssets<T>(); //返回文件
    }

    Dictionary<int, string> dicGameobject = new Dictionary<int, string>();

    public T loadOtherAsset<T>(string abName,string assetName) where T:Object  //读取ab包内文件
    {
        UnityEngine.Object[] objects = loadAsset<T>(abName);
        Object obj = null;
        foreach (var item in objects)
        {
            if (item.name==assetName)
            {
                obj = item;
                break;
            }
        }
        return obj as T;
    }

    public GameObject loadGameObject(string abName, string assetName)//实例化包内的gameobject
    {
        Object obj = loadOtherAsset<GameObject>(abName, assetName);
        GameObject go = GameObject.Instantiate(obj) as GameObject;
        dicGameobject.Add(go.GetInstanceID(), abName);
        return go;
    }

    public Sprite loadSprite(string abName, string assetName,string spriteName) //实例化包内图集的单张图片
    {
        SpriteAtlas obj = loadOtherAsset<SpriteAtlas>(abName, assetName);
        
        return obj.GetSprite(spriteName);
    }
    public Sprite[] loadSprites(string abName, string assetName) //实例化包内图集的多张图片
    {
        SpriteAtlas obj = loadOtherAsset<SpriteAtlas>(abName, assetName);
        Sprite[] spritrs = new Sprite[obj.spriteCount];
        obj.GetSprites(spritrs);
        return spritrs;
    }
    public void DestoryGameObject(GameObject go)    //销毁实例化的物品
    {
        int id = go.GetInstanceID();
        string abName = dicGameobject[id];
        GameObject.Destroy(go);
        dicGameobject.Remove(id);
    }

    public void UnLoadAB(string abName)     //卸载ab包
    {
        //ab包不使用的时候记得要卸载
        string[] dependencies = assetBundleManifest.GetAllDependencies(abName);

        foreach (var item in dependencies)//卸载依赖
        {
            if (dicBundles.ContainsKey(item))
            {
                dicBundles[item].count--;
                if (dicBundles[item].count<=0)
                {
                    dicBundles[item].Unload();
                }
            }
        }
        if (dicBundles.ContainsKey(abName))//卸载本体
        {
            dicBundles[abName].count--;
            if (dicBundles[abName].count <= 0)
            {
                dicBundles[abName].Unload();
            }
        }
    }
}

public class BundleData{
    public AssetBundle ab;
    public int count;

    public BundleData(AssetBundle ab)
    {
        this.ab = ab;
        this.count = 1;
        
    }
    public void Unload()
    {
        ab.Unload(true);
    }
}