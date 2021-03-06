﻿using MessageCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
namespace MessageCenter.Code
{
    /// <summary>
    /// A class that handles interaction with the data api
    /// </summary>
    public class ApiCaller
    {
        private string apiUrl;

        private HttpClient httpClient;


        public ApiCaller()
        {
            //get url to the api
            apiUrl = Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.API_URL);

            //Setup Api manager
            httpClient = new HttpClient();

            //expected return format is Json
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }




        /// <summary>
        /// Generic method for retrieving model data from api
        /// </summary>
        /// <typeparam name="ModelType">The type of the return object (has to implement the IModel) Interface</typeparam>
        /// <param name="urlParameters">The api url parametres (Get them from the Configurations class)</param>
        /// <returns></returns>
        public List<ModelType> GetDataFromApi<ModelType>(string urlParameters) where ModelType : IModel
        {
            //Makes call and then waits for response (does not proceed untill it has results, or is timed out)
            HttpResponseMessage response = httpClient.GetAsync(apiUrl + urlParameters).Result;

            List<ModelType> returnElements = new List<ModelType>();

            if (response.IsSuccessStatusCode)
            {
                //Convert the result into  
                returnElements = (List<ModelType>)response.Content.ReadAsAsync<IEnumerable<ModelType>>().Result;
            }
            else
            {
                //ERROR - write errors
                Utility.WriteLog(String.Format("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase));
            }

            return returnElements;
        }
    }
}










//Backup 
/*
        public static readonly string getAllEmployeesParameters = "employees/";
        public static readonly string getAllCustomerParameters = "customers/";
        public static readonly string getEmployeeFromTUserParameters = "employees/tuser/";
        public static readonly string getCustomerFromCprParameters = "customers/cpr/";
        public static readonly string getEmployeeFromCredentials = "employees/login/";
        public static readonly string getCustomerFromAdvisor = "customers/advisor/";
*/

/*
  /// <summary>
  /// Retrieves the input employee from the data api
  /// </summary>
  /// <param name="tuser">The employees TUser</param>
  /// <returns></returns>
  private List<Customer> GetTusersCustomers(string tuser)
  {
      //Gets all customers with a relation the input t user
      List<Customer> customers = this.MakeRestCall<Customer>(
          Configurations.GetConfigurationsValue(CONFIGURATIONS_ATTRIBUTES.GET_CUSTOMER_FROM_ADVISOR_TUSER_API_PARAMETERS) + tuser);
      return customers;
  }
  */
//System.Net.Http.Formatting.dll - incase calls dont work on other machines