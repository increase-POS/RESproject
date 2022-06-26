using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Security.Claims;
using System.Drawing.Printing;
using Restaurant.ApiClasses;
using Newtonsoft.Json.Converters;

namespace Restaurant.Classes
{
    public class PosSetting
    {
        public long posSettingId { get; set; }
        public Nullable<long> posId { get; set; }
        public Nullable<long> saleInvPrinterId { get; set; }
        public Nullable<long> reportPrinterId { get; set; }
        public Nullable<long> saleInvPapersizeId { get; set; }

        public string posSerial { get; set; }

        public Nullable<long> repprinterId { get; set; }
        public string repname { get; set; }
        public string repprintFor { get; set; }

        public Nullable<long> salprinterId { get; set; }
        public string salname { get; set; }
        public string salprintFor { get; set; }

        public Nullable<long> sizeId { get; set; }
        public string paperSize1 { get; set; }
        public Nullable<long> docPapersizeId { get; set; }
        public string docPapersize { get; set; }
        public string saleSizeValue { get; set; }
        public string docSizeValue { get; set; }
        public string kitchenSizeValue { get; set; }

        public Nullable<long> kitchenPrinterId { get; set; }
        public Nullable<long> kitchenPapersizeId { get; set; }
        public string kitchenPrinter { get; set; }
        public string kitchenPapersize { get; set; }
        public string kitchenprintFor { get; set; }
     
        /// <summary>
        /// ///////////////////////////////////////
        /// </summary>
        /// <returns></returns>
        /// 

        public string getdefaultPrinters()
        {

            PrinterSettings settings = new PrinterSettings();
            string defaultPrinterName = settings.PrinterName;


            return defaultPrinterName;
        }

        public PosSetting MaindefaultPrinterSetting(PosSetting oldsetting)
        {


            PosSetting defpossetting = new PosSetting();
            defpossetting = oldsetting;
             

            string printname = getdefaultPrinters();

            Printers defpr = new Printers();

            defpr.name = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(printname));
            if (oldsetting.saleInvPrinterId == null)
            {

                defpossetting.salname = defpr.name;

            }
            if (oldsetting.kitchenPrinterId == null)
            {

                defpossetting.kitchenPrinter = defpr.name;

            }
            if (oldsetting.reportPrinterId == null)
            {

                defpossetting.repname = defpr.name;


            }

            // paper
            if (oldsetting.saleInvPapersizeId == null)
            {

                defpossetting.saleSizeValue = "5.7cm";
            }
            if (oldsetting.kitchenPapersizeId == null)
            {

                defpossetting.kitchenPapersize = "5.7cm";
            }

            if (oldsetting.docPapersizeId == null)
            {

                defpossetting.docPapersize = "A5";

            }


            return defpossetting;

        }

        public async Task<int> Save(PosSetting obj)
        {

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "PosSetting/Save";

            var myContent = JsonConvert.SerializeObject(obj);
            parameters.Add("Object", myContent);
            return await APIResult.post(method, parameters);

          
        }
 

        public async Task<PosSetting> GetByposId(long posId)
        {

            PosSetting item = new PosSetting();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("posId", posId.ToString());
            //#################
            IEnumerable<Claim> claims = await APIResult.getList("PosSetting/GetByposId", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    item = JsonConvert.DeserializeObject<PosSetting>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
                    break;
                }
            }
            return item;


      
        }

        public async Task<int> Delete(long posSettingId)
        {


            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("posSettingId", posSettingId.ToString());


            string method = "PosSetting/Delete";
            return await APIResult.post(method, parameters);

       
        }





    }
}
 
