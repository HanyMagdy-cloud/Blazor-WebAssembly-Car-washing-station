namespace CarWashStation.Models;

public sealed record LoginRequest(string Email, string Password, bool RememberMe = false);
public sealed record AuthStatus(bool IsAuthenticated, string? Email = null);
public sealed record ApiMessage(string Message);
public sealed record BookingResult(bool Success, string Message);
public sealed record BlockSlotRequest(DateTime Date, string? TimeSlot, string? Reason);
