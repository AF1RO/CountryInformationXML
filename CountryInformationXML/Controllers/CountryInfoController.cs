using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using CountryInformationXML.Models;

namespace CountryInformationXML.Controllers
{
    public class CountryInfoController : Controller
    {
        private readonly string _serviceUrl = "http://webservices.oorsprong.org/websamples.countryinfo/CountryInfoService.wso?op=FullCountryInfo";
        public ActionResult Index()
        {
            return View(new CountryInfoRequest());
        }

        [HttpPost]
        public async Task<ActionResult> Index(CountryInfoRequest request)
        {
            if (!string.IsNullOrEmpty(request.CountryIsoCode))
            {
                var xmlResponse = await GetCountryInfoXml(request.CountryIsoCode);
                request.XmlResult = xmlResponse;
                ExtractCountryInfoFromXml(xmlResponse, request);
            }

            return View(request);
        }
        private async Task<string> GetCountryInfoXml(string countryIsoCode)
        {
            var soapEnvelope = $@"<?xml version=""1.0"" encoding=""utf-8""?>
                <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                               xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                  <soap:Body>
                    <FullCountryInfo xmlns=""http://www.oorsprong.org/websamples.countryinfo"">
                      <sCountryISOCode>{countryIsoCode}</sCountryISOCode>
                    </FullCountryInfo>
                  </soap:Body>
                </soap:Envelope>";

            using (var httpClient = new HttpClient())
            {
                var httpContent = new StringContent(soapEnvelope, System.Text.Encoding.UTF8, "text/xml");

                var response = await httpClient.PostAsync(_serviceUrl, httpContent);
                return await response.Content.ReadAsStringAsync();
            }
        }

        private void ExtractCountryInfoFromXml(string xmlResponse, CountryInfoRequest request)
        {
            XDocument doc = XDocument.Parse(xmlResponse);

            XNamespace ns = "http://www.oorsprong.org/websamples.countryinfo";

                request.CountryIsoCode = doc.Descendants(ns + "sISOCode").FirstOrDefault()?.Value ?? "Unknown";
                request.CountryName = doc.Descendants(ns + "sName").FirstOrDefault()?.Value ?? "Unknown";
                request.CapitalCity = doc.Descendants(ns + "sCapitalCity").FirstOrDefault()?.Value ?? "Unknown";
                request.PhoneCode = doc.Descendants(ns + "sPhoneCode").FirstOrDefault()?.Value ?? "Unknown";
                request.Continent = doc.Descendants(ns + "sContinentCode").FirstOrDefault()?.Value ?? "Unknown";
                request.Currency = doc.Descendants(ns + "sCurrencyISOCode").FirstOrDefault()?.Value ?? "Unknown";
                //Won't display the flag in the index.cshtml, something is blocking it. I'll find a fix eventually (probably not (or maybe))
                request.CountryFlag = doc.Descendants(ns + "sCountryFlag").FirstOrDefault()?.Value ?? "Unknown";
                var languageElements = doc.Descendants(ns + "tLanguage");

                foreach (var languageElement in languageElements)
                {
                    var language = new Language
                    {
                        IsoCode = languageElement.Element(ns + "sISOCode")?.Value ?? "Unknown",
                        Name = languageElement.Element(ns + "sName")?.Value ?? "Unknown"
                    };

                    request.Languages.Add(language);
                }
        }
    }
}
