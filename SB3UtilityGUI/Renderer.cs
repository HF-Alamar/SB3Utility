// Based on code from:
//  http://www.thehazymind.com/3DEngine.htm
//  http://www.c-unit.com/tutorials/mdirectx/

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using SlimDX;
using SlimDX.Direct3D9;

namespace SB3Utility
{
	public class Renderer : IDisposable
	{
		public static Material NullMaterial = new Material();

		public float Sensitivity
		{
			get { return camera.Sensitivity; }
			set { camera.Sensitivity = value; }
		}

		public Device Device { get; protected set; }

		bool showNormals;
		public bool ShowNormals
		{
			get { return showNormals; }
			set { Gui.Config["showNormals"] = showNormals = value; }
		}

		bool showBones;
		public bool ShowBones
		{
			get { return showBones; }
			set { Gui.Config["ShowBones"] = showBones = value; }
		}

		bool wireframe;
		public bool Wireframe
		{
			get { return wireframe; }
			set { Gui.Config["Wireframe"] = wireframe = value; }
		}

		bool culling;
		public bool Culling
		{
			get { return culling; }
			set { Gui.Config["Culling"] = culling = value; }
		}

		public Color Background { get; set; }

		public Color Diffuse
		{
			get
			{
				Light light = Device.GetLight(0);
				return light.Diffuse.ToColor();
			}
			set
			{
				Light light = Device.GetLight(0);
				light.Diffuse = new Color4(value);
				Device.SetLight(0, light);
				Gui.Config["LightDiffuseARGB"] = value.ToArgb().ToString("X8");
			}
		}

		public Color Ambient
		{
			get
			{
				Light light = Device.GetLight(0);
				return light.Ambient.ToColor();
			}
			set
			{
				Light light = Device.GetLight(0);
				light.Ambient = new Color4(value);
				Device.SetLight(0, light);
				Gui.Config["LightAmbientARGB"] = value.ToArgb().ToString("X8");
			}
		}

		public Color Specular
		{
			get
			{
				Light light = Device.GetLight(0);
				return light.Specular.ToColor();
			}
			set
			{
				Light light = Device.GetLight(0);
				light.Specular = new Color4(value);
				Device.SetLight(0, light);
				Gui.Config["LightSpecularARGB"] = value.ToArgb().ToString("X8");
			}
		}

		protected Camera camera = null;
		protected bool isInitialized = false;
		protected bool isRendering = false;
		protected Point lastMousePos = new Point();
		protected MouseButtons mouseDown = MouseButtons.None;

		Mesh CursorMesh;
		Material CursorMaterial;
		SlimDX.Direct3D9.Font TextFont;
		Color4 TextColor;

		Control renderControl;
		SwapChain swapChain;
		Rectangle renderRect;

		protected Dictionary<int, IRenderObject> renderObjects = new Dictionary<int, IRenderObject>();
		protected List<int> renderObjectFreeIds = new List<int>();

		public Control RenderControl
		{
			get { return renderControl; }

			set
			{
				if ((renderControl != null) && !renderControl.IsDisposed)
				{
					UnregisterControlEvents();
				}

				renderControl = value;
				if ((renderControl != null) && !renderControl.IsDisposed)
				{
					CreateSwapChain();
					RegisterControlEvents();
				}

				camera.RenderControl = value;
				Render();
			}
		}

		public bool IsInitialized
		{
			get { return this.isInitialized; }
		}

		public Renderer(Control control)
		{
			PresentParameters presentParams = new PresentParameters();
			presentParams.Windowed = true;
			presentParams.BackBufferCount = 0;
			presentParams.BackBufferWidth = Screen.PrimaryScreen.WorkingArea.Width;
			presentParams.BackBufferHeight = Screen.PrimaryScreen.WorkingArea.Height;
			Device = new Device(new Direct3D(), 0, DeviceType.Hardware, control.Handle, CreateFlags.SoftwareVertexProcessing, presentParams);

			camera = new Camera(control);
			RenderControl = control;
			
			Device.SetRenderState(RenderState.Lighting, true);
			Device.SetRenderState(RenderState.DiffuseMaterialSource, ColorSource.Material);
			Device.SetRenderState(RenderState.EmissiveMaterialSource, ColorSource.Material);
			Device.SetRenderState(RenderState.SpecularMaterialSource, ColorSource.Material);
			Device.SetRenderState(RenderState.SpecularEnable, true);
			Device.SetRenderState(RenderState.AlphaBlendEnable, true);
			Device.SetRenderState(RenderState.BlendOperationAlpha, BlendOperation.Add);
			Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
			Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);

