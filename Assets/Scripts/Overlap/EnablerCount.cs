using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Overlap
{
    public struct EnablerCount
    {
        public EnablerCount(int numberOfDirections)
        {
            ByDirection = new int[numberOfDirections];
        }
        public int[] ByDirection;

        public void SetCountByDirection(int direction, int count)
        {
            ByDirection[direction] = count;
        }

        public int GetCountByDirection(int direction)
        {
            return ByDirection[direction];
        }

    }
}
