﻿using RMDesktopUI.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RMDesktopUI.Library.Api
{
    public class SaleEndpoint : ISaleEndpoint
    {
        private readonly IAPIHelper _apiHelper;

        public SaleEndpoint(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task PostSale(SaleModel sale)
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("Api/Sale", sale))
            {
                if (response.IsSuccessStatusCode)
                {
                    // Log successful call ?
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
