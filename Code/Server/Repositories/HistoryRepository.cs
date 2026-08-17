using Server.Database;

namespace Server.Repositories;

public class HistoryRepository
{
    private readonly DatabaseHelper _dbHelper;

    public HistoryRepository(DatabaseHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }
}