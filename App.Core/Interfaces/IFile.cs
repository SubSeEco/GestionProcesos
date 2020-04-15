namespace App.Core.Interfaces
{
    public interface IFile 
    {
        Model.DTO.DTOFileMetadata BynaryToText(byte[] content);
        byte[] CreateQR(string id);
        byte[] CreateBarCode(string code);
    }
}