using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Common
{
    /// <summary>
    /// Wrapper for generating uid for entities
    /// </summary>
    public static class EntitySequence
    {
        private static int id = 0;

        /// <summary>
        /// Generates uid for entities in game
        /// </summary>
        /// <returns>unique entity id</returns>
        public static int GetNewEntityId()
        {
            id++;
            return id;
        }
    }
}
