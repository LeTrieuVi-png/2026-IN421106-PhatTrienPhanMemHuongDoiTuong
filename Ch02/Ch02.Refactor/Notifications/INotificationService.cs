namespace EduHub.Ch02.Notifications;

/// <summary>Hợp đồng gửi thông báo. Thêm kênh mới = thêm class, không sửa code cũ (OCP).</summary>
public interface INotificationService
{
    void Send(string to, string message);
}
