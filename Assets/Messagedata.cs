using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Messagedata : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
public class VersionData
{
    public string downLoadUrl;
    public string version;
    public int versonCode;
    public List<AssetData> assetDatas;
}
public class AssetData
{
    public string abName;
    public int len;
    public string md5;
}
