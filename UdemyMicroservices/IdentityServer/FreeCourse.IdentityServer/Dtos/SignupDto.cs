namespace FreeCourse.IdentityServer.Dtos
{
    public class SignupDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public int AgencyId { get; set; }
    }
}