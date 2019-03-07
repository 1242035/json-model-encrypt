using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Com.Viauco.JsonOrm
{

    public class Session : ISession
    {
        private static Session _session = null;

        private static JObject _ConfigData;

        public static JObject ConfigData
        {
            get
            {
                _ConfigData = _ConfigData == null ? new JObject() : _ConfigData;
                return _ConfigData;
            }
            set
            {
                _ConfigData = value;
            }
        }

        private string _path = "storage.dat";

        

        protected Session()
        {
            ConfigData = Load(_path);
        }

        #region Private Methods


        private static JObject Load(string _path)
        {
            string data = "";
            if( File.Exists(_path) )
            {
                data = EncryptStream.read(_path);
            }

            ConfigData = JsonConvert.DeserializeObject<JObject>(data);
            
            return ConfigData;
        }

        private void write(JObject data)
        {
            EncryptStream.write(_path, JsonConvert.SerializeObject(data));
        }

        #endregion

        
        public static Session Instance
        {
            get
            {
                if (_session == null)
                {
                    _session = new Session();
                }
                return _session;
            }
        }

        public string getFilePath()
        {
            return _path;
        }

        public void setFilePath(string path)
        {
            _path = path;
        }

        #region ISession Implementation

        public T GetElementById<T>(Func<T, bool> query, string entityname) where T : new()
        {
            T data = default(T);
            try
            {
                data = ConfigData[entityname].ToObject<List<T>>().Where(query).SingleOrDefault<T>();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            return data;
        }

        public List<T> GetElementList<T>(string entityname)
        {

            List<T> data = default(List<T>);
            try
            {
                data = ConfigData[entityname].ToObject<List<T>>().ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            return data;
        }

        public List<T> GetElementList<T>(Func<T, bool> query, string entityname)
        {
            List<T> data = default(List<T>);
            try
            {
                data = ConfigData[entityname].ToObject<List<T>>().Where(query).ToList<T>();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            return data;
        }

        public List<T> GetElementList<T>(string[] elementPath, string parameter)
        {

            ///Working
            ///
            List<JToken> data = ConfigData["apporgsync"].ToList();
            List<T> returnList = new List<T>();
            try
            {
                foreach (string s in elementPath)
                {
                    data = data.Values(s).ToList();
                }

                data = data.SelectMany(c => c.Select(cin => cin)
                  .Where(m => m["TouchPointId"].ToString() == parameter))
                  .ToList();
                //returnList.Add(Converter.Instance.DeserializeObject<T>(data[0].ToString()));

                returnList = data.Select(m => Commons.Instance.DeserializeObject<T>(m.ToString())).ToList();

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            return returnList;

        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public IList<T> FindPage<T>(int pageStartRow, int pageSize)
        {
            throw new NotImplementedException();
        }

        public IList<T> FindSortedPage<T>(int pageStartRow, int pageSize, string sortBy, bool descending)
        {
            throw new NotImplementedException();
        }

        public void Save<T>(T entity, string entityname)
        {

            try
            {
                if ( ! typeof(T).IsGenericType)
                {
                    JArray data = ConfigData[entityname] != null ? ConfigData[entityname].ToObject<JArray>() : new JArray();
                    data.Add(JToken.FromObject(entity));
                    ConfigData[entityname] = data;
                }
                else
                {
                    ConfigData[entityname] = JArray.FromObject(entity);
                }
                write(ConfigData);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        public void AddRange<T>(string resource, string entityname) where T : new()
        {
            JArray JsonConfigarr = JArray.Parse(resource);

            ConfigData[entityname] = JsonConfigarr;

            write(ConfigData);
        }

        public void Update<T>(T entity, Func<T, bool> query, string entityname)
        {

            try
            {
                JArray Jdata = ConfigData[entityname].ToObject<JArray>();
                List<T> ldata = ConfigData[entityname].ToObject<List<T>>();

                Jdata.RemoveAt(ldata.IndexOf(ldata.Where(query).SingleOrDefault<T>()));
                JToken jto = JToken.FromObject(entity);
                Jdata.Add(jto);
                JArray JsonConfigarr = JArray.FromObject(Jdata);
                ConfigData[entityname] = JsonConfigarr;

                write(ConfigData);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            
        }

        public void SaveOrUpdate<T>(T entity, Func<T, bool> query, string entityname) where T : new()
        {

            T temp = GetElementById<T>(query, entityname);

            if (temp != null)
            {
                this.Update<T>(entity, query, entityname);
            }
            else
            {
                this.Save<T>(entity, entityname);
            }
        }

        public void Delete<T>(T entity, string entityname)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
