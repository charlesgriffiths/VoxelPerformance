using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// VoxelPerformance/Scripts/VoxelMapFormat.cs
// Copyright 2016 Charles Griffiths

namespace VoxelPerformance
{
  // This class is a C# interface to VoxelPerformance/Shaders/MeshGeneration.compute
  public class VoxelMapFormat
  {
  ComputeShader meshGen;
  int FaceGenKernel, FaceSumKernel, FaceCopyKernel, GetFacesKernel;
  ComputeBuffer ShownVoxels, ShownVoxelCount, ShownVoxelOffset, TotalVoxelCount; // temporary buffers
  ComputeBuffer SolidVoxels;  // passed to geometry shader

  int totalShownVoxelCount = 0;
  VoxelMapData voxelMapData;


    public VoxelMapFormat( ComputeShader shader, VoxelMapData data )
    {
      meshGen = shader;

      initKernels();
      initTemporaryBuffers();
      voxelMapData = data;
      setTemporaryBuffers();

      SolidVoxels = null;
    }


    void initKernels()
    {
      FaceGenKernel = meshGen.FindKernel( "FaceGen" );
      FaceSumKernel = meshGen.FindKernel( "FaceSum" );
      FaceCopyKernel = meshGen.FindKernel( "FaceCopy" );
      GetFacesKernel = meshGen.FindKernel( "GetFaces" );
    }


    void initTemporaryBuffers()
    {
      ShownVoxelCount = new ComputeBuffer( 256 * 256, sizeof( int ));
      ShownVoxelOffset = new ComputeBuffer( 256 * 256, sizeof( int ));
      ShownVoxels = new ComputeBuffer( 256 * 256 * 256, sizeof( int ));
      TotalVoxelCount = new ComputeBuffer( 1, sizeof( int ));
    }


    public ComputeBuffer takeSolidVoxels()
    {
    ComputeBuffer cb = SolidVoxels;

      SolidVoxels = null;

      return cb;
    }


    public void releaseSolidVoxels()
    {
      SolidVoxels.Release();
      SolidVoxels = null;
    }


    public void callFaceGenKernel()
    {
      meshGen.Dispatch( FaceGenKernel, 8, 1, 8 );
    }


    public void callFaceSumKernel()
    {
      meshGen.Dispatch( FaceSumKernel, 1, 1, 1 );
    }


    public void callFaceCopyKernel()
    {
    int[] totalfaceblocks = new int[1];

      TotalVoxelCount.GetData( totalfaceblocks );
      totalShownVoxelCount = totalfaceblocks[0];

      if (null != SolidVoxels)
        releaseSolidVoxels();

      SolidVoxels = new ComputeBuffer( totalfaceblocks[0], sizeof( int ));

      meshGen.SetBuffer( FaceCopyKernel, "SolidVoxels", SolidVoxels );

      meshGen.Dispatch( FaceCopyKernel, 8, 1, 8 );
    }


    void setTemporaryBuffers()
    {
      meshGen.SetBuffer( FaceGenKernel, "MapVoxels", voxelMapData.MapVoxels );
      meshGen.SetBuffer( FaceGenKernel, "MapHeights", voxelMapData.MapHeights );
      meshGen.SetBuffer( FaceGenKernel, "ShownVoxelCount", ShownVoxelCount );
      meshGen.SetBuffer( FaceGenKernel, "ShownVoxels", ShownVoxels );

      meshGen.SetBuffer( FaceSumKernel, "ShownVoxelCount", ShownVoxelCount );
      meshGen.SetBuffer( FaceSumKernel, "ShownVoxelOffset", ShownVoxelOffset );
      meshGen.SetBuffer( FaceSumKernel, "TotalVoxelCount", TotalVoxelCount );

      meshGen.SetBuffer( FaceCopyKernel, "ShownVoxels", ShownVoxels );
      meshGen.SetBuffer( FaceCopyKernel, "ShownVoxelCount", ShownVoxelCount );
      meshGen.SetBuffer( FaceCopyKernel, "ShownVoxelOffset", ShownVoxelOffset );


      meshGen.SetBuffer( GetFacesKernel, "MapVoxels", voxelMapData.MapVoxels );
      meshGen.SetBuffer( GetFacesKernel, "TotalVoxelCount", TotalVoxelCount );
    }


    public Mesh[] getMeshes()
    {
    ComputeBuffer SolidFaces = new ComputeBuffer( totalShownVoxelCount, sizeof( int ));

      meshGen.SetBuffer( GetFacesKernel, "SolidVoxels", SolidVoxels );
      meshGen.SetBuffer( GetFacesKernel, "SolidFaces", SolidFaces );

      meshGen.Dispatch( GetFacesKernel, (totalShownVoxelCount+1023)/1024, 1, 1 );

    int[] voxels = new int[totalShownVoxelCount];

      SolidFaces.GetData( voxels );
      SolidFaces.Dispose();

    List<Mesh> meshes = new List<Mesh>();

      for (int offset=0; offset<totalShownVoxelCount; offset += 2730)
      {
        meshes.Add( makeMesh( voxels, offset, offset+2730 <= totalShownVoxelCount ? 2730 : totalShownVoxelCount-offset ));
      }

      return meshes.ToArray();
    }


