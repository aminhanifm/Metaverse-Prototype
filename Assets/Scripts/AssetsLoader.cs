using System.IO;
using System.Collections;
using UnityEngine;

public class AssetsLoader : MonoBehaviour
{
    [Tooltip("Name of assets you want to load")]
    public string assetName = "";
    [Tooltip("Name of bundle you want to load")]
    public string bundleName = "";

    public GameObject test;

    IEnumerator Start()
    {
        AssetBundleCreateRequest asyncBundleRequest = AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, bundleName));
        yield return asyncBundleRequest;

        AssetBundle localAssetBundle = asyncBundleRequest.assetBundle;

        if (localAssetBundle == null) {
            Debug.LogError("Failed to load AssetBundle!");
            yield break;
        }

        AssetBundleRequest assetRequest = localAssetBundle.LoadAssetAsync<GameObject>(assetName);
        yield return assetRequest;

        test = assetRequest.asset as GameObject;

        localAssetBundle.Unload(false);
    }
}
