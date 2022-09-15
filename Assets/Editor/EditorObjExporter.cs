using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

public struct ObjMaterial
{
    public string name, textureName;
}

public abstract class EditorObjExporter<T, R> : ScriptableObject
    where T : Component
    where R : EditorObjExporter<T, R>
{
    #region 单例
    /// <summary>
    /// 内部类
    /// </summary>
    private static class InnerClass
    {
        public static R instance = NewR();
        private static R NewR()
        {
            ConstructorInfo[] ctors;
            //获取public的构造方法
            ctors = typeof(R).GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            //一般情况单例不需要public的构造方法
            if (ctors.Length != 0)
                throw new Exception("Singleton cannot have Public Constructor()!");
            //先获取所有非public的构造方法
            ctors = typeof(R).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
            //从ctors中获取无参的构造方法
            ConstructorInfo ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);
            if (ctor == null)
                throw new Exception("Non-public Constructor() not found!");
            return ctor.Invoke(null) as R;
        }
    }
    public static R GetInstance()
    {
        return InnerClass.instance;
    }
    protected EditorObjExporter()
    {

    }
    #endregion

    /// <summary>
    /// 顶点偏移量
    /// </summary>
    protected static int vertexOffset = 0;
    /// <summary>
    /// 法线偏移量
    /// </summary>
    protected static int normalOffset = 0;
    /// <summary>
    /// UV的偏移量
    /// </summary>
    protected static int uvOffset = 0;

    //User should probably be able to change this. It is currently left as an excercise for the reader.
    protected static readonly string targetFolder = "ExportedObj";

    protected abstract string MeshToString(T t, Dictionary<string, ObjMaterial> materialList);
    /// <summary>
    /// Mesh保存成String字符串
    /// </summary>
    /// <param name="mesh">Mesh对象</param>
    /// <param name="materials">材质</param>
    /// <param name="name">对象名</param>
    /// <param name="transform">对象transform</param>
    /// <param name="materialList">材质字典</param>
    /// <returns>结果字符串</returns>
    protected static string MeshToString(Mesh mesh, Material[] materials, string name, Transform transform, Dictionary<string, ObjMaterial> materialList)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("g ").Append(name).Append("\n");//数据起始
        foreach (Vector3 lv in mesh.vertices)
        {//顶点数据循环输入
            Vector3 wv = transform.TransformPoint(lv);
            //This is sort of ugly - inverting x-component since we're in
            //a different coordinate system than "everyone" is "used to".
            sb.Append(string.Format("v {0} {1} {2}\n", -wv.x, wv.y, wv.z));
        }
        sb.Append("\n");//空格
        foreach (Vector3 lv in mesh.normals)
        {//法线数据循环输入
            Vector3 wv = transform.TransformDirection(lv);
            sb.Append(string.Format("vn {0} {1} {2}\n", -wv.x, wv.y, wv.z));
        }
        sb.Append("\n");//空格
        foreach (Vector3 v in mesh.uv)
        {//UV数据循环输入
            sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
        }
        for (int material = 0; material < mesh.subMeshCount; material++)
        {//材质数据循环输入
            sb.Append("\n");//空格
            sb.Append("usemtl ").Append(materials[material].name).Append("\n");
            sb.Append("usemap ").Append(materials[material].name).Append("\n");
            try
            {//See if this material is already in the materiallist.看看这个字典是否已经在字典中
                ObjMaterial objMaterial = new ObjMaterial
                {
                    name = materials[material].name
                };
                if (materials[material].mainTexture)
                    objMaterial.textureName = AssetDatabase.GetAssetPath(materials[material].mainTexture);//另一种方式EditorUtility.GetAssetPath(mats[material].mainTexture)
                else
                    objMaterial.textureName = null;
                materialList.Add(objMaterial.name, objMaterial);
            }
            catch (ArgumentException)
            {//已经在字典中
                //Already in the dictionary
            }
            int[] triangles = mesh.GetTriangles(material);
            for (int i = 0; i < triangles.Length; i += 3)
            {//Because we inverted the x-component, we also needed to alter the triangle winding.
                sb.Append(string.Format("f {1}/{1}/{1} {0}/{0}/{0} {2}/{2}/{2}\n",
                    triangles[i] + 1 + vertexOffset, triangles[i + 1] + 1 + normalOffset, triangles[i + 2] + 1 + uvOffset));
            }
        }
        vertexOffset += mesh.vertices.Length;
        normalOffset += mesh.normals.Length;
        uvOffset += mesh.uv.Length;
        return sb.ToString();
    }

    /// <summary>
    /// 清理
    /// </summary>
    protected static void Clear()
    {
        vertexOffset = 0;
        normalOffset = 0;
        uvOffset = 0;
    }

    /// <summary>
    /// 准备文件写入
    /// </summary>
    /// <returns></returns>
    protected static Dictionary<string, ObjMaterial> PrepareFileWrite()
    {
        Clear();
        return new Dictionary<string, ObjMaterial>();
    }

    #region 文件保存数据
    /// <summary>
    /// 材质保存到文件
    /// </summary>
    /// <param name="materialList">材质字典</param>
    /// <param name="folder">文件夹</param>
    /// <param name="filename">文件名</param>
    protected static void MaterialsToFile(Dictionary<string, ObjMaterial> materialList, string folder, string filename)
    {
        using (StreamWriter sw = new StreamWriter(folder + "/" + filename + ".mtl"))
        {//数据流写入
            foreach (KeyValuePair<string, ObjMaterial> kvp in materialList)
            {
                sw.Write("\n");
                sw.Write("newmtl {0}\n", kvp.Key);
                sw.Write("Ka  0.6 0.6 0.6\n");
                sw.Write("Kd  0.6 0.6 0.6\n");
                sw.Write("Ks  0.9 0.9 0.9\n");
                sw.Write("d  1.0\n");
                sw.Write("Ns  0.0\n");
                sw.Write("illum 2\n");
                if (kvp.Value.textureName != null)
                {
                    string destinationFile = kvp.Value.textureName;
                    int stripIndex = destinationFile.LastIndexOf('/');//FIXME: Should be Path.PathSeparator;
                    if (stripIndex >= 0)
                        destinationFile = destinationFile.Substring(stripIndex + 1).Trim();
                    string relativeFile = destinationFile;
                    destinationFile = folder + "/" + destinationFile;
                    Debug.Log("Copying texture from " + kvp.Value.textureName + " to " + destinationFile);
                    try
                    {//复制源文件
                        File.Copy(kvp.Value.textureName, destinationFile);
                    }
                    catch
                    {

                    }
                    sw.Write("map_Kd {0}", relativeFile);
                }
                sw.Write("\n\n\n");
            }
        }
    }
    /// <summary>
    /// Mesh保存到文件
    /// </summary>
    /// <param name="t">组件对象</param>
    /// <param name="folder">文件夹</param>
    /// <param name="filename">文件名</param>
    protected static void MeshToFile(T t, string folder, string filename)
    {
        Dictionary<string, ObjMaterial> materialList = PrepareFileWrite();
        using (StreamWriter sw = new StreamWriter(folder + "/" + filename + ".obj"))
        {//数据流写入
            sw.Write("mtllib ./" + filename + ".mtl\n");
            sw.Write(GetInstance().MeshToString(t, materialList));
        }
        MaterialsToFile(materialList, folder, filename);
    }
    /// <summary>
    /// Mesh保存到文件
    /// </summary>
    /// <param name="ts">组件对象数组</param>
    /// <param name="folder">文件夹</param>
    /// <param name="filename">文件名</param>
    protected static void MeshesToFile(T[] ts, string folder, string filename)
    {
        Dictionary<string, ObjMaterial> materialList = PrepareFileWrite();
        using (StreamWriter sw = new StreamWriter(folder + "/" + filename + ".obj"))
        {//数据流写入
            sw.Write("mtllib ./" + filename + ".mtl\n");
            for (int i = 0; i < ts.Length; i++)
            {
                sw.Write(GetInstance().MeshToString(ts[i], materialList));
            }
        }
        MaterialsToFile(materialList, folder, filename);
    }
    #endregion

    /// <summary>
    /// 创建目标文件夹
    /// </summary>
    /// <returns>是否创建完成</returns>
    protected static bool CreateTargetFolder()
    {
        try
        {
            Directory.CreateDirectory(targetFolder);
        }
        catch
        {
            EditorUtility.DisplayDialog("Error!", "Failed to create target folder!", "");
            return false;
        }
        return true;
    }

    /// <summary>
    /// 选择的网格分别输出到一个Obj文件
    /// </summary>
    protected static void ExportSelectionToSeparate()
    {
        if (!CreateTargetFolder())
            return;
        Transform[] selection = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);
        if (selection.Length == 0)
        {
            EditorUtility.DisplayDialog("No source object selected!", "Please select one or more target objects", "");
            return;
        }
        int exportedObjects = 0;
        for (int i = 0; i < selection.Length; i++)
        {
            Component[] components = selection[i].GetComponentsInChildren(typeof(T));
            for (int m = 0; m < components.Length; m++)
            {
                exportedObjects++;
                MeshToFile((T)components[m], targetFolder, selection[i].name + "_" + i + "_" + m);
            }
        }
        if (exportedObjects > 0)
            EditorUtility.DisplayDialog("Objects exported", "Exported " + exportedObjects + " objects", "");
        else
            EditorUtility.DisplayDialog("Objects not exported", "Make sure at least some of your selected objects have mesh filters!", "");
    }
    /// <summary>
    /// 输出所有网格到单一的Obj文件
    /// </summary>
    protected static void ExportWholeSelectionToSingle()
    {
        if (!CreateTargetFolder())
            return;
        Transform[] selection = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);
        if (selection.Length == 0)
        {
            EditorUtility.DisplayDialog("No source object selected!", "Please select one or more target objects", "");
            return;
        }
        int exportedObjects = 0;
        ArrayList list = new ArrayList();
        for (int i = 0; i < selection.Length; i++)
        {
            Component[] components = selection[i].GetComponentsInChildren(typeof(T));
            for (int m = 0; m < components.Length; m++)
            {
                exportedObjects++;
                list.Add(components[m]);
            }
        }
        if (exportedObjects > 0)
        {
            T[] arr = new T[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                arr[i] = (T)list[i];
            }
            string filename = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + "_" + exportedObjects;//另一种方式EditorApplication.currentScene
            int stripIndex = filename.LastIndexOf('/');//FIXME: Should be Path.PathSeparator
            if (stripIndex >= 0)
                filename = filename.Substring(stripIndex + 1).Trim();
            MeshesToFile(arr, targetFolder, filename);
            EditorUtility.DisplayDialog("Objects exported", "Exported " + exportedObjects + " objects to " + filename, "");
        }
        else
            EditorUtility.DisplayDialog("Objects not exported", "Make sure at least some of your selected objects have mesh filters!", "");
    }
    /// <summary>
    /// 输出每个选择的模型到单一Obj文件
    /// </summary>
    protected static void ExportEachSelectionToSingle()
    {
        if (!CreateTargetFolder())
            return;
        Transform[] selection = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);
        if (selection.Length == 0)
        {
            EditorUtility.DisplayDialog("No source object selected!", "Please select one or more target objects", "");
            return;
        }
        int exportedObjects = 0;
        for (int i = 0; i < selection.Length; i++)
        {
            Component[] components = selection[i].GetComponentsInChildren(typeof(T));
            T[] mf = new T[components.Length];
            for (int m = 0; m < components.Length; m++)
            {
                exportedObjects++;
                mf[m] = (T)components[m];
            }
            MeshesToFile(mf, targetFolder, selection[i].name + "_" + i);
        }

        if (exportedObjects > 0)
            EditorUtility.DisplayDialog("Objects exported", "Exported " + exportedObjects + " objects", "");
        else
            EditorUtility.DisplayDialog("Objects not exported", "Make sure at least some of your selected objects have mesh filters!", "");
    }
}