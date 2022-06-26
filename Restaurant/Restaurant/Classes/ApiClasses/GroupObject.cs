﻿using Newtonsoft.Json;
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
using Restaurant.Classes;

namespace Restaurant.Classes
{
    public class GroupObject
    {
        public long id { get; set; }
        public Nullable<long> groupId { get; set; }
        public Nullable<long> objectId { get; set; }
        public string notes { get; set; }
        public byte addOb { get; set; }
        public byte updateOb { get; set; }
        public byte deleteOb { get; set; }
        public byte showOb { get; set; }
        public byte reportOb { get; set; }
        public byte levelOb { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public Nullable<System.DateTime> updateDate { get; set; }
        public Nullable<long> createUserId { get; set; }
        public Nullable<long> updateUserId { get; set; }
        public int isActive { get; set; }



        public string objectName { get; set; }
        public string desc { get; set; }

        public Boolean canDelete { get; set; }
        public Nullable<long> parentObjectId { get; set; }
        public string objectType { get; set; }
        public string parentObjectName { get; set; }
         
        public string GroupName { get; set; }

        public async Task<List<GroupObject>> GetAll()
        {

            List<GroupObject> list = new List<GroupObject>();
            //#################
            IEnumerable<Claim> claims = await APIResult.getList("GroupObject/Get");

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<GroupObject>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;
          
        }

   
        public async Task<int> Save(GroupObject newObject)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string method = "GroupObject/Save";

            var myContent = JsonConvert.SerializeObject(newObject);
            parameters.Add("Object", myContent);
           return await APIResult.post(method, parameters);

        }        

         
        //
        public async Task<List<GroupObject>> GetUserpermission(long userId)
        {

            List<GroupObject> list = new List<GroupObject>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("userId", userId.ToString());
            //#################
            IEnumerable<Claim> claims = await APIResult.getList("GroupObject/GetUserpermission", parameters);

            foreach (Claim c in claims)
            {
                if (c.Type == "scopes")
                {
                    list.Add(JsonConvert.DeserializeObject<GroupObject>(c.Value, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }));
                }
            }
            return list;


            //List<GroupObject> list = null;
            //// ... Use HttpClient.
            //ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            //using (var client = new HttpClient())
            //{
            //    ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            //    client.BaseAddress = new Uri(Global.APIUri);
            //    client.DefaultRequestHeaders.Clear();
            //    client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            //    client.DefaultRequestHeaders.Add("Keep-Alive", "3600");
            //    HttpRequestMessage request = new HttpRequestMessage();
            //    request.RequestUri = new Uri(Global.APIUri + "GroupObject/GetUserpermission?userId=" + userId);
            //    request.Headers.Add("APIKey", Global.APIKey);
            //    request.Method = HttpMethod.Get;
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    HttpResponseMessage response = await client.SendAsync(request);

            //    if (response.IsSuccessStatusCode)
            //    {
            //        var jsonString = await response.Content.ReadAsStringAsync();
            //        jsonString = jsonString.Replace("\\", string.Empty);
            //        jsonString = jsonString.Trim('"');
            //        // fix date format
            //        JsonSerializerSettings settings = new JsonSerializerSettings
            //        {
            //            Converters = new List<JsonConverter> { new BadDateFixingConverter() },
            //            DateParseHandling = DateParseHandling.None
            //        };
            //        list = JsonConvert.DeserializeObject<List<GroupObject>>(jsonString, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
            //        return list;
            //    }
            //    else //web api sent error response 
            //    {
            //        list = new List<GroupObject>();
            //    }
            //    return list;
            //}
        }
        public bool HasPermission(string objectname, List<GroupObject> GOList)
        {
            if (HelpClass.isAdminPermision())
                return true;
            List<GroupObject> currlist = new List<GroupObject>();
            currlist = GetObjSons(objectname, GOList);
            currlist = currlist.Where(X => (X.addOb == 1 || X.updateOb == 1 || X.deleteOb == 1 || X.showOb == 1 || X.reportOb == 1)).ToList();
            if (currlist != null)
            {
                if (currlist.Count > 0)
                {
                    if (HelpClass.branchPermision(objectname, MainWindow.branchLogin.type))
                        return true;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        //
        private List<GroupObject> objlist = new List<GroupObject>();
        //

        public List<GroupObject> GetObjSons(string objectName, List<GroupObject> GOList)
        {
            objlist = new List<GroupObject>();
            List<GroupObject> opl = new List<GroupObject>();

            // objlist;
            GroupObject firstelement = new GroupObject();

            firstelement = GOList.Where(X => X.objectName == objectName).FirstOrDefault();
            //  firstelement.objectId = objectId;
            if (firstelement != null)
            {
                objlist.Add(firstelement);
                SonsofObject(objlist, GOList);
                return (objlist);
            }
            else
            {
                return opl;
            }

        }

        private void SonsofObject(List<GroupObject> objlist1, List<GroupObject> mainobjlist)
        {

            List<GroupObject> templist = new List<GroupObject>();
            List<GroupObject> templist2 = new List<GroupObject>();
            if (objlist1.Count > 0)
            {
                foreach (GroupObject row in objlist1.ToList())
                {
                    //   templist = null;
                    if (row != null)
                    {
                        templist = mainobjlist.Where(X => X.parentObjectId == row.objectId).ToList();
                        /*
                                                    templist = (from O in mainobjlist.ToList()
                                                            where O.parentObjectId == row.objectId

                                                            select new GroupObject
                                                            {
                                                                objectId = O.objectId,
                                                                parentObjectId = O.parentObjectId,
                                                            }

                                                            ).ToList();
                                                            */

                    }

                    if (templist.Count > 0)
                    {
                        objlist.AddRange(templist);
                        templist2.AddRange(templist);
                    }


                }
                if (templist2.Count > 0)
                {
                    SonsofObject(templist2, mainobjlist);
                }


            }


        }

        // get groupObject row by objectName

        public GroupObject GetGObjByObjName(string objectName, List<GroupObject> GOList)
        {
       
            GroupObject element = new GroupObject();
   
            element = GOList.Where(X => X.objectName == objectName).FirstOrDefault();

                return element;
        }
        //

        public bool HasPermissionAction(string objectname, List<GroupObject> GOList, string type)
        {
            if (HelpClass.isAdminPermision())
                return true;

            bool hasPermission = false;
           
            GroupObject groupObject =  GetGObjByObjName(objectname, GOList);
            if (groupObject != null)
            if (type == "add" && groupObject.addOb == 1)
                     hasPermission = true;
            else if (type == "update" && groupObject.updateOb == 1)
                hasPermission = true;
            else if (type == "delete" && groupObject.deleteOb == 1)
                hasPermission = true;
            else if (type == "show" && groupObject.showOb == 1)
                hasPermission = true;
            else if (type == "report" && groupObject.reportOb == 1)
                hasPermission = true;
            else if (type == "one" && groupObject.showOb == 1)
                hasPermission = true;

            return hasPermission;
        }


    }
}

