using System;
using System.IO;
using App.Core.Interfaces;
using iTextSharp.text.pdf;
using iTextSharp.text;
using App.Infrastructure.FirmaElock;
using App.Util;
using System.Collections.Generic;
using System.Linq;

namespace App.Infrastructure.HSM
{
    public class HSM : IHSM
    {
        //Método deprecado, se recomienda usar la nueva sobrecarga
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
                                //obtener informacion de la primera pagina
                                var pagesize = reader.GetPageSize(1);
                                var pdfContentFirstPage = stamper.GetOverContent(1);

                                //estampa de folio
                                ColumnText.ShowTextAligned(pdfContentFirstPage, Element.ALIGN_LEFT, new Phrase(string.Format("Folio {0}", folio), new Font(Font.FontFamily.HELVETICA, 13, Font.BOLD, BaseColor.DARK_GRAY)), pagesize.Width - 182, pagesize.Height - 167, 0);

                                //estampa de fecha
                                ColumnText.ShowTextAligned(pdfContentFirstPage, Element.ALIGN_LEFT, new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.HELVETICA, 13, Font.BOLD, BaseColor.DARK_GRAY)), pagesize.Width - 182, pagesize.Height - 182, 0);

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

                //sin respuesta 
                if (respuesta == null)
                    throw new System.Exception("El servicio externo de firma electrónica no retornó respuesta");

                //respuesta con error
                if (respuesta != null && respuesta.status.Contains("FAIL"))
                    throw new System.Exception("El servicio externo de firma electrónica retornó falla.");

                return respuesta.message;
            }
            catch (System.Exception ex)
            {
                throw new System.Exception("Error al firmar documento: " + ex.Message);
            }
        }

        //public string[] GetSigners()
        //{

        //    SignFileImplClient ws = new SignFileImplClient();
        //    var obj = ws.getSignerNameList();
        //    if (obj != null)
        //        return ws.getSignerNameList().signer;

        //    return null;
        //}

        //Nuevo método, agrega tabla e verificación de documentos
        public byte[] Sign(byte[] documento, List<string> firmantes, int documentoId, string folio, string url, byte[] QR)
        {
            //validaciones
            if (documentoId == 0)
                throw new System.Exception("No se especificó el código de verificación del documento.");
            if (documento == null)
                throw new System.Exception("No se especificó el contenido del documento.");
            if (!firmantes.Any())
                throw new System.Exception("Debe especificar al menos un firmante.");
            if (url.IsNullOrWhiteSpace())
                throw new System.Exception("No se especificó la url de verificación del documento.");
            if (QR == null)
                throw new System.Exception("No se especificó el código QR.");

            using (MemoryStream ms = new MemoryStream())
            {
                using (var reader = new PdfReader(documento))
                {
                    using (PdfStamper stamper = new PdfStamper(reader, ms, '\0', true))
                    {
                        //agregar folio
                        if (!folio.IsNullOrWhiteSpace())
                        {
                            try
                            {
                                //obtener informacion de la primera pagina
                                var pagesize = reader.GetPageSize(1);
                                var pdfContentFirstPage = stamper.GetOverContent(1);

                                //estampa de folio
                                ColumnText.ShowTextAligned(pdfContentFirstPage, Element.ALIGN_LEFT, new Phrase(string.Format("Folio {0}", folio), new Font(Font.FontFamily.HELVETICA, 13, Font.BOLD, BaseColor.DARK_GRAY)), pagesize.Width - 182, pagesize.Height - 167, 0);

                                //estampa de fecha
                                ColumnText.ShowTextAligned(pdfContentFirstPage, Element.ALIGN_LEFT, new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.HELVETICA, 13, Font.BOLD, BaseColor.DARK_GRAY)), pagesize.Width - 182, pagesize.Height - 182, 0);
                            }
                            catch (System.Exception ex)
                            {
                                throw new System.Exception("Error al insertar folio en el documento:" + ex.Message);
                            }
                        }

                        //agregar tabla de verificacion
                        try
                        {
                            var img = Image.GetInstance(QR);
                            var fontStandard = new Font(Font.FontFamily.HELVETICA, 9, Font.NORMAL, BaseColor.DARK_GRAY);
                            var fontBold = new iTextSharp.text.Font(Font.FontFamily.HELVETICA, 9, Font.BOLD, BaseColor.DARK_GRAY);
                            var pdfContentLastPage = stamper.GetOverContent(reader.NumberOfPages);
                            var table = new PdfPTable(3) { HorizontalAlignment = Element.ALIGN_CENTER, WidthPercentage = 100 };

                            table.TotalWidth = 520f;
                            table.SetWidths(new float[] { 8f, 25f, 6f });
                            table.AddCell(new PdfPCell(new Phrase("Información de firma electrónica:", fontBold)) { Colspan = 2, BorderColor = BaseColor.DARK_GRAY });
                            table.AddCell(new PdfPCell() { Rowspan = 5 }).AddElement(img);
                            table.AddCell(new PdfPCell(new Phrase("Firmantes", fontBold)) { });
                            table.AddCell(new PdfPCell(new Phrase(string.Join(", ", firmantes), fontStandard)) { BorderColor = BaseColor.DARK_GRAY });
                            table.AddCell(new PdfPCell(new Phrase("Fecha de firma", fontBold)) { BorderColor = BaseColor.DARK_GRAY });
                            table.AddCell(new PdfPCell(new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), fontStandard)) { BorderColor = BaseColor.DARK_GRAY });
                            table.AddCell(new PdfPCell(new Phrase("Código de verificación", fontBold)) { BorderColor = BaseColor.DARK_GRAY });
                            table.AddCell(new PdfPCell(new Phrase(documentoId.ToString(), fontStandard)) { BorderColor = BaseColor.DARK_GRAY });
                            table.AddCell(new PdfPCell(new Phrase("URL de verificación", fontBold)) { BorderColor = BaseColor.DARK_GRAY });
                            table.AddCell(new PdfPCell(new Phrase(url, fontStandard)) { BorderColor = BaseColor.DARK_GRAY });
                            table.WriteSelectedRows(0, -1, 43, 100, pdfContentLastPage);
                        }
                        catch (System.Exception ex)
                        {
                            throw new System.Exception("Error al insertar tabla de validación de firma electrónica:" + ex.Message);
                        }

                        stamper.Close();
                    }
                }
                documento = ms.ToArray();
            }

            //firma documento
            var documentoFirmado = documento;
            var respuesta = new signFileResponse();
            using (var ws = new SignFileImplClient())
            {
                try
                {
                    foreach (var firmante in firmantes)
                    {
                        //ejecutar llamada a servicio
                        respuesta = ws.SignFile(documentoFirmado, firmante.Trim(), "BOTTOM_EDGE_CENTER", "0", null, null, 0, 0, 0, 0);

                        //sin respuesta 
                        if (respuesta == null)
                            throw new System.Exception("El servicio externo de firma electrónica no retornó respuesta");

                        //respuesta con error
                        else if (respuesta != null && respuesta.status.Contains("FAIL"))
                            throw new System.Exception("El servicio externo de firma electrónica retornó falla.");

                        //firma ok
                        else
                            documentoFirmado = respuesta.message;
                    }
                }
                catch (System.Exception ex)
                {
                    throw new System.Exception("Error al firmar documento: " + ex.Message);
                }
            }

            return documentoFirmado;
        }
    }
}
