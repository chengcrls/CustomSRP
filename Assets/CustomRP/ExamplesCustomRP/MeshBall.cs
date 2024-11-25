using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBall : MonoBehaviour
{
    static int _baseColorId=Shader.PropertyToID("_BaseColor");

    [SerializeField] private Mesh mesh = default;
    
    [SerializeField] private Material material = default;
    
    Matrix4x4[] _matrices = new Matrix4x4[1023];
    Vector4[] _baseColors = new Vector4[1023];
    
    MaterialPropertyBlock _block;

    void Awake()
    {
        for (int i = 0; i < _matrices.Length; i++)
        {
            _matrices[i] = Matrix4x4.TRS(
                Random.insideUnitSphere * 10f, 
                Quaternion.Euler(Random.value*360f,Random.value*360f,Random.value*360f), 
                Vector3.one*Random.Range(0.5f,1.5f));
            _baseColors[i] = new Vector4(Random.value, Random.value, Random.value, Random.Range(0.5f,1.5f));
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_block == null)
        {
            _block = new MaterialPropertyBlock();
            _block.SetVectorArray(_baseColorId, _baseColors);
        }
        Graphics.DrawMeshInstanced(mesh, 0, material, _matrices, 1023, _block);
    }
}
