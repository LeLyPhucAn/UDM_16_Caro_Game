using CaroGame.Repository;

namespace CaroGame.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepo = new UserRepository();

        public bool Register(string username, string password)
        {
            try
            {
                _userRepo.InsertUser(username, password);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}