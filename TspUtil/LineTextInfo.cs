using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TspUtil
{
    public class LineTextInfo
    {
        private string _text;
        private int _lineNum;
        private uint _marker;

        public LineTextInfo(string text, int lineNum, uint marker)
        {
            this._text = text;
            this._lineNum = lineNum;
            this._marker = marker;
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public int LineNum
        {
            get { return _lineNum; }
            set { _lineNum = value; }
        }

        public uint Marker
        {
            get { return _marker; }
            set { _marker = value; }
        }
    }
}
