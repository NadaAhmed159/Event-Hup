using EventHub.BLL.Models;

namespace EventHub.BLL.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
        Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
        Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);
    }
}
