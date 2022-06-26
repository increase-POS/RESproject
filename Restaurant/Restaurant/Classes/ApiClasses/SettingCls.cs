using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Restaurant;
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

using Restaurant.ApiClasses;

namespace Restaurant.Classes
{
    public class SettingCls
    {
        public long settingId { get; set; }
        public string name { get; set; }
        public string trName { get; set; }
        public string notes { get; set; }
        public async Task<List<SettingCls>> GetAll()
        {

            List<SettingCls> list = new List<SettingCls>();
            //#################
            IEnumerable<Claim> claims = await APIResult.getList("setting/Get");

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<SettingCls>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

        }

        
        public async Task<List<SettingCls>> GetByNotes(string notes)
        {
            List<SettingCls> list = new List<SettingCls>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("notes", notes);
            //#################
            IEnumerable<Claim> claims = await APIResult.getList("setting/GetByNotes", parameters);

      

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<SettingCls>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;

        }
       

    }
}

