using Microsoft.AspNetCore.Authorization;

namespace Library.Authorization
{
    public class MustBeOwner : IAuthorizationRequirement
    {
        public MustBeOwner()
        {

        }
    }
}
