using ComtradeCustomerServiceWinFormsApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ComtradeCustomerServiceWinFormsApplication.Services
{
    class CustomerApiService
    {

        private readonly HttpClient _httpClient;

        public CustomerApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<Identification>> GetListByNameAsync(string prefix)
        {
            var requestUrl = $"https://www.crcind.com/csp/samples/SOAP.Demo.cls?soap_method=GetListByName&name={prefix}";

            try
            {
                var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return ParseCustomerListFromXml(content);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching customer data: {ex.Message}");
                return null; 
            }
        }

        public async Task<Customer> FindPersonAsync(string customerId)
        {
            var requestUrl = $"https://www.crcind.com/csp/samples/SOAP.Demo.cls?soap_method=FindPerson&id={customerId}";

            try
            {
                var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return ParseCustomerFromXml(content);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching customer data: {ex.Message}");
                return null; 
            }
        }

        private Customer ParseCustomerFromXml(string xmlContent)
        {
            var xDocument = XDocument.Parse(xmlContent);

            XNamespace ns = "http://tempuri.org";

            var customerElement = xDocument
                .Descendants(XName.Get("FindPersonResult", "http://tempuri.org"))
                .FirstOrDefault();

            if (customerElement != null)
            {
                return new Customer
                {
                    Name = customerElement.Element(ns + "Name")?.Value,
                    SSN = customerElement.Element(ns + "SSN")?.Value,
                    DateOfBirth = DateTime.Parse(customerElement.Element(ns + "DOB")?.Value),
                    HomeStreet = customerElement.Element(ns + "Home")?.Element(ns + "Street")?.Value,
                    HomeCity = customerElement.Element(ns + "Home")?.Element(ns + "City")?.Value,
                    HomeState = customerElement.Element(ns + "Home")?.Element(ns + "State")?.Value,
                    HomeZip = customerElement.Element(ns + "Home")?.Element(ns + "Zip")?.Value,
                    OfficeStreet = customerElement.Element(ns + "Office")?.Element(ns + "Street")?.Value,
                    OfficeCity = customerElement.Element(ns + "Office")?.Element(ns + "City")?.Value,
                    OfficeState = customerElement.Element(ns + "Office")?.Element(ns + "State")?.Value,
                    OfficeZip = customerElement.Element(ns + "Office")?.Element(ns + "Zip")?.Value
                };
            }

            return null;
        }

        private List<Identification> ParseCustomerListFromXml(string xmlContent)
        {
            var xDocument = XDocument.Parse(xmlContent);

            XNamespace ns = "http://tempuri.org";

            var personIdentifications = xDocument.Descendants(ns + "PersonIdentification")
                .Select(person => new
                {
                    ID = person.Element(ns+"ID")?.Value,
                    Name = person.Element(ns+"Name")?.Value,
                    SSN = person.Element(ns+"SSN")?.Value,
                    DOB = person.Element(ns+"DOB")?.Value
                }).ToList();

            List<Identification> customerIdentifications = new List<Identification>();

            foreach (var person in personIdentifications)
            {
                customerIdentifications.Add(new Identification(person.ID, person.Name,  person.SSN ));
            }

            if (customerIdentifications.Count != 0)

                return customerIdentifications;
            else return null;
        }

    }
}
