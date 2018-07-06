using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Common
{
    public static class EntitySequence
    {
        private static int id = 0;

        public static int GetNewEntityId()
        {
            id++;
            return id;
        }
    }
}