			Light light = new Light();
			light.Type = LightType.Directional;
			light.Ambient = new Color4(int.Parse((string)Gui.Config["LightAmbientARGB"], System.Globalization.NumberStyles.AllowHexSpecifier));
			light.Diffuse = new Color4(int.Parse((string)Gui.Config["LightDiffuseARGB"], System.Globalization.NumberStyles.AllowHexSpecifier));
			light.Specular = new Color4(int.Parse((string)Gui.Config["LightSpecularARGB"], System.Globalization.NumberStyles.AllowHexSpecifier));
			Device.SetLight(0, light);
			Device.EnableLight(0, true);

			TextFont = new SlimDX.Direct3D9.Font(Device, new System.Drawing.Font("Arial", 8));
			TextColor = new Color4(Color.White);

			CursorMesh = Mesh.CreateSphere(Device, 1, 10, 10);
			CursorMaterial = new Material();
			CursorMaterial.Ambient = new Color4(1, 1f, 1f, 1f);
			CursorMaterial.Diffuse = new Color4(1, 0.6f, 1, 0.3f);

			showNormals = (bool)Gui.Config["ShowNormals"];
			showBones = (bool)Gui.Config["ShowBones"];
			wireframe = (bool)Gui.Config["Wireframe"];
			culling = (bool)Gui.Config["Culling"];
			Background = Color.FromArgb(255, 10, 10, 60);

			isInitialized = true;
			Render();
		}

		~Renderer()
		{
			Dispose();
		}

		public void Dispose()
		{
			isInitialized = false;

			if ((renderControl != null) && !renderControl.IsDisposed)
			{
				UnregisterControlEvents();
				renderControl = null;
			}

			if (swapChain != null)
			{
				swapChain.Dispose();
				swapChain = null;
			}

			if (TextFont != null)
			{
				TextFont.Dispose();
				TextFont = null;
			}

			if (CursorMesh != null)
			{
				CursorMesh.Dispose();
				CursorMesh = null;
			}

			if (Device != null)
			{
				Device.Direct3D.Dispose();
				Device.Dispose();
				Device = null;
			}
		}

		void RegisterControlEvents()
		{
			renderControl.Disposed += new EventHandler(renderControl_Disposed);
			renderControl.Resize += new EventHandler(renderControl_Resize);
			renderControl.VisibleChanged += new EventHandler(renderControl_VisibleChanged);
			renderControl.Paint += new PaintEventHandler(renderControl_Paint);
			renderControl.MouseWheel += new MouseEventHandler(renderControl_MouseWheel);
			renderControl.MouseDown += new MouseEventHandler(renderControl_MouseDown);
			renderControl.MouseMove += new MouseEventHandler(renderControl_MouseMove);
			renderControl.MouseUp += new MouseEventHandler(renderControl_MouseUp);
			renderControl.MouseHover += new EventHandler(renderControl_MouseHover);
		}

		void UnregisterControlEvents()
		{
			renderControl.Disposed -= new EventHandler(renderControl_Disposed);
			renderControl.Resize -= new EventHandler(renderControl_Resize);
			renderControl.VisibleChanged -= new EventHandler(renderControl_VisibleChanged);
			renderControl.Paint -= new PaintEventHandler(renderControl_Paint);
			renderControl.MouseWheel -= new MouseEventHandler(renderControl_MouseWheel);
			renderControl.MouseDown -= new MouseEventHandler(renderControl_MouseDown);
			renderControl.MouseMove -= new MouseEventHandler(renderControl_MouseMove);
			renderControl.MouseUp -= new MouseEventHandler(renderControl_MouseUp);
			renderControl.MouseHover -= new EventHandler(renderControl_MouseHover);
		}

		void renderControl_Disposed(object sender, EventArgs e)
		{
			isInitialized = false;
			renderControl = null;
		}

		void renderControl_Resize(object sender, EventArgs e)
		{
			CreateSwapChain();
			Render();
		}

		void renderControl_VisibleChanged(object sender, EventArgs e)
		{
			CreateSwapChain();
			Render();
		}

