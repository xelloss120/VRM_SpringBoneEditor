using System.IO;
using UnityEngine;
using SFB;
using UniGLTF;
using VRM;
using VRMShaders;

public class Export : MonoBehaviour
{
    [SerializeField] Import Import;

    public void OnClick()
    {
        var path = StandaloneFileBrowser.SaveFilePanel("Save VRM", "", "", "vrm");
        if (path == "") return;

        var vrm = VRMExporter.Export(new GltfExportSettings(), Import.Root, new RuntimeTextureSerializer());
        var bytes = vrm.ToGlbBytes();

        File.WriteAllBytes(path, bytes);
    }
}
