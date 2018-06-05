using myzy.Util;

namespace TspUtil
{
    public class Gbl : IntrinsicCfg
    {
        public Gbl()
        {
            ClientHeight = 640;
            ClientWidth = 1024;

            ExpPixWidth = 828;
            ExpPixHeight = 1792;

            OddRgbA = "BGR";
            EvenRgbA = "BGR";
            PadStr = "00";
            PadLoc = $"{TspUtil.PadLoc.Left}";
            MaskCount = 4;
        }
        public int ClientWidth { get; set; }
        public int ClientHeight  { get; set; }

        public int  ExpPixWidth { get; set; }
        public int ExpPixHeight { get; set; }
        public string OddRgbA { get; set; }
        public string EvenRgbA { get; set; }
        public string PadStr { get; set; }
        public string PadLoc { get; set; }

        public int MaskCount { get; set; }
    }
}