		void CreateSwapChain()
		{
			if (swapChain != null)
			{
				swapChain.Dispose();
				swapChain = null;
			}

			if ((renderControl.Width > 0) && (renderControl.Height > 0) && renderControl.Visible)
			{
				PresentParameters presentParams = new PresentParameters();
				presentParams.Windowed = true;
				presentParams.SwapEffect = SwapEffect.Discard;
				presentParams.BackBufferCount = 1;
				presentParams.BackBufferWidth = renderControl.Width;
				presentParams.BackBufferHeight = renderControl.Height;
				swapChain = new SwapChain(Device, presentParams);
				renderRect = new Rectangle(0, 0, renderControl.Width, renderControl.Height);
			}
		}

		public IRenderObject GetRenderObject(int id)
		{
			IRenderObject renderObj;
			renderObjects.TryGetValue(id, out renderObj);
			return renderObj;
		}

		public virtual int AddRenderObject(IRenderObject renderObject)
		{
			int id;
			if (renderObjectFreeIds.Count > 0)
			{
				id = renderObjectFreeIds[0];
				renderObjectFreeIds.RemoveAt(0);
			}
			else
			{
				id = renderObjects.Count;
			}

			renderObjects.Add(id, renderObject);

			Render();
			return id;
		}

		public virtual void RemoveRenderObject(int id)
		{
			renderObjects.Remove(id);
			renderObjectFreeIds.Add(id);
			Render();
		}

		public void ResetPose()
		{
			foreach (var pair in renderObjects)
			{
				pair.Value.ResetPose();
			}
			Render();
		}

		public void CenterView()
		{
			BoundingBox bounds = new BoundingBox();
			bool first = true;
			foreach (var pair in renderObjects)
			{
				if (first)
				{
					bounds = pair.Value.Bounds;
					first = false;
				}
				else
				{
					bounds = BoundingBox.Merge(bounds, pair.Value.Bounds);
				}
			}

			Vector3 center = (bounds.Minimum + bounds.Maximum) / 2;
			float radius = Math.Max(bounds.Maximum.X - bounds.Minimum.X, bounds.Maximum.Y - bounds.Minimum.Y);
			radius = Math.Max(radius, bounds.Maximum.Z - bounds.Minimum.Z) * 1.7f;
			camera.radius = radius;
			camera.SetTarget(center);
			Render();
		}

		protected void renderControl_Paint(object sender, PaintEventArgs e)
		{
			Render();
		}

		public virtual void Render()
		{
			try
			{
				if (isInitialized && !isRendering && (swapChain != null))
				{
					isRendering = true;
					using (Surface surface = swapChain.GetBackBuffer(0))
					{
						Device.SetRenderTarget(0, surface);
					}

					Light light = this.Device.GetLight(0);
					light.Direction = camera.Direction;
					Device.SetLight(0, light);

					Device.SetTransform(TransformState.View, camera.View);
					Device.SetTransform(TransformState.Projection, camera.Projection);

					Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Background, 1.0f, 0);
					Device.BeginScene();

					foreach (var pair in renderObjects)
					{
						pair.Value.Render();
					}

					if (mouseDown != MouseButtons.None)
					{
						DrawCursor();
						DrawAxes();

						string camStr = "(" + camera.target.X.ToString("0.##") + ", " + camera.target.Y.ToString("0.##") + ", " + camera.target.Z.ToString("0.##") + ")";
						TextFont.DrawString(null, camStr, renderRect, DrawTextFormat.Right | DrawTextFormat.Top, TextColor);
					}

					Device.EndScene();
					swapChain.Present(Present.None, renderRect, renderRect, renderControl.Handle);

					isRendering = false;
				}
			}
			catch (Exception)
			{
				swapChain.Dispose();
				swapChain = null;
				TextFont.Dispose();
				TextFont = null;
				CursorMesh.Dispose();
				CursorMesh = null;

				PresentParameters presentParams = new PresentParameters();
				presentParams.Windowed = true;
				presentParams.BackBufferCount = 0;
				presentParams.BackBufferWidth = Screen.PrimaryScreen.WorkingArea.Width;
				presentParams.BackBufferHeight = Screen.PrimaryScreen.WorkingArea.Height;
				Device.Reset(new PresentParameters[] { presentParams });

				RenderControl = renderControl;

				Device.SetRenderState(RenderState.Lighting, true);
				Device.SetRenderState(RenderState.DiffuseMaterialSource, ColorSource.Material);
				Device.SetRenderState(RenderState.EmissiveMaterialSource, ColorSource.Material);
				Device.SetRenderState(RenderState.SpecularMaterialSource, ColorSource.Material);
				Device.SetRenderState(RenderState.SpecularEnable, true);
				Device.SetRenderState(RenderState.AlphaBlendEnable, true);
				Device.SetRenderState(RenderState.BlendOperationAlpha, BlendOperation.Add);
				Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
				Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);

