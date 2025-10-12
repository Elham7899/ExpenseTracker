using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
        => await _userRepository.GetAllAsync();

    public async Task<User?> GetUserByIdAsync(long id)
        => await _userRepository.GetByIdAsync(id);

    public async Task<User?> GetUserByEmailAsync(string email)
        => await _userRepository.GetUserByEmailAsync(email);

    public async Task AddUserAsync(User user)
    {
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(long id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user != null)
        {
            _userRepository.Remove(user);
            await _userRepository.SaveChangesAsync();
        }
    }
}