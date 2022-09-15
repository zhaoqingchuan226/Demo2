using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorObjExporter_MeshFilter : EditorObjExporter<MeshFilter, EditorObjExporter_MeshFilter>
{
    protected EditorObjExporter_MeshFilter() { }

    protected override string MeshToString(MeshFilter t, Dictionary<string, ObjMaterial> materialList)
    {
        Material[] mats = t.GetComponent<Renderer>().sharedMaterials;
        return MeshToString(t.sharedMesh, mats, t.name, t.transform, materialList);
    }

    [MenuItem("Custom/Export Obj/MeshFilter/导出所有选择的网格过滤器以分离的Obj形式")]//Export all MeshFilters in selection to separate Objs
    protected static void ExportSelectionToSeparate_MF()
    {
        ExportSelectionToSeparate();
    }
    [MenuItem("Custom/Export Obj/MeshFilter/导出整个选择的网格过滤器到单一Obj")]//Export whole selection to single Obj
    protected static void ExportWholeSelectionToSingle_MF()
    {
        ExportWholeSelectionToSingle();
    }
    [MenuItem("Custom/Export Obj/MeshFilter/导出每个选择的网格过滤器到单一Obj")]//Export each selected to single Obj
    protected static void ExportEachSelectionToSingle_MF()
    {
        ExportEachSelectionToSingle();
    }
}