using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using System.Resources;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Shapes;
using TspUtil.ViewModel;

namespace TspUtil
{
    public class AlgProgmFiles
    {
        private Dictionary<string, AlgCmd> _algMap;
        private EditorContent _editContent;
        private string _curProgmFile;

        public AlgProgmFiles(string curProgmFile, EditorContent editContent, Dictionary<string, AlgCmd> algMap)
        {
            _curProgmFile = curProgmFile;
            _algMap = algMap;
            _editContent = editContent;
        }

        public AlgProgmFiles()
        {
        }

        public List<byte[]> CompileLine(string lineText,int lineNum)
        {
            var data = new List<byte[]>();
            var cmd = string.Empty;
            try
            {
                var rgx = new Regex("(0[xX])([0-9a-fA-F]+)");
                var numberRegx = new Regex("\\d+");
                var percentage = new Regex("\\d+%");
                var kNum = new Regex("\\d+kHz", RegexOptions.IgnoreCase);
                var strParam = new Regex("\\dlane", RegexOptions.IgnoreCase);
                var strBit = new Regex("\\d+bit", RegexOptions.IgnoreCase);
                var tmp = new List<byte>();
                var curData = new List<byte>();


                if (!lineText.StartsWith("//") && !string.IsNullOrEmpty(lineText) && lineText!="\r\n") //代表数据行
                {
                    var finish = lineText.IndexOf(";");
                    var ann = lineText.IndexOf("//");
                    var stringIndex = lineText.IndexOf('\"',0);
                    var last = lineText.LastIndexOf('\"');
                    if (finish > ann && finish > 0 && ann > 0)
                    {
                        lineText = lineText.Substring(0, ann).Trim();
                    }
                    else if (finish < ann && finish > 0 && ann > 0)
                    {
                        lineText = lineText.Substring(0, finish).Trim();
                    }
                    else if (ann > 0)
                    {
                        lineText = lineText.Substring(0, ann).Trim();
                    }
                    else if (finish > 0)
                    {
                        lineText = lineText.Substring(0, finish).Trim();
                    }
                    else
                    {
                        lineText = lineText.Replace("\r\n", " ").Trim();
                    }


                    var allData = new List<string>();
                    var stringStr = string.Empty;
                   // var allDataLen = 0;
                    if (stringIndex > 0)
                    {
                        stringStr = lineText.Substring(stringIndex, last - stringIndex+1);
                        lineText = lineText.Substring(0, stringIndex - 1);
                    }
               
                    var para = lineText.Split(' ');
                    Array.ForEach(para, p =>
                    {
                        if (!string.IsNullOrEmpty(p) && p != "\t")
                        {
                            allData.Add(p);
                        }
                    });
                    cmd = allData[0].Trim().Replace('.', '_');

                    if (allData.Count > 1) //有数据产生
                    {
                        if (cmd.Contains("driver_register_read"))
                        {
                            cmd = "ssd_read";
                        }
                        else if (cmd.Contains("driver_register_write"))
                        {
                            cmd = "ssd_write";
                        }
                        else if (cmd.Contains("mipi_pll"))
                        {
                            cmd = "ssd_pll";
                        }
                        else if (cmd.Contains("mipi_order"))
                        {
                            cmd = "ssd_write";
                        }
                        else if (cmd.Contains("mipi_interface"))
                        {
                            cmd = "ssd_write";
                        }
                        else if (cmd.Contains("mipi_dsi"))
                        {
                            cmd = "ssd_write";
                        }
                        else if (cmd.Contains("driver_power"))
                        {
                            cmd = "ssd_powermode";
                        }


                        _algMap.TryGetValue(cmd, out AlgCmd val);
                        if (val == null)
                        {
                            tmp.AddRange(SpecialData(lineText));
                        }
                        else
                        {
                            tmp.AddRange(ProgrammeUtil.ParserCmdHL(val.CmdData));
                            var tmpValue = 0;
                            for (int i = 1; i < allData.Count; i++)
                            {
                                var match = rgx.Match(allData[i]);
                                var number = numberRegx.Match(allData[i]);
                                var percent = percentage.Match(allData[i]);
                                var k = kNum.Match(allData[i]);
                                var param = strParam.Match(allData[i]);
                                var bit = strBit.Match(allData[i]);

                                if (match.Success) //hex
                                {
                                    var str = match.Groups;

                                    if (str[2].Value.Length < 4)
                                    {
                                        var num = int.Parse(str[2].Value,
                                            System.Globalization.NumberStyles.AllowHexSpecifier);
                                        tmp.Add((byte) num);
                                    }
                                    else
                                    {
                                        var num = int.Parse(str[2].Value,
                                            System.Globalization.NumberStyles.AllowHexSpecifier);
                                        tmp.AddRange(ProgrammeUtil.ParserDataLH(num));
                                    }
                                }
                                else if (allData[i].ToUpper().Contains("ch1ch2ch3ch4")) //ch1ch2ch3ch4
                                {
                                    tmp.AddRange(
                                        ProgrammeUtil.ParserDataLH(int.Parse(allData[i].LastOrDefault()
                                            .ToString())));
                                }
                                else if (percent.Success)
                                {
                                    var str = number.Groups;
                                    int.TryParse(str[0].Value, out int num);

                                    if (cmd.Contains("pwm"))
                                    {
                                        var pwmV = (tmpValue * num) / 100;
                                        tmp.AddRange(ProgrammeUtil.ParserDataLH(pwmV));
                                    }
                                }
                                else if (k.Success)
                                {
                                    var str = number.Groups;
                                    int.TryParse(str[0].Value, out int num);
                                    if (cmd.Contains("pwm"))
                                    {
                                        var pwmV = 1000 / num;
                                        tmpValue = pwmV;
                                        tmp.AddRange(ProgrammeUtil.ParserDataLH(pwmV));
                                    }
                                }
                                else if (number.Success && !param.Success && !bit.Success) //number
                                {
                                    var str = number.Groups;
                                    int.TryParse(str[0].Value, out int num);
                                    if (num > byte.MaxValue)
                                    {
                                        tmp.AddRange(ProgrammeUtil.ParserDataLH(num));
                                    }
                                    else
                                    {
                                        if (cmd.Contains("delay_s"))
                                        {
                                            tmp.AddRange(ProgrammeUtil.ParserDataLH(num));
                                        }
                                        else if (cmd.Contains("delay_ms"))
                                        {
                                            tmp.AddRange(ProgrammeUtil.ParserDataLH(num));
                                        }
                                        else if (cmd.Contains("delay_us"))
                                        {
                                            tmp.AddRange(ProgrammeUtil.ParserDataLH(num));
                                        }
                                        else
                                        {
                                            tmp.AddRange(ProgrammeUtil.ParserDataLH(num));
                                        }
                                    }
                                }
                                else if (allData[i].ToUpper().Contains("RGB")) //bgr
                                {
                                    tmp.AddRange(ProgrammeUtil.ParserDataLH(0x00d6));
                                    tmp.AddRange(ProgrammeUtil.ParserDataLH(0x01fd));
                                    tmp.AddRange(ProgrammeUtil.ParserDataLH(0x1000));
                                }
                                else if (allData[i].ToUpper().Contains("BGR")) //rgb
                                {
                                    tmp.AddRange(ProgrammeUtil.ParserDataLH(0x00d6));
                                    tmp.AddRange(ProgrammeUtil.ParserDataLH(0x01fc));
                                    tmp.AddRange(ProgrammeUtil.ParserDataLH(0x1000));
                                }
                                else if (param.Success) //lane
                                {
                                    var datas = allData[i];
                                    tmp.AddRange(ProgrammeUtil.ParserDataLH(0x00de));
                                    if ("1lane".Contains(allData[i]))
                                    {
                                        tmp.AddRange(ProgrammeUtil.ParserDataLH(tmpValue & 0xfff0));
                                    }
                                    else if ("2lane".Contains(allData[i]))
                                    {
                                        tmp.AddRange(ProgrammeUtil.ParserDataLH(tmpValue & 0xfff3));
                                    }
                                    else if ("3lane".Contains(allData[i]))
                                    {
                                        tmp.AddRange(ProgrammeUtil.ParserDataLH(tmpValue & 0xfffa));
                                    }
                                    else if ("4lane".Contains(allData[i]))
                                    {
                                        tmp.AddRange(ProgrammeUtil.ParserDataLH(tmpValue & 0xffff));
                                    }

                                    tmp.Add(0);
                                    tmp.Add(0);
                                }
                                else if (bit.Success) //bit
                                {
                                    var datas = allData[i];
                                    tmp.AddRange(ProgrammeUtil.ParserDataLH(0x00b6));
                                    if ("16bit".Contains(allData[i]))
                                    {
                                        tmp.AddRange(ProgrammeUtil.ParserDataLH(tmpValue & 0xfffc));
                                    }
                                    else if ("18bit".Contains(allData[i]))
                                    {
                                        tmp.AddRange(ProgrammeUtil.ParserDataLH(tmpValue & 0xfffd));
                                    }
                                    else if ("24bit".Contains(allData[i]))
                                    {
                                        tmp.AddRange(ProgrammeUtil.ParserDataLH(tmpValue & 0xffff));
                                    }

                                    tmp.AddRange(ProgrammeUtil.ParserDataLH(2));
                                }
                                else if (allData[i].ToUpper().Contains("SINGLEMODE")) //enable
                                {
                                    tmpValue = 0x0023;
                                }
                                else if (allData[i].ToUpper().Contains("DUALMODE")) //enable
                                {
                                    tmpValue = 0x003f;
                                }
                                else if (allData[i].ToUpper().Contains("EXTERNAL")) //enable
                                {
                                    tmp.Add(2);
                                }
                                else if (allData[i].ToUpper().Contains("ENABLE")) //enable
                                {
                                    tmp.Add(1);
                                }
                                else if (allData[i].ToUpper().Contains("DISABLE")) //disable
                                {
                                    tmp.Add(0);
                                }
                                else if (allData[i].ToUpper().Contains("COLORBAR")) //colorbar
                                {
                                    tmp.Add(0x10);
                                }
                                else if (allData[i].ToUpper().Contains("SIGLECOLOR")) //colorbar
                                {
                                    tmp.Add(0x00);
                                }
                                else if (allData[i].ToUpper().Contains("OE")) //colorbar
                                {
                                    tmpValue = 0x102f;
                                }
                                else if (allData[i].ToUpper().Contains("LR")) //colorbar
                                {
                                    tmpValue = 0x142f;
                                }
                                else if (allData[i].ToUpper().Contains("BD")) //colorbar
                                {
                                    tmpValue = 0x182f;
                                }
                                else if (allData[i].ToUpper().Contains("NONBURSTPULSES")) //colorbar
                                {
                                    tmpValue = tmpValue & 0xfff3;
                                }
                                else if (allData[i].ToUpper().Contains("NONBURSTEVENTS")) //colorbar
                                {
                                    tmpValue = tmpValue & 0xfff7;
                                }
                                else if (allData[i].ToUpper().Contains("BURSTMODE")) //colorbar
                                {
                                    tmpValue = tmpValue & 0xfffb;
                                }
                                else if (allData[i].ToUpper().Contains("DOWN"))
                                {
                                    tmp.Add(0);
                                }
                                else if (allData[i].ToUpper().Contains("ALL"))
                                {
                                    tmp.Add(0);
                                }
                                else if (allData[i].ToUpper().Contains("USART"))
                                {
                                    tmp.Add(1);
                                }
                                else if (allData[i].ToUpper().Contains("DEBUG"))
                                {
                                    tmp.Add(2);
                                }
                                else if (allData[i].ToUpper().Contains("ETH"))
                                {
                                    tmp.Add(3);
                                }
                                else if (allData[i].ToUpper().Contains("USB"))
                                {
                                    tmp.Add(4);
                                }
                                else if (allData[i].ToUpper().Contains("CPHY"))
                                {
                                    tmp.Add(0);
                                }
                                else if (allData[i].ToUpper().Contains("DPHY"))
                                {
                                    tmp.Add(1);
                                }
                                
                            }
                        }

                    }
                    else //无数据产生
                    {
                        _algMap.TryGetValue(cmd, out AlgCmd val);
                        if (val != null)
                        {
                            tmp.AddRange(ProgrammeUtil.ParserCmdHL(val.CmdData));
                        }
                        else
                        {
                            tmp.AddRange(SpecialData(cmd));

                        }
                    }

                    if (stringStr.StartsWith("\"") && stringStr.EndsWith("\"")) //string
                    {
                        var str = stringStr.Substring(1, stringStr.LastIndexOf('\"') - 1);
                        var index = 0;
                        if (str.Contains("\\r\\n"))
                        {
                            index = str.LastIndexOf("\\r\\n");
                            str = str.Substring(0, index);
                            tmp.AddRange(ProgrammeUtil.StringToByteArray(str));
                            tmp.Add(0x0d);
                            tmp.Add(0x0a);
                        }
                        else if (str.Contains("\\n"))
                        {
                            index = str.LastIndexOf("\\r");
                            str = str.Substring(0, index);
                            tmp.AddRange(ProgrammeUtil.StringToByteArray(str));
                            tmp.Add(0x0d);
                        }
                        else if (str.Contains("\\r"))
                        {
                            index = str.LastIndexOf("\\n");
                            str = str.Substring(0, index);
                            tmp.AddRange(ProgrammeUtil.StringToByteArray(str));
                            tmp.Add(0x0a);
                        }
                        else
                        {
                            tmp.AddRange(ProgrammeUtil.StringToByteArray(str));
                        }
                        stringStr = String.Empty;
                    }
                }
                else if (string.IsNullOrEmpty(lineText)|| lineText == "\r\n") //空行
                {
                    cmd = "next_line";
                    _algMap.TryGetValue(cmd, out AlgCmd val);
                    if (val != null)
                    {
                        tmp.AddRange(ProgrammeUtil.ParserCmdHL(val.CmdData));
                    }
                    else
                    {
                        App.Locator.Main.AddLogMsg("未查找到该命令");
                    }
                }
                else if (lineText.StartsWith("//")) //注释行
                {
                    cmd = "comment";
                    _algMap.TryGetValue(cmd, out AlgCmd val);
                    if (val != null)
                    {
                        tmp.AddRange(ProgrammeUtil.ParserCmdHL(val.CmdData));
                        tmp.AddRange(ProgrammeUtil.StringToByteArray(lineText));
                    }
                    else
                    {
                        App.Locator.Main.AddLogMsg("未查找到该命令");
                    }
                }
               

                curData.Add((byte) tmp.Count);
                curData.AddRange(tmp);
                if (curData.Any())
                {
                    data.Add(curData.ToArray());
                }

            }
            catch (Exception ex)
            {
                App.Locator.Main.AddLogMsg($"文件:{_curProgmFile},第{lineNum}行，命令为：{cmd}， 产生错误：{ex.Message}");
            }

            return data;
        }

