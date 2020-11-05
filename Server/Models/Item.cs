namespace Server.Models
{
    public class Item
    {
        public string name = "NoName";
        public int cost = 0;
        public bool got = false;
        public bool canStack = false;
        public int buyOrder = 0;
        public string phrase = "BOOTS";

        public Item(string name, int cost, bool got, bool canstack, int buyorder, string word)
        {
            this.name = name;
            this.cost = cost;
            this.got = got;
            canStack = canstack;
            buyOrder = buyorder;
            phrase = word;
        }
    }
}
