// From Internet https://gist.github.com/mdomrach/a66602ee85ce45f8860c36b2ad31ea14#file-gridmesh-cs

using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Game;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class GridMesh : MonoBehaviour
{
    //public int GridSize;

    public void Initialize(MapSize size)
    {
        int x = size.X;
        int y = size.Y;

        MeshFilter filter = gameObject.GetComponent<MeshFilter>();
        var mesh = new Mesh();
        var verticies = new List<Vector3>();

        var indicies = new List<int>();

        int counter = 0;

        for (int i = 0; i <= x; i++)
        {
            verticies.Add(new Vector3(i, 0, 0));
            verticies.Add(new Vector3(i, 0, y));

            indicies.Add(counter++);
            indicies.Add(counter++);
        }
        for (int i = 0; i <= y; i++)
        {
            verticies.Add(new Vector3(0, 0, i));
            verticies.Add(new Vector3(x, 0, i));

            indicies.Add(counter++);
            indicies.Add(counter++);
        }
        
        mesh.vertices = verticies.ToArray();
        mesh.SetIndices(indicies.ToArray(), MeshTopology.Lines, 0);
        filter.mesh = mesh;

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        meshRenderer.material.color = Color.white;
        meshRenderer.material.shader = Shader.Find("Legacy Shaders/VertexLit");
    }
}