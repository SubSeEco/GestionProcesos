namespace App.Core.Interfaces
{
    public interface IFile 
    {
        Model.DTO.DTOFileMetadata BynaryToText(byte[] content);
        byte[] CreateQr(string id);
        //byte[] CreateBarCode(string code);
        byte[] EstamparCodigoEnDocumento(byte[] documento, string text);
    }
}