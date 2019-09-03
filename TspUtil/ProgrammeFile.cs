using ICSharpCode.AvalonEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Document;

namespace TspUtil
{
    public class ProgrammeUtil
    {
        private static Dictionary<string, AlgCmd> _mapKey;
        public static Dictionary<string, AlgCmd> InitCmdMap()
        {
            CmdMap.Add("file_name", new AlgCmd("CMD_FILE_NAME", 0x80));
            CmdMap.Add("next_line", new AlgCmd("CMD_FILE_NEXTLINE", 0x81));
            CmdMap.Add("file_end", new AlgCmd("CMD_FILE_END", 0x82));

            CmdMap.Add("comment", new AlgCmd("CMD_COMMENT", 0x84));
            CmdMap.Add("print", new AlgCmd("CMD_PRINTF", 0x85));

            CmdMap.Add("delay_s", new AlgCmd("CMD_DELAY_S", 0x86));
            CmdMap.Add("delay_ms", new AlgCmd("CMD_DELAY_MS", 0x87));
            CmdMap.Add("delay_us", new AlgCmd("CMD_DELAY_US", 0x88));

            CmdMap.Add("ssd_powermode", new AlgCmd("CMD_SSD2832_POWERMODE", 0x90));
            CmdMap.Add("ssd_rest", new AlgCmd("CMD_SSD2832_RESET", 0x91));
            CmdMap.Add("ssd_pll", new AlgCmd("CMD_SSD2832_PLL", 0x92));
            CmdMap.Add("ssd_write", new AlgCmd("CMD_SSD2832_WRITE", 0x93));
            CmdMap.Add("ssd_read", new AlgCmd("CMD_SSD2832_READ", 0x94));

            
            CmdMap.Add("mipi_write", new AlgCmd("CMD_MIPI_WRITE", 0xA0));
            CmdMap.Add("mipi_read", new AlgCmd("CMD_MIPI_READ", 0xA1));
            CmdMap.Add("mipi_video", new AlgCmd("CMD_MIPI_VIDEO", 0xA2));

            CmdMap.Add("fpga_reset", new AlgCmd("CMD_FPGA_RESET", 0xB0));
            CmdMap.Add("fpga_timing", new AlgCmd("CMD_FPGA_WRITE", 0xB1));
            CmdMap.Add("fpga_read", new AlgCmd("CMD_FPGA_READ", 0xB2));

            CmdMap.Add("show_image", new AlgCmd("CMD_SHOW_IMAGE", 0xB8));
            CmdMap.Add("show_color", new AlgCmd("CMD_SHOW_COLOR", 0xB9));
            CmdMap.Add("show_special", new AlgCmd("CMD_SHOW_SPECIAL", 0xBa));

            CmdMap.Add("set_volt", new AlgCmd("CMD_VOLT_SET", 0xC0));
            CmdMap.Add("set_curr", new AlgCmd("CMD_CURR_SET", 0xC1));
            CmdMap.Add("get_volt", new AlgCmd("CMD_VOLT_GET", 0xC2));
            CmdMap.Add("get_curr", new AlgCmd("CMD_CURR_GET", 0xC3));

            CmdMap.Add("gpio_reset", new AlgCmd("CMD_GPIO_RESET", 0xD0));
            CmdMap.Add("gpio_on", new AlgCmd("CMD_GPIO_ON", 0xD1));
            CmdMap.Add("gpio_off", new AlgCmd("CMD_GPIO_OFF", 0xD2));

            CmdMap.Add("pwm", new AlgCmd("CMD_PWM", 0xE0));

            CmdMap.Add("iic_write", new AlgCmd("CMD_IIC_WRITE", 0xE5));
            CmdMap.Add("iic_read", new AlgCmd("CMD_IIC_READ", 0xE6));
            return CmdMap;
        }

        public static Dictionary<string, AlgCmd> CmdMap
        {
            get => _mapKey ?? (_mapKey = new Dictionary<string, AlgCmd>());
        }
        public static void DisplayValidationError(TextEditor editor, TextMarkerServices text, string message, int linePosition, int lineNumber)
        {
            if (lineNumber >= 1 && lineNumber <= editor.Document.LineCount)
            {
                int offset = editor.Document.GetOffset(new TextLocation(lineNumber, linePosition));
                int endOffset = TextUtilities.GetNextCaretPosition(editor.Document, offset, System.Windows.Documents.LogicalDirection.Forward, CaretPositioningMode.WordBorderOrSymbol);
                if (endOffset < 0)
                {
                    endOffset = editor.Document.TextLength;
                }
                int length = endOffset - offset;

                if (length < 2)
                {
                    length = Math.Min(2, editor.Document.TextLength - offset);
                }
                text.Create(offset, length, message);
            }
        }
        /// <summary>
        /// Make a byte array for the string
        /// </summary>
        /// <param name="command">The string what you need to transfer</param>
        /// <returns></returns>
        public static byte[] StringToByteArray(String command)
        {
            return System.Text.Encoding.Default.GetBytes(command);
        }

        public static byte[] ParseCmd(string cmd, string fileCount, string lineNum, byte[] data)
        {
            var cmdLst = new List<byte>();
            cmdLst.AddRange(StringToByteArray("{"));
            cmdLst.AddRange(StringToByteArray(cmd));
            cmdLst.AddRange(StringToByteArray(fileCount));
            cmdLst.AddRange(StringToByteArray(lineNum));
            if (data.Any())
            {
                cmdLst.AddRange(StringToByteArray($"|"));
                cmdLst.AddRange(data);
                cmdLst.AddRange(StringToByteArray("}"));
            }
            else
            {
                cmdLst.AddRange(StringToByteArray("}"));
            }

            return cmdLst.ToArray();
        }

        public static byte[] ParserCmdHL(int data)
        {
            var lst = new List<byte>();
            if (data > byte.MaxValue && data < Int16.MaxValue)
            {
                lst.Add((byte)(data >> 8));  
                lst.Add((byte)(data & 0xff));
            }
            else if(data <= byte.MaxValue)
            {
                lst.Add(0);   
                lst.Add((byte)(data & 0xff));
                
            }
            return lst.ToArray();
        }
        public static byte[] ParserDataLH(int data)
        {
            var lst = new List<byte>();
            if (data > byte.MaxValue && data < UInt16.MaxValue)
            {
                lst.Add((byte)(data & 0xff));
                lst.Add((byte)(data >> 8));
               
            }
            else if (data <= byte.MaxValue)
            {
                lst.Add((byte)(data & 0xff));
                lst.Add(0);
            }
            return lst.ToArray();
        }

        public static int ParseToInt(byte high,byte low)
        {
            var data = ((high & 0xff) << 8|(low & 0xff));
            return data;

        }
        public static int ParseToInverseInt(byte high, byte low)
        {
            var data = ((high & 0xff00) | (low & 0xff00) >> 8);
            return data;

        }
    }
    public class AlgCmd
    {
        private string _cmd;
        private byte _cmdData;

        public AlgCmd(string cmd, byte cmdData)
        {
            _cmd = cmd;
            _cmdData = cmdData;
        }
        public string Cmd { get => _cmd; }
        public byte CmdData { get => _cmdData; }
        
    }



    //public enum mipi
    //{

    //}
}
