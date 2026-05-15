namespace DirectoryService.Dto;

public record CreateLocationRequest(
    string Name,
    string Street,
    string City,
    string State,
    string ZipCode,
    string Timezone);
