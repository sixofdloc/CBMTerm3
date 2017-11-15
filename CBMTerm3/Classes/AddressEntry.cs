namespace CBMTerm3
{
    public class AddressEntry
    {
        public string SystemName {get; set;}
        public string Address { get; set; }
        public string Port { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            string s = "";
            s = SystemName.Replace("|", "%PIPE%")
                + "|" + Address.Replace("|", "%PIPE%")
                + "|" + Port.Replace("|", "%PIPE%")
                + "|" + Description.Replace("|", "%PIPE%");
            return s;
        }

        public AddressEntry() { }

        public AddressEntry(string s)
        {
            string[] t = s.Split('|');
            SystemName = t[0].Replace("%PIPE%", "|");
            Address = t[1].Replace("%PIPE%", "|");
            Port = t[2].Replace("%PIPE%", "|");
            Description = t[3].Replace("%PIPE%", "|");
        }

    }
}
