using Library.Interfaces;
using Library.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace Library.Authorization
{
    public class MustBeOwnerHandler : AuthorizationHandler<MustBeOwner>
    {
        private IStudentRepository _studentRepository;
        private IHttpContextAccessor _httpContextAccessor;

        public MustBeOwnerHandler(IStudentRepository studentRepository, IHttpContextAccessor httpContextAccessor)
        {
            _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MustBeOwner requirement)
        {
            var userStatus = context.User.IsInRole("Admin");
            if (!userStatus)
            {

                var studentId = _httpContextAccessor.HttpContext?.GetRouteValue("id")?.ToString();
                var mine = context.User.Claims.FirstOrDefault(x => x.Type == "Student")?.Value;

                if (!_studentRepository.IsStudent(Convert.ToInt32(studentId), Convert.ToInt32(mine)))
                {
                    context.Fail();
                    return Task.CompletedTask;
                }
            }
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
