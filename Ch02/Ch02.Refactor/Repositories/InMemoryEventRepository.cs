using EduHub.Ch02.Domain;

namespace EduHub.Ch02.Repositories;

public class InMemoryEventRepository : IEventRepository
{
    private readonly List<Event> _events = new();

    public Event? GetById(Guid id) => _events.FirstOrDefault(e => e.Id == id);
    public IReadOnlyList<Event> GetByClub(Guid clubId) =>
        _events.Where(e => e.ClubId == clubId).ToList();
    public void Add(Event ev) => _events.Add(ev);
}