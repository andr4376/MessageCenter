using MessageCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
namespace MessageCenter.Code
{
    public class ApiManager
    {
        private static ApiManager instance;

        private readonly string apiUrl = "http://localhost:58616/api/";

        public static readonly string getAllEmployeesParameters = "employees/";
        public static readonly string getAllCustomerParameters = "customers/";
        public static readonly string getEmployeeFromTUserParameters = "employees/tuser/";
        public static readonly string getCustomerFromCprParameters = "customers/cpr/";
        public static readonly string getEmployeeFromCredentials = "employees/login/";


        private HttpClient httpClient;

        public static ApiManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ApiManager();
                }
                return instance;
            }
        }

        private ApiManager()
        {
            Intitialize();
        }

        private void Intitialize()
        {
            //Setup Api manager
            httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">The type of the return object</typeparam>
        /// <param name="urlParameters">Append the Apimanager's parameter fields fx: (ApiManager.getCustomerFromCprParameters+'010119010101')</param>
        /// <returns></returns>
        public List<T> MakeRestCall<T>(string urlParameters)
        {
            // List data response.
            // Blocking call! Program will wait here until a response is received or a timeout occurs.
            HttpResponseMessage response = httpClient.GetAsync(apiUrl+urlParameters).Result;  

            List<T> returnElements = new List<T>();

            if (response.IsSuccessStatusCode)
            {
                // parse response 
                returnElements = (List<T>) response.Content.ReadAsAsync<IEnumerable<T>>().Result;  //Make sure to add a reference to System.Net.Http.Formatting.dll
                
            }
            else
            {
                Utility.WriteLog(String.Format("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase));
            }
            
            

            return returnElements;
        }
    }
}