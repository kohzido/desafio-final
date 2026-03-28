namespace Application.DTOs;

public record CreateClienteRequest(string Nome, string Email);

public record UpdateClienteRequest(string Nome, string Email);

public record ClienteResponse(Guid Id, string Nome, string Email);
