using Server.Database;

namespace Server.Repositories;

public class MatchRepository
{
    private readonly DatabaseHelper _dbHelper;

    public MatchRepository(DatabaseHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }
}