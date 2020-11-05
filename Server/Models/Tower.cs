using System.Drawing;

namespace Server.Models
{
    public enum Lanes
    {
        Top,
        Mid,
        Bot
    }
    class Tower
    {
        public int Level { get; set; }
        public Point Point { get; set; }
        public Color Color { get; set; }
        public Lanes Lane { get; set; }
    }
}
