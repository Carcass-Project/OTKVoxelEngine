using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTKVoxelEngine.Core
{
    public class Chunk
    {
        public int x, y, z;
        public Voxel.BlockType[,,] _voxels = new Voxel.BlockType[8,16,8];

        public Chunk(int x, int y, int z)
        {
            for (int xs = 0; x < 8; x++)
            {
                for (int ys = 0; y < 16; y++)
                {
                    for (int zs = 0; z < 8; z++)
                    {
                        _voxels[xs, ys, zs] = Voxel.BlockType.Grass;
                    }
                }
            }
        }
    }
}
