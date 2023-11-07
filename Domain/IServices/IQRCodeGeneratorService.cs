namespace Domain.IServices;

public interface IQRCodeGeneratorService
{
    string GeneratorQRCode(string text);
}
