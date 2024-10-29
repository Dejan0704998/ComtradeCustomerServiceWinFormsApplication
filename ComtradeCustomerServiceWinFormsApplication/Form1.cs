﻿using ComtradeCustomerServiceWinFormsApplication.Models;
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

        private async void ComboBoxName_TextChanged(object sender, EventArgs e)
        {
            var input = comboBoxName.Text.ToLower();
            if (input.Length >= 2 && input.Length <= 12)
            {

                filteredClients = await customerApiService.GetListByNameAsync(input);

                var filteredNames = filteredClients?.Select(x => x.Name).ToList() ?? new List<string>();

                comboBoxName.Items.Clear(); 
                comboBoxName.Items.AddRange(filteredNames.ToArray()); 
                comboBoxName.DroppedDown = true; 
            }
            if (input.Length > 12) {
                var choosenClient = filteredClients.Where(y => y.Name == comboBoxName.Text.ToLower()).FirstOrDefault();
                if (choosenClient != null) {
                    choosenID = choosenClient.Id;
                }
            }
        }

        private async void AddDiscountForClient(object sender, EventArgs e) {
            
            var choosenClient = filteredClients.Where(y => y.Name == comboBoxName.Text);
            if (choosenClient != null)
            {
                choosenID = choosenClient.Select(x=>x.Id).FirstOrDefault();
            }
            
            var customer = await customerApiService.FindPersonAsync(choosenID);

            if (customer != null)
            {
                int day = int.Parse(comboBoxDay.Text);
                StatusData status = campaignService.RewardCustomer(customer, day);
                if (status.IsSuccessfull)
                {
                    Console.WriteLine($"Rewarded {customer.Name}");
                    InfoMessage.Text = $"Rewarded {customer.Name}";
                }
                else
                {
                    Console.WriteLine($"Failed to reward {customer.Name}. Daily limit reached or already rewarded.");
                    InfoMessage.Text = $"Failed to reward {customer.Name}. " + status.message;

                }
            }
            else
            {
                Console.WriteLine("Customer not found or error occurred.");
                InfoMessage.Text = "Customer not found or error occurred.";
            }
        }

        private void GenerateCSVreport(object sender, EventArgs e)
        {
            var rewardedCustomers = campaignService.GetRewardedCustomers();
            var csvFilePath = Path.Combine(Environment.CurrentDirectory, "RewardedCustomers.csv");

            GenerateCsv(rewardedCustomers, csvFilePath);

            Console.WriteLine($"CSV file generated at: {csvFilePath}");
            InfoMessage.Text = $"CSV file generated at: {csvFilePath}";
        }

        public static void GenerateCsv(List<Customer> customers, string filePath)
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
