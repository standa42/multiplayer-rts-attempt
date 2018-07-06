using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Game.Entities
{
    public class PlayerWorker : Entity
    {
        protected GameObject go;
        protected GameObject selection;

        public PlayerWorker(int playerId, Vector2Int position, Map map, GameManager gameManager) : base(playerId, position, map, gameManager)
        {
            selection = GameObject.Instantiate(Resources.Load(@"Map/Entities/SelectedCube") as GameObject);
            selection.transform.position = new Vector3(position.x + 0.5f, position.y + 0.5f, -0.37f);

            selection.SetActive(false);
        }

        public virtual void Move()
        {

        }

        public virtual void MoveCmd(Vector2 position)
        {

        }

        public virtual void Select()
        {
            selection.SetActive(true);
        }

        public virtual void Unselect()
        {
            selection.SetActive(false);
        }
    }
}
