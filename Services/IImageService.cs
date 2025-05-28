namespace BlogProject.Services
{
    public interface IImageService
    {
        // Work with iForm file
        Task<byte[]> EncodeImageAsync(IFormFile file);

        // Return byte array of image
        Task<byte[]> EncodeImageAsync(string fileName);

        // Display image
        string DecodeImage(byte[] data, string type);

        // Collect content type
        string ContentType(IFormFile file);

        // Record the size of the file
        int Size(IFormFile file);


    }
}

/* LEARN : what exactly is an interface? What is the difference between the IImageService interface vs the BasicImageService? */