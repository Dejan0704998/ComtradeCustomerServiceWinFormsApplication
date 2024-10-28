using ComtradeCustomerServiceWinFormsApplication.Models;
using ComtradeCustomerServiceWinFormsApplication.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComtradeCustomerServiceWinFormsApplication
{
    public partial class Form1 : Form
    {

        private string choosenID;
        private List<Identification> filteredClients;

        private CustomerApiService customerApiService;
        private CampaignService campaignService;

        public Form1()
        {
            InitializeComponent();
            comboBoxName.TextChanged += ComboBoxName_TextChanged;
            customerApiService = new CustomerApiService();
            campaignService = new CampaignService();
            //namesList
        }

        private void ComboBoxDay_TextChanged(object sender, EventArgs e)
        {

            var temporaryArray = new string[] { "1", "2", "3", "4", "5", "6", "7" };

            comboBoxDay.Items.Clear();
            comboBoxDay.Items.AddRange(temporaryArray);
            comboBoxDay.DroppedDown = true;
        }

        private async void addDiscountForClient(object sender, EventArgs e) {
            var choosenClient = filteredClients.Where(y => y.Name == comboBoxName.Text);
            if (choosenClient != null)
            {
                choosenID = choosenClient.Select(x=>x.Id).FirstOrDefault();
            }
            var customer = await customerApiService.FindPersonAsync(choosenID);
            if (customer != null)
            {
                if (campaignService.RewardCustomer(customer))
                {
                    Console.WriteLine($"Rewarded {customer.Name}");
                }
                else
                {
                    Console.WriteLine($"Failed to reward {customer.Name}. Daily limit reached or already rewarded.");
                }
            }
            else
            {
                Console.WriteLine("Customer not found or error occurred.");
            }


        }

        private void generateCSVreport(object sender, EventArgs e)
        {
            var rewardedCustomers = campaignService.GetRewardedCustomers();
            Console.WriteLine("Rewarded Customers:");
            foreach (var rewardedCustomer in rewardedCustomers)
            {
                Console.WriteLine($"{rewardedCustomer.Name} ({rewardedCustomer.Email})");
            }

            var csvFilePath = Path.Combine(Environment.CurrentDirectory, "RewardedCustomers.csv");
            GenerateCsv(rewardedCustomers, csvFilePath);

            Console.WriteLine($"CSV file generated at: {csvFilePath}");
        }

        public static void GenerateCsv(List<Customer> customers, string filePath)
        {
            var csvBuilder = new StringBuilder();

            csvBuilder.AppendLine("Name,SSN,DOB,Home Street, Home City, HomeState, Home Zip,Office Street,Office City, Office State, Office Zip, IsRewarded");

            foreach (var customer in customers)
            {
                //var line = $"{customer.Name},{customer.SSN},{customer.DateOfBirth.ToShortDateString()},{customer.OfficeStreet},{customer.OfficeCity},{customer.OfficeState},{customer.OfficeZip},{customer.HomeStreet},{customer.HomeCity},{customer.HomeState},{customer.HomeZip},{customer.Email},{customer.IsRewarded}";


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
                    EscapeCsvField(customer.OfficeZip),
                    customer.IsRewarded
                );

                csvBuilder.AppendLine(line);
            }

            File.WriteAllText(filePath, csvBuilder.ToString());
        }

        private static string EscapeCsvField(string field)
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
