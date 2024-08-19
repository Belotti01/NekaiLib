using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Nekai.Razor;

public class NekaiDbContext<TUser> : IdentityDbContext<TUser, IdentityRole<Guid>, Guid>
where TUser : NekaiUser
{
	public NekaiDbContext(DbContextOptions config)
	: base(config)
	{
		
	}
}