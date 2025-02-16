using System.IO;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;

public class CloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IConfiguration config)
    {
        var account = new Account(
            config["Cloudinary:CloudName"], // e.g. "dvp4wikev"
            config["Cloudinary:ApiKey"], // e.g. "862394414244217"
            config["Cloudinary:ApiSecret"] // e.g. "loiWCtS2UcJzA-Fzwh9-j2d0lFU"
        );
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadImageAsync(
        Stream imageStream,
        string fileName,
        string imageType
    )
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, imageStream),
            Transformation = GetTransformation(imageType),
        };

        var result = await _cloudinary.UploadAsync(uploadParams);
        return result.SecureUrl?.ToString();
    }

    private Transformation GetTransformation(string imageType)
    {
        // Return a transformation based on the image type
        return imageType switch
        {
            // Profile pictures: 500x500, fill-crop around the face
            "profile" => new Transformation().Width(500).Height(500).Crop("fill").Gravity("face"),

            // Header images: 1200x400, fill-crop for wide layouts
            "header" => new Transformation().Width(1200).Height(400).Crop("fill"),

            // Default fallback
            _ => new Transformation().Width(800).Height(600).Crop("limit"),
        };
    }
}
