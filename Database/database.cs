using System;
using System.Web;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using DBreeze;
using DBreeze.DataTypes;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace Adorable.Database
{
    public static class database
    {
        static string path = Path.Combine((string)AppDomain.CurrentDomain.GetData("ContentRootPath"),("Database\\data"));
        private static DBreezeEngine engine = new DBreezeEngine(path);
        
        public static bool Create(string tablename, string _key, object _value)
        {
            string key = JsonConvert.SerializeObject(_key);
            string value = JsonConvert.SerializeObject(_value);
            using (var trans = engine.GetTransaction())
            {
                try
                {
                    trans.Insert<string,string>(tablename, key, value);
                    trans.Commit();
                    return true;
                }
                catch (System.Exception)
                {
                    return false;
                }
            }
        }

        public static KeyValuePair<string,string> Read(string tablename, string _key)
        {
            string key = JsonConvert.SerializeObject(_key);
            using (var trans = engine.GetTransaction())
            {
                try
                {
                    var data = trans.Select<string,string>(tablename,key);
                    if (data.Exists)
                    {
                        return new KeyValuePair<string,string>(data.Key, data.Value);
                    }
                }
                catch (System.Exception)
                {
                   return new KeyValuePair<string,string>("","");
                }
            }
            return new KeyValuePair<string,string>("","");
        }

        public static List<KeyValuePair<string,string>> ReadAll(string tablename)
        {
            var list = new List<KeyValuePair<string,string>>();
            using (var trans = engine.GetTransaction())
            {
                try
                {
                    var data = trans.SelectForward<string,string>(tablename);
                    if (data != null & data.Count() != 0)
                    {
                       foreach (var d in data)
                       {
                           list.Add(new KeyValuePair<string, string>(d.Key,d.Value));
                       } 
                       return list;
                    }
                }
                catch (System.Exception)
                {
                  return list;
                }
            }
            return list;
        }

        public static bool Delete(string tablename, string _key)
        {
            string key = JsonConvert.SerializeObject(_key);
            using (var trans = engine.GetTransaction())
            {
                try
                {
                    if (trans.Select<string,string>(tablename, key).Exists)
                    {
                        trans.RemoveKey<string>(tablename,key);
                        trans.Commit();
                        return true;
                    }
                }
                catch (System.Exception)
                {
                   return false;
                }
            }
            return false;
        }

        public static bool Exists(string tablename, string _key)
        {
            string key = JsonConvert.SerializeObject(_key);
            using (var trans = engine.GetTransaction())
            {
                try
                {
                    if (trans.Select<string,string>(tablename, key).Exists)
                    {
                        return true;
                    }
                }
                catch (System.Exception)
                {
                   return false;
                }
            }
            return false;
        }
        
        public static bool DeleteAll(string tablename, bool recreateFile)
        {
            using (var trans = engine.GetTransaction())
            {
                try
                {
                    trans.RemoveAllKeys(tablename, recreateFile);
                    return true;
                }
                catch (System.Exception)
                {
                    return false;
                }
            }
           // return false;
        }
    }
}