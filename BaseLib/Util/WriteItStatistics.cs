using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using log4net;
using myzy.Util.Annotations;

namespace myzy.Util
{
    /// <summary>
    /// 统计项
    /// </summary>
    public class StattisticsItem : INotifyPropertyChanged
    {
        private int _channelId;
        private int _total;
        private int _errCount;
        private double _yield;

        public StattisticsItem(int channelId)
        {
            ChannelId = channelId;
        }

        public int ChannelId
        {
            get { return _channelId; }
            private set
            {
                if (value == _channelId) return;
                _channelId = value;
                OnPropertyChanged();
            }
        }

        public int Total
        {
            get { return _total; }
            set
            {
                if (value == _total) return;
                _total = value;
                OnPropertyChanged();
            }
        }

        public int ErrCount
        {
            get { return _errCount; }
            set
            {
                if (value == _errCount) return;
                _errCount = value;
                OnPropertyChanged();
            }
        }

        public double Yield
        {
            get { return _yield; }
            set
            {
                if (value.Equals(_yield)) return;
                _yield = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// 通道数据统计信息
    /// </summary>
    public class StatisticsManage : ObservableCollection<StattisticsItem>
    {
        private readonly int _count;
        private double _yieldAll;

        public StatisticsManage(int count)
        {
            _count = count;
            for (int i = 0; i < _count; i++)
            {
                Add(new StattisticsItem(i));
            }
        }

        public double YieldAll
        {
            get { return _yieldAll; }
            set
            {
                if (value.Equals(_yieldAll)) return;
                _yieldAll = value;
                OnPropertyChanged(new PropertyChangedEventArgs("YieldAll"));
            }
        }

        private static readonly ILog _log = LogManager.GetLogger("exlog");

        public void AddSample(int channelIdx, TestResult result)
        {
            int errCount = result == TestResult.Pass ? 0 : 1;
            var item = this[channelIdx];
            if (item != null)
            {
                item.Total++;
                item.ErrCount += errCount;
            }
            else
            {
                _log.Error($"Channel Idx {channelIdx} Is Err.");
            }
            CalcStatics();
        }

        public void ClearStatics(int channelIdx)
        {
            var item = this[channelIdx];
            if (item != null)
            {
                item.ErrCount = 0;
                item.Total = 0;
            }
            else
            {
                _log.Error($"Channel Idx {channelIdx} Is Err.");
            }
            CalcStatics();
        }

        private void CalcStatics()
        {
            for (int i = 0; i < _count; i++)
            {
                var item = this[i];
                if (item.Total != 0)
                {
                    item.Yield = (item.Total - item.ErrCount) / (double)item.Total;
                }
                else
                {
                    item.Yield = 0d;
                }
            }

            var total = this.Sum(p => p.Total);
            if (total == 0)
            {
                YieldAll = 0d;
            }
            else
            {
                var errTotal = this.Sum(p => p.ErrCount);
                YieldAll = (total - errTotal) / (double)total;
            }
        }
    }
}