    public GameObject getTerrain()
    {
    int[] heights = new int[256*256];

      voxelMapData.MapHeights.GetData( heights );

    float[,] terrainHeights = new float[256,256];

      for (int x=0; x<256; x++)
        for (int z=0; z<256; z++)
          terrainHeights[x,z] = heights[x*256+z]/256.0f;

    TerrainData terrainData = new TerrainData();

//      terrainData.size = new Vector3( 256, 256, 256 );
      terrainData.size = new Vector3( 128, 1024, 128 );
      terrainData.heightmapResolution = 256;
      terrainData.baseMapResolution = 256;
      terrainData.SetDetailResolution( 256, 16 );

      terrainData.SetHeightsDelayLOD( 0, 0, terrainHeights );

      return Terrain.CreateTerrainGameObject( terrainData );
    }


    static Vector3
      v3000 = new Vector3( -0.5f, -0.5f, -0.5f ),
      v3001 = new Vector3( -0.5f, -0.5f,  0.5f ),
      v3010 = new Vector3( -0.5f,  0.5f, -0.5f ),
      v3011 = new Vector3( -0.5f,  0.5f,  0.5f ),
      v3100 = new Vector3(  0.5f, -0.5f, -0.5f ),
      v3101 = new Vector3(  0.5f, -0.5f,  0.5f ),
      v3110 = new Vector3(  0.5f,  0.5f, -0.5f ),
      v3111 = new Vector3(  0.5f,  0.5f,  0.5f );

    Mesh makeMesh( int[] vox, int start, int count )
    {
    Vector3[] vertices = new Vector3[count * 4 * 6];
    int vertexCount=0;

      for (int i=0; i<count; i++)
      {
      int v = vox[start+i];
      Vector3 pos = new Vector3( (v>>16)&255, (v>>8)&255, v&255 );
      int facing = (v>>24)&255;

        if (0 != (facing & 0x1))
        {
          vertices[vertexCount++] = v3001 + pos;
          vertices[vertexCount++] = v3011 + pos;
          vertices[vertexCount++] = v3000 + pos;
          vertices[vertexCount++] = v3010 + pos;
        }

        if (0 != (facing & 0x2))
        {
          vertices[vertexCount++] = v3100 + pos;
          vertices[vertexCount++] = v3110 + pos;
          vertices[vertexCount++] = v3101 + pos;
          vertices[vertexCount++] = v3111 + pos;
        }

        if (0 != (facing & 0x4))
        {
          vertices[vertexCount++] = v3001 + pos;
          vertices[vertexCount++] = v3000 + pos;
          vertices[vertexCount++] = v3101 + pos;
          vertices[vertexCount++] = v3100 + pos;
        }

        if (0 != (facing & 0x8))
        {
          vertices[vertexCount++] = v3010 + pos;
          vertices[vertexCount++] = v3011 + pos;
          vertices[vertexCount++] = v3110 + pos;
          vertices[vertexCount++] = v3111 + pos;
        }

        if (0 != (facing & 0x10))
        {
          vertices[vertexCount++] = v3000 + pos;
          vertices[vertexCount++] = v3010 + pos;
          vertices[vertexCount++] = v3100 + pos;
          vertices[vertexCount++] = v3110 + pos;
        }

        if (0 != (facing & 0x20))
        {
          vertices[vertexCount++] = v3101 + pos;
          vertices[vertexCount++] = v3111 + pos;
          vertices[vertexCount++] = v3001 + pos;
          vertices[vertexCount++] = v3011 + pos;
        }
      }

      Array.Resize<Vector3>( ref vertices, vertexCount );

    int[] triangles = new int[vertexCount / 4 * 6];

      for (int i=0, j=0; i<vertexCount; i += 4, j += 6)
      {
        triangles[j]   = i;
        triangles[j+1] = i+1;
        triangles[j+2] = i+2;
        triangles[j+3] = i+2;
        triangles[j+4] = i+1;
        triangles[j+5] = i+3;
      }

    Mesh mesh = new Mesh();

      mesh.vertices = vertices;
      mesh.triangles = triangles;
      mesh.RecalculateNormals();
      mesh.Optimize();

      return mesh;
    }


    public void releaseTemporaryBuffers()
    {
      ShownVoxelCount.Release();
      ShownVoxelCount = null;

      ShownVoxelOffset.Release();
      ShownVoxelOffset = null;

      ShownVoxels.Release();
      ShownVoxels = null;

      TotalVoxelCount.Release();
      TotalVoxelCount = null;
    }
  }
}

