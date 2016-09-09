// VoxelPerformance/Shaders/VoxelGeometry.shader
// Copyright 2016 Charles Griffiths

Shader "VoxelPerformance/VoxelGeometryShader" 
{
  Properties
  {
    _Sprite( "Sprite", 2D ) = "white" {}
    _Color( "Color", Color ) = ( 1, 1, 1, 1 )
    _Color1( "Color1", Color ) = ( 1, 1, 1, 1 )
    _Color2( "Color2", Color ) = ( 1, 1, 1, 1 )
    _Color3( "Color3", Color ) = ( 1, 1, 1, 1 )
    _Color4( "Color4", Color ) = ( 1, 1, 1, 1 )
    _Color5( "Color5", Color ) = ( 1, 1, 1, 1 )
    _Color6( "Color6", Color ) = ( 1, 1, 1, 1 )
    _Size( "Size", float ) = 1
  }


  SubShader
  {
    Tags{ "Queue" = "Geometry" "RenderType" = "Opaque" }

    Pass
    {
    CGPROGRAM
    #pragma target 5.0

    #pragma vertex vert
    #pragma geometry geom
    #pragma fragment frag

    #include "UnityCG.cginc"

    sampler2D _Sprite;
    float4 _Color = float4( 1, 1, 1, 1 );
    float4 _Color1 = float4( 1, 1, 1, 1 );
    float4 _Color2 = float4( 1, 1, 1, 1 );
    float4 _Color3 = float4( 1, 1, 1, 1 );
    float4 _Color4 = float4( 1, 1, 1, 1 );
    float4 _Color5 = float4( 1, 1, 1, 1 );
    float4 _Color6 = float4( 1, 1, 1, 1 );
    float _Size = 1;

    float4 _cameraPosition, _chunkPosition;

    matrix _worldMatrixTransform;

      struct data
      {
        uint voxel;
      };

    StructuredBuffer<data> _displayPoints;

      struct input
      {
        float4 pos : SV_POSITION;
        float4 _color : COLOR;
        float2 uv : TEXCOORD0;
      };

      struct inputGS
      {
        float4 pos : SV_POSITION;
        float4 _color : COLOR;
      };


      /// The four bytes in voxel are: color id, x, y, z
      inputGS vert( uint id : SV_VertexID )
      {
      inputGS o;
      uint pos = _displayPoints[id].voxel;

        o.pos = float4( pos/65536 % 256, pos/256 % 256, pos % 256, 1.0f );

      uint voxelColor = pos/16777216 % 256;

        if (1 == voxelColor) o._color = _Color1;
        else if (2 == voxelColor) o._color = _Color2;
        else if (3 == voxelColor) o._color = _Color3;
        else if (4 == voxelColor) o._color = _Color4;
        else if (5 == voxelColor) o._color = _Color5;
        else if (6 == voxelColor) o._color = _Color6;
        else o._color = _Color;

        return o;
      }

      // For each voxel that is visible from some angle, paint the three sides
      // that the given camera might see.
      [maxvertexcount(12)]
      void geom( point inputGS p[1], inout TriangleStream<input> triStream )
      {
      float4 pos = p[0].pos * float4( _Size, _Size, _Size, 1 );
      float4 shift;
      float4 voxelPosition = pos + _chunkPosition;
      float halfS = _Size * 0.5;  // x, y, z is the center of the voxel, paint sides offset by half of Size
      input pIn1, pIn2, pIn3, pIn4;

        pIn1._color = p[0]._color;
        pIn1.uv = float2( 0.0f, 0.0f );

        pIn2._color = p[0]._color;
        pIn2.uv = float2( 0.0f, 1.0f );

        pIn3._color = p[0]._color;
        pIn3.uv = float2( 1.0f, 0.0f );

        pIn4._color = p[0]._color;
        pIn4.uv = float2( 1.0f, 1.0f );


        shift = (_cameraPosition.x < voxelPosition.x)?float4( 1, 1, 1, 1 ):float4( -1, 1, -1, 1 );

        pIn1.pos = mul( UNITY_MATRIX_VP, mul( _worldMatrixTransform, pos + shift*float4( -halfS, -halfS, halfS, 0 ) ));
        triStream.Append( pIn1 );

        pIn2.pos = mul( UNITY_MATRIX_VP, mul( _worldMatrixTransform, pos + shift*float4( -halfS, halfS, halfS, 0 ) ));
        triStream.Append( pIn2 );

        pIn3.pos = mul( UNITY_MATRIX_VP, mul( _worldMatrixTransform, pos + shift*float4( -halfS, -halfS, -halfS, 0 ) ));
        triStream.Append( pIn3 );

        pIn4.pos = mul( UNITY_MATRIX_VP, mul( _worldMatrixTransform, pos + shift*float4( -halfS, halfS, -halfS, 0 ) ));
        triStream.Append( pIn4 );

        triStream.RestartStrip();


        shift = (_cameraPosition.y < voxelPosition.y)?float4( 1, 1, 1, 1 ):float4( 1, -1, -1, 1 );

        pIn1.pos = mul( UNITY_MATRIX_VP, mul( _worldMatrixTransform, pos + shift*float4( -halfS, -halfS, halfS, 0 ) ));
        triStream.Append( pIn1 );

        pIn2.pos = mul( UNITY_MATRIX_VP, mul( _worldMatrixTransform, pos + shift*float4( -halfS, -halfS, -halfS, 0 ) ));
        triStream.Append( pIn2 );

        pIn3.pos = mul( UNITY_MATRIX_VP, mul( _worldMatrixTransform, pos + shift*float4( halfS, -halfS, halfS, 0 ) ));
        triStream.Append( pIn3 );

        pIn4.pos = mul( UNITY_MATRIX_VP, mul( _worldMatrixTransform, pos + shift*float4( halfS, -halfS, -halfS, 0 ) ));
        triStream.Append( pIn4 );

        triStream.RestartStrip();


        shift = (_cameraPosition.z < voxelPosition.z)?float4( 1, 1, 1, 1 ):float4( -1, 1, -1, 1 );

        pIn1.pos = mul( UNITY_MATRIX_VP, mul( _worldMatrixTransform, pos + shift*float4( -halfS, -halfS, -halfS, 0 ) ));
        triStream.Append( pIn1 );

        pIn2.pos = mul( UNITY_MATRIX_VP, mul( _worldMatrixTransform, pos + shift*float4( -halfS, halfS, -halfS, 0 ) ));
        triStream.Append( pIn2 );

        pIn3.pos = mul( UNITY_MATRIX_VP, mul( _worldMatrixTransform, pos + shift*float4( halfS, -halfS, -halfS, 0 ) ));
        triStream.Append( pIn3 );

        pIn4.pos = mul( UNITY_MATRIX_VP, mul( _worldMatrixTransform, pos + shift*float4( halfS, halfS, -halfS, 0 ) ));
        triStream.Append( pIn4 );

        triStream.RestartStrip();
      }


      float4 frag( input i ) : COLOR
      {
        return tex2D( _Sprite, i.uv ) * i._color;
      }

    ENDCG
    }
  }

  Fallback Off
}

