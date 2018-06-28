using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Common;
using Assets.Scripts.Game.Entities;

namespace Assets.Scripts.Game.Races
{
    public static class Races
    {
        public static Dictionary<RaceEnum, Race> RacesDictionary;

        static Races()
        {
            RacesDictionary.Add(RaceEnum.Universal, new UniversalRace());
            RacesDictionary.Add(RaceEnum.Second, new SecondRace());
        }

        public static IEnumerable<Building> GetBuildings(RaceEnum race)
        {
            Race raceObject;

            if (RacesDictionary.TryGetValue(race, out raceObject))
            {
                return raceObject.Buildings;
            }
            else
            {
                Log.LogMessage("Races.GetBuildings throw error");
                throw new ArgumentException("Races.GetBuildings got wrong parameter");
            }
        }
    }

    public abstract class Race
    {
        public List<Building> Buildings { get; }
    }

    public class UniversalRace : Race
    {
        public UniversalRace()
        {
            //Buildings.Add();
        }
    }

    public class SecondRace : Race
    {
        public SecondRace()
        {
            // Buildings.Add();
        }
    }


    /// <summary>
    /// Races that exist in the game and can be played
    /// </summary>
    public enum RaceEnum : byte
    {
        Universal,
        Second
    }
}
