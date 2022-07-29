namespace API.Services
{
    public class DbCodes
    {
        public enum Codes
        {
            Succes=1, Error=2
        }
        public static Codes GetCodes()
        {
            Codes codes = new Codes();
            return codes;
        }
    }
}
