using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/api/images")]
public class ImageController : ControllerBase
{
    private readonly IAmazonS3 _s3;

    public ImageController(IAmazonS3 s3)
    {
        _s3 = s3;
    }

    [HttpPost]
    public async Task<IActionResult> UploadImage([FromForm(Name = "Data")] IFormFile file)
    {
        string fileName = file.FileName;
        
        var putObjectRequest = new PutObjectRequest()
        {
            BucketName = "petfinderproject",
            Key = $"Images/PetImages/{fileName}",
            ContentType = file.ContentType,
            InputStream = file.OpenReadStream()
        };

        var response = await _s3.PutObjectAsync(putObjectRequest);
        return Ok(response);
    }
}