using StartupCentral.DAL.EntityModels;
using StartupCentral.DAL.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StartupCentral.Code.Class
{
    public class ActiveMembersService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ActiveMembersService()
        {
            _unitOfWork = new UnitOfWork();
        }

        public async Task Add(int memberId, DateTime date)
        {
            activeMembers dbMembers = new activeMembers();
            var members = _unitOfWork.ActiveStudentsRepo.GetAll();


            if (members.Any(x => x.MemberId == memberId && x.LoginDate.CompareTo(date) == 0))
            {

                await _unitOfWork.SaveAsync();
            }

            else
            {
                dbMembers.MemberId = memberId;
                dbMembers.LoginDate = date;
                _unitOfWork.ActiveStudentsRepo.Insert(dbMembers);
                await _unitOfWork.SaveAsync();
            }
        }


    }
}