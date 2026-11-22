namespace EduHub.Ch02.Patterns.Strategies;

/// <summary>Một dòng thống kê của CLB, dữ liệu đầu vào chung cho mọi strategy.</summary>
public record ClubStat(string ClubName, int Members, int Events, int Registered, int CheckedIn);