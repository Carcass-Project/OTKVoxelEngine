using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OTKVoxelEngine.Renderer;
using OTKVoxelEngine.Engine;

namespace OTKVoxelEngine.Core
{
    /// <summary>
    /// Voxel class, optimized. :)
    /// </summary>
    public class Voxel
    {
        public enum BlockType
        {
            Air,
            Grass
        }

        public static BlockType? GetVoxAtPos(int x, int y, int z)
        {
            try
            {
                if (x + y + z < _world.Length)
                    return _world[x, y, z];
            }
            catch(Exception e)
            {
                Console.WriteLine("Invalid voxel position.");
                
            }
            return BlockType.Air;

        }

        public static BlockType?[,,] _world = new BlockType?[8,16,8];

        private static bool isBlockAir(int x, int y, int z)
        {
            
            if (x < 0 && y < 0 && z < 0)
                return true;
            if (x + y + z < _world.Length)
                return true;
            if (_world[x, y, z] == BlockType.Air)
                return true;
            return false;
        }
        public static Random rnd = new Random();
        public static void InitVoxelWorld()
        {
            for(int x = 0; x < 8; x++)
            {
                for(int y = 0; y < 16; y++)
                {
                    for(int z = 0; z < 8; z++)
                    {
                        _world[x, y, z] = BlockType.Grass;
                    }
                }
            }
        }

        public static Mesh GenVoxelMesh(int x, int y, int z)
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

            List<uint> indices = new List<uint>();

            var f = GetVoxAtPos(x, y, z);

            if(f != BlockType.Air)
            {
                /*
                 0, 1, 3, 3, 1, 2,
                 1, 5, 2, 2, 5, 6,
                 5, 4, 6, 6, 4, 7,
                 4, 0, 7, 7, 0, 3,
                 3, 2, 7, 7, 2, 6,
                 4, 5, 0, 0, 5, 1.
                 */
                var blockBelow = isBlockAir(x, y - 1, z);
                var blockAbove = isBlockAir(x,y + 1, z);
                var blockLeft = isBlockAir(x + 1, y, z);
                var blockRight = isBlockAir(x - 1, y, z);
                var blockFront = isBlockAir(x, y, z + 1);
                var blockBack = isBlockAir(x, y, z - 1);

                if(blockAbove)
                {

                    indices.AddRange(new uint[] { 3, 2, 7, 7, 2, 6 });
                }
                if (blockBelow)
                {
                    indices.AddRange(new uint[] { 4, 5, 0, 0, 5, 1 });
                }
                if (blockLeft)
                {
                 
                    indices.AddRange(new uint[] { 5, 4, 6, 6, 4, 7 });
                }
                if (blockRight)
                {
                  
                    indices.AddRange(new uint[] { 4, 0, 7, 7, 0, 3 });
                }
                if (blockFront)
                {
                  
                    indices.AddRange(new uint[] { 0, 1, 3, 3, 1, 2 });
                }
                if (blockBack)
                {
           
                    indices.AddRange(new uint[] { 1, 5, 2, 2, 5, 6 });
                }
            }
            var msh = new Mesh(vertices, indices, new List<Texture>());
            msh.pos = new Vector3(x, y, z);
            switch (f)
            {
                case BlockType.Grass:
                    msh.col = Color4.DarkGreen;
                    break;
            }
            return msh;
        }
    }
}
