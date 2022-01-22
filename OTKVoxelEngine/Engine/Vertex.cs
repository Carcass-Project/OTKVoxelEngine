using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace OTKVoxelEngine.Engine
{
    /// <summary>
    /// A 3D Vertex structure. 2 ctor overloads -> for float x,y,z and a Vector3.
    /// </summary>
    public struct Vertex
    {
        Vector3 position = new Vector3();
        Vector3 normal = new Vector3();
        Vector3 texCoords = new Vector3();

        public Vertex(Vector3 position)
        {
            this.position = position;
        }
        /*public Vertex(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public Vertex(Vector3 _vert)
        {
            x = _vert.X; 
            y = _vert.Y; 
            z = _vert.Z;
        }*/
    }
}
