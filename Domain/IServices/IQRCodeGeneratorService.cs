namespace Domain.IServices;

// Interface for a service that generates QR codes
public interface IQRCodeGeneratorService
{
    // Generate a QR code from a text string
    string GenerateQRCodeFromText(string text);
}
