using System;

namespace RedButton.Common.TeklaStructures.CSLib
{
    public static class Constants
    {
        public const string MATERIAL_STEEL = "STEEL";
        public const string MATERIAL_CONCRETE = "CONCRETE";
        public const string MATERIAL_MISCELANEOUS = "MISCELLANEOUS";
        public const string MATERIAL_TIMBER = "TIMBER";
        public const double DRAW_PLANE_POINTS_DISTANCE = 1500.0;
        public const string PLACE_OF_POINT = "...";
        public const string ENGLISH_CULTURE = "en-US";
        public const string CZECH_CULTURE = "cs-CZ";
        public const string INP_ABOUT_DIALOG = @"         {
              attribute (_, ""16.12.2010"", label, , , none, , , 258, 20)     
              attribute (_, j_date,     label, , , none, , , 208, 20)     
              attribute (_, TS 18.0, label, , , none, , , 138, 20)        
              attribute (_, j_version,  label, , , none, , ,  63, 20)     
              
              picture(""cs_net_j001_construsoft_logo"", 350, 320, 5, 50)  
         }
";
        public static readonly double DEF = (double)int.MinValue;
        public static readonly double CS_EPSILON = 0.01;
        public static readonly string DEF_STR = "";
        public static readonly int CLASS_CONCRETE = 6;
        public static readonly double PI = Math.PI;
        public static readonly double RAD_TO_DEG = 180.0 / PI;
        public static readonly double DEG_TO_RAD = 1.0 / RAD_TO_DEG;

        public enum Rotation
        {
            FRONT,
            TOP,
            BACK,
            BELOW,
        }

        public enum Culture
        {
            ENGLISH,
            CZECH,
        }

        public enum ConnectionType
        {
            CREATE_NO_CONNECTION,
            WELD,
            CAST_UNIT,
            SUBASSEMBLY,
        }
    }
}