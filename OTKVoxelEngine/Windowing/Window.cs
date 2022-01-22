using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OTKVoxelEngine.Engine;
using OTKVoxelEngine.Renderer;
using OTKVoxelEngine.Core;

namespace OTKVoxelEngine.Windowing
{
    public class Window : GameWindow
    {
        public static string title = "Unnamed OTKVE Windoww";
        public static Shader? defaultShader;
        public static Shader? skyboxShader;
        protected double time = 0;
        protected Matrix4 model = Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(0));
        private Matrix4 _view;
        private Matrix4 _projection;
        public Camera camera;
        uint cubemapTex;
        
        float x = 0, y = 0, z = -5;
        float yaw = -90.0f, pitch = 0.0f;

        Vector3 CameraFront = new Vector3(0,0,1);

        int vaoID;
        int vboID;
        int eboID;

        public static GameWindowSettings gameSettings
        {
            get
            {
                var f = new GameWindowSettings();
                f.RenderFrequency = 60.0;
                return f;
            }
        }
        public static NativeWindowSettings nativeSettings
        {
            get
            {
                var f = new NativeWindowSettings();
                f.Size = new OpenTK.Mathematics.Vector2i(800, 600);
                f.Title = title;
                f.API = ContextAPI.OpenGL;
                f.Flags = ContextFlags.ForwardCompatible;
                f.NumberOfSamples = 4;
                return f;
            }
        }

        float[] vertices = {


                                    };
        uint[] indices =
        {
            
        };
        Mesh msh2;
        public Frustum _frustum = new Frustum();
        List<Mesh> mshs = new List<Mesh>();
        protected override void OnLoad()
        {
            base.OnLoad();
            

            GL.Enable(EnableCap.DepthTest);
            //GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Front);
            GL.Enable(EnableCap.Multisample);

            GL.ClearColor(Color4.LightSkyBlue);


            vboID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            vaoID = GL.GenVertexArray();
            GL.BindVertexArray(vaoID);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            skyboxVAO = GL.GenVertexArray();
            GL.BindVertexArray(skyboxVAO);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);

            eboID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, eboID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            defaultShader = new Shader("default.vs", "fdefault.fs");
            skyboxShader = new Shader("skybox.vs", "skybox.fs");

            defaultShader.Use();
            skyboxShader.Use();

            var vertexLocation = defaultShader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);


            msh = Mesh.GenCubeMesh();
            msh2 = Mesh.GenCubeMesh();
            Voxel.InitVoxelWorld();

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    for (int z = 0; z < 8; z++)
                    {
                        
                        mshs.Add(Voxel.GenVoxelMesh(x,y,z));
                    }
                }
            }
            

            camera = new Camera(new Vector3(0, 0, -3), Size.X / (float)Size.Y);
            CursorGrabbed = true;

            cubemapTex = Texture.LoadCubemapFromFiles(new List<string> { "front.png", "back.png", "left.png", "right.png", "top.png", "bottom.png" });
        }

        Mesh msh;

        int skyboxVAO;
        int skyboxVBO;

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            time += 64.0 * args.Time;

            
            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            defaultShader?.Use();
            GL.DepthMask(false);
            skyboxShader?.Use();
            skyboxShader?.SetMatrix4("view", camera.GetViewMatrix());
            skyboxShader?.SetMatrix4("projection", camera.GetProjectionMatrix());
            model = Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(time));
            

            

            

            GL.BindVertexArray(skyboxVAO);
            GL.BindTexture(TextureTarget.TextureCubeMap, cubemapTex);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            GL.DepthMask(true);

            foreach (var x in mshs)
            {
                _frustum.CalculateFrustum(_projection, _view);
                if (!_frustum.CubeVsFrustum(x.pos.X, x.pos.Y, x.pos.Z, 1))
                    x.Draw(defaultShader);
            }
            

            SwapBuffers();
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            
        }

        bool firstMouse = true;
        float lastX=0, lastY=0;
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if(firstMouse)
            {
                lastX = e.X;
                lastY = e.Y;
                firstMouse = false;
            }
            float xOffset = (e.X - lastX);
            float yOffset = (lastY-e.Y);
            lastX = e.X;
            lastY = e.Y;
            float sensitivity = 0.5f;
            xOffset *= sensitivity;
            yOffset *= sensitivity;

            base.OnMouseMove(e);
            camera.Yaw += xOffset;
            camera.Pitch += yOffset;

        }
        Random rnd = new Random();
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            defaultShader?.SetVector3("inColor", new Vector3((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));
        }
        bool isLined = false;
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            var state = KeyboardState;
            if (state.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape))
                this.Close();
            if(state.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.W))
            {
                camera.Position += camera.Front * 1.5f * (float)args.Time;
            }
            if (state.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.S))
            {
                camera.Position -= camera.Front * 1.5f * (float)args.Time;
            }
            if (state.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.A))
            {
                camera.Position -= camera.Right * 1.5f * (float)args.Time;
            }
            if (state.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.D))
            {
                camera.Position += camera.Right * 1.5f * (float)args.Time;
            }
            if(state.IsKeyReleased(OpenTK.Windowing.GraphicsLibraryFramework.Keys.P))
            {
                if (isLined == true)
                {
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                    isLined = false;
                }
                else
                {
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                    isLined = true;
                }
            }
        }

        public Window(string _title) : base(gameSettings, nativeSettings)
        {
            title = _title;
            
            Run();
        }
    }
}
