namespace GWDataCenter.Database
{
    public class SetParmTableRow
    {
        public int sta_n { get; set; }
        public int equip_no { get; set; }
        public int set_no { get; set; }
        public string set_nm { get; set; }
        public string set_type { get; set; }
        public string main_instruction { get; set; }
        public string minor_instruction { get; set; }
        public bool record { get; set; }
        public string action { get; set; }
        public string value { get; set; }
        public bool canexecution { get; set; }
        public string VoiceKeys { get; set; }
        public bool EnableVoice { get; set; }
        public int qr_equip_no { get; set; } = 0;
        public string Reserve1 { get; set; }
        public string Reserve2 { get; set; }
        public string Reserve3 { get; set; }
        public string set_code;
    }
}
