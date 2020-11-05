using Server.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Server.Game
{
    class Champions
    {
        static Dictionary<string, int> champlist;
        static List<string> AvailableChamps = new List<string> { "ashe", "sivir", "tristana" };
        public static int RandomChamp()
        {
            System.Random rand = new System.Random();
            int r = rand.Next(0, AvailableChamps.Count);
            return GetIdByChamp(AvailableChamps[r]);
        }
        public static List<Item> GetItems(int champ)
        {
            List<Item> list = new List<Item> { // ashe
                /*
                            new Item("Boots of Speed",300,false,false, 0, "SPEED"),
                            new Item("B.F. Sword", 1300, false, false, 0,  "BF"),
                            new Item("Caufield's Warhammer",1100,false,false, 0, "CAU"),
                            new Item("Blade of Ruined King", 3300, false, false, 0, "BORK"),
                            new Item("Essence Reaver", 2000, false, false, 0, "CANB"),
                            new Item("Berserker Greaves", 800, false, false, 0, "SAVASCI"),
                            new Item("Runaans", 2600, false, false, 0, "RUNAA"),
                            new Item("Infinity Edge", 2100, false, false, 0, "EBEDI"),
                            new Item("Mortal Reminder", 2800, false, false, 0, "FANI")*/
                
                            new Item("Boots of Speed",300,false,false, 0, "SPEED"),
                            new Item("B.F. Sword", 1300, false, false, 0,  "BF"),
                            new Item("Caufield's Warhammer",1100,false,false, 0, "CAU"),
                            new Item("Blade of Ruined King", 3300, false, false, 0, "BORK"),
                            new Item("Essence Reaver", 2000, false, false, 0, "ESSENCE"),
                            new Item("Berserker Greaves", 800, false, false, 0, "BERSERKER"),
                            new Item("Runaans", 2600, false, false, 0, "RUNAA"),
                            new Item("Infinity Edge", 2100, false, false, 0, "INFINITY"),
                            new Item("Mortal Reminder", 2800, false, false, 0, "MORTAL")
                };
            switch (GetChampById(champ).ToString())
            {
                case "annie":
                    list = new List<Item> {
                            //new Item("Dark Seal",350,false,false, 0, "KARA"),
                            /*
                            new Item("Doran's Ring", 400, false, false, 0,  "DORAN"),
                            new Item("Boots of Speed",300,false,false, 0, "SPEED"),
                            new Item("Lost Chapter",1300,false,false, 0, "KAYIP"),
                            new Item("Sorcerer's Shoes", 800, false, false, 0, "SIHIRB"),
                            new Item("Blasting Wand", 850, false, false, 0, "INFILAK"),
                            new Item("Rabadon's Deathcap", 3600, false, false, 0, "RABADON"),
                            new Item("Morellenomicon", 3000, false, false, 0, "MORELLENOM"),
                            new Item("Zhonya's Hourglass", 2900, false, false, 0, "ZHONYA")*/
                            
                            new Item("Doran's Ring", 400, false, false, 0,  "DORAN"),
                            new Item("Boots of Speed",300,false,false, 0, "SPEED"),
                            new Item("Lost Chapter",1300,false,false, 0, "LOST"),
                            new Item("Sorcerer's Shoes", 800, false, false, 0, "SORCERER"),
                            new Item("Blasting Wand", 850, false, false, 0, "BLASTING"),
                            new Item("Rabadon's Deathcap", 3600, false, false, 0, "RABADON"),
                            new Item("Morellenomicon", 3000, false, false, 0, "MORELLENOM"),
                            new Item("Zhonya's Hourglass", 2900, false, false, 0, "ZHONYA")
                };
                    break;
                case "tristana":
                    list = new List<Item> {
                            //new Item("Dark Seal",350,false,false, 0, "KARA"),
                            /*new Item("Doran's Blade", 450, false, false, 0,  "KILICI"),
                            new Item("B.F. Sword", 1300, false, false, 0,  "BF"),
                            new Item("Boots of Speed",300,false,false, 0, "SPEED"),
                            new Item("Berserker Greaves", 800, false, false, 0, "SAVASC"),
                            new Item("Infinity Edge", 2100, false, false, 0, "EBEDI"),
                            new Item("Stormrazor", 850, false, false, 0, "FIRTINA"),
                            new Item("Rapid Firecannon", 2600, false, false, 0, "BOMBARD"),
                            new Item("Statikk Shiv", 2600, false, false, 0, "STATIKK"),
                            new Item("Guardian Angel", 2800, false, false, 0, "KORUYUCU")*/
                            
                            new Item("Doran's Blade", 450, false, false, 0,  "DB"),
                            new Item("B.F. Sword", 1300, false, false, 0,  "BF"),
                            new Item("Boots of Speed",300,false,false, 0, "SPEED"),
                            new Item("Berserker Greaves", 800, false, false, 0, "BERSERKER"),
                            new Item("Infinity Edge", 2100, false, false, 0, "INFINITY"),
                            new Item("Stormrazor", 850, false, false, 0, "STORMRAZ"),
                            new Item("Rapid Firecannon", 2600, false, false, 0, "FIRECANN"),
                            new Item("Statikk Shiv", 2600, false, false, 0, "STATIKK"),
                            new Item("Guardian Angel", 2800, false, false, 0, "GUARDIAN")
                };
                    break;
                case "sivir":
                    list = new List<Item> { // sivir
                
                            new Item("Boots of Speed",300,false,false, 0, "SPEED"),
                            new Item("B.F. Sword", 1300, false, false, 0,  "BF"),
                            new Item("Caufield's Warhammer",1100,false,false, 0, "CAU"),
                            new Item("Statikk Shiv", 2600, false, false, 0, "STATIKK"),
                            new Item("Essence Reaver", 2000, false, false, 0, "ESSENCE"),
                            new Item("Berserker Greaves", 800, false, false, 0, "BERSERKER"),
                            new Item("Runaans", 2600, false, false, 0, "RUNAA"),
                            new Item("Infinity Edge", 2100, false, false, 0, "INFINITY"),
                            new Item("Mortal Reminder", 2800, false, false, 0, "MORTAL")
                };
                    break;
            }
            return list;
        }
        private static void GetChamps()
        {
            try
            {
                ReadFile(Directory.GetCurrentDirectory() + @"\champions.txt");
            }
            catch
            {
                Logger.Log("Champion IDs could not be found.");
            }
        }
        public static int GetIdByChamp(string champ)
        {
            GetChamps();
            if (!champlist.TryGetValue(champ.ToLower(), out int id))
            {
                return 0;
            }
            return id;
        }
        public static string GetChampById(int id)
        {
            GetChamps();
            try
            {
                var champ = champlist.FirstOrDefault(a => a.Value == id);
                string name = champ.Key;
                if (name != null)
                {
                    return name;
                }
            }
            catch
            {
                return "N/A";
            }
            return "N/A";
        }

        private static void ReadFile(string path)
        {
            champlist = new Dictionary<string, int>();
            if (File.Exists(path))
            {
                string[] allchamps = File.ReadAllLines(path);

                foreach (string champ in allchamps)
                {

                    string[] x = champ.Split(':');
                    champlist.Add(x[1], int.Parse(x[0]));
                }

            }
            else
            {
                Logger.Log("Champions.txt could not be found");
            }
        }

    }
}
