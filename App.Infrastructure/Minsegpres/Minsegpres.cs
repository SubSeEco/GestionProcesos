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
        readonly string key = "e31f64431e424c60a26436de31aad96b";
        readonly string url = @"https://api.firma.test.digital.gob.cl/firma/v2/files/tickets";

        public byte[] Sign(byte[] documento, string OTP, int id, string Run)
        {
            byte[] binario = null;
            var fileContent = Convert.ToBase64String(documento);
            var expires = DateTime.Now.AddMinutes(30);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var header = new JwtHeader(credentials);
            var payload = new JwtPayload {
                   {"entity", "Subsecretaría de Economía y Empresas de Menor Tamaño"},
                   {"run", Run},
                   {"expiration", expires},
                   {"purpose", "Propósito General"},
               };

            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();
            var tokenString = handler.WriteToken(secToken);

            var request = new RestRequest(Method.POST);
            request.AddHeader("OTP", OTP);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n\r\n\"api_token_key\": \"2c368309-d7b6-49ad-9e93-b27fdc58128e\",\r\n\"token\":\r\n\"" + tokenString + "\",\r\n \"files\": [\r\n {\r\n \"content-type\": \"application/pdf\",\r\n \"content\": \"" + fileContent + "\",\r\n \"description\": \"str\",\r\n \"checksum\": \"C4863E4F3CB93450C63F8BB24725E8AB8FC03B7B71619B756294BFB1E55D6507\"\r\n }\r\n ]\r\n}", ParameterType.RequestBody);

            var client = new RestClient(url);
            client.Timeout = -1;
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
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                //Root respuesta = JsonConvert.DeserializeObject<Root>(response.ErrorMessage);

                //foreach (var file in respuesta.files)
                //{
                //    var contenido = file.content;
                //    binario = Convert.FromBase64String(contenido);
                //}
                // manejar error
                throw new System.Exception("El servicio externo de firma electrónica retornó falla.");

            }

            return binario;
        }
    }
}
