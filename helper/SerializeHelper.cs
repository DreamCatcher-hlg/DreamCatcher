using System;
using System.Runtime.Serialization;

namespace MyCommon.helper
{
    public class SerializeHelper
    {
        public static byte[] Serialize(object entity)
        {
            try
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                {
                    formatter.Serialize(stream, entity);
                    var bytes = new byte[stream.Length];
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                    stream.Read(bytes, 0, bytes.Length);
                    return bytes;
                }
            }
            catch(Exception ex)
            {
                MyCommon.log.Log.WriteLog(ex.ToString());
            }
            return null;
        }

        public static object Deserialize(byte[] bytes)
        {
            try
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                    return formatter.Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                MyCommon.log.Log.WriteLog(ex.ToString());
            }
            return null;
        }
    }
}
