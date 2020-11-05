using System.Drawing;

namespace Server.Image
{
    //Use image recognition to find game values
    public class ImageValues
    {/*
        static List<Tower> Towers = new List<Tower>
        {
            new Tower{ Lane = Lanes.Top, Level = 1, Point = new Point(1446, 684), Color = Color.FromArgb(87, 24, 23) },
            new Tower{ Lane = Lanes.Top, Level = 2, Point = new Point(1497, 696), Color = Color.FromArgb(122, 34, 33) },
            new Tower{ Lane = Lanes.Top, Level = 3, Point = new Point(1533, 693), Color = Color.FromArgb(185, 50, 50) },

            new Tower{ Lane = Lanes.Mid, Level = 1, Point = new Point(1513, 762), Color = Color.FromArgb(58, 16, 16) },
            new Tower{ Lane = Lanes.Mid, Level = 2, Point = new Point(1519, 739), Color = Color.FromArgb(103, 29, 28) },
            new Tower{ Lane = Lanes.Mid, Level = 3, Point = new Point(1537, 724), Color = Color.FromArgb(153, 43, 41) },

            new Tower{ Lane = Lanes.Bot, Level = 1, Point = new Point(1584, 818), Color = Color.FromArgb(35, 10, 9) },
            new Tower{ Lane = Lanes.Bot, Level = 2, Point = new Point(1569, 767), Color = Color.FromArgb(84, 24, 23) },
            new Tower{ Lane = Lanes.Bot, Level = 3, Point = new Point(1578, 737), Color = Color.FromArgb(90, 25, 24) },
          };*/

        /*new Tower{ Lane = Lanes.Top, Level = 1, Point = new Point(1599, 775), Color = Color.FromArgb(175, 47, 47) },
        new Tower{ Lane = Lanes.Top, Level = 1, Point = new Point(1606, 769), Color = Color.FromArgb(87, 24, 23) },
        new Tower{ Lane = Lanes.Top, Level = 1, Point = new Point(1598, 778), Color = Color.FromArgb(13, 4, 3) },
        new Tower{ Lane = Lanes.Top, Level = 1, Point = new Point(1600, 776), Color = Color.FromArgb(31, 9, 8) },
        new Tower{ Lane = Lanes.Top, Level = 2, Point = new Point(1649, 781), Color = Color.FromArgb(182, 49, 49) },
        new Tower{ Lane = Lanes.Top, Level = 3, Point = new Point(1688, 777), Color = Color.FromArgb(165, 44, 45) },

        new Tower{ Lane = Lanes.Mid, Level = 1, Point = new Point(1673, 846), Color = Color.FromArgb(177, 48, 48) },
        new Tower{ Lane = Lanes.Mid, Level = 1, Point = new Point(1666, 852), Color = Color.FromArgb(58, 16, 16) },
        new Tower{ Lane = Lanes.Mid, Level = 1, Point = new Point(1665, 855), Color = Color.FromArgb(10, 3, 3) },
        new Tower{ Lane = Lanes.Mid, Level = 2, Point = new Point(1677, 828), Color = Color.FromArgb(167, 44, 45) },
        new Tower{ Lane = Lanes.Mid, Level = 3, Point = new Point(1697, 812), Color = Color.FromArgb(167, 44, 45) },

        new Tower{ Lane = Lanes.Bot, Level = 1, Point = new Point(1737, 909), Color = Color.FromArgb(187, 51, 51) },
        new Tower{ Lane = Lanes.Bot, Level = 1, Point = new Point(1743, 904), Color = Color.FromArgb(75, 20, 20) },
        new Tower{ Lane = Lanes.Bot, Level = 2, Point = new Point(1729, 855), Color = Color.FromArgb(164, 44, 44) },
        new Tower{ Lane = Lanes.Bot, Level = 3, Point = new Point(1732, 822), Color = Color.FromArgb(164, 44, 45) },*/
        /*static Color mapFogColor = Color.FromArgb(52, 55, 40);
        static Color mapOpenColor = Color.FromArgb(145, 153, 112);
        static Color mapBlackColor = Color.FromArgb(6, 9, 7);
        static List<Tower> Towers = new List<Tower>
        {
        new Tower{ Lane = Lanes.Top, Level = 1, Point = new Point(1605, 775), Color = Color.Black },
        new Tower{ Lane = Lanes.Top, Level = 2, Point = new Point(1649, 781), Color = Color.FromArgb(182, 49, 49) },
        new Tower{ Lane = Lanes.Top, Level = 3, Point = new Point(1688, 777), Color = Color.FromArgb(165, 44, 45) },

        new Tower{ Lane = Lanes.Mid, Level = 1, Point = new Point(1666, 849), Color = Color.Black },
        new Tower{ Lane = Lanes.Mid, Level = 2, Point = new Point(1677, 828), Color = Color.FromArgb(167, 44, 45) },
        new Tower{ Lane = Lanes.Mid, Level = 3, Point = new Point(1697, 812), Color = Color.FromArgb(167, 44, 45) },

        new Tower{ Lane = Lanes.Bot, Level = 1, Point = new Point(1737, 903), Color = Color.Black },
        new Tower{ Lane = Lanes.Bot, Level = 2, Point = new Point(1729, 855), Color = Color.FromArgb(164, 44, 44) },
        new Tower{ Lane = Lanes.Bot, Level = 3, Point = new Point(1732, 822), Color = Color.FromArgb(164, 44, 45) },
          };*/
        static int EnemyColor = Color.FromArgb(148, 36, 24).ToArgb();
        //static int MyColor = Color.FromArgb(198, 166, 24).ToArgb();
        static int AllyColor = Color.FromArgb(24, 138, 198).ToArgb();
        static int EnemyCreepColor = Color.FromArgb(197, 90, 89).ToArgb();
        static int AllyCreepColor = Color.FromArgb(73, 143, 197).ToArgb();
        static int EnemyTowerColor = Color.FromArgb(169, 45, 36).ToArgb();
        //static int MapWhiteColor = Color.FromArgb(255, 255, 255).ToArgb();
        static int MapWhiteColor = Color.FromArgb(57, 110, 158).ToArgb();

