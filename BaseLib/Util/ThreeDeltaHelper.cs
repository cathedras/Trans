using System;
using System.Collections.Generic;
using System.Linq;

namespace myzy.Util
{
    public class ValuePair
    {
        public Double Voltage { get; set; }
        public int CountV { get; set; }
        public ValuePair()
        {
        }

        public ValuePair(Double voltage, int countV)
        {
            this.Voltage = voltage;
            this.CountV = countV;
        }
    }

    public class ThreeDeltaHelper
    {
        List<ValuePair> listVoltageCount;
        double average = 0.0;
        int _badDataCount = -1;//奇异值个数

        /// <summary>
        /// 获取奇异值个数
        /// </summary>
        public int BadDataCount
        {
            get { return _badDataCount; }
        }

        public ThreeDeltaHelper(List<ValuePair> list)
        {
            this.listVoltageCount = list;
            SetAverage();
        }

        /// <summary>
        /// 取得平均电压值
        /// </summary>
        /// <returns></returns>
        protected double GetAvgVoltage()
        {
            double avg = 0;
            double total = 0;
            int allCount = 0;
            foreach (ValuePair vc in listVoltageCount)
            {
                double v = vc.Voltage;
                int c = vc.CountV;
                total += v * c;
                allCount += c;
            }
            avg = total / (allCount * 1.0);

            return Math.Round(avg, 3, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// 平均值
        /// </summary>
        /// <returns></returns>
        void SetAverage()
        {
            this.average = GetAvgVoltage();
        }

        /// <summary>
        /// 标准差
        /// </summary>
        /// <returns></returns>
        double StandardDeviation()
        {
            var listDataV = new List<double>();
            foreach (ValuePair vc in this.listVoltageCount)
            {
                double v = vc.Voltage;
                int countV = vc.CountV;
                for (int i = 0; i < countV; i++)
                {
                    listDataV.Add((v - this.average) * (v - this.average));
                }
            }
            double sumDataV = listDataV.Sum();
            double std = Math.Sqrt(sumDataV / (listDataV.Count - 1));

            return std;
        }

        public List<ValuePair> GetGoodList()
        {
            _badDataCount = 0;
            double sd3 = StandardDeviation() * 3;//3倍标准差
            List<ValuePair> listVC = new List<ValuePair>();
            foreach (ValuePair vc in this.listVoltageCount)
            {
                if (Math.Abs(vc.Voltage - this.average) <= sd3)
                {
                    listVC.Add(vc);
                }
                else
                {
                    _badDataCount += vc.CountV;
                }
            }

            return listVC;
        }

    }
}