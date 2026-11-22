using EduHub.Ch02.Domain;

namespace EduHub.Ch02.Repositories;

public interface IMemberRepository
{
    Member? GetById(Guid id);
    IReadOnlyList<Member> GetByClub(Guid clubId);
    void Add(Member member);
}