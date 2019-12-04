using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElCommon.Util;

namespace TspUtil
{
    public class SaveFileList:IXmlDbListObj
    {
        public int _index;
        public bool IsActived
        {
            get;
            set;
           
        }

        public string Des
        {
            get;
            set;
           
        }

        public string FnPath
        {
            get;
            set;
        }

        public string Cs
        {
            get;
            set;
        }

        public string UniKey => $"{_index}";
    }
     public class ProgrammeFileList:IXmlDbListObj
    {
        public int _index;
        public bool IsActived
        {
            get;
            set;
           
        }

        public string Des
        {
            get;
            set;
           
        }

        public string FnPath
        {
            get;
            set;
        }

        public string Cs
        {
            get;
            set;
        }

        public string UniKey => $"{_index}";
    }



}
