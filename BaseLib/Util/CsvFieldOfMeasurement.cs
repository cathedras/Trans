namespace myzy.Util
{
    /// <summary>
    /// 测试项信息
    /// </summary>
    public class CsvFieldOfMeasurement
    {
        public string FieldName { get; set; }
        public string Value { get; set; }
        public string Upper { get; set; }
        public string Lower { get; set; }
        public string Unit { get; set; }

        public CsvFieldOfMeasurement()
        {
            FieldName = string.Empty;
            Value = string.Empty;
            Unit = string.Empty;
            Lower = string.Empty;
            Upper = string.Empty;
        }
    }
}