        private byte[] SpecialData(string cmd)
        {
            var lst = new List<byte>();
            AlgCmd val = null;
            byte[] tmpData = null;
            var ssdrst = "ssd_rest";
            switch (cmd)
            {
                case "driver_reset":
                    _algMap.TryGetValue(ssdrst, out val);
                    tmpData = new byte[] { };
                    break;

            }
            if (val != null)
            {
                lst.AddRange(ProgrammeUtil.ParserCmdHL(val.CmdData));
                lst.AddRange(tmpData);
            }
            return lst.ToArray();
        }

        public List<byte[]> CompileFile(string fname, int fileCount,out byte[] checkSum)
        {
            List<byte[]> contentAndLast = new List<byte[]>();//添加文件内容和最后一行
            List<byte[]> newAllLine = new List<byte[]>();//所有行
            List<byte> tmp = new List<byte>();//临时使用无意义
            List<byte> lastLst = new List<byte>();//最后一行的结束命令
            List<byte> zeroLine = new List<byte>();//第一行的checksum和文件名，约定为{。。。。|长度 命令 命令 32bit的checksum 文件名}
            var lineResult = new List<byte[]>();
            _editContent = App.Locator.TextModal.CurFileEdit(fname);
            _algMap = App.Locator.Main.AlgMap;
            if (_editContent != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var line in _editContent.Editor.Lines)
                    {
                        var oneLine = CompileLine(line.Text, _editContent.Editor.CurrentLine);
                        contentAndLast.AddRange(oneLine);
                    }
                });
               

