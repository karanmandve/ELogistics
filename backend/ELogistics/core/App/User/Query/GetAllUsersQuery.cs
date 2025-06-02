//using core.Interface;
//using Dapper;
//using domain.ModelDto;
//using Mapster;
//using MediatR;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace core.App.User.Query
//{
//    public class GetAllUsersQuery : IRequest<List<UserDto>>
//    {
//    }

//    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserDto>>
//    {
//        private readonly IAppDbContext _context;

//        public GetAllUsersQueryHandler(IAppDbContext context)
//        {
//            _context = context;
//        }
//        public async Task<List<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
//        {

//            using var connection = _context.GetConnection();

//            var query = "SELECT * FROM Users WHERE Role = @Role;";

//            var data = await connection.QueryAsync<UserDto>(query, new { Role = "User" });

//            return data.ToList();

//        }
//    }
//}
