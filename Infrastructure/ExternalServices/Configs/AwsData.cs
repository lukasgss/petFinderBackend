namespace Infrastructure.ExternalServices.Configs;

public class AwsData
{
	public string Region { get; init; } = null!;
	public string BucketName { get; init; } = null!;
	public string PetImagesFolder { get; init; } = null!;
	public string UserImagesFolder { get; init; } = null!;
	public string DefaultUserProfilePicture { get; init; } = null!;
	public string DefaultUserProfilePictureMetadata { get; init; } = null!;
}