				Light light = new Light();
				light.Type = LightType.Directional;
				light.Ambient = new Color4(int.Parse((string)Gui.Config["LightAmbientARGB"], System.Globalization.NumberStyles.AllowHexSpecifier));
				light.Diffuse = new Color4(int.Parse((string)Gui.Config["LightDiffuseARGB"], System.Globalization.NumberStyles.AllowHexSpecifier));
				light.Specular = new Color4(int.Parse((string)Gui.Config["LightSpecularARGB"], System.Globalization.NumberStyles.AllowHexSpecifier));
				Device.SetLight(0, light);
				Device.EnableLight(0, true);

				TextFont = new SlimDX.Direct3D9.Font(Device, new System.Drawing.Font("Arial", 8));
				TextColor = new Color4(Color.White);

				CursorMesh = Mesh.CreateSphere(Device, 1, 10, 10);
				CursorMaterial = new Material();
				CursorMaterial.Ambient = new Color4(1, 1f, 1f, 1f);
				CursorMaterial.Diffuse = new Color4(1, 0.6f, 1, 0.3f);

				Culling = true;
				Background = Color.FromArgb(255, 10, 10, 60);

				isInitialized = true;
				isRendering = false;
			}
		}

		void DrawCursor()
		{
			Device.SetRenderState(RenderState.AlphaBlendEnable, true);
			Device.SetRenderState(RenderState.BlendOperationAlpha, BlendOperation.Add);

// very colourful
			Device.SetRenderState(RenderState.SourceBlend, Blend.InverseSourceColor);
			Device.SetRenderState(RenderState.DestinationBlend, Blend.DestinationColor);

// a bit dark
/*
			Device.SetRenderState(RenderState.SourceBlend, Blend.DestinationColor);
			Device.SetRenderState(RenderState.DestinationBlend, Blend.DestinationColor);
 */

			Device.SetRenderState(RenderState.ZEnable, ZBufferType.DontUseZBuffer);
			Device.SetRenderState(RenderState.ZWriteEnable, false);

			Device.SetRenderState(RenderState.VertexBlend, VertexBlend.Disable);
			Device.SetRenderState(RenderState.Lighting, true);
			Device.SetRenderState(RenderState.FillMode, FillMode.Solid);
			Device.SetRenderState(RenderState.CullMode, Cull.None);
			Device.SetTransform(TransformState.World, Matrix.Translation(camera.target));

			Light light = Device.GetLight(0);
			Color4 ambient = light.Ambient;
			Color4 diffuse = light.Diffuse;

			light.Ambient = new Color4(1, 0.3f, 0.3f, 0.3f);
			light.Diffuse = new Color4(1, 0.8f, 0.8f, 0.8f);
			Device.SetLight(0, light);

			Device.Material = CursorMaterial;
			Device.SetTexture(0, null);
			CursorMesh.DrawSubset(0);

			light.Ambient = ambient;
			light.Diffuse = diffuse;
			Device.SetLight(0, light);

			Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
			Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
			Device.SetRenderState(RenderState.ZEnable, ZBufferType.UseZBuffer);
			Device.SetRenderState(RenderState.ZWriteEnable, true);
		}

		void DrawAxes()
		{
			PositionColored[] xAxis = new PositionColored[2] {
				new PositionColored(new Vector3(0), Color.Red.ToArgb()),
				new PositionColored(new Vector3(10, 0, 0), Color.Red.ToArgb()) };
			PositionColored[] yAxis = new PositionColored[2] {
				new PositionColored(new Vector3(0), Color.Green.ToArgb()),
				new PositionColored(new Vector3(0, 10, 0), Color.Green.ToArgb()) };
			PositionColored[] zAxis = new PositionColored[2] {
				new PositionColored(new Vector3(0), Color.Blue.ToArgb()),
				new PositionColored(new Vector3(0, 0, 10), Color.Blue.ToArgb()) };

			Device.SetRenderState(RenderState.ZEnable, ZBufferType.UseZBuffer);
			Device.SetRenderState(RenderState.VertexBlend, VertexBlend.Disable);
			Device.SetRenderState(RenderState.Lighting, false);
			Device.SetTransform(TransformState.World, Matrix.Identity);
			Device.VertexFormat = PositionColored.Format;
			Device.Material = Renderer.NullMaterial;
			Device.SetTexture(0, null);

			Device.DrawUserPrimitives(PrimitiveType.LineList, 1, xAxis);
			Device.DrawUserPrimitives(PrimitiveType.LineList, 1, yAxis);
			Device.DrawUserPrimitives(PrimitiveType.LineList, 1, zAxis);
		}

		private void renderControl_MouseDown(object sender, MouseEventArgs e)
		{
			mouseDown |= e.Button;
			lastMousePos = new Point(e.X, e.Y);
			Render();
		}

		private void renderControl_MouseUp(object sender, MouseEventArgs e)
		{
			mouseDown &= ~e.Button;
			Render();
		}

		private void renderControl_MouseMove(object sender, MouseEventArgs e)
		{
			MouseButtons left = mouseDown & MouseButtons.Left;
			MouseButtons right = mouseDown & MouseButtons.Right;
			MouseButtons middle = mouseDown & MouseButtons.Middle;

			if (((left != MouseButtons.None) && (right != MouseButtons.None)) || (middle != MouseButtons.None))
			{
				camera.TranslateInOut((float)((e.X - lastMousePos.X) + (lastMousePos.Y - e.Y)));
				lastMousePos = new Point(e.X, e.Y);
				Render();
			}
			else if (left != MouseButtons.None)
			{
				camera.Rotate((float)(e.X - lastMousePos.X), (float)(e.Y - lastMousePos.Y));
				lastMousePos = new Point(e.X, e.Y);
				Render();
			}
			else if (right != MouseButtons.None)
			{
				camera.Translate((float)(e.X - lastMousePos.X), (float)(e.Y - lastMousePos.Y));
				lastMousePos = new Point(e.X, e.Y);
				Render();
			}
		}

		private void renderControl_MouseWheel(object sender, MouseEventArgs e)
		{
			camera.Zoom(e.Delta);
			Render();
		}

		private void renderControl_MouseHover(object sender, EventArgs e)
		{
			renderControl.Focus();
		}

		public static Plane[] BuildViewFrustum(Matrix view, Matrix projection)
		{
			Plane[] frustum = new Plane[6];
			Matrix viewProj = Matrix.Multiply(view, projection);

			// Left plane 
			frustum[0].Normal.X = viewProj.M14 + viewProj.M11;
			frustum[0].Normal.Y = viewProj.M24 + viewProj.M21;
			frustum[0].Normal.Z = viewProj.M34 + viewProj.M31;
			frustum[0].D = viewProj.M44 + viewProj.M41;

			// Right plane 
			frustum[1].Normal.X = viewProj.M14 - viewProj.M11;
			frustum[1].Normal.Y = viewProj.M24 - viewProj.M21;
			frustum[1].Normal.Z = viewProj.M34 - viewProj.M31;
			frustum[1].D = viewProj.M44 - viewProj.M41;

			// Top plane 
			frustum[2].Normal.X = viewProj.M14 - viewProj.M12;
			frustum[2].Normal.Y = viewProj.M24 - viewProj.M22;
			frustum[2].Normal.Z = viewProj.M34 - viewProj.M32;
			frustum[2].D = viewProj.M44 - viewProj.M42;

			// Bottom plane 
			frustum[3].Normal.X = viewProj.M14 + viewProj.M12;
			frustum[3].Normal.Y = viewProj.M24 + viewProj.M22;
			frustum[3].Normal.Z = viewProj.M34 + viewProj.M32;
			frustum[3].D = viewProj.M44 + viewProj.M42;

			// Near plane 
			frustum[4].Normal.X = viewProj.M13;
			frustum[4].Normal.Y = viewProj.M23;
			frustum[4].Normal.Z = viewProj.M33;
			frustum[4].D = viewProj.M43;

			// Far plane 
			frustum[5].Normal.X = viewProj.M14 - viewProj.M13;
			frustum[5].Normal.Y = viewProj.M24 - viewProj.M23;
			frustum[5].Normal.Z = viewProj.M34 - viewProj.M33;
			frustum[5].D = viewProj.M44 - viewProj.M43;

			// Normalize planes 
			for (int i = 0; i < 6; i++)
			{
				frustum[i] = Plane.Normalize(frustum[i]);
			}

			return frustum;
		}
	}

	public class Camera
	{
		public float radius = 30.0f;
		public float hRotation = (float)(Math.PI / 2);
		public float vRotation = 0;

		public Vector3 position = new Vector3(0, 0, 0);
		public Vector3 target = new Vector3(0, 0, 0);
		private Vector3 upVector = new Vector3(0, 1, 0);

		float sensitivity;
		public float Sensitivity
		{
			get { return sensitivity; }
			set { Gui.Config["Sensitivity"] = sensitivity = value; }
		}

		private float nearClip = 0.01f;
		private float farClip = 100000f;
		private float fov = (float)Math.PI / 4;

		public Control RenderControl { get; set; }

		public Matrix View { get { return Matrix.LookAtRH(position, target, upVector); } }
		public Matrix Projection { get { return Matrix.PerspectiveFovRH(fov, (float)RenderControl.Width / RenderControl.Height, nearClip, farClip); } }

		public Camera(Control renderControl)
		{
			RenderControl = renderControl;
			Sensitivity = (float)Gui.Config["Sensitivity"];
			UpdatePosition();
		}

		public Vector3 Direction
		{
			get { return Vector3.Subtract(target, position); }
		}

		public void SetTarget(Vector3 target)
		{
			this.target = target;
			vRotation = 0;
			hRotation = (float)(Math.PI / 2);
			UpdatePosition();
		}

		public void Zoom(float dist)
		{
			radius -= dist / 100;
			if (radius < nearClip)
			{
				radius = nearClip;
			}

			UpdatePosition();
		}

		public void Rotate(float h, float v)
		{
			hRotation += h * Sensitivity;
			vRotation += v * Sensitivity;

			UpdatePosition();
		}

		public void Translate(float h, float v)
		{
			h *= Sensitivity * 3;
			v *= Sensitivity * 3;

			Vector3 diff = Vector3.Subtract(position, target);
			Vector3 hVector = Vector3.Cross(diff, upVector);
			Vector3 vVector = Vector3.Cross(hVector, diff);
			hVector.Normalize();
			vVector.Normalize();
			hVector *= h;
			vVector *= v;
			target += hVector;
			target += vVector;

			UpdatePosition();
		}

		public void TranslateInOut(float dist)
		{
			target += Direction * dist * Sensitivity / 8;

			UpdatePosition();
		}

		public void UpdatePosition()
		{
			Vector3 oldPos = position;

			// (radius * Math.Cos(vRotation)) is the temporary radius after the y component shift
			position.X = (float)(radius * Math.Cos(vRotation) * Math.Cos(hRotation));
			position.Y = (float)(radius * Math.Sin(vRotation));
			position.Z = (float)(radius * Math.Cos(vRotation) * Math.Sin(hRotation));

			// Keep all rotations between 0 and 2PI
			hRotation = hRotation > (float)Math.PI * 2 ? hRotation - (float)Math.PI * 2 : hRotation;
			hRotation = hRotation < 0 ? hRotation + (float)Math.PI * 2 : hRotation;

			vRotation = vRotation > (float)Math.PI * 2 ? vRotation - (float)Math.PI * 2 : vRotation;
			vRotation = vRotation < 0 ? vRotation + (float)Math.PI * 2 : vRotation;

			// Switch up-vector based on vertical rotation
			upVector = vRotation > Math.PI / 2 && vRotation < Math.PI / 2 * 3 ?
				new Vector3(0, -1, 0) : new Vector3(0, 1, 0);

			// Translate these coordinates by the target objects spacial location
			position += target;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct PositionColored
	{
		public Vector3 Position;
		public int Color;

		public static readonly VertexFormat Format = VertexFormat.Position | VertexFormat.Diffuse;

		public PositionColored(Vector3 pos, int color)
		{
			Position = pos;
			Color = color;
		}
	}
}
