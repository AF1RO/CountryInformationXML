namespace CountryInformationXML.Models
{
    public class CountryInfoRequest
    {
        public string CountryIsoCode { get; set; }
        public string XmlResult { get; set; }
        public string CountryName { get; set; }
        public string CapitalCity { get; set; }
        public string PhoneCode { get; set; }
        public string Continent { get; set; }
        public string Currency { get; set; }
        public string CountryFlag { get; set; }
        public string LanguageIsoCode { get; set; }
        public string LanguageName { get; set; }

        public List<Language> Languages { get; set; } = new List<Language>();
    }
}
