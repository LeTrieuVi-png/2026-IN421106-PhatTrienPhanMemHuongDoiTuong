using EduHub.Ch02.Domain;

namespace EduHub.Ch02.Repositories;

public interface IEventRepository
{
    Event? GetById(Guid id);
    IReadOnlyList<Event> GetByClub(Guid clubId);
    void Add(Event ev);
}