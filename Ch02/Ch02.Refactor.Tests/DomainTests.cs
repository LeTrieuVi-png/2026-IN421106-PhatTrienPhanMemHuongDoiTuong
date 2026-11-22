using EduHub.Ch02;
using EduHub.Ch02.Domain;

namespace Ch02.Refactor.Tests;

// Mẫu AAA: Arrange (chuẩn bị) - Act (thực hiện) - Assert (kiểm tra).
public class DomainTests
{
    // Test mẫu của thầy: chạy trên ClubManager cũ. Sẽ đổi sang Club.Create khi xoá ClubManager.
    [Fact]
    public void CreateClub_WithEmptyName_ThrowsArgumentException()
    {
        // Arrange
        var manager = new ClubManager();

        // Act
        var act = () => manager.CreateClub("", "mô tả", "p@gmail.com");

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Club_Create_WithValidName_ReturnsClub()
    {
        // Arrange
        var name = "Tech Club";

        // Act
        var club = Club.Create(name, "Câu lạc bộ công nghệ");

        // Assert
        Assert.Equal(name, club.Name);
        Assert.NotEqual(Guid.Empty, club.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Club_Create_WithInvalidName_ThrowsArgumentException(string name)
    {
        // Arrange - tên rỗng hoặc toàn khoảng trắng

        // Act
        var act = () => Club.Create(name, "mô tả");

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Club_Create_WithNameOver100Chars_ThrowsArgumentException()
    {
        // Arrange
        var longName = new string('a', Club.MaxNameLength + 1);

        // Act
        var act = () => Club.Create(longName, "mô tả");

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Club_HasRoom_AtMaxMembers_ReturnsFalse()
    {
        // Arrange
        var club = Club.Create("Tech Club", "mô tả");

        // Act
        var result = club.HasRoom(Club.MaxMembers);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Club_HasRoom_BelowMaxMembers_ReturnsTrue()
    {
        // Arrange
        var club = Club.Create("Tech Club", "mô tả");

        // Act
        var result = club.HasRoom(Club.MaxMembers - 1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Event_Register_NewMember_ReturnsTrue()
    {
        // Arrange
        var ev = new Event { Title = "Workshop SOLID" };
        var memberId = Guid.NewGuid();

        // Act
        var result = ev.Register(memberId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Event_Register_SameMemberTwice_ReturnsFalse()
    {
        // Arrange
        var ev = new Event { Title = "Workshop SOLID" };
        var memberId = Guid.NewGuid();
        ev.Register(memberId);

        // Act
        var result = ev.Register(memberId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Event_CheckIn_UnregisteredMember_ThrowsInvalidOperationException()
    {
        // Arrange
        var ev = new Event { Title = "Workshop SOLID" };

        // Act
        var act = () => ev.CheckIn(Guid.NewGuid());

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void Event_CheckIn_RegisteredMember_AddsToCheckedIn()
    {
        // Arrange
        var ev = new Event { Title = "Workshop SOLID" };
        var memberId = Guid.NewGuid();
        ev.Register(memberId);

        // Act
        ev.CheckIn(memberId);

        // Assert
        Assert.Contains(memberId, ev.CheckedInMemberIds);
    }
}