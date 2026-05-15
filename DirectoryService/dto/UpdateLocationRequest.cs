namespace DirectoryService.Dto;

public record UpdateLocationRequest(
    string Name,
    string Street,
    string City,
    string State,
    string ZipCode,
    string Timezone);
