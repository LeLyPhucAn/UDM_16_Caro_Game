using Server.Database;

namespace Server.Repositories;

public class UserRepository
{
    private readonly DatabaseHelper _dbHelper;

    public UserRepository(DatabaseHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }
}