// Based on code from:
//  http://www.thehazymind.com/3DEngine.htm
//  http://www.c-unit.com/tutorials/mdirectx/

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D9;

namespace SB3Utility
{
	public class AnimationRenderer : Renderer, IRenderer
	{
		public event EventHandler RenderObjectAdded;

		private Dictionary<int, AnimationSet> animationSets = new Dictionary<int, AnimationSet>();
		private List<int> animationFreeIds = new List<int>();

		public AnimationRenderer(Control renderControl)
			: base(renderControl)
		{
		}

		public AnimationSet GetAnimationSet(int id)
		{
			AnimationSet set;
			animationSets.TryGetValue(id, out set);
			return set;
		}

		public override int AddRenderObject(IRenderObject renderObject)
		{
			int id = base.AddRenderObject(renderObject);
			foreach (var pair in animationSets)
			{
				RegisterAnimationSet(pair.Value, pair.Key, renderObject);
			}

			var handler = RenderObjectAdded;
			if (handler != null)
			{
				handler(this, new EventArgs());
			}

			Render();
			return id;
		}

		public override void RemoveRenderObject(int id)
		{
			IRenderObject renderObject;
			if (renderObjects.TryGetValue(id, out renderObject))
			{
				foreach (var pair in animationSets)
				{
					renderObject.AnimationController.UnregisterAnimationSet(pair.Value);
				}

				base.RemoveRenderObject(id);
			}
		}

		public int AddAnimationSet(AnimationSet animationSet)
		{
			int id;
			if (animationFreeIds.Count > 0)
			{
				id = animationFreeIds[0];
				animationFreeIds.RemoveAt(0);
			}
			else
			{
				id = animationSets.Count;
			}

			animationSets.Add(id, animationSet);

			foreach (var pair in renderObjects)
			{
				RegisterAnimationSet(animationSet, id, pair.Value);
			}

			Render();
			return id;
		}

		private void RegisterAnimationSet(AnimationSet animationSet, int id, IRenderObject renderObject)
		{
			AnimationController animationController = renderObject.AnimationController;
			animationController.RegisterAnimationSet(animationSet);
			animationController.SetTrackAnimationSet(id, animationSet);
			animationController.SetTrackWeight(id, 1);
			animationController.SetTrackSpeed(id, 0);
			animationController.SetTrackPosition(id, 0);
			animationController.SetTrackPriority(id, TrackPriority.High);
		}

		public void RemoveAnimationSet(int id)
		{
			foreach (var pair in renderObjects)
			{
				AnimationController animationController = pair.Value.AnimationController;
				animationController.EnableTrack(id);
				animationController.SetTrackPosition(id, 0);
				animationController.AdvanceTime(0, null);
				animationController.DisableTrack(id);
				animationController.UnregisterAnimationSet(animationSets[id]);
			}
			animationSets.Remove(id);
			animationFreeIds.Add(id);

			Render();
		}

		public void EnableTrack(int id)
		{
			foreach (var pair in renderObjects)
			{
				AnimationController animationController = pair.Value.AnimationController;
				animationController.EnableTrack(id);
			}

			Render();
		}

		public void DisableTrack(int id)
		{
			foreach (var pair in renderObjects)
			{
				AnimationController animationController = pair.Value.AnimationController;
				animationController.DisableTrack(id);
			}

			Render();
		}

		public void SetTrackPosition(int id, double position)
		{
			foreach (var pair in renderObjects)
			{
				AnimationController animationController = pair.Value.AnimationController;
				animationController.SetTrackPosition(id, position);
			}

			Render();
		}

		public void AdvanceTime(int id, double time, AnimationCallback handler)
		{
			foreach (var pair in renderObjects)
			{
				AnimationController animationController = pair.Value.AnimationController;
				animationController.SetTrackSpeed(id, 1);		
				animationController.AdvanceTime(time, handler);
				animationController.SetTrackSpeed(id, 0);
			}

			Render();
		}

		public double GetTime()
		{
			double time = 0;
			foreach (var pair in renderObjects)
			{
				AnimationController animationController = pair.Value.AnimationController;
				time = animationController.Time;
				break;
			}
			return time;
		}
	}
}
