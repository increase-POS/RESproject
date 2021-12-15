﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant
{
    class Global
    {
        public const string APIKey = "1234";
        //public const string APIUri = "http://141.95.1.58:4747/api/";

        public const string APIUri = "http://192.168.1.5:44370/api/";
        //public const string APIUri = "http://141.95.1.58:44/api/";



        public static string ScannedImageLocation = "Thumb/Scan/scan.jpg";
        public const string TMPFolder = "Thumb";
        public const string TMPItemsFolder = "Thumb/items"; // folder to save items photos locally 
        public const string TMPAgentsFolder = "Thumb/agents"; // folder to save agents photos locally 
        public const string TMPUsersFolder = "Thumb/users"; // folder to save users photos locally 
        public const string TMPSettingFolder = "Thumb/setting"; // folder to save Logo photos locally 
        public const string TMPCardsFolder = "Thumb/cards"; // folder to save Logo photos locally 
    }
}