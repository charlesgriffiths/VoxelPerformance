using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

// VoxelPerformance/Scripts/MapGen.cs
// Copyright 2016 Charles Griffiths

namespace VoxelPerformance
{
  public class MapGen : MonoBehaviour
  {
  public bool createSponge = false;
  public bool showVoxels = true;
  public bool createMesh = false;
  public bool createUnityTerrain = false;

  public ComputeShader perlinGen;
  public ComputeShader meshGen;
  public Shader geometryShader;

  VoxelMapData mapChunkCreation;
  VoxelMapFormat mapChunkMeshing;

  public GameObject displayPrefab;
  public GameObject meshPrefab;

  public Color[] color;
  public float size;
  public Texture2D sprite;

  static public Camera mainCamera;
  static public Plane[] cameraPlanes;

  int offsetIndex;
  bool createChunks;

  [Tooltip("Maximum dimension (x and z)")]
  public int chunkMax = 1;


    void Start ()
    {
      mainCamera = Camera.main;

      mapChunkCreation = new VoxelMapData( perlinGen );
      mapChunkMeshing = new VoxelMapFormat( meshGen, mapChunkCreation );

      offsetIndex = 0;

      if (createSponge)
      {
        mapChunkCreation.createSponge();
      }
      else
      {
        mapChunkCreation.callPerlinMapGenKernel( mapOffset( offsetIndex ));
      }

      mapChunkMeshing.callFaceGenKernel();
      mapChunkMeshing.callFaceSumKernel();
      createChunks = true;
    }


    Vector3 mapOffset( int index )
    {
      return new Vector3( index/chunkMax, 0, index%chunkMax );
    }

    Dictionary<Vector3,Terrain> terrains = new Dictionary<Vector3,Terrain>();
    void Update ()
    {
      if (createChunks)
      {
      Stopwatch timer = new Stopwatch();

        mapChunkMeshing.callFaceCopyKernel();

        if (createUnityTerrain && !createSponge)
        {
          timer.Start();

        GameObject terrain = mapChunkMeshing.getTerrain();

          terrain.transform.SetParent( transform );
          terrain.transform.localPosition = transform.localPosition + mapOffset( offsetIndex )*4*256*size - new Vector3( 0, 256*6*size, 0 );

        Terrain t = terrain.GetComponent<Terrain>();

          t.drawHeightmap = true;
          t.enabled = true;
          t.ApplyDelayedHeightmapModification();
          t.Flush();

          terrains.Add( mapOffset( offsetIndex ), t );

          timer.Stop();
          print( "terrain ms: " + timer.ElapsedMilliseconds );
        }


        if (createMesh)
        {
          timer.Start();

        Mesh[] m = mapChunkMeshing.getMeshes();

          for (int i=0; i<m.Length; i++)
          {
          GameObject mesh = (GameObject) Instantiate( meshPrefab );

            mesh.transform.SetParent( transform );
            mesh.transform.localPosition = transform.localPosition + mapOffset( offsetIndex )*2*256*size - new Vector3( 0, 256*2*size, 0 );
            mesh.GetComponent<MeshFilter>().mesh = m[i];
            mesh.transform.localScale = new Vector3( 2*size, 2*size, 2*size );
          }

          timer.Stop();
          print( "mesh ms: " + timer.ElapsedMilliseconds );
        }


        if (showVoxels)
        {
        GameObject go = (GameObject) Instantiate( displayPrefab );

          go.transform.SetParent( transform );
          go.transform.localPosition = transform.localPosition + mapOffset( offsetIndex )*256*size;

        MapDisplay md = go.GetComponent<MapDisplay>();

          md.initialize( geometryShader, color, size, sprite, mapChunkMeshing.takeSolidVoxels() );
        }
        else
        {
          mapChunkMeshing.releaseSolidVoxels();
        }


        if (++offsetIndex < chunkMax*chunkMax)
        {
          mapChunkCreation.callPerlinMapGenKernel( mapOffset( offsetIndex ));
          mapChunkMeshing.callFaceGenKernel();
          mapChunkMeshing.callFaceSumKernel();
        }
        else
        {
          createChunks = false;
          releaseTemporaryBuffers();

          if (createUnityTerrain && !createSponge)
          {
            foreach (Vector3 tpos in terrains.Keys)
            {
            Terrain t = terrains[tpos];
            Terrain left=null, top=null, right=null, bottom=null;

              terrains.TryGetValue( tpos - new Vector3( 1, 0, 0 ), out left );
              terrains.TryGetValue( tpos + new Vector3( 1, 0, 0 ), out right );
              terrains.TryGetValue( tpos - new Vector3( 0, 0, 1 ), out top );
              terrains.TryGetValue( tpos + new Vector3( 0, 0, 1 ), out bottom );

              t.SetNeighbors( left, top, right, bottom );
            }
          }
        }
      }
    }


    void releaseTemporaryBuffers()
    {
      mapChunkCreation.releaseTemporaryBuffers();
      mapChunkMeshing.releaseTemporaryBuffers();
    }


    public void releaseDisplayBuffers()
    {
      foreach( MapDisplay md in gameObject.GetComponentsInChildren<MapDisplay>( true ))
        md.releaseDisplayBuffers();
    }


    void LateUpdate()
    {
      cameraPlanes = GeometryUtility.CalculateFrustumPlanes( mainCamera );
    }
  }
}

