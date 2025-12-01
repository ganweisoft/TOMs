namespace GWDataCenter.Database
{
    public class GWExProcCmdTableRow
    {
        public int proc_code { get; set; }
        public string cmd_nm { get; set; }
        public string main_instruction { get; set; }
        public string minor_instruction { get; set; }
        public string value { get; set; }
        public bool record { get; set; }
    }
}
