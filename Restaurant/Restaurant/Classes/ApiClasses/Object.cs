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
    public class Object
    {

        public int objectId { get; set; }
        public string name { get; set; }
        public Nullable<int> parentObjectId { get; set; }
        public string objectType { get; set; }
        public string translate { get; set; }
        public string translateHint { get; set; }
        public string icon { get; set; }


        public string note { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<int> createUserId { get; set; }
        public Nullable<int> updateUserId { get; set; }
        public byte isActive { get; set; }
        public Boolean canDelete { get; set; }

        List<Object> plist = new List<Object>();
        List<Object> clist = new List<Object>();

        public async Task<List<Object>> GetAll()
        {


            List<Object> list = new List<Object>();

            //#################
            IEnumerable<Claim> claims = await APIResult.getList("Object/Get");

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<Object>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;
        }


        public List<Object> GetParents(List<Object> all, string objName)
        {
            plist = new List<Object>();
            List<Object> list = new List<Object>();
            Object tempob = null;
            tempob = all.Where(o => o.name == objName).FirstOrDefault();
            if (tempob != null)
            {
                int? firstpid = tempob.parentObjectId;
                plist.Add(tempob);

                list = findparentslist(firstpid, all);
                plist.Reverse();
                plist.Remove(plist.Where(x => x.parentObjectId == null).FirstOrDefault());
                return plist;
            }

            return plist;

        }

        private List<Object> findparentslist(int? parentid, List<Object> all)
        {


            // findparentslist(int ? parentid, List < Object > all)
            if (parentid != null)
            {
                Object Ptempob = null;
                Ptempob = findparents(parentid, all);
                plist.Add(Ptempob);
                return findparentslist(Ptempob.parentObjectId, all);
            }
            else
            {
                return plist;
            }


        }

        private Object findparents(int? parentid, List<Object> all)
        {
            Object ptempob = null;
            ptempob = all.Where(o => o.objectId == parentid).FirstOrDefault();

            return ptempob;
        }
        static public List<Object> findChildrenList(string objectName, List<Object> all)
        {
            try
            {
                int objectId = FillCombo.objectsList.Where(x => x.name == objectName.ToString()).FirstOrDefault().objectId;
                return all.Where(x => x.parentObjectId == objectId).ToList();
            }
           catch
            {
                return new List<Object>();
            }

        }
      
     

    }
}

