using System;
using System.Collections.Generic;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;

namespace SB3Utility
{
	public interface IRenderObject
	{
		BoundingBox Bounds { get; }
		AnimationController AnimationController { get; }

		void Render();
		void ResetPose();
	}
}