                _algMap.TryGetValue("file_end", out AlgCmd end);
                tmp.AddRange(ProgrammeUtil.ParserCmdHL(end.CmdData));
                lastLst.Add((byte)tmp.Count);
                lastLst.AddRange(tmp);
                contentAndLast.Add(lastLst.ToArray());
                tmp.Clear();



                _algMap.TryGetValue("file_name", out AlgCmd start);
                tmp.AddRange(ProgrammeUtil.ParserCmdHL(start.CmdData));
                checkSum = GetNewCheckSum(contentAndLast);
                var fileId = ProgrammeUtil.StringToByteArray((fileCount).ToString());
                tmp.AddRange(fileId);
                var linesCount = ProgrammeUtil.StringToByteArray((contentAndLast.Count + 1).ToString("000"));
                tmp.AddRange(linesCount);
                foreach (var b in checkSum)
                {
                    tmp.AddRange(ProgrammeUtil.StringToByteArray($"{b.ToString("x2")}"));
                }
                tmp.AddRange(ProgrammeUtil.StringToByteArray(fname));
                zeroLine.Add((byte)tmp.Count);
                zeroLine.AddRange(tmp);
                newAllLine.Add(zeroLine.ToArray());
                newAllLine.AddRange(contentAndLast.ToArray());

