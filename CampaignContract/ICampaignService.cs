using CampaignContract.Models;
using System;
using System.Collections.Generic;

namespace CampaignContract
{

    public interface ICampaignService
    {

        StatusData RewardCustomer(Customer customer, int day);

        List<Customer> GetRewardedCustomers();

        StatusData GenerateCsv(List<Customer> customers, string filePath);

    }
}
