using System.IO;
using UnityEngine;
using SFB;
using VRM;
using VRMShaders;

public class Export : MonoBehaviour
{
    [SerializeField] Import Import;

    public void OnClick()
    {
        var path = StandaloneFileBrowser.SaveFilePanel("Save VRM", "", "", "vrm");
        if (path == "") return;

        var vrm = VRMExporter.Export(new UniGLTF.GltfExportSettings(), Import.Root, new RuntimeTextureSerializer());
        var bytes = vrm.ToGlbBytes();

        File.WriteAllBytes(path, bytes);
    }
}
