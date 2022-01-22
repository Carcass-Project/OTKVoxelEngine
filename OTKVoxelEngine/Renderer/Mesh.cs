using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using OTKVoxelEngine.Engine;

namespace OTKVoxelEngine.Renderer
{
    public class Mesh
    {
        // PUBLIC:
        public List<Vertex> vertices = new List<Vertex>();
        public List<uint> indices = new List<uint>();
        public List<Texture> textures = new List<Texture>();
        public Color4 col = Color4.DarkGreen;
        public Vector3 pos = new Vector3(0,0,0);

        public Mesh(List<Vertex> _verts, List<uint> _indc, List<Texture> _texs)
        {
            vertices = _verts;
            indices = _indc;
            textures = _texs;

            setupMesh();
        }

        public void Draw(Shader shad)
        {
            GL.BindVertexArray(VAO);
            //shad.SetVector3("pos", pos);
            shad.SetMatrix4("model", Matrix4.Identity * Matrix4.CreateTranslation(pos));
            shad.SetVector3("inColor", ((Vector4)col).Xyz);
            GL.DrawElements(PrimitiveType.Triangles, indices.Count, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        /// <summary>
        /// Generates a 1x1x1 cube.
        /// </summary>
        /// <returns>A cube mesh.</returns>
        public static Mesh GenCubeMesh()
        {
            List<Vertex> vertices = new List<Vertex>();
            vertices.Add(new Vertex(new Vector3(-1, -1, -1)));
            vertices.Add(new Vertex(new Vector3(1, -1, -1)));
            vertices.Add(new Vertex(new Vector3(1, 1, -1)));
            vertices.Add(new Vertex(new Vector3(-1, 1, -1)));
            vertices.Add(new Vertex(new Vector3(-1, -1, 1)));
            vertices.Add(new Vertex(new Vector3(1, -1, 1)));
            vertices.Add(new Vertex(new Vector3(1, 1, 1)));
            vertices.Add(new Vertex(new Vector3(-1, 1, 1)));
            List<uint> indices = new List<uint>
            {
                 0, 1, 3, 3, 1, 2,
                 1, 5, 2, 2, 5, 6,
                 5, 4, 6, 6, 4, 7,
                 4, 0, 7, 7, 0, 3,
                 3, 2, 7, 7, 2, 6,
                 4, 5, 0, 0, 5, 1
            };
            Mesh msh = new Mesh
            (
                vertices,
                indices,
                new List<Texture>()
            );

            return msh;
        }

        // PRIVATE:

        private uint VAO, VBO, EBO;

        private unsafe void setupMesh()
        {
            GL.GenVertexArrays(1, out VAO);
            GL.GenBuffers(1, out VBO);
            GL.GenBuffers(1, out EBO);

            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            IntPtr intPtr = Marshal.UnsafeAddrOfPinnedArrayElement(vertices.ToArray(), 0);
            IntPtr indiPtr = Marshal.UnsafeAddrOfPinnedArrayElement(indices.ToArray(), 0);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * Marshal.SizeOf(typeof(Vertex)), intPtr, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(uint), indiPtr, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(Vertex), 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), "normal"));

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), "texCoords"));

            GL.BindVertexArray(0);
        }
    }
}
