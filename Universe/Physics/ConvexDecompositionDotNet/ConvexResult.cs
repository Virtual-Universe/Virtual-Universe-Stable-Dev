﻿/*
 * Copyright (c) Contributors, http://virtual-planets.org/
 * See CONTRIBUTORS.TXT for a full list of copyright holders.
 * For an explanation of the license of each contributor and the content it 
 * covers please see the Licenses directory.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the Virtual Universe Project nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;

namespace Universe.Physics.ConvexDecompositionDotNet
{
	public class ConvexResult
	{
		public List<float3> HullVertices;
		public List<int> HullIndices;

		public float mHullVolume;
		// the volume of the convex hull.

		//public float[] OBBSides = new float[3]; // the width, height and breadth of the best fit OBB
		//public float[] OBBCenter = new float[3]; // the center of the OBB
		//public float[] OBBOrientation = new float[4]; // the quaternion rotation of the OBB.
		//public float[] OBBTransform = new float[16]; // the 4x4 transform of the OBB.
		//public float OBBVolume; // the volume of the OBB

		//public float SphereRadius; // radius and center of best fit sphere
		//public float[] SphereCenter = new float[3];
		//public float SphereVolume; // volume of the best fit sphere

		public ConvexResult ()
		{
			HullVertices = new List<float3> ();
			HullIndices = new List<int> ();
		}

		public ConvexResult (List<float3> hvertices, List<int> hindices)
		{
			HullVertices = hvertices;
			HullIndices = hindices;
		}

		public ConvexResult (ConvexResult r)
		{
			HullVertices = new List<float3> (r.HullVertices);
			HullIndices = new List<int> (r.HullIndices);
		}

		public void Dispose ()
		{
			HullVertices = null;
			HullIndices = null;
		}
	}
}