        public static int GetHPDifference(DirectBitmap src)
        {
            int ally = GetTotalWidth(src, AllyColor);
            int enemy = GetTotalWidth(src, EnemyColor);
            return ally - enemy;
        }
        public static int GetCreepHPDifference(DirectBitmap src)
        {
            int ally = GetTotalWidth(src, AllyCreepColor);
            int enemy = GetTotalWidth(src, EnemyCreepColor);
            return ally - enemy;
        }
        /*
        public static bool EnemyTowerExists(DirectBitmap src)
        {
            return PixelExists(src, EnemyTowerColor);
        }

        public static bool AllyCreepExists(DirectBitmap src)
        {
            return PixelExists(src, AllyCreepColor);
        }

        public static bool EnemyCreepExists(DirectBitmap src)
        {
            return PixelExists(src, EnemyCreepColor);
        }*/
        public static System.Drawing.Point AllyCreepPosition(DirectBitmap src)
        {
            return GetPoint(src, AllyCreepColor);
        }
        public static System.Drawing.Point EnemyCreepPosition(DirectBitmap src)
        {
            return GetPoint(src, EnemyCreepColor);
        }
        public static System.Drawing.Point EnemyPosition(DirectBitmap src)
        {
            return GetPoint(src, EnemyColor);
        }
        public static System.Drawing.Point AllyPosition(DirectBitmap src)
        {
            return GetPoint(src, AllyColor);
        }
        public static System.Drawing.Point EnemyTowerPosition(DirectBitmap src)
        {
            return GetPoint(src, EnemyTowerColor);
        }
        public static Point GetMyPosition(DirectBitmap src)
        {
            Point pos = GetMapPosition(src, MapWhiteColor);
            pos.X -= 844; //+ 30; // x-30 char pos
            pos.Y -= 586; //- 20; //y+20 char pos
            return pos;
        }
        /*
                public static int GetTowerStatus(Bitmap src, Lanes lane)
                {
                    foreach (var x in Towers)
                    {
                        if (x.Lane != lane)
                            continue;
                        Color col = src.GetPixel(x.Point.X, x.Point.Y);
                        if (x.Color == Color.Black)
                        {
                            if (col == mapFogColor || col == mapOpenColor)
                                continue;
                            return x.Level;
                        }
                        else if (src.GetPixel(x.Point.X, x.Point.Y) == x.Color)
                            return x.Level;
                    }
                    return 0;
                }
        */
        public static bool IsPointInTriangle(Point one, Point two, Point three, Point point)
        {
            double x1 = one.X, y1 = one.Y;
            double x2 = two.X, y2 = two.Y;
            double x3 = three.X, y3 = three.Y;

            double x, y;
            x = point.X;
            y = point.Y;

            double a = ((y2 - y3) * (x - x3) + (x3 - x2) * (y - y3)) / ((y2 - y3) * (x1 - x3) + (x3 - x2) * (y1 - y3));
            double b = ((y3 - y1) * (x - x3) + (x1 - x3) * (y - y3)) / ((y2 - y3) * (x1 - x3) + (x3 - x2) * (y1 - y3));
            double c = 1 - a - b;

            /*if (a == 0 || b == 0 || c == 0) Console.WriteLine("Point is on the side of the triangle");
            else */
            if (a >= 0 && a <= 1 && b >= 0 && b <= 1 && c >= 0 && c <= 1) return true;
            return false;
        }
        public static System.Drawing.Point GetPoint(DirectBitmap src, int color)
        {
            for (int x = src.Width - 50; x > 100; x -= 5)
            {
                for (int y = 10; y < src.Height - 1; y += 5)
                {
                    if (src.GetPixel(x, y).ToArgb() == color)
                        return new System.Drawing.Point(x, y);
                }
            }
            return new System.Drawing.Point(0, 0);
        }
        /*public static System.Drawing.Point GetPoint(DirectBitmap src, int color)
        {
            for (int x = src.Width; x > 0; x--)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    if (src.GetPixel(x, y).ToArgb() == color)
                        return new System.Drawing.Point(x, y);
                }
            }
            return new System.Drawing.Point(0, 0);
        }*/
        public static Point GetMapPosition(DirectBitmap src, int color)
        {
            //   src.Bitmap.Save("map.bmp");
            for (int x = src.Width - 1; x > 844; x--)
            {
                for (int y = 586; y < src.Height - 1; y++)
                {
                    if (src.GetPixel(x, y).ToArgb() == color)
                        return new System.Drawing.Point(x, y);
                }
            }
            return new System.Drawing.Point(0, 0);
        }
        public static bool PixelExists(DirectBitmap src, int color)
        {
            for (int x = src.Width - 1; x > 0; x--)
            {
                for (int y = 0; y < src.Height - 1; y++)
                {
                    if (src.GetPixel(x, y).ToArgb() == color)
                        return true;
                }
            }
            return false;
        }
        public static int GetTotalWidth(DirectBitmap src, int color)
        {
            int result = 0;
            for (int y = 0; y < src.Height / 2; y++)
            {
                for (int x = 0; x < src.Width; x++)
                {
                    if (src.GetPixel(x, y).ToArgb() == color)
                        result++;
                }
            }
            return result;
        }
        public static int GetMaxWidth(Bitmap src, int color)
        {
            int result = 0, temp = 0;
            for (int y = 0; y < src.Size.Height; y++)
            {
                for (int x = 0; x < src.Size.Width; x++)
                {
                    if (src.GetPixel(x, y).ToArgb() == color)
                        temp++;
                }
                if (temp > result)
                {
                    result = temp;
                }
                temp = 0;
            }
            return result;
        }
        /*public static int AllyCreepHealth()
        {
            //Get matching x pixels of the health bar
            int value = ImageRecognition.MatchingXPixels("Game/allycreephealth.png", 3);

            //Get total pixels of health bar
            int total = PixelCache.GetWidth("Game/allycreephealth.png");

            //Get percentage of 100
            return (int)Math.Round(100d * value / total);
        }*/
        /*public static Point characterLeveled()
        {
            Point position = ImageRecognition.FindImagePosition("Game/levelup.png", 5);
            return position;
        }*/
        /*public static Point AllyCreepPosition()
        {
            Point position = ImageRecognition.FindImagePosition("Game/allycreephealth.png", 3);
            return position;
        }
        public static Point EnemyCreepPosition()
        {
            Point position = ImageRecognition.FindImagePositionNearest("Game/enemycreephealth.png", 3);
            return position;
        }
        public static Point EnemyTowerStructure()
        {
            Point position = ImageRecognition.FindImagePosition("Game/towerstructure.png", 3);
            return position;
        }
        public static Point EnemyTowerStructure2()
        {
            Point position = ImageRecognition.FindImagePosition("Game/towerstructure2.png", 3);
            return position;
        }
        public static Point EnemyTowerStructure3()
        {
            Point position = ImageRecognition.FindImagePosition("Game/towerstructure3.png", 3);
            return position;
        }
        public static Point EnemyTowerStructure4()
        {
            Point position = ImageRecognition.FindImagePosition("Game/towerstructure4.png", 3);
            return position;
        }
        */
        /*//Return mana value percentage
        public static int Mana()
        {
            //Get matching x pixels of the health bar
            int value = ImageRecognition.MatchingXPixels("Game/mana.png", 40);

            //Get total pixels of health bar
            int total = PixelCache.GetWidth("Game/mana.png");

            //Get percentage of 100
            return (int)Math.Round((double)(100 * value) / total);

        }*/
        /*
        internal static Point EnemyChampion()
        {
            Point position = ImageRecognition.FindImagePositionNearest("Game/enemycharacter.png", 4);
            return position;
        }
        internal static Point EnemyTowerHp()
        {
            Point position = ImageRecognition.FindImagePositionNearest("Game/enemytowerhp.png", 4);
            return position;
        }

        internal static Point AllyChampion()
        {
            Point position = ImageRecognition.FindImagePositionNearest("Game/allycharacter.png", 4);
            return position;
        }
        internal static Point BotChampion()
        {
            Point position = ImageRecognition.FindImagePositionNearest("Game/botcharacter.png", 4);
            return position;
        }

        internal static Point EnemyTowerHp2()
        {
            Point position = ImageRecognition.FindImagePositionNearest("Game/enemytowerhp2.png", 4);
            return position;
        }*/
    }
}
