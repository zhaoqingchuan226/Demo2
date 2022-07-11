using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorObjExporter_SkinnedMeshRenderer : EditorObjExporter<SkinnedMeshRenderer, EditorObjExporter_SkinnedMeshRenderer>
{
    protected EditorObjExporter_SkinnedMeshRenderer() { }

    protected override string MeshToString(SkinnedMeshRenderer t, Dictionary<string, ObjMaterial> materialList)
    {
        Mesh m = new Mesh();
        t.BakeMesh(m);
        return MeshToString(m, t.sharedMaterials, t.name, t.transform, materialList);
    }

    [MenuItem("Custom/Export Obj/SkinnedMeshRenderer/导出所有选择的蒙皮网格渲染器以分离的Obj形式")]//Export all SkinnedMeshRenderer in selection to separate Objs
    protected static void ExportSelectionToSeparate_SMR()
    {
        ExportSelectionToSeparate();
    }
    [MenuItem("Custom/Export Obj/SkinnedMeshRenderer/导出整个选择的蒙皮网格渲染器到单一Obj")]//Export whole selection to single Obj
    protected static void ExportWholeSelectionToSingle_SMR()
    {
        ExportWholeSelectionToSingle();
    }
    [MenuItem("Custom/Export Obj/SkinnedMeshRenderer/导出每个选择的蒙皮网格渲染器到单一Obj")]//Export each selected to single Obj
    protected static void ExportEachSelectionToSingle_SMR()
    {
        ExportEachSelectionToSingle();
    }
}