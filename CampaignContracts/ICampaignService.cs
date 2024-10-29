using CampaignContracts.Models;
using System;
using System.Collections.Generic;

namespace CampaignContracts
{

    public interface ICampaignService
    {

        public StatusData RewardCustomer(Customer customer, int day);

        public List<Customer> GetRewardedCustomers();

        public void GenerateCsv(List<Customer> customers, string filePath);

    }
}
