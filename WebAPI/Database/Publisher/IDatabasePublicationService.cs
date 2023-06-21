namespace PharmacySystem.WebAPI.Database.Publisher;

public interface IDatabasePublicationService
{
    DatabasePublicationResult Publish(DatabasePublicationOptions options);
}