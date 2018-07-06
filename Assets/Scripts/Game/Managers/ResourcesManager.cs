using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Game.Managers;
using Assets.Scripts.Menu;

namespace Assets.Scripts.Game
{
    public class ResourcesManager
    {
        public event ResourcesUpdateDelegate ResourcesUpdate;
        public delegate void ResourcesUpdateDelegate(int resourceAmount, int woodAmount);

        public List<PlayerStats> PlayerStats { get; }
        private readonly int playerId;

        public int GetPlayerWoodAmount(int playerId)
        {
            return PlayerStats[playerId].Wood;
        }

        public int GetPlayerResourceAmount(int playerId)
        {
            return PlayerStats[playerId].Resources;
        }

        public ResourcesManager(int playerId, NumberOfPlayersInGame numberOfPlayers)
        {
            this.playerId = playerId;

            PlayerStats = new List<PlayerStats>();

            for (var i = 0; i < (int)numberOfPlayers; i++)
            {
                PlayerStats.Add(new PlayerStats(i, Config.InitialResourceAmount, Config.InitialWoodAmount));
            }

        }

        public bool RemoveWoodFromPlayer(int playerId, int woodAmount)
        {
            if (PlayerStats[playerId].Wood >= woodAmount)
            {
                PlayerStats[playerId].Wood -= woodAmount;

                UpdateDevicePlayerResourcesUI(playerId);

                return true;
            }

            return false;
        }

        public bool RemoveResourcesFromPlayer(int playerId, int resourceAmount)
        {
            if (PlayerStats[playerId].Resources >= resourceAmount)
            {
                PlayerStats[playerId].Resources -= resourceAmount;

                UpdateDevicePlayerResourcesUI(playerId);

                return true;
            }

            return false;
        }

        public void AddWoodToPlayer(int playerId, int woodAmount)
        {
            PlayerStats[playerId].Wood += woodAmount;

            UpdateDevicePlayerResourcesUI(playerId);
        }

        public void AddResourcesToPlayer(int playerId, int resourceAmount)
        {
            PlayerStats[playerId].Resources += resourceAmount;

            UpdateDevicePlayerResourcesUI(playerId);
        }

        private void UpdateDevicePlayerResourcesUI(int invokingPlayer)
        {
            if (playerId == invokingPlayer)
            {
                ResourcesUpdate?.Invoke(PlayerStats[playerId].Resources, PlayerStats[playerId].Wood);
            }
        }


    }
}
