Thank you for trying Voxel Performance for Unity. Voxel Performance is a
simple, high-performance asset that may be used to create and manipulate
voxel based terrain or other large voxel structures. This asset uses a
combination of compute shaders and a geometry shader to create map chunks
with Perlin Noise and display them quickly. Chunks may also be extracted
from the compute shaders as an array of raw voxel data, an array of Unity Mesh,
or a Unity TerrainData.

Voxel Performance is Copyright 2016 Charles Griffiths, All Rights Reserved.
This asset is licensed to you under the terms of the Apache License 2.0 which may
be found at http://www.apache.org/licenses/LICENSE-2.0


Grateful acknowledgement and credit is given to the following persons or legal entities:


For the inspiration to create this asset, and for freely sharing his video and demo linking a compute and geometry shader
 Charles Humphrey of Randomchaos Ltd

See more of Humphrey's work at

https://www.facebook.com/groups/CharlesWillCodeIt/
http://www.twitter.com/#!/NemoKrad
http://randomchaosdx11adventures.blogspot.co.uk/
http://xnauk-randomchaosblogarchive.blogspot.co.uk/
https://plus.google.com/u/0/113265422666328219332
https://plus.google.com/103026455724142455023
http://randomchaosxnaadventures.blogspot.co.uk/
http://www.gamedev-uk.net/

Also see his video tutorials at https://www.youtube.com/user/NemoKradXNA/videos
 particularly https://www.youtube.com/watch?v=YP0_aA_wKfU
 and Unity Store Assets at https://www.assetstore.unity3d.com/en/#!/search/page=1/sortby=popularity/query=publisher:17807



For Perlin Noise and related functions included in the file PerlinGeneration.compute
 Brian Sharpe of http://briansharpe.wordpress.com and https://github.com/BrianSharpe
 Giliam de Carpentier of Scape Software

 I changed the provided code in minor ways, original source available on github and
 a derivative on the Unity Asset Store at https://www.assetstore.unity3d.com/en/#!/content/3957


--- Licensing For Perlin Noise ---

//	Code repository for GPU noise development blog
//	http://briansharpe.wordpress.com
//	https://github.com/BrianSharpe
//
//	I'm not one for copyrights.  Use the code however you wish.
//	All I ask is that credit be given back to the blog or myself when appropriate.
//	And also to let me know if you come up with any changes, improvements, thoughts or interesting uses for this stuff. :)
//	Thanks!
//
//	Brian Sharpe
//	brisharpe CIRCLE_A yahoo DOT com
//	http://briansharpe.wordpress.com
//	https://github.com/BrianSharpe
//
//===============================================================================
//  Scape Software License
//===============================================================================
//
//Copyright (c) 2007-2012, Giliam de Carpentier
//All rights reserved.
//
//Redistribution and use in source and binary forms, with or without
//modification, are permitted provided that the following conditions are met: 
//
//1. Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer. 
//2. Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution. 
//
//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNERS OR CONTRIBUTORS BE LIABLE 
//FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
//DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
//SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
//CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
//OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
//OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.;
