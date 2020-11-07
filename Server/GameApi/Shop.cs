namespace Server.GameApi
{
    public enum ShopItemTypeEnum
    {
        Starting,
        Early,
        Essential,
        Offensive,
        Defensive
    }
    public class Shop
    {/*
        560,261 --- 620,261 --- 680,261
        560,378 --- 620,378 --- 680,378
        560,492 --- 620,492 --- 680,492
        560,610 --- 620,610 --- 680,610
        560,725 --- 620,725 --- 680,725
*/
        /*private static Dictionary<ShopItemTypeEnum, Point[]> ItemPositions = new Dictionary<ShopItemTypeEnum, Point[]>()
        {
            { ShopItemTypeEnum.Starting,  new Point[]{   new Point(560, 261), new Point(620, 261), new Point(680, 261) } },
            { ShopItemTypeEnum.Early,     new Point[]{   new Point(560, 378),new Point(620, 378), new Point(680, 378) } },
            { ShopItemTypeEnum.Essential, new Point[]{   new Point(560, 492), new Point(620, 492), new Point(680, 492) } },
            { ShopItemTypeEnum.Offensive, new Point[]{   new Point(560, 610), new Point(620, 610), new Point(680, 610) } },
            { ShopItemTypeEnum.Defensive, new Point[]{   new Point(560, 725), new Point(620, 725), new Point(680, 725), new Point(740, 725) } },

        };
        private static Dictionary<ShopItemTypeEnum, Point[]> ItemPositions = new Dictionary<ShopItemTypeEnum, Point[]>()
        {
            { ShopItemTypeEnum.Starting,  new Point[]{   new Point(580, 330), new Point(740, 330), new Point(940, 330) } },
            { ShopItemTypeEnum.Early,     new Point[]{   new Point(580, 440),new Point(740, 440), new Point(940, 440) } },
            { ShopItemTypeEnum.Essential, new Point[]{   new Point(580, 550), new Point(740, 550), new Point(940, 550)} },
            { ShopItemTypeEnum.Offensive, new Point[]{   new Point(580, 660), new Point(740, 660), new Point(940, 660) } },
            { ShopItemTypeEnum.Defensive, new Point[]{   new Point(580, 770), new Point(740, 770), new Point(940, 770), new Point(940, 770) } },

        };*/
        public static bool Opened
        {
            get;
            set;
        }
        public Shop()
        {
            Opened = false;
        }
        public static void Toggle()
        {
            //Logger.Log("toggle shop " + Opened);
            if (!Opened)
                InputManager.Keyboard.KeyPress(System.Windows.Forms.Keys.P, GameSession.RandomTimeGenerator(120));
            else
                InputManager.Keyboard.KeyPress(System.Windows.Forms.Keys.Escape, 350);
            //BotHelper.InputIdle();
            Opened = !Opened;
        }

    }
}
