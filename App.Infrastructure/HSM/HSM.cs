﻿using System;
using System.IO;
using App.Core.Interfaces;
using iTextSharp.text.pdf;
using iTextSharp.text;
using App.Infrastructure.FirmaElock;

namespace App.Infrastructure.HSM
{
    public class HSM : IHSM
    {
        public byte[] Sign(byte[] documento, string Firmante, string unidadOrganizacional, string folio = null, string razon = "Documento firmado electrónicamente Ley 19.799")
        {
            if (documento == null)
                throw new System.Exception("No se especificó el contenido del documento.");
            if (string.IsNullOrWhiteSpace(Firmante))
                throw new System.Exception("No se especificó el firmante documento.");

            if (!string.IsNullOrWhiteSpace(folio))
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (var reader = new PdfReader(documento))
                        {
                            using (PdfStamper stamper = new PdfStamper(reader, ms, '\0', true))
                            {
                                Rectangle pagesize = reader.GetPageSize(1);

                                var pdfContentFirstPage = stamper.GetOverContent(1);
                                ColumnText.ShowTextAligned(pdfContentFirstPage, Element.ALIGN_RIGHT, new Phrase(string.Format("Folio N° {0}", folio), new Font(Font.FontFamily.HELVETICA, 20, Font.BOLD, BaseColor.DARK_GRAY)), 550, pagesize.Height - 95, 0);
                                ColumnText.ShowTextAligned(pdfContentFirstPage, Element.ALIGN_RIGHT, new Phrase(DateTime.Now.ToString("dd-M-yyyy"), new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD, BaseColor.DARK_GRAY)), 550, pagesize.Height - 115, 0);

                                stamper.Close();
                            }
                        }
                        documento = ms.ToArray();
                    }
                }
                catch (System.Exception ex)
                {
                    throw new System.Exception("Error al insertar folio en el documento:" + ex.Message);
                }
            }

            try
            {
                SignFileImplClient ws = new SignFileImplClient();
                var respuesta = ws.SignFile(documento, Firmante.Trim(), "BOTTOM_EDGE_CENTER", "0", "Documento firmado electrónicamente Ley 19.799", unidadOrganizacional, 10, 10, 150, 150);

                if (respuesta == null)
                    throw new System.Exception("El servicio de firma no retornó respuesta");
                if (respuesta != null && respuesta.status.Contains("FAIL"))
                    throw new System.Exception("Error al firmar el documento: " + respuesta.error);

                return respuesta.message;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public string[] GetSigners()
        {

            SignFileImplClient ws = new SignFileImplClient();
            signerInfo firmantes = new signerInfo();
            signBase64EncodedResponse respBase64 = new signBase64EncodedResponse();

            var obj = ws.getSignerNameList();

            if (obj != null)
                return ws.getSignerNameList().signer;

            return null;
        }
    }
}