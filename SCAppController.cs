using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Machines.Domain.Service.Implementations;
using Machines.Domain.Service.Interfaces;
using Machines.Domain.Service.Messaging;
using System.Web.Http.Cors;
using System.IO;
using Newtonsoft.Json;
using WebApiMachine.Models;
//using Nexus.Net.Security;

namespace WebApiMachine.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/SCApp")]

  //  [TokenAuthorize] // Add token authorize attribute to routes

    public class SCAppController : ApiController
    {
        IMachineQueryService _machineQueryService = new MachinesQueryService();
        //GET ALL
        [HttpGet]
        [Route("GetAllMachines")]
        public System.Web.Http.Results.JsonResult<System.Data.DataTable> GetAllMachines()
        {
            //if (request.MachineId == "") request.MachineId = null;
            //if (request.AddDay == 0) request.AddDay = null;

            QueryResponse response = _machineQueryService.getMachines();
            if (!response.Success) throw new Exception(response.Message);
            return Json(response.Result);
        }

        [HttpPost]
        [Route("GetAllParameters")]
        public System.Web.Http.Results.JsonResult<System.Data.DataTable> GetAllParameters([FromBody]ParamsRequest request)
        {
            //if (request.MachineId == "") request.MachineId = null;
            //if (request.AddDay == 0) request.AddDay = null;

            QueryResponse response = _machineQueryService.getParams(request);
            if (!response.Success) throw new Exception(response.Message);
            return Json(response.Result);
        }

        [HttpPost]
        [Route("getMeasurements")]
        public System.Web.Http.Results.JsonResult<System.Data.DataTable> getMeasurements([FromBody]MeasurementsRequest request)
        {
            if (request != null)
            {
                QueryResponse response = _machineQueryService.getTheMeasurements(request);
                if (!response.Success) throw new Exception(response.Message);
                return Json(response.Result);
            }
            else
                return null;
        }

        [HttpGet]
        [Route("ReadCSVFile")]
        public String ReadCSVFile()
        {
            // Read the file as one string.
            string text = System.IO.File.ReadAllText(@"C:\OEE\GAP\data\Training sets\B3_0623_2_threshold_and_quantiles.csv");

            // Display the file contents to the console. Variable text is a string.
           // System.Console.WriteLine("Contents of .txt = {0}", text);

            // Example #2
            // Read each line of the file into a string array. Each element
            // of the array is one line of the file.
            string[] lines = System.IO.File.ReadAllLines(@"C:\OEE\GAP\data\Training sets\B3_0623_2_threshold_and_quantiles.csv");

            // Display the file contents by using a foreach loop.
            // System.Console.WriteLine("Contents of B3_0623_2_threshold_and_quantiles = ");
            String texts = "";
            foreach (string line in lines)
            {
                // Use a tab to indent each line of the file.
                //Console.WriteLine("\t" + line);
                texts += line;
            }
            /*write logic for these 
                1. get input parameter as machinename and read the file that starts with that name...(filename dynamic)
                2. in foreach block, look for certain lines beginning with certain words and return these lines alone
                to UI 
                3. In the UI parse the line and get the threshold values 
            */

            return texts;
        }

        [HttpPost]
        [Route("GetThreshold")]
        public String GetThreshold([FromBody]QueryThresholdRequest request)
        {
            /* iterate through all file names in C:\OEE\GAP\data\Training sets folder
             * pick the one starting with request.machineName
             * */
            //String directoryPath = @"C:\OEE\GAP\data\Training sets\";
            String directoryPath = @"C:\Users\mecusr\TBDdelete\";
            string[] fileEntries = Directory.GetFiles(directoryPath);

            string str = "No File found matching this machine name";
            foreach (var file_name in fileEntries)
            {
                string fileName = file_name.Substring(directoryPath.Length);
                Console.WriteLine(fileName);
                //if fileName startsWith machineName, read its contents, and return the threshold values 
                //add this to array               
                if (fileName.ToLower().StartsWith(request.machineName.ToLower()))
                {
                    string textX = System.IO.File.ReadAllText(directoryPath + fileName);
                    Console.WriteLine(textX);
                    str = ConvertCsvFileToJsonObject(directoryPath + fileName, request.parameter);
                    // str = str.Replace(@"\", " ");               
                }             
            }          
            return str;
        }


        ///used Dictionary and returned json using newtonsoft
        public string ConvertCsvFileToJsonObject(string path, string parameter)
            {
            /*   Example data  */
            //lines[0] has parameter names-  Cycle time,Max injection speed,VP switch position,Plasiciation time,Inj. time
            //lines[1] has strong_lower values -  strong_lower,46.27,113.08,22.84,9.72,3.8
            //lines[2]  weak_lower values - weak_lower,46.27,113.08,22.84,9.72,3.8
            //lines[3] weak_upper values -weak_upper,46.27,113.08,22.84,9.72,3.8
            //lines[4]  strong_upper values -strong_upper,46.27,113.08,22.84,9.72,3.8
            //lines[5] good_mean values -good_mean,46.27,113.08,22.84,9.72,3.8
            //lines[6] good_std values - good_std,46.27,113.08,22.84,9.72,3.8

            var csv = new List<string[]>();
            var lines = File.ReadAllLines(path);

              var strong_lower_array =   lines[1].Split(',');
              var weak_lower_array   =   lines[2].Split(',');
              var weak_upper_array   =   lines[3].Split(',');
              var strong_upper_array =   lines[4].Split(',');
              var good_mean_array    =   lines[5].Split(',');
              var good_std_array     =   lines[6].Split(',');

            var obj = new ParameterThresholds();
            obj.parameter_name = parameter;  //input

            if (parameter.ToLower().Equals("cycle_time"))
            {
                obj.strong_lower = strong_lower_array[1];    //value is position 1
                obj.weak_lower = weak_lower_array[1];
                obj.weak_upper = weak_upper_array[1];
                obj.strong_upper = strong_upper_array[1];
                obj.good_mean = good_mean_array[1];
                obj.good_std = good_std_array[1];
            }
            else if (parameter.ToLower().Equals("max_injection_speed"))
            {
                obj.strong_lower = strong_lower_array[2];    //value is position 2
                obj.weak_lower = weak_lower_array[2];
                obj.weak_upper = weak_upper_array[2];
                obj.strong_upper = strong_upper_array[2];
                obj.good_mean = good_mean_array[2];
                obj.good_std = good_std_array[2];
            }          
            else if (parameter.ToLower().Equals("switchover_position")) //VP switch position
            {
                obj.strong_lower = strong_lower_array[3];    //value is position 3
                obj.weak_lower = weak_lower_array[3];
                obj.weak_upper = weak_upper_array[3];
                obj.strong_upper = strong_upper_array[3];
                obj.good_mean = good_mean_array[3];
                obj.good_std = good_std_array[3];
            }
            else if (parameter.ToLower().Equals("plastification_time"))  //Plasiciation time in CSV
            {
                obj.strong_lower = strong_lower_array[4];    //value is position 4
                obj.weak_lower = weak_lower_array[4];
                obj.weak_upper = weak_upper_array[4];
                obj.strong_upper = strong_upper_array[4];
                obj.good_mean = good_mean_array[4];
                obj.good_std = good_std_array[4];
            }
            else if (parameter.ToLower().Equals("inj_time"))
            {
                obj.strong_lower = strong_lower_array[5];    //value is position 5
                obj.weak_lower = weak_lower_array[5];
                obj.weak_upper = weak_upper_array[5];
                obj.strong_upper = strong_upper_array[5];
                obj.good_mean = good_mean_array[5];
                obj.good_std = good_std_array[5];
            }

            return JsonConvert.SerializeObject(obj);
         }

    //sdsdsdsd


}
}
