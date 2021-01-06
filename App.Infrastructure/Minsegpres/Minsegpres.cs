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
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace App.Infrastructure.Minsegpres
{
    public class Minsegpres : IMinsegpres
    {
        public byte[] Sign(byte[] documento, string OTP, int id, string Run)
        {
            byte[] binario = null;
            string fileContent = Convert.ToBase64String(documento);

            DateTime issuedAt = DateTime.Now;
            DateTime expires = DateTime.Now.AddMinutes(30);

            string key = "e31f64431e424c60a26436de31aad96b";

            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials
                              (securityKey, SecurityAlgorithms.HmacSha256Signature);

            var header = new JwtHeader(credentials);

            var payload = new JwtPayload
               {
                   {"entity", "Subsecretaría de Economía y Empresas de Menor Tamaño"},
                   { "run", Run},
                   { "expiration", expires},
                   { "purpose", "Propósito General"},
               };

            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            var tokenString = handler.WriteToken(secToken);

            var client = new RestClient("https://api.firma.test.digital.gob.cl/firma/v2/files/tickets");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("OTP", OTP);
            request.AddHeader("Content-Type", "application/json");

            request.AddParameter("application/json", "{\r\n\r\n\"api_token_key\": \"2c368309-d7b6-49ad-9e93-b27fdc58128e\",\r\n\"token\":\r\n\"" + tokenString + "\",\r\n \"files\": [\r\n {\r\n \"content-type\": \"application/pdf\",\r\n \"content\": \"" + fileContent + "\",\r\n \"description\": \"str\",\r\n \"checksum\": \"C4863E4F3CB93450C63F8BB24725E8AB8FC03B7B71619B756294BFB1E55D6507\"\r\n }\r\n ]\r\n}", ParameterType.RequestBody);

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
            else
            {
                // manejar error
            }


            return binario;
        }
    }
}
