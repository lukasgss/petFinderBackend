namespace Infrastructure.ExternalServices.Configs;

public class AwsData
{
    public string Region { get; set; } = null!;
    public string BucketName { get; set; } = null!;
    public string PetImagesFolder { get; set; } = null!;
    public string UserImagesFolder { get; set; } = null!;
}