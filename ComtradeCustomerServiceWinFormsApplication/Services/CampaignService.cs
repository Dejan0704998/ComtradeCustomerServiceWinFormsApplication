using CampaignContract;
using CampaignContract.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComtradeCustomerServiceWinFormsApplication.Services
{
    class CampaignService : ICampaignService
    {

        private readonly List<Customer>[] _rewardedCustomers = new List<Customer>[7];

        private const int DailyLimit = 5;

        private const int NumOfDays = 7;

        public CampaignService() {

            for (int i = 0; i < _rewardedCustomers.Length; i++)
            {
                _rewardedCustomers[i] = new List<Customer>();
            }
        }

        public StatusData RewardCustomer(Customer customer, int day)
        {
            StatusData status = new StatusData();

            if (!_rewardedCustomers.SelectMany(list => list).Where(x=>x.SSN==customer.SSN).Any() && _rewardedCustomers[day].Count < DailyLimit)
            {
                _rewardedCustomers[day].Add(customer);
                status.IsSuccessfull = true;
                return status;
            }
            else if (_rewardedCustomers[day].Count >= DailyLimit) 
            {
                status.IsSuccessfull = false;
                status.message = "Daily limit reached for " + day + ". day !";
                return status;
            }
            status.IsSuccessfull = false;
            status.message = "Customer " + customer.Name + " already rewarded!";
            return status;
        }

        public List<Customer> GetRewardedCustomers()
        {
            return _rewardedCustomers.SelectMany(list => list).ToList(); ;
        }

        public StatusData GenerateCsv(List<Customer> customers, string filePath)
        {
            var csvBuilder = new StringBuilder();

            csvBuilder.AppendLine("Name,SSN,DOB,Home Street, Home City, HomeState, Home Zip,Office Street,Office City, Office State, Office Zip");

            foreach (var customer in customers)
            {

                var line = string.Join(",",
                    EscapeCsvField(customer.Name),
                    EscapeCsvField(customer.SSN),
                    customer.DateOfBirth.ToShortDateString(),
                    EscapeCsvField(customer.HomeStreet),
                    EscapeCsvField(customer.HomeCity),
                    EscapeCsvField(customer.HomeState),
                    EscapeCsvField(customer.HomeZip),
                    EscapeCsvField(customer.OfficeStreet),
                    EscapeCsvField(customer.OfficeCity),
                    EscapeCsvField(customer.OfficeState),
                    EscapeCsvField(customer.OfficeZip)
                );

                csvBuilder.AppendLine(line);
            }

            StatusData status = new StatusData();

            try
            {
                File.WriteAllText(filePath, csvBuilder.ToString());
                status.IsSuccessfull = true;
                status.message = "ok";
            }
            catch (IOException ex)
            {
                
                Console.WriteLine($"The file is currently in use. Error: {ex.Message}");
                status.IsSuccessfull = false;
                status.message = $"The file is currently in use.Error: { ex.Message}";
            }

            return status;
        }

        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field)) return string.Empty;

            field = field.Replace("\"", "\"\"");

            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
            {
                field = $"\"{field}\"";
            }

            return field;
        }

    }
}
