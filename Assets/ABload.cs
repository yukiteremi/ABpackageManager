using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.Networking;

public class ABload : MonoBehaviour
{
    public GameObject father;
    // Start is called before the first frame update
    void Start()
    {
        GameObject go1 = ABManager.getsingelton().loadGameObject("cube.u3d", "Cube");
        //AssetBundle assetBundle = AssetBundle.LoadFromFile(Application.dataPath + "/Tools/cube.u3d");
        //GameObject game = Instantiate(go1);

        //AssetBundle mat = AssetBundle.LoadFromFile(Application.dataPath + "/Tools/mat.u3d");
        ////game.GetComponent<MeshRenderer>().material = mat.LoadAsset<Material>("mat");

        //AssetBundle assetBundleatlas= AssetBundle.LoadFromFile(Application.dataPath + "/Tools/new sprite atlas.u3d");
        //gameObject.GetComponent<Image>().sprite = assetBundleatlas.LoadAsset<SpriteAtlas>("New Sprite Atlas").GetSprite("diamond_01");

        //string json = File.ReadAllText("1.json");
        //Debug.Log(json);
        //List<contont> list = JsonConvert.DeserializeObject< List<contont>>(json);
        //for (int i = 0; i < list.Count; i++)
        //{
        //    GameObject clone = GameObject.Instantiate(Resources.Load<GameObject>("imgitem"));
        //    clone.transform.SetParent(father.transform);
        //    clone.GetComponent<Imgountroller>().Init(assetBundleatlas.LoadAsset<SpriteAtlas>("New Sprite Atlas").GetSprite(list[i].path),list[i].name);
        //}
        //for (int i = 0; i < 10; i++)
        //{
        //    list.Add(new contont("fight100"+(i+1), "玩家"+(i+1)));
        //}
        //string json = JsonConvert.SerializeObject(list);
        //File.WriteAllText("1.json",json);
        //StartCoroutine(loadAssetBundle());
    }


    IEnumerator loadAssetBundle()
    {
        //UnityWebRequest unityweb = UnityWebRequestAssetBundle.GetAssetBundle("file://"+ Application.dataPath + "/Tools/cube.u3d");
        //yield return unityweb.SendWebRequest();
        //if (unityweb.isNetworkError)
        //{
        //    Debug.Log(unityweb.error);
        //}
        //else
        //{
        //    AssetBundle assetbundle = DownloadHandlerAssetBundle.GetContent(unityweb);
        //    GameObject game = Instantiate(assetbundle.LoadAsset("Cube")) as GameObject;
        //}
        AssetBundleCreateRequest assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(Application.dataPath + "/Tools/cube.u3d");
        yield return assetBundleCreateRequest;
        Instantiate(assetBundleCreateRequest.assetBundle.LoadAsset<GameObject>("Cube"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

class contont
{
    public string path;
    public string name;

    public contont(string path, string name)
    {
        this.path = path;
        this.name = name;
    }
}
