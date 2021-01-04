using System;
using System.Collections.Generic;
using System.Linq;
using App.Core.Interfaces;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using RestSharp;
using App.Model.Core;
using System.Net;
using App.Model.FirmaDocumentoGenerico;
using Newtonsoft.Json;

namespace App.Infrastructure.Minsegpres
{
    public class Minsegpres : IMinsegpres
    {
        public byte[] Sign(byte[] documento, string OTP, string tokenJWT, int id, string Run)
        {
            var client = new RestClient("https://api.firma.test.digital.gob.cl/firma/v2/files/tickets");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("OTP", OTP);
            request.AddHeader("Content-Type", "application/json");

            request.AddParameter("application/json", "{\r\n\r\n\"api_token_key\": \"2c368309-d7b6-49ad-9e93-b27fdc58128e\",\r\n\"token\":\r\n\"" + tokenJWT + "\",\r\n \"files\": [\r\n {\r\n \"content-type\": \"application/pdf\",\r\n \"content\": \"" + documento + "\",\r\n \"description\": \"str\",\r\n \"checksum\": \"C4863E4F3CB93450C63F8BB24725E8AB8FC03B7B71619B756294BFB1E55D6507\"\r\n }\r\n ]\r\n}", ParameterType.RequestBody);

            byte[] binario = null;

            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Root respuesta = JsonConvert.DeserializeObject<Root>(response.Content);

                foreach (var file in respuesta.files)
                {
                    var contenido = file.content;
                    binario = Convert.FromBase64String(contenido);
                }


            }

            return binario;
        }

    }
}
