using UnityEngine;
using System.Collections;

// VoxelPerformance/Scripts/VoxelMapData.cs
// Copyright 2016 Charles Griffiths

namespace VoxelPerformance
{
  // This class is a C# interface to VoxelPerformance/Shaders/PerlinGeneration.compute
  public class VoxelMapData 
  {
  public ComputeBuffer MapVoxels { get; private set; }
  public ComputeBuffer MapHeights { get; private set; }

  ComputeShader perlinGen;
  int PerlinMapGenKernel;
  Vector3 mapOffset;


    public VoxelMapData( ComputeShader shader )
    {
      perlinGen = shader;
      setMapOffset( Vector3.zero );

      initKernels();
      initTemporaryBuffers();
    }


    void initKernels()
    {
      PerlinMapGenKernel = perlinGen.FindKernel( "PerlinMapGen" );
    }


    void initTemporaryBuffers()
    {
      MapVoxels = new ComputeBuffer( 64 * 256 * 256, sizeof(int) );  // each int stores four voxels
      MapHeights = new ComputeBuffer( 256 * 256, sizeof(int) );

      perlinGen.SetBuffer( PerlinMapGenKernel, "MapVoxels", MapVoxels );
      perlinGen.SetBuffer( PerlinMapGenKernel, "MapHeights", MapHeights );
    }


    public int[] getVoxels()
    {
    int[] buffer = new int[64 * 256 * 256];

      MapVoxels.GetData( buffer );

      return buffer;
    }


    public int[] getHeights()
    {
    int[] buffer = new int[256 * 256];

      MapHeights.GetData( buffer );

      return buffer;
    }


    public void releaseTemporaryBuffers()
    {
      MapVoxels.Release();
      MapVoxels = null;

      MapHeights.Release();
      MapHeights = null;
    }


    public void setMapOffset( Vector3 mapoffset )
    {
      this.mapOffset = mapoffset * 256;
      perlinGen.SetVector( "MapOffset", mapOffset );
    }


    public void callPerlinMapGenKernel( Vector3 mapoffset )
    {
      MonoBehaviour.print( "offset: " + mapoffset );
      setMapOffset( mapoffset );
      callPerlinMapGenKernel();
    }


    public void callPerlinMapGenKernel()
    {
      perlinGen.Dispatch( PerlinMapGenKernel, 8, 1, 8 );
    }


    public void createSponge()
    {
    int[] voxels = new int[256*64*256];
    int[] heights = new int[256*256];
    int size = 3;

      for (int i=0; i<voxels.Length; i++) voxels[i] = 0;
      for (int i=0; i<heights.Length; i++) heights[i] = 255;

      while (size * 3 <= 256) size *= 3;

      putSponge( voxels, size, 0, 0, 0 );

      MapVoxels.SetData( voxels );
      MapHeights.SetData( heights );
    }


    // writes a Menger Sponge into voxels
    void putSponge( int[] voxels, int size, int x, int y, int z )
    {
      if (1==size)
      {
      int index = (z * 256 + x) * 64 + y/4;

        if (0 == y%4)
        {
          voxels[index] &= (int)0xffffff;
          voxels[index] |= (int)0x2000000;
        }
        if (1 == y%4)
        {
          voxels[index] &= (int)0x7f00ffff;
          voxels[index] |= (int)0x20000;
        }
        if (2 == y%4)
        {
          voxels[index] &= (int)0x7fff00ff;
          voxels[index] |= (int)0x200;
        }
        if (3 == y%4)
        {
          voxels[index] &= (int)0x7fffff00;
          voxels[index] |= (int)0x2;
        }
      }
      else
      {
        size = size/3;
        putSponge( voxels, size, x, y, z );
        putSponge( voxels, size, x+size, y, z );
        putSponge( voxels, size, x+2*size, y, z );
        putSponge( voxels, size, x, y+size, z );
        putSponge( voxels, size, x+2*size, y+size, z );
        putSponge( voxels, size, x, y+2*size, z );
        putSponge( voxels, size, x+size, y+2*size, z );
        putSponge( voxels, size, x+2*size, y+2*size, z );

        putSponge( voxels, size, x, y, z+size );
        putSponge( voxels, size, x+2*size, y, z+size );
        putSponge( voxels, size, x, y+2*size, z+size );
        putSponge( voxels, size, x+2*size, y+2*size, z+size );

        putSponge( voxels, size, x, y, z+2*size );
        putSponge( voxels, size, x+size, y, z+2*size );
        putSponge( voxels, size, x+2*size, y, z+2*size );
        putSponge( voxels, size, x, y+size, z+2*size );
        putSponge( voxels, size, x+2*size, y+size, z+2*size );
        putSponge( voxels, size, x, y+2*size, z+2*size );
        putSponge( voxels, size, x+size, y+2*size, z+2*size );
        putSponge( voxels, size, x+2*size, y+2*size, z+2*size );
      }
    }
  }
}

