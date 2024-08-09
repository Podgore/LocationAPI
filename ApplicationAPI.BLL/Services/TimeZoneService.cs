using System.Net.Http.Json;
using System.Net;
using System;
using System.Runtime.CompilerServices;

namespace LocationAPI.BLL.Services
{
    public class TimeZoneService
    {
        //public static DateTime GetLocalDateTime(double latitude, double longitude, DateTime utcDate)
        //{
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        //    request.Method = "POST";

        //    System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
        //    Byte[] byteArray = encoding.GetBytes(jsonContent);

        //    request.ContentLength = byteArray.Length;
        //    request.ContentType = @"application/json";

        //    using (Stream dataStream = request.GetRequestStream())
        //    {
        //        dataStream.Write(byteArray, 0, byteArray.Length);
        //    }
        //    long length = 0;
        //    try
        //    {
        //        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        //        {
        //            length = response.ContentLength;
        //        }
        //    }
        //}
    }
}