                for (int i = 0; i < newAllLine.Count; i++)
                {
                    var lineStr = (i).ToString("000");
                    lineResult.Add(ProgrammeUtil.ParseCmd("WL", $"{fileCount}", $"{lineStr}", newAllLine[i].ToArray()));
                }
            }
            else
            {
                checkSum = Encoding.ASCII.GetBytes("00000000");
            }
           
           
            return lineResult;
        }

        public List<List<byte[]>> CompileAllFiles(ObservableCollection<ImgItemInfo> fileCoList)
        {
            int index = 0;
            var allFilesLines = new List<List<byte[]>>();
           
            foreach (var file in fileCoList)
            {
                if (file.IsActived)
                {
                    var curFile = CompileFile(file.Des, index,out byte[] checkSum);
                    allFilesLines.Add(curFile);
                    file.FileIndex = index;
                    index++;
                    file.Cs = string.Join("",Array.ConvertAll(checkSum, input => $"{input:X2}"));
                    App.Locator.KeyBind.ParamLst.Add(new KeyBindParam($"File_{index}_{file.Des}",$"0x000{index}"));
                }
            }
            return allFilesLines;
        }




        public byte[] GetNewCheckSum(List<byte[]> sourceData)
        {
            uint crc = 0xffffffff;
            Func<byte[], uint> act = (src) =>
            {
                for (int num=0; num < src.Length; num++)              /* Step through bytes in memory */
                {
                    crc = crc + src[num];     /* Fetch byte from memory, XOR into CRC top byte*/
                    crc &= 0xFFFFFFFF;                  /* Ensure CRC remains 16-bit value */
                }                               /* Loop until num=0 */
                return (crc);                    /* Return updated CRC */
            };
            byte[] CheckSum = new byte[4];
            var checksumData = new List<byte>();
            for (int i = 0; i < sourceData.Count; i++)
            {
                act(sourceData[i]);
            }
            checksumData.AddRange(BitConverter.GetBytes(crc));
            if (checksumData.Any())
            {
                CheckSum[0] = checksumData[0]; CheckSum[1] = checksumData[1];
                CheckSum[2] = checksumData[2]; CheckSum[3] = checksumData[3];
            }
            return CheckSum;
        }
    }
}
