using UnityEngine;
using System.Collections;

// VoxelPerformance/Scripts/MapDisplay.cs
// Copyright 2016 Charles Griffiths

namespace VoxelPerformance
{
  // This class is used to display one chunk produced by compute shaders in VoxelPerformance/Shaders
  // using a geometry shader found in VoxelPerformance/Shaders/VoxelGeometry.shader
  public class MapDisplay : MonoBehaviour
  {
  Color[] color;
  Texture2D sprite;

  ComputeBuffer display;
  Material material;

  public float size { get; private set; }
  public Bounds bounds { get; private set; }


    public void initialize( Shader geometryShader, Color[] color, float size, Texture2D sprite, ComputeBuffer display )
    {
      this.color = color;
      this.size = size;
      this.sprite = sprite;
      this.display = display;

      material = new Material( geometryShader );
      bounds = new Bounds( transform.position + new Vector3( 128, 128, 128 )*size, new Vector3( 256, 256, 256 )*size);
      enabled = true;
    }


    public void releaseDisplayBuffers()
    {
      display.Release();
    }


    public bool isVisible()
    {
      return enabled
        && bounds.SqrDistance( MapGen.mainCamera.transform.position ) < 1.2f * MapGen.mainCamera.farClipPlane * MapGen.mainCamera.farClipPlane
        && GeometryUtility.TestPlanesAABB( MapGen.cameraPlanes, bounds );
    }


    void OnRenderObject()
    {
      if (isVisible())
      {
////        Shader.globalMaximumLOD = 100;
////        material.shader.maximumLOD = 100;

        material.SetPass( 0 );
        material.SetColor( "_Color", color[0] );
        material.SetColor( "_Color1", color[1] );
        material.SetColor( "_Color2", color[2] );
        material.SetColor( "_Color3", color[3] );
        material.SetColor( "_Color4", color[4] );
        material.SetColor( "_Color5", color[5] );
        material.SetColor( "_Color6", color[6] );

        material.SetVector( "_cameraPosition", MapGen.mainCamera.transform.position );
        material.SetVector( "_chunkPosition", transform.position );

        material.SetTexture( "_Sprite", sprite );
        material.SetFloat( "_Size", size );
        material.SetMatrix( "_worldMatrixTransform", transform.localToWorldMatrix );

        material.SetBuffer( "_displayPoints", display );

        Graphics.DrawProcedural( MeshTopology.Points, display.count );
      }
    }
  }
}

